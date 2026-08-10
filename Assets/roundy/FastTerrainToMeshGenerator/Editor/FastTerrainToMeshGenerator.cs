using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public enum TerrainShaderQuality
{
    Basic,
    WithResampling
}
public enum TerrainHandling
{
    KeepEnabled,
    DisableGameObject,
    DisableRendering
}

[System.Serializable]
public class TerrainShaderConfig
{
    public TerrainShaderQuality quality;
    public string shaderPath;

    public static TerrainShaderConfig GetConfig(TerrainShaderQuality quality)
    {
        return new TerrainShaderConfig
        {
            quality = quality,
            shaderPath = "Roundy/FastUnlitTerrainShader"
        };
    }
}

public class FastTerrainToMeshGenerator : EditorWindow
{
    private const float SPACING = 10f;
    private const float HEADER_SPACING = 15f;

    private Terrain selectedTerrain;
    private TerrainShaderQuality shaderQuality = TerrainShaderQuality.Basic;
    private TerrainHandling terrainHandling = TerrainHandling.DisableGameObject;
    private bool addMeshColliders = true;
    private bool markAsStatic = true;
    private int chunkAmount = 1;
    private int resolutionPerChunk = 32;
    private int splatResolution = 1024;
    private string[] splatResolutionOptions = { "128", "256", "512", "1024", "2048" };
    private int selectedSplatResolutionIndex = 3;
    private string[] textureArrayResolutionOptions = { "256", "512", "1024", "2048", "4096" };
    private int selectedTextureArrayResolutionIndex = 3;
    //  private bool disableTerrain = true;
    private int terrainCounter = 0;

    // Tree export properties
    private bool exportTrees = false;
    private bool disableSourceTrees = false;
    private List<GameObject> createdTreeObjects = new List<GameObject>();
    private GameObject treeRootParent;
    // private bool originalTreesVisible = true;

    private bool enableLOD = false;
    private int lodLevels = 2; // Default to 2 levels (can be 2,3,4)
    private float lodReductionStrength = 1f; // 1 = half size, 0.5 = 75%, 2 = 25%
    private float[] lodTransitionHeights = new float[] { 0.6f, 0.3f, 0.1f }; // Default transition heights

    // GUI styling
    private GUIStyle headerStyle;
    private GUIStyle sectionStyle;

    private Vector2 scrollPosition;

    [MenuItem("Tools/Roundy/Fast Terrain To Mesh Generator")]
    public static void ShowWindow()
    {
        FastTerrainToMeshGenerator window = GetWindow<FastTerrainToMeshGenerator>("Fast Terrain To Mesh Generator");
        window.minSize = new Vector2(400, 600);
    }

    private void OnEnable()
    {
        InitializeStyles();
    }

    private void InitializeStyles()
    {
        headerStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 14,
            alignment = TextAnchor.MiddleLeft,
            margin = new RectOffset(0, 0, 10, 10)
        };

        sectionStyle = new GUIStyle(EditorStyles.helpBox)
        {
            padding = new RectOffset(10, 10, 10, 10),
            margin = new RectOffset(0, 0, 8, 8)
        };
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        EditorGUILayout.Space(SPACING);
        DrawHeader();
        EditorGUILayout.Space(HEADER_SPACING);

        using (new EditorGUILayout.VerticalScope(sectionStyle))
        {
            DrawTerrainSelection();
        }

        EditorGUILayout.Space(SPACING);

        using (new EditorGUILayout.VerticalScope(sectionStyle))
        {
            DrawQualitySettings();
        }

        EditorGUILayout.Space(SPACING);

        using (new EditorGUILayout.VerticalScope(sectionStyle))
        {
            DrawMeshSettings();
        }

        EditorGUILayout.Space(SPACING);

        using (new EditorGUILayout.VerticalScope(sectionStyle))
        {
            DrawLODSettings();
        }

        EditorGUILayout.Space(SPACING);

        using (new EditorGUILayout.VerticalScope(sectionStyle))
        {
            DrawTextureSettings();
        }

        EditorGUILayout.Space(SPACING);

        using (new EditorGUILayout.VerticalScope(sectionStyle))
        {
            DrawTreeSettings();
        }

