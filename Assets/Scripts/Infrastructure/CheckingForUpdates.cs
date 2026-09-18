using Assets.Scripts.Data;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

namespace Assets.Scripts.Infrastructure
{
    public class CheckingForUpdates : MonoBehaviour
    {
        [SerializeField] private string _allDataLabel = "allData";
        [SerializeField] private string _cloudContentLabel = "cloud";
        [SerializeField] private TMP_Text _statusText;

        [SerializeField] private SceneContext _sceneContext;
        [SerializeField] private RemoteConfigLoader _remoteConfigLoader;
        [SerializeField] private GameRunner _gameRunner;
        public static AllDataContainer DataContainer { get; private set; }
        public static AsyncOperationHandle DataContainerHandle { get; private set; }
        private async UniTaskVoid Start()
        {
            await Init();
        }

        private async UniTask Init()
        {
            try
            {
                SetStatus("Initializing...");
                await Addressables.InitializeAsync().ToUniTask();

                //if (await CheckForUpdates() == false)
                //{
                //    SetStatus("Something went wrong, restart the game");
                //    return;
                //}
                if (await LoadData() == false)
                {
                    SetStatus("Something went wrong, restart the game");
                    return;
                }
                SetStatus("Remote config synchronization...");
                _remoteConfigLoader.OnInit += () => StartGame().Forget();
                _remoteConfigLoader.Run().Forget();
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                SetStatus("Something went wrong, restart the game");
            }
        }

        private async UniTask<bool> CheckForUpdates()
        {
            SetStatus("Checking for updates...");
            AsyncOperationHandle<long> checkDownloadHandle = default;
            try
            {
                checkDownloadHandle = Addressables.GetDownloadSizeAsync(_cloudContentLabel);
                long bytes = await checkDownloadHandle.ToUniTask();

                if (bytes > 0)
                {
                    return await Download();
                }

                return checkDownloadHandle.Status == AsyncOperationStatus.Succeeded;
            }

            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
            finally
            {
                if (checkDownloadHandle.IsValid())
                    Addressables.Release(checkDownloadHandle);
            }
        }

        private async UniTask<bool> Download()
        {
            AsyncOperationHandle handle = default;
            try
            {
                handle = Addressables.DownloadDependenciesAsync(_cloudContentLabel);
                await handle.ToUniTask(progress: Progress.Create<float>(_ =>
                {
                    DownloadStatus status = handle.GetDownloadStatus();
                    SetStatus($"{FormatBytes(status.DownloadedBytes)}/{FormatBytes(status.TotalBytes)}");
                }));

                return handle.Status == AsyncOperationStatus.Succeeded;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
            finally
            {
                if (handle.IsValid())
                    Addressables.Release(handle);
            }
        }

        private async UniTask<bool> LoadData()
        {
            SetStatus("Loading data...");

            bool loaded;
            AllDataContainer data;
            AsyncOperationHandle<AllDataContainer> dataHandle = default;
            try
            {
                dataHandle = Addressables.LoadAssetAsync<AllDataContainer>(_allDataLabel);
                data = await dataHandle.ToUniTask();
                loaded = data != null;
            }
            catch (System.Exception e)
            {
                if (dataHandle.IsValid())
                    Addressables.Release(dataHandle);
                Debug.LogException(e);
                return false;
            }
            DataContainer = data;
            DataContainerHandle = dataHandle;
            return loaded;
        }
        private async UniTask StartGame()
        {
            try
            {
                SetStatus("Launching...");
                _sceneContext.Run();
                await _gameRunner.Run();
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                SetStatus("Something went wrong, restart the game");
            }
        }
        private void SetStatus(string status)
        {
            _statusText.text = status;
        }

        private static string FormatBytes(long bytes)
        {
            if (bytes < 1024)
                return $"{bytes} B";

            if (bytes < 1024 * 1024)
                return $"{bytes / 1024f:F1} KB";

            return $"{bytes / (1024f * 1024f):F1} MB";
        }
    }
}