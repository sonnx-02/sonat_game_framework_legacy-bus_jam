#define dummy
//#define use_playfab


using System;
#if (dummy || global_dummy) && !use_playfab
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerProfileModel
{
    public string displayName;
    public int score;
    public int avatarIndex;
    public static PlayerProfileModel Load()
    {
        var model = JsonUtility.FromJson<PlayerProfileModel>(PlayerPrefs.GetString(nameof(PlayerProfileModel)));
        if(model == null)
            model = new PlayerProfileModel()
            {
                displayName = Names.TakeRandom() + SystemInfo.deviceUniqueIdentifier.Substring(0, 10),
            };
        return model;
    }

    public void Save()
    {
        PlayerPrefs.SetString(nameof(PlayerProfileModel), JsonUtility.ToJson(this));

    }
    
    private static readonly string[] Names =
    {
        "Mangoes",
        "Banana",
        "Tomatoes",
        "Strawberry",
        "Chocolate",
    };
}

public partial class PlayFabManager2 : BaseService
{
    [SerializeField] private FacebookController facebookController;
    public int BestScore { get; private set; }
    public const string Key = "BestScore";
    public Sprite avatar;
    public PlayerProfileModel PlayerProfile { get; private set; }
    public ProfileData[] AroundPeople { get; set; }
    [SerializeField] private int totalAvatar = 8;
    public int TotalAvatar => totalAvatar;
    
    protected override void Register()
    {
        base.Register();
        Initialized = true;
        PlayerProfile = PlayerProfileModel.Load();
    }

    public void SaveLocal()
    {
        PlayerProfile.Save();
    }

    public void SetBestScore(int parse)
    {
    }

    public void UpdateDisplayName(string displayName)
    {
        onNameChanged?.Invoke(displayName);
        PlayerProfile.displayName = displayName;
        PlayerProfile.Save();
        UIDebugLog.Log("The player's display name is now: " + displayName);
        SubjectController.CustomUpdatedEvent.OnNext(CustomUpdateEvent.PlayerProfile);
        PlayerProfile.displayName = displayName;
    }

    public bool Logged { get; set; }
    public int GlobalBestScore { get; set; }
    public int Rank { get; set; }
    public int AvatarInGame { get; set; }

 

    public void UpdateAvatar(string avt)
    {
    }

    public void LoadGlobalBestScore(Action<object> action)
    {
    }

    public void GetLeaderBoardRank(Action<object> action)
    {
    }

    public string GetPlayerDisplayName()
    {
        if (PlayerProfile == null)
            return string.Empty;
        return PlayerProfile.displayName;
    }


    public void UpdateBestScore(int bestScoreValue, bool b)
    {
    }

    public void LoadPlayerRank(Action<object> action)
    {
    }

    public void UpdateAvatarIndex(int index)
    {
        if (onAvatarChanged != null)
            onAvatarChanged.Invoke(index);
        AvatarInGame = index;
        PlayerProfile.avatarIndex = index;
    }
}
#else
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Internal;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;

public partial class PlayFabManager2 : BaseService
{
    [SerializeField] private FacebookController facebookController;
    public PlayerProfileModel PlayerProfile { get; private set; }
    public UserAccountInfo AccountInfo { get; private set; }

    bool IsThisDevice => AccountInfo.AndroidDeviceInfo != null &&
                         DeviceId == AccountInfo.AndroidDeviceInfo.AndroidDeviceId;

    private bool IsFacebookLogged => AccountInfo != null && AccountInfo.FacebookInfo != null;
    [SerializeField] private int totalAvatar = 8;
    public int TotalAvatar => totalAvatar;


    void Start()
    {
        local_player_name = new PlayerPrefString(nameof(local_player_name),"YOU");
        facebookController.loggedReadyCallback += data =>
        {
            if (string.IsNullOrEmpty(data))
                LoginAnonymous();
            else
                CheckClearOldDataAndLoginFacebook(data);
        };

        facebookController.onLoggingCallback += CheckClearOldDataAndLoginFacebook;
        facebookController.loggedOutCallback += LoggedOutFbAndLoginAnonymous;
        
        Observable.Interval(TimeSpan.FromSeconds(30))
            .TakeWhile(x => !Logged)
            .Subscribe(data =>
            {
                Debug.Log("duong : relog playfab");
                facebookController.CheckPlayFab();
            });

        10f.Timer(UpdateRandomName);
    }
    
    protected override void Register()
    {
        base.Register();
        Logged = false;
        Initialized = true;
    }

