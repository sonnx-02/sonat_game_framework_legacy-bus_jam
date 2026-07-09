using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

#pragma warning disable 649


public enum MusicEnum
{
    None = 0,
    MainMenu = 1,

    GamePlay0 = 10,
    GamePlay1 = 11,
    GamePlay2 = 12,
    GamePlay3 = 13,
    GamePlay4 = 14,
    GamePlay5 = 15,
    
    GamePlayBoss0 = 20,
    GamePlayBoss1 = 21,
    GamePlayBoss2 = 22,
    GamePlayBoss3 = 23,
    GamePlayBoss4 = 24,
    GamePlayBoss5 = 25,
}

[System.Serializable]
public struct SoundItem
{
    [IndexAsEnum(BuiltInEnumType.SoundEnum)]
    public int key;

    public AudioClip audioClip;
    [Range(0, 1)] public float volume;
}

[System.Serializable]
public struct MusicItem
{
    [IndexAsEnum(BuiltInEnumType.MusicEnum)]
    public int key;

    public AudioClip audioClip;
    [Range(0, 1)] public float volume;
}


public class AudioManager : BaseFrameworkView
{
#if UNITY_EDITOR
    [SerializeField] [IndexAsEnum(BuiltInEnumType.SoundEnum)]
    private int testSound;

    [MyButton(nameof(PlayTestSound))] [SerializeField]
    private int testPlaySound;

    public void PlayTestSound()
    {
        Play(testSound);
    }

#endif

    private static IntReactiveProperty MusicEnableProperty => PlayerData.isMusicEnable;
    private static IntReactiveProperty SoundEnableProperty => PlayerData.isSoundEnable;

    [SerializeField] private FloatReactiveProperty volumeProperty;

    // for custom editor
    [SerializeField] protected List<SoundItem> soundDictionary;
    [SerializeField] protected List<MusicItem> musicDictionary;

    private AudioSource[] _soundAudioSourcePools;

    [SerializeField] private AudioSource oneShotAudioSourceSound;
    [SerializeField] private AudioSource audioSourceMusic;
    [SerializeField] private int music;
    private static bool _play;


    protected override void Register()
    {
        base.Register();
        audioSourceMusic.loop = true;
        _soundAudioSourcePools = new AudioSource[10];
        for (var i = 0; i < _soundAudioSourcePools.Length; i++)
            _soundAudioSourcePools[i] = gameObject.AddComponent<AudioSource>();

#if UNITY_EDITOR
        foreach (var soundItem in soundDictionary)
        {
            if (soundDictionary.Count(x => (int) x.key == (int) soundItem.key) > 1)
            {
                Debug.LogError("duplicate sound enum" + soundItem.key);
                Debug.Break();
            }
        }
#endif
    }

    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        PlayCurrentMusic();
        PlayerData.isMusicEnable.Subscribe(data =>
        {
            if (PlayerData.isMusicEnable.BoolValue)
            {
                ResumeMusic();
            }
            else
            {
                StopMusic();
            }
        });
    }

    public int PlayPool(int soundEnum, bool loop = true)
    {
        if (soundEnum != 0)
            if (SoundEnableProperty.BoolValue)
                for (var i = 0; i < _soundAudioSourcePools.Length; i++)
                {
                    var source = _soundAudioSourcePools[i];
                    if (source != null && !source.isPlaying)
                    {
                        foreach (var item in soundDictionary)
                            if (item.key == soundEnum)
                            {
                                source.loop = loop;
                                source.clip = item.audioClip;
                                source.volume = item.volume;
                                source.Play();
                                
                                return i;
                            }
                    }
                }

        return -1;
    }

    public void StopPool(int index)
    {
        if (index >= 0)
            _soundAudioSourcePools[index].Stop();
    }

    public void StopAllPool()
    {
        foreach (var soundAudioSourcePool in _soundAudioSourcePools)
            soundAudioSourcePool.Stop();
    }

    public List<int> ignoreSounds = new List<int>(); // like do not trigger sound when win

    public void ClearIgnoreSounds() => ignoreSounds.Clear();
    public void Play(int soundKey)
    {
        if (soundKey != 0)
            if (SoundEnableProperty.BoolValue)
                foreach (var item in soundDictionary)
                    if (item.key == soundKey)
                    {
                        
                        if (item.audioClip != null)
                        {
                            if(ignoreSounds.Count == 0 || ignoreSounds.Count(x=> x == soundKey) == 0)
                                oneShotAudioSourceSound.PlayOneShot(item.audioClip, item.volume);
#if UNITY_EDITOR
                            UIDebugLog.Log0("PlaySound " + BuiltInEnumType.SoundEnum.GetName(soundKey));
#endif
                        }
                        break;
                    }
    }

    public void Play(int soundKey, float delay)
    {
        if (soundKey != 0 && SoundEnableProperty.BoolValue)
        {
            if (delay > 0)
                delay.Timer(() => Play(soundKey));
            else
                Play(soundKey);
        }
    }

    public void Play(AudioClip clip, float volume = 1)
    {
        if (clip != null && SoundEnableProperty.BoolValue)
            oneShotAudioSourceSound.PlayOneShot(clip, volume);
    }

    public void SetVolume(float volume)
    {
        audioSourceMusic.volume = volume;
    }

    public void StopResumeMusic(bool stop)
    {
        if(stop)
            StopMusic();
        else
            ResumeMusic();
    }

    public void PlayMusic(int musicEnum = (int) MusicEnum.MainMenu)
    {
        if (musicEnum == (int) MusicEnum.None)
        {
            music = musicEnum;
            StopMusic();
        }
        else
        {
            if (music != musicEnum)
            {
                music = musicEnum;
                foreach (var item in musicDictionary)
                    if (item.key == musicEnum)
                    {
                        audioSourceMusic.clip = item.audioClip;
                        audioSourceMusic.volume = item.volume;
                        break;
                    }

                if (MusicEnableProperty.BoolValue)
                {
                    _play = true;
                    audioSourceMusic.Play();
                }
            }
            else
            {
                if (MusicEnableProperty.BoolValue && !_play)
                {
                    _play = true;
                    audioSourceMusic.Play();
                }
            }
        }
    }

    public void PlayCurrentMusic()
    {
        foreach (var item in musicDictionary)
            if (item.key == music)
            {
                audioSourceMusic.clip = item.audioClip;
                audioSourceMusic.volume = item.volume;
                break;
            }

        if (!MusicEnableProperty.BoolValue) return;

        _play = true;
        audioSourceMusic.Play();
    }

    public void StopMusic()
    {
        _play = false;
        if (audioSourceMusic != null)
            audioSourceMusic.Stop();
    }

    public void StopAllSound()
    {
        _play = false;
        foreach (var instanceSoundAudioSourcePool in _soundAudioSourcePools)
        {
            instanceSoundAudioSourcePool.Stop();
        }
    }

    public void ResumeMusic()
    {
        if (audioSourceMusic != null && !_play)
            audioSourceMusic.Play();
    }

    public bool IsBossMusic()
    {
        return music >= 20 && music < 30;
    }
}