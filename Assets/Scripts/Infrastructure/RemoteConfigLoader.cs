using Cysharp.Threading.Tasks;
using System;
using Unity.Services.Analytics;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;
using UnityEngine;
using UnityEngine.UnityConsent;
public class RemoteConfigLoader : MonoBehaviour
{
    private const float DefaultuiAnimationSpeed = 6;

    [SerializeField] private string _uiAnimationSpeedKey;

    public static float UIAnimationSpeed { get; private set; }
    public Action OnInit { get; set; }

    public struct userAttributes { }
    public struct appAttributes { }

    public async UniTask Run()
    {
        try
        {
            await UnityServices.InitializeAsync().AsUniTask();

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                try
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync().AsUniTask();
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Auth failed (offline?): {e.Message}");
                }
            }
            EndUserConsent.SetConsentState(new ConsentState { AnalyticsIntent = ConsentStatus.Granted, AdsIntent = ConsentStatus.Denied });
         
            RemoteConfigService.Instance.FetchCompleted += OnConfigsFetched;
            RemoteConfigService.Instance.FetchConfigs(new userAttributes(), new appAttributes());
        }
        catch (Exception e)
        {
            Debug.LogError($"Init failed: {e.Message}");
            ApplyDefault();
        }
    }

    private void OnConfigsFetched(ConfigResponse response)
    {
        UIAnimationSpeed = RemoteConfigService.Instance.appConfig.GetFloat(_uiAnimationSpeedKey, DefaultuiAnimationSpeed);

        if (response.status == ConfigRequestStatus.Success)
            Debug.Log($"Success (origin: {response.requestOrigin}): {UIAnimationSpeed}");
        else
            Debug.Log($"Failed/Cached (origin: {response.requestOrigin}): {UIAnimationSpeed}");

        OnInit?.Invoke();
    }

    private void ApplyDefault()
    {
        UIAnimationSpeed = RemoteConfigService.Instance.appConfig.GetFloat(_uiAnimationSpeedKey, DefaultuiAnimationSpeed);
        Debug.Log($"Using pure default: {UIAnimationSpeed}");
        OnInit?.Invoke();
    }
}