    private void LoggedOutFbAndLoginAnonymous()
    {
        CheckFirstTime = false;
        LoginAnonymous();
    }

    private void CheckClearOldDataAndLoginFacebook(string token)
    {
        var lastBest = BaseFrameworkView.PlayerData.bestScore.Value;
        if (Logged && !IsFacebookLogged)
        {
            BaseFrameworkView.PlayerData.bestScore.Value = 0;
            BaseFrameworkView.PlayerData.Save();
            Debug.Log("ClearScoreThenLoginFacebook");
            if (onBestScoreChanged != null)
                onBestScoreChanged.Invoke(0);
            PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest()
                {
                    Statistics = new List<StatisticUpdate>
                    {
                        new StatisticUpdate() {StatisticName = StatisticKeyScore, Value = 0}
                    }
                },
                result =>
                {
                    UIDebugLog.Log("Reset device score successfully : " + 0);
                    LoginWithFacebook(token, lastBest);
                }, OnError);
        }
        else
            LoginWithFacebook(token, lastBest);
    }

    private void LoginWithFacebook(string token, int lastBest)
    {
        PlayFabClientAPI.LoginWithFacebook(new LoginWithFacebookRequest()
        {
            AccessToken = token,
            CreateAccount = true,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
            {
                GetPlayerProfile = true,
                GetUserAccountInfo = true,
                GetPlayerStatistics = true,
                PlayerStatisticNames = new List<string>()
                {
                    StatisticKeyScore,
                },
                ProfileConstraints =new PlayerProfileViewConstraints()
                {
                    ShowAvatarUrl = true,
                    ShowDisplayName = true,
                }
            }
        }, result =>
        {  
            UIDebugLog.Log("Successfully logged facebook in a player with PlayFabId: " + result.PlayFabId,
                LogType.Service);
            AccountInfo = result.InfoResultPayload.AccountInfo;
            PlayerProfile = result.InfoResultPayload.PlayerProfile;
            
            int avtIndex = UnityEngine.Random.Range(0, totalAvatar);
            try
            {
                string avtUrl = result.InfoResultPayload.PlayerProfile.AvatarUrl;
                 avtIndex = (int.Parse(avtUrl));
            }
            catch
            {
                Debug.Log("cannot parse avt url");
            }
            
            if (result.NewlyCreated)
            {
                UIDebugLog.Log("result.NewlyCreated");
                UpdateDisplayName(AccountInfo.FacebookInfo.FullName);
            }
            else
            {
                UIDebugLog.Log("not result.NewlyCreated + " + PlayerProfile.DisplayName);
                onNameChanged?.Invoke(PlayerProfile.DisplayName);
            }
            
            UpdateAvatarIndex(avtIndex);
            onAvatarChanged?.Invoke(avtIndex);
            Logged = true;
            LoadPlayerRank(null);
            LoadGlobalBestScore(null);
            if (result.InfoResultPayload.PlayerStatistics.Count > 0)
            {
                var playFabScore = result.InfoResultPayload.PlayerStatistics[0].Value;
                UIDebugLog.Log("play fab best score" + playFabScore);
                if (result.InfoResultPayload.PlayerStatistics.Count > 0)
                    if (result.InfoResultPayload.PlayerStatistics[0].Value != lastBest)
                    {
                        var sync = Mathf.Max(playFabScore, lastBest);
                        UIDebugLog.Log($"playFabScore={playFabScore} / lastBest{lastBest}");
                        UpdateBestScore(sync, playFabScore < sync);
                        onBestScoreChanged?.Invoke(sync);
                    }
            }
            else
            {
                UpdateBestScore(lastBest, true);
            }
        }, error => Debug.LogError(error.GenerateErrorReport()));
    }


    private void LoginAnonymous()
    {
        PlayFabClientAPI.LoginWithAndroidDeviceID(new LoginWithAndroidDeviceIDRequest()
        {
            AndroidDeviceId = DeviceId,
            CreateAccount = true,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
            {
                GetPlayerProfile = true,
                GetUserAccountInfo = true,
                GetPlayerStatistics = true,
                PlayerStatisticNames = new List<string>()
                {
                    StatisticKeyScore,
                }
            }
        }, result =>
        {
            UIDebugLog.Log("Successfully logged in a player with PlayFabId: " + result.PlayFabId);
            Logged = true;
            AccountInfo = result.InfoResultPayload.AccountInfo;
            PlayerProfile = result.InfoResultPayload.PlayerProfile;
            if (result.NewlyCreated || !CheckFirstTime)
            {
                Debug.Log("duong FirstTimeLoggedAnonymous");
                FirstTimeLoggedAnonymous();
            }
            else
            {
                LoadPlayerRank(null);
                LoadGlobalBestScore(null);
                if (result.InfoResultPayload.PlayerStatistics.Count > 0)
                    if (result.InfoResultPayload.PlayerStatistics[0].Value !=
                        BaseFrameworkView.PlayerData.bestScore.Value)
                    {
                        var sync = Mathf.Max(result.InfoResultPayload.PlayerStatistics[0].Value,
                            BaseFrameworkView.PlayerData.bestScore.Value);
                        UpdateBestScore(sync, true);
                        onBestScoreChanged?.Invoke(sync);
                    }
            }
        }, error => Debug.LogError(error.GenerateErrorReport()));
    }