        EditorGUILayout.Space(SPACING);

        DrawConvertButton();

        EditorGUILayout.EndScrollView();
    }


    private void DrawHeader()
    {
        EditorGUILayout.LabelField("Fast Terrain To Mesh Generator v0.1", headerStyle);
        EditorGUILayout.LabelField("Convert Unity Terrain to mesh with fast unlit shaders",
            EditorStyles.miniLabel);
    }
    private void DrawTreeSettings()
    {
        EditorGUILayout.LabelField("Tree Settings", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        exportTrees = EditorGUILayout.Toggle(
            new GUIContent("Export Trees", "Convert terrain trees to game objects"),
            exportTrees);

        if (exportTrees)
        {
            EditorGUI.indentLevel++;
            disableSourceTrees = EditorGUILayout.Toggle(
                new GUIContent("Disable Source Trees", "Hide the original terrain trees after export"),
                disableSourceTrees);
            EditorGUI.indentLevel--;
        }
    }
    private void DrawLODSettings()
    {
        EditorGUILayout.LabelField("LOD Settings", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        enableLOD = EditorGUILayout.Toggle(
            new GUIContent("Enable LOD Groups", "Generate LOD levels for each chunk"),
            enableLOD);

        if (enableLOD)
        {
            EditorGUI.indentLevel++;

            lodLevels = EditorGUILayout.IntSlider(
                new GUIContent("LOD Levels", "Number of LOD levels (2-4)"),
                lodLevels, 2, 4);

            lodReductionStrength = EditorGUILayout.Slider(
                new GUIContent("Reduction Strength", "1 = half size, 0.5 = 75%, 2 = 25%"),
                lodReductionStrength, 0.5f, 2f);

            EditorGUILayout.LabelField("LOD Transition Heights", EditorStyles.boldLabel);
            for (int i = 1; i < lodLevels; i++)
            {
                lodTransitionHeights[i - 1] = EditorGUILayout.Slider(
                    $"LOD {i - 1} to {i}",
                    lodTransitionHeights[i - 1],
                    0.01f,
                    i == 1 ? 0.9f : lodTransitionHeights[i - 2]);
            }

            EditorGUI.indentLevel--;
        }
    }

    private void DrawTerrainSelection()
    {
        EditorGUILayout.LabelField("Terrain Source", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        selectedTerrain = (Terrain)EditorGUILayout.ObjectField(
            new GUIContent("Source Terrain", "The terrain to convert into a mesh"),
            selectedTerrain, typeof(Terrain), true);

        terrainHandling = (TerrainHandling)EditorGUILayout.EnumPopup(
            new GUIContent("Original Terrain Handling",
                "KeepEnabled: Keep terrain as is\n" +
                "DisableGameObject: Disable the terrain GameObject\n" +
                "DisableRendering: Keep terrain enabled but disable its rendering"),
            terrainHandling);

        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Generated Mesh Settings", EditorStyles.boldLabel);

        addMeshColliders = EditorGUILayout.Toggle(
            new GUIContent("Add Mesh Colliders", "Add mesh colliders to generated chunks"),
            addMeshColliders);

        markAsStatic = EditorGUILayout.Toggle(
            new GUIContent("Mark As Static", "Mark generated chunks as static objects"),
            markAsStatic);
    }

    private void DrawQualitySettings()
    {
        EditorGUILayout.LabelField("Quality Settings", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        shaderQuality = (TerrainShaderQuality)EditorGUILayout.EnumPopup(
            new GUIContent("Quality Level",
                "Basic: Simple terrain texturing\n" +
                "WithResampling: Includes distance-based texture resampling"),
            shaderQuality);
    }

    private void DrawMeshSettings()
    {
        EditorGUILayout.LabelField("Mesh Generation", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        chunkAmount = EditorGUILayout.IntSlider(
            new GUIContent("Chunk Amount", "Number of chunks per axis (total chunks = this value squared)"),
            chunkAmount, 1, 10);

        resolutionPerChunk = EditorGUILayout.IntSlider(
            new GUIContent("Resolution Per Chunk", "Vertices per chunk (higher = more detailed but slower)"),
            resolutionPerChunk, 16, 256);
    }

    private void DrawTextureSettings()
    {
        EditorGUILayout.LabelField("Texture Settings", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        selectedSplatResolutionIndex = EditorGUILayout.Popup(
            new GUIContent("Splat Resolution", "Resolution of the generated splat texture"),
            selectedSplatResolutionIndex, splatResolutionOptions);
        splatResolution = int.Parse(splatResolutionOptions[selectedSplatResolutionIndex]);

        selectedTextureArrayResolutionIndex = EditorGUILayout.Popup(
            new GUIContent("Texture Array Resolution", "Resolution for all textures in the array"),
            selectedTextureArrayResolutionIndex, textureArrayResolutionOptions);
    }

    private void DrawConvertButton()
    {
        GUI.enabled = selectedTerrain != null;

        EditorGUILayout.Space(SPACING);

        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Convert Terrain", GUILayout.Width(200), GUILayout.Height(30)))
            {

                ConvertTerrainToMesh();

            }

            GUILayout.FlexibleSpace();
        }

        GUI.enabled = true;
    }

    private void ConvertTerrainToMesh()
    {
        TerrainData terrainData = selectedTerrain.terrainData;
        TerrainShaderConfig shaderConfig = TerrainShaderConfig.GetConfig(shaderQuality);

        string folderPath = $"Assets/FastTerrainMesh/Terrain{terrainCounter}";
        CreateFolders(folderPath);

        List<TextureImporter> importersToRestore = MakeTerrainTexturesReadable(terrainData);

        GameObject parentObject = CreateParentObject();

        // Process mesh conversion
        Texture2D splatTexture = CreateSplatTexture(terrainData, splatResolution);
        string splatPath = SaveTextureAsset(splatTexture, $"TerrainSplat_{terrainCounter}", folderPath);

        ProcessMeshChunks(terrainData, parentObject, folderPath);

        Material material = CreateTerrainMaterial(terrainData, splatPath, shaderConfig, folderPath);
        AssignMaterialToChunks(parentObject, material);

        // Handle tree export if enabled
        if (exportTrees)
        {
            ExportTrees(parentObject);
        }

        RestoreTextureImporters(importersToRestore);
        FinalizeConversion(parentObject);
    }

    private void CreateFolders(string folderPath)
    {
        if (!AssetDatabase.IsValidFolder("Assets/FastTerrainMesh"))
        {
            AssetDatabase.CreateFolder("Assets", "FastTerrainMesh");
        }
        AssetDatabase.CreateFolder("Assets/FastTerrainMesh", $"Terrain{terrainCounter}");
    }

    private GameObject CreateParentObject()
    {
        GameObject parentObject = new GameObject($"FastTerrainMesh_{terrainCounter}");
        Undo.RegisterCreatedObjectUndo(parentObject, "Create Fast Terrain Mesh");
        parentObject.transform.position = selectedTerrain.transform.position;
        return parentObject;
    }

    private List<TextureImporter> MakeTerrainTexturesReadable(TerrainData terrainData)
    {
        List<TextureImporter> importersToRestore = new List<TextureImporter>();

        foreach (TerrainLayer layer in terrainData.terrainLayers)
        {
            if (layer != null && layer.diffuseTexture != null)
            {
                string assetPath = AssetDatabase.GetAssetPath(layer.diffuseTexture);
                TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                if (importer != null && !importer.isReadable)
                {
                    importersToRestore.Add(importer);
                    importer.isReadable = true;
                    importer.SaveAndReimport();
                }
            }
        }

        return importersToRestore;
    }

    private Material CreateTerrainMaterial(TerrainData terrainData, string splatPath, TerrainShaderConfig shaderConfig, string folderPath)
    {
        // Load and validate shader
        Shader shader = Shader.Find(shaderConfig.shaderPath);
        if (shader == null)
        {
            Debug.LogError($"Shader not found: {shaderConfig.shaderPath}. Falling back to default shader.");
            return new Material(Shader.Find("Universal Render Pipeline/Lit"));
        }

        Material material = new Material(shader);

        // Set splat control texture
        Texture2D splatControl = AssetDatabase.LoadAssetAtPath<Texture2D>(splatPath);
        if (splatControl != null)
        {
            material.SetTexture("_SplatTex", splatControl);
        }
        else
        {
            Debug.LogError($"Failed to load splat texture at path: {splatPath}");
        }

        // Process terrain layers
        TerrainLayer[] layers = terrainData.terrainLayers;
        int maxLayers = Mathf.Min(layers.Length, 4); // Shader supports up to 4 layers

        for (int i = 0; i < maxLayers; i++)
        {
            TerrainLayer layer = layers[i];
            string mainTexProperty = $"_MainTex{i}";
            string tintColorProperty = $"_TintColor{i}";

            if (layer != null && layer.diffuseTexture != null)
            {
                // Assign main texture
                material.SetTexture(mainTexProperty, layer.diffuseTexture);

                // Calculate tiling based on terrain size and layer settings
                float scaleX = terrainData.size.x / layer.tileSize.x;
                float scaleY = terrainData.size.z / layer.tileSize.y;

                // Use Unity's standard _ST suffix for tiling and offset
                // xy = tiling, zw = offset
                material.SetVector($"{mainTexProperty}_ST", new Vector4(
                    scaleX,
                    scaleY,
                    layer.tileOffset.x,
                    layer.tileOffset.y
                ));

                // Set tint color - use diffuse color if available, otherwise white
                Color tintColor = layer.diffuseRemapMax;
                material.SetColor(tintColorProperty, tintColor);
            }
            else
            {
                // Handle missing or null layers with default values
                ConfigureDefaultLayerProperties(material, mainTexProperty, tintColorProperty);
            }
        }

        // Handle any remaining texture slots (if less than 4 layers)
        for (int i = maxLayers; i < 4; i++)
        {
            string mainTexProperty = $"_MainTex{i}";
            string tintColorProperty = $"_TintColor{i}";
            ConfigureDefaultLayerProperties(material, mainTexProperty, tintColorProperty);
        }

        // Configure quality-specific shader features
        ConfigureShaderQualitySettings(material, shaderConfig.quality);

        // Save material asset
        string materialPath = Path.Combine(folderPath, $"TerrainMaterial_{terrainCounter}.mat");
        AssetDatabase.CreateAsset(material, materialPath);
        AssetDatabase.SaveAssets();

        return material;
    }
    private void ConfigureDefaultLayerProperties(Material material, string texProperty, string colorProperty)
    {
        // Use 1x1 white texture for better performance than Texture2D.whiteTexture
        Texture2D defaultTex = GetDefaultTexture();
        material.SetTexture(texProperty, defaultTex);
        material.SetVector($"{texProperty}_ST", new Vector4(1, 1, 0, 0)); // Default tiling/offset
        material.SetColor(colorProperty, Color.white);
    }

    private void ConfigureShaderQualitySettings(Material material, TerrainShaderQuality quality)
    {
        if (quality == TerrainShaderQuality.WithResampling)
        {
            material.EnableKeyword("ENABLE_RESAMPLING");
            material.SetVector("_ResampleDistance", new Vector4(20, 50, 0, 0));
            material.SetVector("_ResampleTiling", new Vector4(0.2f, 0.2f, 0, 0));
        }
        else
        {
            material.DisableKeyword("ENABLE_RESAMPLING");
        }
    }

    // Cache the default texture to avoid recreation
    private static Texture2D defaultTexture;
    private Texture2D GetDefaultTexture()
    {
        if (defaultTexture == null)
        {
            defaultTexture = new Texture2D(1, 1, TextureFormat.RGB24, false);
            defaultTexture.SetPixel(0, 0, Color.white);
            defaultTexture.Apply();
        }
        return defaultTexture;
    }



    private void ProcessMeshChunks(TerrainData terrainData, GameObject parentObject, string folderPath)
    {
        int heightmapResolution = terrainData.heightmapResolution;
        float[,] heights2D = terrainData.GetHeights(0, 0, heightmapResolution, heightmapResolution);
        float chunkSizeFloat = (heightmapResolution - 1) / (float)chunkAmount;

        int totalChunks = chunkAmount * chunkAmount;
        int currentChunk = 0;

        for (int chunkY = 0; chunkY < chunkAmount; chunkY++)
        {
            for (int chunkX = 0; chunkX < chunkAmount; chunkX++)
            {
                EditorUtility.DisplayProgressBar("Converting Terrain to Mesh",
                    $"Processing chunk {currentChunk + 1} of {totalChunks}",
                    (float)currentChunk / totalChunks);

                if (enableLOD)
                {
                    ProcessChunkWithLOD(chunkX, chunkY, chunkSizeFloat, heights2D, heightmapResolution,
                        terrainData, parentObject, folderPath);
                }
                else
                {
                    ProcessSingleChunk(chunkX, chunkY, chunkSizeFloat, heights2D, heightmapResolution,
                        terrainData, parentObject, folderPath);
                }

                currentChunk++;
            }
        }
    }
    private void ProcessChunkWithLOD(int chunkX, int chunkY, float chunkSizeFloat, float[,] heights2D,
        int heightmapResolution, TerrainData terrainData, GameObject parentObject, string folderPath)
    {
        // Create LOD Group parent for this chunk
        GameObject lodGroupObject = new GameObject($"TerrainChunk_LOD_{chunkX}_{chunkY}");
        Undo.RegisterCreatedObjectUndo(lodGroupObject, "Create LOD Group");
        lodGroupObject.transform.SetParent(parentObject.transform);

        LODGroup lodGroup = lodGroupObject.AddComponent<LODGroup>();

        // Position the LOD group
        float chunkWorldWidth = terrainData.size.x / chunkAmount;
        float chunkWorldLength = terrainData.size.z / chunkAmount;
        lodGroupObject.transform.localPosition = new Vector3(chunkX * chunkWorldWidth, 0,
            chunkY * chunkWorldLength);

        // Create LOD levels
        LOD[] lods = new LOD[lodLevels];

        for (int lodLevel = 0; lodLevel < lodLevels; lodLevel++)
        {
            // Calculate resolution for this LOD level
            int currentResolution = CalculateLODResolution(lodLevel);

            // Create mesh for this LOD level
            GameObject lodObject = new GameObject($"LOD_{lodLevel}");
            lodObject.transform.SetParent(lodGroupObject.transform);
            lodObject.transform.localPosition = Vector3.zero;

            Mesh lodMesh = CreateTerrainChunkMesh(
                Mathf.RoundToInt(chunkX * chunkSizeFloat),
                Mathf.RoundToInt(chunkY * chunkSizeFloat),
                Mathf.RoundToInt(chunkSizeFloat),
                Mathf.RoundToInt(chunkSizeFloat),
                heights2D,
                heightmapResolution,
                terrainData,
                chunkX,
                chunkY,
                currentResolution
            );

            // Add components
            lodObject.AddComponent<MeshFilter>().sharedMesh = lodMesh;
            lodObject.AddComponent<MeshRenderer>();

            if (addMeshColliders && lodLevel == 0) // Only add collider to highest LOD
            {
                MeshCollider meshCollider = lodObject.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = lodMesh;
            }

            // Save mesh asset
            SaveMeshAsset(lodMesh, $"TerrainChunk_{chunkX}_{chunkY}_LOD{lodLevel}", folderPath);

            // Create LOD with renderer
            Renderer[] renderers = new Renderer[] { lodObject.GetComponent<Renderer>() };
            float lodPercentage = lodLevel == lodLevels - 1 ? 0.01f : lodTransitionHeights[lodLevel];
            lods[lodLevel] = new LOD(lodPercentage, renderers);
        }

        // Set up LOD group
        lodGroup.SetLODs(lods);
        lodGroup.RecalculateBounds();

        if (markAsStatic)
        {
            lodGroupObject.isStatic = true;
        }
    }

    private int CalculateLODResolution(int lodLevel)
    {
        if (lodLevel == 0)
            return resolutionPerChunk;

        // Calculate base reduction factor (2x, 4x, 8x for LOD levels)
        float reductionFactor = Mathf.Pow(2, lodLevel); // Base reduction: LOD1=2x, LOD2=4x, LOD3=8x

        // Apply strength modifier
        reductionFactor = Mathf.Pow(reductionFactor, lodReductionStrength);

        int newResolution = Mathf.Max(4, Mathf.RoundToInt(resolutionPerChunk / reductionFactor));

        // Ensure resolution is always even and at least 4
        newResolution = Mathf.Max(4, newResolution + (newResolution % 2));

        // Debug log
        Debug.Log($"LOD {lodLevel}: Original Resolution = {resolutionPerChunk}, " +
                  $"Reduction Factor = {reductionFactor}, " +
                  $"New Resolution = {newResolution}");

        return newResolution;
    }

    private void ProcessSingleChunk(int chunkX, int chunkY, float chunkSizeFloat, float[,] heights2D,
        int heightmapResolution, TerrainData terrainData, GameObject parentObject, string folderPath)
    {
        int startX = Mathf.RoundToInt(chunkX * chunkSizeFloat);
        int startY = Mathf.RoundToInt(chunkY * chunkSizeFloat);
        int endX = (chunkX == chunkAmount - 1) ? (heightmapResolution - 1) :
            Mathf.RoundToInt((chunkX + 1) * chunkSizeFloat);
        int endY = (chunkY == chunkAmount - 1) ? (heightmapResolution - 1) :
            Mathf.RoundToInt((chunkY + 1) * chunkSizeFloat);

        int currentChunkWidth = endX - startX;
        int currentChunkHeight = endY - startY;

        Mesh mesh = CreateTerrainChunkMesh(startX, startY, currentChunkWidth,
            currentChunkHeight, heights2D, heightmapResolution, terrainData, chunkX, chunkY,
            resolutionPerChunk); // Explicitly pass resolutionPerChunk

        GameObject chunkObject = new GameObject($"TerrainChunk_{chunkX}_{chunkY}");
        Undo.RegisterCreatedObjectUndo(chunkObject, "Create Terrain Chunk");
        chunkObject.transform.SetParent(parentObject.transform);
        chunkObject.AddComponent<MeshFilter>().sharedMesh = mesh;
        chunkObject.AddComponent<MeshRenderer>();
        if (addMeshColliders)
        {
            MeshCollider meshCollider = chunkObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;
        }

        // Mark as static if enabled
        if (markAsStatic)
        {
            chunkObject.isStatic = true;
        }



        float chunkWorldWidth = terrainData.size.x / chunkAmount;
        float chunkWorldLength = terrainData.size.z / chunkAmount;
        chunkObject.transform.localPosition = new Vector3(chunkX * chunkWorldWidth, 0,
            chunkY * chunkWorldLength);

        SaveMeshAsset(mesh, $"TerrainChunk_{chunkX}_{chunkY}", folderPath);
    }

    private void RestoreTextureImporters(List<TextureImporter> importers)
    {
        foreach (TextureImporter importer in importers)
        {
            importer.isReadable = false;
            importer.SaveAndReimport();
        }
    }

    private Mesh CreateTerrainChunkMesh(int startX, int startY, int chunkWidth, int chunkHeight,
        float[,] heights2D, int terrainResolution, TerrainData terrainData, int chunkX, int chunkY,
        int resolution)
    {
        float chunkWorldWidth = terrainData.size.x / chunkAmount;
        float chunkWorldLength = terrainData.size.z / chunkAmount;

        int vertexResolutionX = resolution + 1;
        int vertexResolutionY = resolution + 1;

        Vector3[] vertices = new Vector3[vertexResolutionX * vertexResolutionY];
        Vector2[] uvs = new Vector2[vertexResolutionX * vertexResolutionY];
        List<int> triangles = new List<int>();

        float xScale = chunkWorldWidth / resolution;  // Changed from resolutionPerChunk
        float zScale = chunkWorldLength / resolution; // Changed from resolutionPerChunk

        // Calculate world-space position offset for this chunk
        float worldStartX = chunkX * chunkWorldWidth;
        float worldStartZ = chunkY * chunkWorldLength;

        // Calculate UV scale based on terrain size
        float uvScaleX = 1.0f / terrainData.size.x;
        float uvScaleZ = 1.0f / terrainData.size.z;

        for (int y = 0; y < vertexResolutionY; y++)
        {
            for (int x = 0; x < vertexResolutionX; x++)
            {
                int index = y * vertexResolutionX + x;

                float normalizedX = (float)x / resolution;  // Changed from resolutionPerChunk
                float normalizedY = (float)y / resolution;  // Changed from resolutionPerChunk

                float terrainX = Mathf.Lerp(startX, startX + chunkWidth, normalizedX);
                float terrainY = Mathf.Lerp(startY, startY + chunkHeight, normalizedY);

                int mapX = Mathf.Clamp(Mathf.FloorToInt(terrainX), 0, terrainResolution - 1);
                int mapY = Mathf.Clamp(Mathf.FloorToInt(terrainY), 0, terrainResolution - 1);

                float heightValue = heights2D[mapY, mapX] * terrainData.size.y;

                // Calculate world position for vertex
                float worldX = worldStartX + (x * xScale);
                float worldZ = worldStartZ + (y * zScale);
                vertices[index] = new Vector3(x * xScale, heightValue, y * zScale);

                // Calculate UVs based on world position
                uvs[index] = new Vector2(
                    worldX * uvScaleX,
                    worldZ * uvScaleZ
                );

                // Generate triangles - adjusted for current resolution
                if (x < resolution && y < resolution)  // Changed from resolutionPerChunk
                {
                    int topLeft = index;
                    int topRight = topLeft + 1;
                    int bottomLeft = topLeft + vertexResolutionX;
                    int bottomRight = bottomLeft + 1;

                    triangles.Add(topLeft);
                    triangles.Add(bottomLeft);
                    triangles.Add(topRight);

                    triangles.Add(topRight);
                    triangles.Add(bottomLeft);
                    triangles.Add(bottomRight);
                }
            }
        }

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }
    private Texture2D CreateSplatTexture(TerrainData terrainData, int resolution)
    {
        Texture2D splatTexture = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false);
        float[,,] splatmapData = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);
        int layerCount = terrainData.terrainLayers.Length;

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                float splatX = (float)x / resolution * (terrainData.alphamapWidth - 1);
                float splatY = (float)y / resolution * (terrainData.alphamapHeight - 1);

                int sx = Mathf.FloorToInt(splatX);
                int sy = Mathf.FloorToInt(splatY);

                Color splatColor = new Color(
                    layerCount > 0 ? splatmapData[sy, sx, 0] : 1,
                    layerCount > 1 ? splatmapData[sy, sx, 1] : 0,
                    layerCount > 2 ? splatmapData[sy, sx, 2] : 0,
                    layerCount > 3 ? splatmapData[sy, sx, 3] : 0
                );

                if (splatColor.r + splatColor.g + splatColor.b + splatColor.a == 0)
                {
                    splatColor.r = 1;
                }

                splatTexture.SetPixel(x, y, splatColor);
            }
        }

        splatTexture.Apply();
        return splatTexture;
    }

    private void SaveMeshAsset(Mesh mesh, string name, string folderPath)
    {
        string path = Path.Combine(folderPath, $"{name}.asset");
        AssetDatabase.CreateAsset(mesh, path);
    }

    private string SaveTextureAsset(Texture2D texture, string name, string folderPath)
    {
        string path = Path.Combine(folderPath, $"{name}.png");
        byte[] pngData = texture.EncodeToPNG();
        if (pngData != null)
        {
            File.WriteAllBytes(path, pngData);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Default;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.filterMode = FilterMode.Bilinear;
                importer.mipmapEnabled = true;
                importer.SaveAndReimport();
            }
        }
        return path;
    }

    private void AssignMaterialToChunks(GameObject parentObject, Material material)
    {
        MeshRenderer[] meshRenderers = parentObject.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in meshRenderers)
        {
            renderer.sharedMaterial = material;
        }
    }

    private void ExportTrees(GameObject parentObject)
    {
        if (selectedTerrain == null) return;

        TerrainData terrainData = selectedTerrain.terrainData;
        TreeInstance[] trees = terrainData.treeInstances;
        TreePrototype[] treePrototypes = terrainData.treePrototypes;

        // Create tree parent under the main parent object
        treeRootParent = new GameObject("Trees");
        Undo.RegisterCreatedObjectUndo(treeRootParent, "Create Tree Parent");
        treeRootParent.transform.SetParent(parentObject.transform, false);
        createdTreeObjects.Add(treeRootParent);

        // Dictionary to store prototype parents and their LOD status
        Dictionary<GameObject, (GameObject parent, bool hasLOD)> prototypeInfo =
            new Dictionary<GameObject, (GameObject parent, bool hasLOD)>();

        // Create prototype parents
        foreach (TreePrototype prototype in treePrototypes)
        {
            GameObject prefab = prototype.prefab;
            if (!prototypeInfo.ContainsKey(prefab))
            {
                GameObject prototypeParent = new GameObject(prefab.name);
                prototypeParent.transform.SetParent(treeRootParent.transform, false);
                bool hasLOD = prefab.GetComponent<LODGroup>() != null;
                prototypeInfo.Add(prefab, (prototypeParent, hasLOD));
                createdTreeObjects.Add(prototypeParent);
            }
        }

        // Process trees in batches
        int batchSize = 1000;
        int currentBatch = 0;
        int totalTrees = trees.Length;

        for (int i = 0; i < totalTrees; i++)
        {
            if (i % batchSize == 0)
            {
                currentBatch++;
                EditorUtility.DisplayProgressBar("Exporting Trees",
                    $"Processing trees {i + 1}-{Mathf.Min(i + batchSize, totalTrees)} of {totalTrees}",
                    (float)i / totalTrees);
                Undo.IncrementCurrentGroup();
            }

            TreeInstance tree = trees[i];
            GameObject prefab = treePrototypes[tree.prototypeIndex].prefab;
            var (prototypeParent, hasLOD) = prototypeInfo[prefab];

            Vector3 position = new Vector3(
                tree.position.x * terrainData.size.x,
                terrainData.GetInterpolatedHeight(tree.position.x, tree.position.z),
                tree.position.z * terrainData.size.z
            );

            GameObject treeInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            createdTreeObjects.Add(treeInstance);

            treeInstance.transform.SetParent(prototypeParent.transform, false);
            treeInstance.transform.localPosition = position;

            if (!hasLOD)
            {
                treeInstance.transform.localRotation = Quaternion.Euler(0, tree.rotation * 360f, 0);
            }

            Vector3 baseScale = prefab.transform.localScale;
            treeInstance.transform.localScale = new Vector3(
                baseScale.x * tree.widthScale,
                baseScale.y * tree.heightScale,
                baseScale.z * tree.widthScale
            );

            if (i % batchSize == batchSize - 1 || i == totalTrees - 1)
            {
                Undo.SetCurrentGroupName($"Export Trees Batch {currentBatch}");
            }
        }

        EditorUtility.ClearProgressBar();

        // Store original trees visibility state and handle visibility
        //  originalTreesVisible = selectedTerrain.drawTreesAndFoliage;
        if (disableSourceTrees)
        {
            Undo.RecordObject(selectedTerrain, "Toggle Terrain Trees Visibility");
            selectedTerrain.drawTreesAndFoliage = false;
        }
    }



    private void FinalizeConversion(GameObject parentObject)
    {
        switch (terrainHandling)
        {
            case TerrainHandling.DisableGameObject:
                Undo.RecordObject(selectedTerrain.gameObject, "Disable Terrain");
                selectedTerrain.gameObject.SetActive(false);
                break;

            case TerrainHandling.DisableRendering:
                Undo.RecordObject(selectedTerrain, "Disable Terrain Rendering");
                selectedTerrain.drawHeightmap = false;
                selectedTerrain.drawInstanced = false;
                break;

            case TerrainHandling.KeepEnabled:
                // Do nothing
                break;
        }

        if (markAsStatic)
        {
            parentObject.isStatic = true;
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
        terrainCounter++;

        EditorUtility.DisplayDialog("Conversion Complete",
            "Terrain has been successfully converted to mesh.", "OK");
    }
}