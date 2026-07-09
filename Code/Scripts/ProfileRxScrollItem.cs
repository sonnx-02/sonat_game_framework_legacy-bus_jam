using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProfileData
{
    public string name;
    public int score;
    public int rank;
    public int avatar;
    public int state;

    public ProfileData Clone()
    {
        return new ProfileData()
        {
            name = name,
            score = score,
            rank = rank,
            avatar = avatar,
            state = state
        };
    }
}

public class ProfileRxScrollItem : RxScrollItemBase
{
    [SerializeField] private BindingToStringTextChange txtName;
    [SerializeField] private IndexBindingScriptBase[] scoreBindings;
    [SerializeField] private IndexBindingScriptBase[] rankBindings;
    [SerializeField] private IndexBindingScriptBase[] avatarBindings;
    [SerializeField] private IndexBindingScriptBase[] stateBindings2;


    [DisplayAsDisable] [SerializeField] private ProfileData data;
    public void SetData(ProfileData profileData)
    {
        this.data = profileData;
        if (txtName != null)
            txtName.OnChanged(profileData.name);
        if (scoreBindings != null)
            scoreBindings.OnChanged(profileData.score);
        if (rankBindings != null)
            rankBindings.OnChanged(profileData.rank);
        if (avatarBindings != null)
            avatarBindings.OnChanged(profileData.avatar);
        if (stateBindings2 != null)
            stateBindings2.OnChanged(profileData.state);
    }
}