using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;

public static class RxExtensions 
{
    public static IObservable<Texture2D> GetWwwTexture2D(string url)
    {
        return Observable.FromCoroutine<Texture2D>((observer, cancellationToken) =>
            GetWwwCore(url, observer, cancellationToken));
    }

    private static IEnumerator GetWwwCore(string url, IObserver<Texture2D> observer,
        CancellationToken cancellationToken)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        www.SendWebRequest();

        while (!www.isDone && !cancellationToken.IsCancellationRequested)
            yield return null;

        if (cancellationToken.IsCancellationRequested) yield break;

        if (www.error != null)
        {
            observer.OnError(new Exception(www.error));
        }
        else
        {
            var texture = ((DownloadHandlerTexture) www.downloadHandler).texture;
            observer.OnNext(texture);
            observer.OnCompleted(); // IObserver needs OnCompleted after OnNext!
        }
    }
}
