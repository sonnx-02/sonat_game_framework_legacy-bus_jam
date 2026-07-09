using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class IntValueRetrieverSharedData : IntValueRetrieverBase
{
    [SerializeField] private Dummy dummy = Dummy.NoDummy;
    [Header("Key,Add,IsDummy,AddDummy")]
    [IndexAsEnum(BuiltInEnumType.FrameworkSharedData)] [SerializeField]
    private IntIntBoolInt[] keys;

    [SerializeField] private ListIndexBindingScript[] updateOnEnables;
    public enum Dummy
    {
        NoDummy,
        DummyOnEditorOnly,
        DummyAll
    }

    public override int Length => keys.Length;

    public override int this[int index]
    {
        get
        {
            if (dummy == Dummy.NoDummy)
                return SharedData[keys[index].value] + keys[index].value2;

            if (dummy == Dummy.DummyAll)
                if (keys[index].value3)
                    return keys[index].value4 + keys[index].value2;

            if (dummy == Dummy.DummyOnEditorOnly)
            {
#if UNITY_EDITOR
                if (keys[index].value3)
                    return keys[index].value4 + keys[index].value2;
#endif
            }
            return SharedData[keys[index].value] + keys[index].value2;
        }
    }

    private void OnEnable()
    {
        for (var index = 0; index < updateOnEnables.Length; index++)
        {
            var listIndexBindingScript = updateOnEnables[index];
            listIndexBindingScript.scripts.OnChanged(Values[index]);
        }
    }

    
    public override void LogCustom()
    {
        base.LogCustom();
        for (var i = 0; i < Values.Length; i++)
        {
            
            Debug.LogError(Values[i]);
        }
    }
}
