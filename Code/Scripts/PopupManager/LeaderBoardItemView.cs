using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LeaderBoardItemView : PoolItem
{
    [SerializeField] private IndexBindingScriptBase[] rank;
    [SerializeField] public BindingToStringTextChange profileName;
    [SerializeField] public IndexBindingScriptBase score;
    [SerializeField] private IndexBindingScript avatarBindings;
    [SerializeField] private ToggleScript[] isCurrent;

    private string _url = "";
    private Sprite _sprite;
    public void SetData(string imageUrl, int rankIndex, string playerName, int playerScore)
    {
        if (imageUrl != _url)
        {
            _url = imageUrl;
        }
        UpdateAvatar();
        
        isCurrent.OnChanged(rankIndex == Kernel.Resolve<PlayFabManager2>().Rank && playerName == Kernel.Resolve<PlayFabManager2>().GetPlayerDisplayName());

        rank.OnChanged(rankIndex);
        if(profileName != null)
            profileName.OnChanged(playerName);
        score.OnChanged(playerScore);
    }

    private int _randomAvt;
    protected override void Register()
    {
        base.Register();
        if (avatarBindings != null) 
            _randomAvt = UnityEngine.Random.Range(0,avatarBindings.Size);
    }

    public void UpdateAvatar()
    {
        CheckRegister();

       
        if (string.IsNullOrEmpty(_url))
        {
            
        }
        else
        {
            try
            {
                _randomAvt = int.Parse(_url);
            }
            catch (Exception)
            {
                // ignored
            }

        }
        
        avatarBindings.OnChanged(_randomAvt);
//        
//        if (!string.IsNullOrEmpty(_url))
//            GetWwwTexture2D(_url).Subscribe(tex =>
//            {
//                if (tex != null)
//                {
//                    Debug.Log("success get avatar");
//                    _sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f),
//                        100.0f);
//                    
//                    if (_sprite == null)
//                        avatar.sprite = Kernel.GetDatabase<GraphicAssetDatabase>().avatarSprites[0];
//                    else
//                        avatar.sprite = _sprite;
//                    
//                }
//            });
    }
    
//    private static IObservable<Texture2D> GetWwwTexture2D(string url)
//    {
//        return Observable.FromCoroutine<Texture2D>((observer, cancellationToken) =>
//            GetWwwCore(url, observer, cancellationToken));
//    }
//
//    private static IEnumerator GetWwwCore(string url, IObserver<Texture2D> observer,
//        CancellationToken cancellationToken)
//    {
//        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
//        www.SendWebRequest();
//
//        while (!www.isDone && !cancellationToken.IsCancellationRequested)
//            yield return null;
//
//        if (cancellationToken.IsCancellationRequested) yield break;
//
//        if (www.error != null)
//        {
//            observer.OnError(new Exception(www.error));
//        }
//        else
//        {
//            var texture = ((DownloadHandlerTexture) www.downloadHandler).texture;
//            observer.OnNext(texture);
//            observer.OnCompleted(); // IObserver needs OnCompleted after OnNext!
//        }
//    }

    
}
