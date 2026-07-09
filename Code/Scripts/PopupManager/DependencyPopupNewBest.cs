using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class DependencyPopupNewBest : BasePopupDependency
{
//    [SerializeField] private ScrollRectLevelSelectInfinitive scrollRect;
//    [SerializeField] private ProfileData[] profiles;
//    [SerializeField] private ProfileRxScrollItem playerProfile;
//    [SerializeField] private Vector2Int fromTo;
//
//    private int From => fromTo.x;
//    private int To => fromTo.y;
//
//    [SerializeField] List<ProfileData> clone;
//    [SerializeField] List<ProfileData> aroundPeople;
//
//    public override void OnShow()
//    {
//        CheckRegister();
//        var service = Kernel.Resolve<PlayFabManager2>();
//        clone = profiles.Select(x => x.Clone()).ToList();
//        aroundPeople = Kernel.Resolve<PlayFabManager2>().AroundPeople.ToList();
//        int playerIndex = -1;
//        
//        if (aroundPeople != null)
//        {
//            aroundPeople.Reverse();
//            for (var i = 0; i < aroundPeople.Count; i++)
//                if (aroundPeople[i].rank == service.Rank)
//                {
//                    playerIndex = i;
//                    break;
//                }
//
//            if (playerIndex >= 0)
//            {
//                for (var i = 0; i < clone.Count; i++)
//                {
//                    clone[i].rank = service.Rank - (i - To);
//                    if (clone[i].rank < 0)
//                        clone[i].state = 1;
//
//                    if (playerIndex - To + i >= 0 && playerIndex - To + i < aroundPeople.Count)
//                    {
//                        var correspond = aroundPeople[playerIndex - To + i];
//                        clone[i].avatar = correspond.avatar;
//                        clone[i].score = correspond.score;
//                        clone[i].name = correspond.name;
//                    }
//                }
//            }
//        }
//
//        for (var index = 0; index < clone.Count; index++)
//        {
//            var profileData = clone[index];
//        }
//
//        clone[To] = new ProfileData
//        {
//            avatar = service.AvatarInGame,
//            name = service.GetPlayerDisplayName(),
//            rank = service.Rank,
//            score = PlayerData.bestScore.Value,
////            score = WaveData.currentScore.Value,
//            state = 1,
//        };
//        
//        for (var i = To; i < clone.Count; i++)
//        {
//            if (clone[i].score < clone[i - 1].score && clone[i].score < PlayerData.bestScore.Value)
//                clone[i].score = clone[i - 1].score + UnityEngine.Random.Range(1, 10);
//        }
//
//        for (var i = 0; i < clone.Count; i++)
//            if (clone[i].state != 0)
//            {
//                scrollRect.IndicateIndex(i);
//                break;
//            }
//        
//        scrollRect.InvokeItemRentAll();
//        var player = clone[To].Clone();
//        player.state = 0;
//        playerProfile.SetData(player);
//    }
//
//    protected override void Register()
//    {
//        base.Register();
//        scrollRect.CheckRegister();
//        scrollRect.OnItemRent += HandlerItemChanged;
//    }
//
//    private void HandlerItemChanged(RxScrollItemBase scrollItem, int index)
//    {
//        var cast = scrollItem as ProfileRxScrollItem;
//        if (index < clone.Count)
//            if (cast != null)
//                cast.SetData(clone[index]);
//    }
    public override void OnShow()
    {
        base.OnShow(); 
    }
}