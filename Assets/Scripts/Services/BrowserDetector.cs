using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Assets.Scripts.Services
{
    public class BrowserDetector : MonoBehaviour
    {
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern IntPtr GetBrowserName();

    [DllImport("__Internal")]
    private static extern void FreeMemory(IntPtr ptr);
#endif

        private static string GetBrowser()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
        IntPtr ptr = GetBrowserName();
        
        try
        {
            string browser = Marshal.PtrToStringAnsi(ptr);
            return browser;
        }
        finally
        {
            FreeMemory(ptr);
        }
#else
            return "Editor";
#endif
        }
        private static string _cacheName = string.Empty;
        public static bool IsSafariBrowser()
        {
            if (string.IsNullOrEmpty(_cacheName))
            {
                
                _cacheName = GetBrowser();
            }
            
            return _cacheName == "Safari";
        }
    }
}