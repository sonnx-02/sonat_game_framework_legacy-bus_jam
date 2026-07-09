#define dummy
//#define use_playfab


using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;




public class PopupLeaderBoard_Fake : BasePopup
{
    [SerializeField] private PoolCollection<LeaderBoardItemView_Fake> leaderBoard;
    [SerializeField] private LeaderBoardItemView_Fake[] viewFirsts;
    [SerializeField] private LeaderBoardItemView_Fake player;
    [SerializeField] private IndexBindingScriptBase[] playerRank;
    
    private readonly PlayerPrefInt _showTimes = new PlayerPrefInt(nameof(PopupLeaderBoard_Fake).ToLower() + "_show_time");
    
    [SerializeField] private PopupLeaderBoard_FakeDataGenerator fakeDataGenerator;
    [SerializeField] private int take;

    protected override void Register()
    {
        base.Register();
        leaderBoard.Init(viewFirsts,  0);
    }

    private void UpdateStatus()
    {
        player.score.OnChanged(PlayerData.bestScore.Value);
    }

    public override void OnShowHandler(WhenPopupClose whenPopupClosed)
    {
        base.OnShowHandler(whenPopupClosed);
        Debug.Log("loading");
        UpdateStatus();
        _showTimes.Value++;
        var dataCollection = fakeDataGenerator.GetData(10,out int playerIndex);
        foreach (var data in dataCollection)
        {
            var rent = leaderBoard.pools[0].Rent();
            rent.SetData(data);
        }
    }
}