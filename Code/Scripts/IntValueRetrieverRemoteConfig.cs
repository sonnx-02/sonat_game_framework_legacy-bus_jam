using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntValueRetrieverRemoteConfigString : IntValueRetrieverBase
{
    [SerializeField] private string[] keys;
    public override int this[int index] => (int) RemoteConfigHelper.GetValueInt(keys[index]);

    public override int Length => keys.Length;
}
