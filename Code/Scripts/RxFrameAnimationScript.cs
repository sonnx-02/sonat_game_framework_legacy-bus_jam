using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[System.Serializable]
public class RxFrameData
{
    public string key;
    public int next;
    public int times = 1;
    public float interval;
    public Sprite[] sprites;
    public IntInt[] skip;
}

public class RxFrameAnimationScript : PlayAnimationScript
{
    [SerializeField] private SpriteRenderer targetRenderer;
    [SerializeField] private Image target2;
    [SerializeField] private int startIdleOnEnable = -1;
    [SerializeField] private RxFrameData[] data;


    [SerializeField] private PingPong pingPong;

    [MyButtonInt(nameof(PlayPingPongOnce))] [SerializeField]
    private int testPingPong;

    [MyButtonInt(nameof(Stop), nameof(StartIdle))] [SerializeField]
    private int testrx1;

    [MyButtonInt(nameof(PlayOnce), nameof(PlayReverseOnce))] [SerializeField]
    private int testrx2;

    
    
    [MyButtonInt(nameof(Cut))] [SerializeField]
    private int testrx3;

    private void Cut(int cut)
    {
        
    }


    [Serializable]
    public struct PingPong
    {
        public float delayAfterPing;
        public ToggleScript[] actionAfterPing;
        public bool deactiveWhenPingPong;
        public float delayAfterPong;
    }

    private IDisposable _anim;

    void OnEnable()
    {
        if (startIdleOnEnable >= 0)
        {
            StartIdle(startIdleOnEnable);
        }
    }

    private void SetSprite(Sprite sprite)
    {
        if (targetRenderer != null)
            targetRenderer.sprite = sprite;
        if (target2 != null)
            target2.sprite = sprite;
    }

    public void Stop()
    {
        _anim?.Dispose();
    }


    public void PlayPingPongOnce(int index)
    {
        _anim?.Dispose();
        _anim = Observable.FromCoroutine(() => CoroutinePlayPingPongOnce(index))
            .Subscribe();
    }
    
    public void PlayPingOnce(int index)
    {
        _anim?.Dispose();
        _anim = Observable.FromCoroutine(() => CoroutinePlayPingPongOnce(index,true))
            .Subscribe();
    }


    private IEnumerator CoroutinePlayPingPongOnce(int index,bool notPong = false)
    {
        if (pingPong.deactiveWhenPingPong)
            gameObject.SetActive(true);
        var waitFor = new WaitForSeconds(data[index].interval);
        for (var i = 0; i < data[index].sprites.Length; i++)
        {
            var sprite = data[index].sprites[i];
            SetSprite(sprite);
            yield return waitFor;
        }

        pingPong.actionAfterPing.OnChanged(true);
        yield return new WaitForSeconds(pingPong.delayAfterPing);


        if (!notPong)
        {
            for (var i = 0; i < data[index].sprites.Length; i++)
            {
                var sprite = data[index].sprites[data[index].sprites.Length - 1 - i];
                SetSprite(sprite);
                yield return waitFor;
            }

            yield return new WaitForSeconds(pingPong.delayAfterPong);
        }

        if (pingPong.deactiveWhenPingPong)
            gameObject.SetActive(false);
    }


    public void StartIdle(int index)
    {
        _anim?.Dispose();

        int i = 0;
//        var count = data[index].skip.Any(x => x.value == i)
//            ? data[index].skip.Find(x => x.value == i).value2
//            : 0;
        _anim = Observable.Interval(TimeSpan.FromSeconds(data[index].interval))
            .TakeUntilDisable(this)
            .Subscribe(data1 =>
            {
//                if (count > 0)
//                {
//                    count--;
//                }
//                else
//                {
                    var sprite = data[index].sprites[i % data[index].sprites.Length];
                    if(sprite != null)
                        SetSprite(sprite);
                    i++;
//                    count = data[index].skip.Any(x => x.value == i)
//                        ? data[index].skip.Find(x => x.value == i).value2
//                        : 0;
//                }
            });
    }


    public void PlayOnce(int index)
    {
        _anim?.Dispose();

        int i = 0;
        _anim = Observable.Interval(TimeSpan.FromSeconds(data[index].interval))
            .Take(data[index].sprites.Length)
            .Subscribe(data1 =>
            {
                var sprite = data[index].sprites[i % data[index].sprites.Length];
                SetSprite(sprite);
                i++;
            });
    }

    public void PlayReverseOnce(int index)
    {
        _anim?.Dispose();

        int i = 0;
        _anim = Observable.Interval(TimeSpan.FromSeconds(data[index].interval))
            .Take(data[index].sprites.Length)
            .Subscribe(data1 =>
            {
                var sprite = data[index].sprites[data[index].sprites.Length - 1 - (i % data[index].sprites.Length)];
                SetSprite(sprite);
                i++;
            });
    }


    public void PlayOnceThenIdle(int start, int idle, int times = 1)
    {
        _anim?.Dispose();
        _anim = Observable.FromCoroutine(() => PlayOnceEnumerator(start, times, idle))
            .TakeUntilDisable(this)
            .Subscribe();
    }

    private IEnumerator PlayOnceEnumerator(int start, int idle, int times = 1)
    {
        var waitFor = new WaitForSeconds(data[start].interval);

        for (int j = 0; j < times; j++)
        {
            for (int i = 0; i < data[start].times; i++)
            {
                var sprite = data[start].sprites[i % data[start].sprites.Length];
                SetSprite(sprite);
                yield return waitFor;
            }
        }

        waitFor = new WaitForSeconds(data[idle].interval);
        int s = 0;
        while (true)
        {
            var sprite = data[idle].sprites[s % data[idle].sprites.Length];
            SetSprite(sprite);
            yield return waitFor;
            s++;
        }
    }
}