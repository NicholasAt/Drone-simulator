using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets.Scripts.References
{
    [Serializable]
    public class AssetReferenceSprite : AssetReferenceT<Sprite>
    {
        public AssetReferenceSprite(string guid) : base(guid)
        { }
    }
}