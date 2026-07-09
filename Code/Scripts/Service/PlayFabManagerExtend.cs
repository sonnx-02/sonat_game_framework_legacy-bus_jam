#define dummy
//#define use_playfab

using System;
using System.Linq;
#if (dummy || global_dummy) && !use_playfab
public partial class PlayFabManager2
{
    public Action<string> onNameChanged;
    public Action<int> onAvatarChanged;
    public Action<int> onBestScoreChanged;
    public Action<int> onRankChanged;


    public void LoadPlayerRankAround(int i)
    {
        
    }
}
#else
using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public partial class PlayFabManager2
{
    public ProfileData[] AroundPeople { get; set; }

    public string DeviceId => SystemInfo.deviceUniqueIdentifier;

    public int Rank
    {
        get => PlayerPrefs.GetInt(nameof(Rank));
        set => PlayerPrefs.SetInt(nameof(Rank), value);
    }

    public int GlobalBestScore
    {
        get => PlayerPrefs.GetInt(nameof(GlobalBestScore));
        private set => PlayerPrefs.SetInt(nameof(GlobalBestScore), value);
    }

    public int AvatarInGame
    {
        get
        {
            var avt = PlayerPrefs.GetInt(nameof(AvatarInGame));
            if (avt >= totalAvatar)
                avt = totalAvatar - 1;
            return avt;
        }
        set => PlayerPrefs.SetInt(nameof(AvatarInGame), value);
    }


    public const string StatisticKeyScore = "BestScore";
    public bool Logged { get; private set; }

    private GetLeaderboardResult LeaderboardResult { get; set; }

    public void UpdateAvatar(string avt)
    {
        PlayFabClientAPI.UpdateAvatarUrl(new UpdateAvatarUrlRequest()
            {
                ImageUrl = avt
            },
            onSuccess =>
            {
                Debug.Log("update avatar success");
                UpdateAvatar();
            },
            onFailed => { Debug.LogError(onFailed.Error); });
        ;
    }

    public void UpdateAvatar()
    {
        if (!string.IsNullOrEmpty(PlayerProfile.AvatarUrl))
        {
            Debug.Log("update Avatar");
        }
    }

    private readonly string[] _names =
    {
        "Mangoes",
        "Banana",
        "Tomatoes",
        "Strawberry",
        "Chocolate",
        "Candy",
        "IceCream",
        "Peach",
        "Apple",
        "Lemon",
        "Cherry",
        "Orange",
        "Grape",
        "Kiwi",
        "StarFruit",
        "Custard",
        "Latte",
        "MilkTea",
        "Bread",
        "Taco",
    };

    private PlayerPrefString local_player_name;// = new PlayerPrefString(nameof(local_player_name),"YOU");

    public string GetPlayerDisplayName()
    {
        if (PlayerProfile == null)
            return local_player_name.Value;
        return PlayerProfile.DisplayName;
    }


    public void UpdateBestScore(int value, bool updateServer)
    {
        Debug.Log("UpdateBestScore" + value);
        BaseFrameworkView.PlayerData.bestScore.Value = value;
        BaseFrameworkView.PlayerData.Save();
        if (onBestScoreChanged != null)
            onBestScoreChanged.Invoke(value);

        if (!Logged) return;

        if (updateServer)
            PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest()
                {
                    Statistics = new List<StatisticUpdate>
                    {
                        new StatisticUpdate() {StatisticName = StatisticKeyScore, Value = value}
                    }
                },
                result =>
                {
                    UIDebugLog.Log("Score updated successfully : " + value);
                    LoadPlayerRank(null);
                    LoadGlobalBestScore(null);


                    // test server score updated 
                    GetPlayerStatisticsRequest request = new GetPlayerStatisticsRequest()
                    {
                        StatisticNames = new List<string>()
                        {
                            StatisticKeyScore
                        }
                    };
                    PlayFabClientAPI.GetPlayerStatistics(request, data =>
                    {
                        Debug.Log("re-check playfab score is "+data.Statistics[0].Value);
                    }, OnError);
                }, OnError);
    }


    public void UpdateDisplayName(string displayName)
    {
        if (!Logged) return;

        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
            {
                DisplayName = displayName
            }, result =>
            {
                local_player_name.Value = displayName;
                onNameChanged?.Invoke(displayName);
                if(PlayerProfile != null)
                    PlayerProfile.DisplayName = result.DisplayName;
                UIDebugLog.Log("The player's display name is now: " + result.DisplayName);
                SubjectController.CustomUpdatedEvent.OnNext(CustomUpdateEvent.PlayerProfile);
                //          SubjectController.CustomUpdatedEvent.OnNext(CustomUpdateEvent.PlayerProfile);
            },
            error =>
            {
                Debug.LogError(error.GenerateErrorReport());
                SubjectController.GlobalError.OnNext((int)BaseGlobalError.PlayFabNameTaken);
            });
    }


    public void LoadGlobalBestScore(Action<int> loaded)
    {
        if (!Logged) return;

        var request = new GetLeaderboardRequest
        {
            StatisticName = StatisticKeyScore,
            StartPosition = 0,
            MaxResultsCount = 1
        };

        PlayFabClientAPI.GetLeaderboard(request, data =>
        {
            if (data.Leaderboard.Count > 0)
                GlobalBestScore = data.Leaderboard[0].StatValue;

            loaded?.Invoke(GlobalBestScore);
        }, OnError);
    }

    public void LoadPlayerRank(Action<int> loaded)
    {
        if (!Logged) return;

//        if (AccountInfo == null)
//            return;
        var request = new GetLeaderboardAroundPlayerRequest()
        {
            StatisticName = StatisticKeyScore,
            // PlayFabId = AccountInfo.AccountInfo.PlayFabId,  
            MaxResultsCount = 1, // This is my Playfab ID
        };

        PlayFabClientAPI.GetLeaderboardAroundPlayer(request, data =>
        {
            if (data.Leaderboard.Count > 0)
            {
                Rank = data.Leaderboard[0].Position;
                loaded?.Invoke(Rank);

                if (onRankChanged != null)
                    onRankChanged.Invoke(Rank);
            }
        }, OnError);
    }

    public Action<string> onNameChanged;
    public Action<int> onAvatarChanged;
    public Action<int> onBestScoreChanged;
    public Action<int> onRankChanged;

    public void UpdateAvatarIndex(int value)
    {
        if (onAvatarChanged != null)
            onAvatarChanged.Invoke(value);
        AvatarInGame = value;

        if (!Logged) return;

        PlayFabClientAPI.UpdateAvatarUrl(new UpdateAvatarUrlRequest()
        {
            ImageUrl = value.ToString(),
        }, result =>
        {
            Debug.Log("update avt ok"+value);
        }, OnError);
    }

    public void LoadPlayerRankAround(int amount)
    {
        if (Logged && Kernel.IsInternetConnection())
        {
            var request = new GetLeaderboardAroundPlayerRequest()
            {
                StatisticName = StatisticKeyScore,
                MaxResultsCount = amount,
                ProfileConstraints = new PlayerProfileViewConstraints()
                {
                    ShowAvatarUrl = true,
                    ShowDisplayName = true,
                }
            };

            PlayFabClientAPI.GetLeaderboardAroundPlayer(request, OnLeaderBoardResult, OnError);
        }
    }

    private void OnLeaderBoardResult(GetLeaderboardAroundPlayerResult obj)
    {
        if (obj?.Leaderboard != null)
        {
            var order = obj.Leaderboard.ToList().OrderBy(x => x.Position).ToList();
            AroundPeople = new ProfileData[order.Count];
            for (var i = 0; i < order.Count; i++)
            {
                AroundPeople[i] = new ProfileData()
                {
                    name = order[i].DisplayName,
                    rank = order[i].Position,
                    score = order[i].StatValue,
                };

                if (!string.IsNullOrEmpty(order[i].Profile.AvatarUrl))
                {
                    try
                    {
                        AroundPeople[i].avatar = int.Parse(order[i].Profile.AvatarUrl);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
            }
        }
    }
}
#endif