using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CoreApplication
{
    public static bool IsAndroid { get; private set; }
    public static bool IsIPhone { get; private set; }
    public static bool IsMobileAndroid { get; private set; }
    public static bool IsMobileIPhone { get; private set; }
    public static bool IsMobile { get; private set; }

    public static int GetOSVersion() 
    {
        string osVersion = SystemInfo.operatingSystem;
        Debug.Log("SystemInfo.operationgSystem : " + osVersion);
        if(IsMobileIPhone)
        {
            osVersion = osVersion.Replace("iPhone OS", "");
            return Mathf.FloorToInt(float.Parse(osVersion.Substring(0, 1)));
        }
        return 0;
    }
    public static bool IsIphoneOS9()
    {
        if (GetOSVersion() >= 9 && IsMobileIPhone) return true;
        return false;
    }

    // Static initialize
    static CoreApplication() 
    {
#if UNITY_ANDROID
        IsAndroid = true;
#else
        IsAndroid = false;
#endif

#if UNITY_IOS
        IsIPhone = true;
#else
        IsIPhone = false;
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
        IsMobileAndroid = true;
#else
        IsMobileAndroid = false;
#endif

#if UNITY_IOS && !UNITY_EDITOR
        IsMobileIPhone = true;
#else
        IsMobileIPhone = false;
#endif

#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
        IsMobile = true;
#else
        IsMobile = false;
#endif
    }
    public static string GetPlatformName() 
    {
        if (IsAndroid) return @"android";
        if (IsIPhone) return @"ios";
        return @"";
    }
    public static string GetOSStoreName() 
    {
        if (IsMobileAndroid) return "G";
        if (IsMobileIPhone) return "A";
        if (!IsMobile) return "W";

        return "G";
    }

    public static LANGUAGE_TYPE language;
}
