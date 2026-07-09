using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[Serializable]
public class RemoteLevelDatabase<T>  where T : BaseLevel
{
    public T[] levels;

    public T GetReplaceLevel(int i)
    {
        if (levels == null)
            return null;
        foreach (var baseLevel in levels)
            if (baseLevel.id == i)
                return baseLevel;
        return null;
    }
    
    private static RemoteConfigDatabaseKey RemoteKey => RemoteConfigDatabaseKey.level_database;

    private PlayerPrefString _playerPrefString;

    [SerializeField]
    private PlayerPrefString PlayerPrefString => _playerPrefString ??= new PlayerPrefString(RemoteKey.ToString(),"");

    private void Clear()
    {
        PlayerPrefString.Clear();
    }

    public void LoadRemote()
    {
        if (FireBaseController.FireBaseRemoteReady)
        {
            if (RemoteConfigController.RemoteHasValue(RemoteKey.ToString()))
            {
                var remoteString = RemoteConfigHelper.GetValueString(RemoteKey.ToString());
                if (remoteString != PlayerPrefString.Value)
                {
                    PlayerPrefString.Value = remoteString;
                    ResolveDatabase();
                }
                if (levels == null)
                    ResolveDatabase();
            }
            else
            {
                Clear();
                levels = null;
            }
        }
        else if(PlayerPrefString.Exist)
            ResolveDatabase();
        else
            levels = null; 
    }


    private void ResolveDatabase()
    {
        if (PlayerPrefString.Exist)
        {
            try
            {
                JsonUtility.FromJsonOverwrite(PlayerPrefString.Value, this);
                UIDebugLog.LogError("Remote level database in use!!! ");
            }
            catch (Exception e)
            {
                // ignored
            }
        }
        else
        {
            levels = null;
        }
    }
}