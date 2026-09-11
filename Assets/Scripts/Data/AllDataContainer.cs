using Assets.Scripts.Data.BotsData.CarData;
using Assets.Scripts.Data.BotsData.FlyData;
using Assets.Scripts.Data.CameraAnimationData;
using Assets.Scripts.Data.DestroyVehiclesEffect;
using Assets.Scripts.Data.DronesData;
using Assets.Scripts.Data.HelicoptersData;
using Assets.Scripts.Data.Quests;
using UnityEngine;

namespace Assets.Scripts.Data
{
    [CreateAssetMenu(menuName = "Data/All data")]
    public class AllDataContainer : ScriptableObject
    {
        [field: SerializeField] public DroneData DroneData { get; private set; }
        [field: SerializeField] public HelicopterData HelicopterData { get; private set; }
        [field: SerializeField] public UIData UIData { get; private set; }
        [field: SerializeField] public QuestsData QuestsData { get; private set; }
        [field: SerializeField] public QuestObjectsData QuestObjectsData { get; private set; }
        [field: SerializeField] public CarData CarData { get; private set; }
        [field: SerializeField] public CameraData CameraData { get; private set; }
        [field: SerializeField] public FlyingTransportData FlyingTransportData { get; private set; }
        [field: SerializeField] public OutScreenData OutScreenData { get; private set; }
        [field: SerializeField] public VehiclesDestroyEffectData DestroyVehiclesEffectData { get; private set; }
        [field: SerializeField] public ChunkData ChunkData { get; private set; }
        [field: SerializeField] public MusicData MusicData { get; private set; }
    }
}