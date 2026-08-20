using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.AddressableAssets;

namespace Assets.Scripts.Services.AssetProvider
{
    public interface IAssetProviderService
    {
        UniTask<T> LoadAsync<T>(AssetReference reference, CancellationToken ct = default);
        void Release(AssetReference reference);
        void ReleaseAll();
    }
}