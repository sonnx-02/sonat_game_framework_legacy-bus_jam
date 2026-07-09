#define dummy
//#define use_playfab


using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
#if !(dummy || global_dummy && !use_playfab)
using PlayFab;
using PlayFab.ClientModels;

#endif

public class PopupLeaderBoard : BasePopup
{
    [SerializeField] private PoolCollection<LeaderBoardItemView> leaderBoard;
    [SerializeField] private LeaderBoardItemView[] viewFirsts;
    [SerializeField] private LeaderBoardItemView player;
    [SerializeField] private IndexBindingScriptBase[] playerRank;
    public ToggleScript[] onNotLogin;
    [SerializeField] private ToggleScript[] haveData;

    private PlayFabManager2 PlayFabManager2 => Kernel.Resolve<PlayFabManager2>();

    protected override void Register()
    {
        base.Register();
        leaderBoard.Init(viewFirsts,  0);
    }

    private void UpdateStatus()
    {
        onNotLogin.OnChanged(PlayFabManager2.Logged);
        if (PlayFabManager2 == null)
            return;
        if (player == null)
            return;
        if (PlayFabManager2.Logged)
            if (PlayFabManager2.PlayerProfile != null && player.profileName != null)
                player.profileName.OnChanged(PlayFabManager2.GetPlayerDisplayName()); //.AccountInfo.Username;

        playerRank.OnChanged(PlayFabManager2.Rank);
        PlayFabManager2.LoadPlayerRank(data => playerRank.OnChanged(PlayFabManager2.Rank));

//        if (PlayFabManager.avatar == null)
//            player.avatar.sprite = Kernel.GetDatabase<GraphicAssetDatabase>().avatarSprites[0];
//        else
//            player.avatar.sprite= PlayFabManager.avatar;

        player.score.OnChanged(PlayerData.bestScore.Value);
    }

#if !(dummy || global_dummy && !use_playfab)

    PlayerPrefInt _showTimes = new PlayerPrefInt(nameof(PopupLeaderBoard).ToLower() + "_show_time");
    
    public override void OnShowHandler(WhenPopupClose whenPopupClosed)
    {
        base.OnShowHandler(whenPopupClosed);
        CheckRegister();
        leaderBoard.pools[0].ReturnAll();

        Debug.Log("Kernel.IsInternetConnection()"+Kernel.IsInternetConnection());
        Debug.Log("PlayFabManager2.Logged"+PlayFabManager2.Logged);
        //if (PlayFabManager2.Logged && Kernel.IsInternetConnection())
        if (Kernel.IsInternetConnection())
        {
            haveData.OnChanged(true);
            var request = new GetLeaderboardRequest
            {
                StatisticName = PlayFabManager2.StatisticKeyScore,
                StartPosition = 0,
                MaxResultsCount = 10,
                ProfileConstraints = new PlayerProfileViewConstraints()
                {
                    ShowAvatarUrl = true,
                    ShowDisplayName = true,
                }
            };

            Debug.Log("loading");
            PlayFabClientAPI.GetLeaderboard(request, OnLeaderBoardResult, OnError);
            UpdateStatus();
            _showTimes.Value++;
            Kernel.Resolve<FireBaseController>().LogEvent("show_" + GetType().Name.ToLower(),
                new LogParameter("times", _showTimes.Value));
        }
        else
        {
            haveData.OnChanged(false);
        }
    }

    private void OnLeaderBoardResult(GetLeaderboardResult obj)
    {
        var order = obj.Leaderboard.ToList().OrderBy(x => x.Position).ToList();
        foreach (var data in order)
        {
            var rent = leaderBoard.pools[0].Rent();
            rent.SetData(data.Profile.AvatarUrl, data.Position, data.DisplayName, data.StatValue);
        }
    }

    private void OnError(PlayFabError obj)
    {
        Debug.LogError("err while logging account!");
        Debug.Log(obj.GenerateErrorReport());
    }
#endif
}