using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_IOS
   using CandyCoded.HapticFeedback;
#endif

public static class DuongVibration
{
    private static ReactivePlayerData PlayerData => BaseFrameworkView.PlayerData;

    public static void Vibration(long timeMilliseconds)
    {
        if (PlayerData.isVibrationEnable.BoolValue && timeMilliseconds > 0)
        {
#if UNITY_ANDROID
            if (RDG.Vibration.HasVibrator())
                RDG.Vibration.Vibrate(timeMilliseconds);
#endif
#if UNITY_IOS
            if (timeMilliseconds <= 150)
                CandyCoded.HapticFeedback.HapticFeedback.LightFeedback();
            else if (timeMilliseconds <= 200)
                CandyCoded.HapticFeedback.HapticFeedback.MediumFeedback();
            else
                CandyCoded.HapticFeedback.HapticFeedback.HeavyFeedback();
#endif
        }
    }
}