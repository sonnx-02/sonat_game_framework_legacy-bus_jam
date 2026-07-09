using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntValueRetrieverRemoteConfig : IntValueRetrieverBase
{
    [SerializeField] private RemoteConfigKey[] keys;
    public override int this[int index] => (int) keys[index].GetValueInt();

    public override int Length => keys.Length;
}
