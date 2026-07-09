using UnityEngine;

public class IntValueRetrieverWinReward : IntValueRetrieverBase
{
    [SerializeField] private string[] keys;
    public override int this[int index] => 
        (
            Mathf.Min((int)RemoteConfigHelper.GetValueInt(keys[index]) + GetLevelBonusGold(), GameDefine.WinRewardGoldMax)
        );

    public override int Length => keys.Length;

    private int GetLevelBonusGold()
    {
        return (int)(PlayerData.CurrentLevel / GameDefine.WinRewardGoldLevelStep) * GameDefine.WinRewardGoldValueStep;
    }
}
