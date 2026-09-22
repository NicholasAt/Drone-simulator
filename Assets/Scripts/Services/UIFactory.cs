using Assets.Scripts.Data;
using Assets.Scripts.Logic;
using Assets.Scripts.Services.AssetProvider;
using Assets.Scripts.UI.Windows.Popup;
using Assets.Scripts.UI.Windows.UIMainMenu;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Zenject;

namespace Assets.Scripts.Services
{
    public class UIFactory
    {
        private readonly DiContainer _diContainer;
        private readonly UIData _uIData;
        private readonly IAssetProviderService _assetProvider;

        public UIFactory(DiContainer diContainer, UIData uIData, IAssetProviderService assetProviderService)
        {
            _diContainer = diContainer;
            _uIData = uIData;
            _assetProvider = assetProviderService;
        }

        public async UniTask CreateMobileInput(CancellationToken ct = default)
        {
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(_uIData.MobileInputReference, ct);
            InstantiateInject(prefab);
        }

        public async UniTask<LoadingCurtain> CreateLoadingCurtain()
        {
            GameObject prefab = await Addressables.LoadAssetAsync<GameObject>(_uIData.LoadingCurtainReference).ToUniTask();//ignore release
            GameObject instance = InstantiateInject(prefab);
            if (instance.TryGetComponent(out LoadingCurtain loadingCurtain) == false)
                Debug.LogError("no component");

            return loadingCurtain;
        }

        public async UniTask<PopupWarningOneButton> CreatePopupWarning(CancellationToken ct = default)
        {
            return await CreatePopup<PopupWarningOneButton>(_uIData.PopUpWarningReference, ct);
        }

        public async UniTask<PopupMessage> CreatePopupMessage(CancellationToken ct = default)
        {
            return await CreatePopup<PopupMessage>(_uIData.PopUpMessageReference, ct);
        }

        public async UniTask<PopupTwoButtons> CreatePopupTwoButtons(CancellationToken ct = default)
        {
            return await CreatePopup<PopupTwoButtons>(_uIData.PopUpTwoButtonsReference, ct);
        }

        public async UniTask CreateScreenTarget(CancellationToken ct = default)
        {
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(_uIData.ScreenTargetWindowReference, ct);
            InstantiateInject(prefab);
        }

        public async UniTask CreateHUD(CancellationToken ct = default)
        {
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(_uIData.HUDReference, ct);
            InstantiateInject(prefab);
        }
        public async UniTask CreateMenu(CancellationToken ct = default)
        {
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(_uIData.MainMenuWindowReference, ct);
            GameObject instance = InstantiateInject(prefab);
            if (instance.TryGetComponent(out MainMenuWindow mainMenu))
                await mainMenu.Warmup();
        }
        private async UniTask<T> CreatePopup<T>(AssetReferenceGameObject reference, CancellationToken ct) where T : BasePopup
        {
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(reference, ct);
            GameObject instance = InstantiateInject(prefab);
            if (instance.TryGetComponent(out T popup))
                return popup;

            Debug.LogError("no component");
            return null;
        }

        private GameObject InstantiateInject(GameObject prefab, Transform parent = null)
        {
            GameObject instance = _diContainer.InstantiatePrefab(prefab);
            instance.transform.SetParent(parent);

            if (parent == null)
                SceneManager.MoveGameObjectToScene(instance, SceneManager.GetActiveScene());

            return instance;
        }
    }
}