#define dummy
//#define use_playfab


using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


[System.Serializable]
public class LeaderBoardData
{
    public int avatarIndex = -1;
    public string name;
    public int score;
    public int rank;
    public bool isPlayer;
}

public class PopupLeaderBoard_FakeDataGenerator : BaseFrameworkView
{
    [SerializeField] private string[] names1;
    [SerializeField] private string[] names2;

    [SerializeField] private List<LeaderBoardData> leaderBoard;

    public override void LogCustom()
    {
        base.LogCustom();
        leaderBoard = new List<LeaderBoardData>();
        int score = 0;
        for (int i = 0; i < 100; i++)
        {
            score += Random.Range(1, 100);
            leaderBoard.Add(new LeaderBoardData()
            {
                score = score,
                name = names1.TakeRandom() + names2.TakeRandom(),
                avatarIndex = Random.Range(0, 10),
                isPlayer = true
            });
        }
    }

    private PlayerPrefInt _avatar = new PlayerPrefInt("avatar");

    public List<LeaderBoardData> GetData(int take,out int playerIndex)
    {
        var data = leaderBoard.ToList();
        playerIndex = 0;
        data.Add(new LeaderBoardData()
        {
            name = "Player",
            score = PlayerData.bestScore.Value,
            avatarIndex = _avatar.Value,
        });

        data = data.OrderByDescending(x => x.score).ToList();
        for (var i = 0; i < data.Count; i++)
        {
            data[i].rank = i;
            if (data[i].isPlayer)
                playerIndex = i;
        }

        return data.Skip(Mathf.Max(data.Count-take, playerIndex - take / 2)).Take(take).ToList();
    }
}