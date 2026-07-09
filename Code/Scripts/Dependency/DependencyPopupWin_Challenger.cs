//   data.SetEquipped(SessionData.challenger.Value, true, true);    => Make equip fill script, put in custom Win Popup



//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//
//
//public class DependencyPopupWin_Challenger : BasePopupDependency
//{
//    [SerializeField] private RewardView rewardView;
//    [IndexAsEnum(BuiltInEnumType.UnlockEquipDataEnum)] [SerializeField]
//    private int equipDataEnum;
//
//    [SerializeField] private ToggleScript[] isRewards;
//    public override void OnShow()
//    {
//        var data = PlayerData.equipData[equipDataEnum];
//        if (!data.CheckEquipped(SessionData.challenger.Value))
//        {
//            isRewards.OnChanged(true);
//            var current = SessionData.challenger;
//            var rewards = BaseGameController.BaseCurrentGameSetting.challengerRewards;
//            var reward = rewards[Mathf.Min(rewards.Length - 1, current.Value)].products[0];
//            PlayerData.Claim(reward);
//            rewardView.SetData(reward);
//            data.SetEquipped(SessionData.challenger.Value, true, true);  
//        }
//        else 
//            isRewards.OnChanged(false);
//    }
//}