//    public void PlayfabLoginWithFacebook()
//    {
//        var perms = new List<string>() {"public_profile", "email"};
//        FB.LogInWithReadPermissions(perms, data =>
//        {
//            LoginWithFacebook(AccessToken.CurrentAccessToken.TokenString);
//        });
//    }


//    private void LinkedFacebookCompleted(LinkFacebookAccountResult result)
//    {
//        // maybe it use token string
//        UIDebugLog.Log("Link PlayFab Facebook Auth Complete " + result.CustomData);
//        UpdateAvatar();
//    }
//
//    private void UnlinkDevice()
//    {
//        var request = new UnlinkAndroidDeviceIDRequest()
//        {
//            AndroidDeviceId = SystemInfo.deviceUniqueIdentifier
//        };
//        PlayFabClientAPI.UnlinkAndroidDeviceID(request,
//            complete => { UIDebugLog.Log("Successful unlink " + complete.CustomData); },
//            err =>
//            {
//                PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(),
//                    obj => { UIDebugLog.Log(obj.AccountInfo.AndroidDeviceInfo.AndroidDeviceId); }, OnError);
//                UIDebugLog.Log("PlayFab Device unlink Failed: " + err.GenerateErrorReport());
//            });
//    }
//
//    private void PlayFabFacebookLinkedFailed(PlayFabError error)
//    {
//
//    }

//    private void SyncScoreToPlayFab()
//    {
//        GetPlayerStatisticsRequest request = new GetPlayerStatisticsRequest()
//        {
//            StatisticNames = new List<string>()
//            {
//                Key
//            }
//        };
//        
//        PlayFabClientAPI.GetPlayerStatistics(request, data =>
//        {
//            UIDebugLog.Log(nameof(SyncScoreToPlayFab),LogType.Service);
//            BaseFrameworkView.PlayerData.bestScore.Value = data.Statistics[0].Value;
//            BaseFrameworkView.PlayerData.Save();
//        }, OnError);
//    }

    private bool CheckFirstTime
    {
        get => PlayerPrefs.GetInt(nameof(CheckFirstTime)) == 1;
        set => PlayerPrefs.SetInt(nameof(CheckFirstTime), value ? 1 : 0);
    }

    public void UpdateRandomName()
    {
        if (PlayerProfile != null && string.IsNullOrEmpty(PlayerProfile.DisplayName))
            UpdateDisplayName(_names.TakeRandom() + PlayerProfile.PlayerId.Substring(0, 7));
    }


    private void FirstTimeLoggedAnonymous()
    {
        Debug.Log("FirstTimeLoggedAnonymous" + CheckFirstTime);

        if (!CheckFirstTime)
        {
            CheckFirstTime = true;
            if (PlayerProfile != null)
                UpdateDisplayName(_names.TakeRandom() + PlayerProfile.PlayerId.Substring(0, 10));

            var avt = UnityEngine.Random.Range(0, totalAvatar);
            UpdateAvatarIndex(avt);

            BaseFrameworkView.PlayerData.bestScore.Value = 0;
            UpdatePlayerStatisticsRequest request = new UpdatePlayerStatisticsRequest()
            {
                Statistics = new List<StatisticUpdate>
                {
                    new StatisticUpdate {StatisticName = StatisticKeyScore, Value = 0}
                }
            };

            PlayFabClientAPI.UpdatePlayerStatistics(request, result =>
            {
                Debug.Log("Reset score to " + 0);
                onBestScoreChanged?.Invoke(0);
                LoadPlayerRank(null);
                LoadGlobalBestScore(null);
            }, OnError);
        }
    }

    private void OnError(PlayFabError obj)
    {
        Debug.Log(obj.GenerateErrorReport());
    }

    public void SaveLocal()
    {
    }
}

#endif