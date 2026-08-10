using System.Text;
using UnityEngine;
using UnityEngine.Profiling;

public class Fps : MonoBehaviour
{
    private readonly StringBuilder _sb = new();
    private float _fps;

    private void Update()
    {
        _fps = 1 / Time.deltaTime;
    }
    private void OnGUI()
    {
        _sb.Clear();
        _sb.AppendLine($"{_fps:F1}");
        _sb.AppendLine($"Allocated: {Profiler.GetTotalAllocatedMemoryLong() / 1024f / 1024f:F1} MB");
        _sb.AppendLine($"Reserved: {Profiler.GetTotalReservedMemoryLong() / 1024f / 1024f:F1} MB");
        GUIStyle style = new GUIStyle();
        style.fontSize = 24;
        style.normal.textColor = Color.white;

        GUI.Label(new Rect(20, 70, 400, 50), _sb.ToString(), style);
    }
}
