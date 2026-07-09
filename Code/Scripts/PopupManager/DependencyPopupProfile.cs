using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DependencyPopupProfile : BasePopupDependency
{
    [IndexAsEnum(BuiltInEnumType.SoundEnum)] [SerializeField]
    private int soundClick;
    [SerializeField] private IndexBindingScriptBase avatarBinding;
    [SerializeField] private Button[] btnEdit;
    [SerializeField] private IndexBindingScriptBase bestScore;
    [SerializeField] private IndexBindingScriptBase rank;
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private ToggleScript[] onNotLogin;

    private PlayFabManager2 PlayFabManager => Kernel.Resolve<PlayFabManager2>();

    protected override void Register()
    {
        base.Register();
        SubjectController.CustomUpdatedEvent
            .Where(x => x == CustomUpdateEvent.PlayerProfile || x == CustomUpdateEvent.Avatar)
            .Subscribe(data => UpdateStatus());
       
        foreach (var button in btnEdit)
            button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        soundClick.PlaySound();
        ScreenRoot.popupManager.Resolve<PopupUpdateProfile>().ShowPopup(playerName.text,PlayFabManager.AvatarInGame,
            str => { PlayFabManager.UpdateDisplayName(str); },
            index =>
            {
                avatarBinding.OnChanged(index);
                PlayFabManager.UpdateAvatarIndex(index);
                PlayFabManager.SaveLocal();
            });
    }

    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        playerName.text = PlayFabManager.GetPlayerDisplayName();
        Kernel.Resolve<PlayFabManager2>().onAvatarChanged += index => avatarBinding.OnChanged(index);
        Kernel.Resolve<PlayFabManager2>().onBestScoreChanged += data => bestScore.OnChanged(data);
        Kernel.Resolve<PlayFabManager2>().onRankChanged += data => rank.OnChanged(data);
        Kernel.Resolve<PlayFabManager2>().onNameChanged += data => playerName.text = data;
    }
    
//        readonly PlayerPrefInt _showTimes = new PlayerPrefInt(nameof(PopupProfile).ToLower() + "_show_time");
    public override void OnShow()
    {
        base.OnShow();
        CheckRegister();
        UpdateStatus();
        PlayFabManager.LoadGlobalBestScore(value => rank.OnChanged(PlayFabManager.Rank));
//            _showTimes.Value++;
//            Kernel.Resolve<FireBaseController>().LogEvent("show_" + GetType().Name.ToLower(),
//                new LogParameter("times", _showTimes.Value));
    }

    private void UpdateStatus()
    {
        avatarBinding.OnChanged(PlayFabManager.AvatarInGame);
        rank.OnChanged(PlayFabManager.Rank);
        onNotLogin.OnChanged(PlayFabManager.Logged);
        if (PlayFabManager.Logged)
            if (PlayFabManager.GetPlayerDisplayName() != string.Empty)
                playerName.text = PlayFabManager.GetPlayerDisplayName(); //.AccountInfo.Username;
        bestScore.OnChanged(PlayerData.bestScore.Value);
    }
}