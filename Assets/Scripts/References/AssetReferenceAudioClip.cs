using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets.Scripts.References
{
    [Serializable]
    public class AssetReferenceAudioClip : AssetReferenceT<AudioClip>
    {
        public AssetReferenceAudioClip(string guid) : base(guid)
        { }
    }
}