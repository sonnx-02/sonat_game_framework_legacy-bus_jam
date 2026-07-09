using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;


public static class ShortHand
{
    private static FloatSerializableDictionary limitSoundDict = new FloatSerializableDictionary();
    public static void PlaySoundLimit(this int sound,float limit)
    {
        if (limitSoundDict[sound] - Time.time  < 0)
        {
            PlaySound(sound);
            limitSoundDict[sound] = Time.time + limit;
        }
    }
    
    public static void PlaySoundLimit(this int sound,float delay,float limit)
    {
        if (limitSoundDict[sound] - Time.time  < 0)
        {
            PlaySound(sound,delay);
            limitSoundDict[sound] = Time.time + limit;
        }
    }
    
    public static void LightFeedBack()
    {
        if (BaseFrameworkView.PlayerData.isVibrationEnable.BoolValue)
        {
#if UNITY_ANDROID
           // if (RDG.Vibration.HasVibrator())
        //        RDG.Vibration.Vibrate(100);
#endif
#if UNITY_IOS
            CandyCoded.HapticFeedback.HapticFeedback.LightFeedback();
#endif
        }
    }
    
    public static void PlaySound(this int sound)
    {
        if (RootView.rootView != null)
        {
            RootView.rootView.audioManager.Play(sound);
            RootView.rootView.gameController.OnPlaySound(sound);
        }
    }
    
    public static int PlaySoundPool(this int sound,bool loop)
    {
        if (RootView.rootView != null)
        {
            RootView.rootView.gameController.OnPlaySound(sound);
            return RootView.rootView.audioManager.PlayPool(sound, loop);
        }

        return -1;
    }
    
    public static void PlayMusic(this int music)
    {
        if(RootView.rootView != null)
            RootView.rootView.audioManager.PlayMusic(music);
    }

    
    public static void PlaySound(this int sound,float delay)
    {
        if(RootView.rootView != null)
            RootView.rootView.audioManager.Play(sound,delay);
    }
        
    public static IntReactiveProperty GetPlayerDataCustomProperty(this int key)
    {
        return BaseFrameworkView.PlayerData.GetCustomProperty(key);
    }
    
    public static IntReactiveProperty GetSessionDataCustomProperty(this int key)
    {
        return BaseFrameworkView.SessionData.GetCustomProperty(key);
    }
  
    public static IntReactiveProperty GetDailyDataCustomProperty(this int key)
    {
        return BaseFrameworkView.DailyData.GetCustomProperty(key);
    }
}