using UnityEngine;


public class RentEffectScript : RentEffectScriptBase
{
    [IndexAsEnum(BuiltInEnumType.EffectEnum)] [SerializeField]
    private int keyIndex;
    public void Rent(int index)
    {
        if (index < 0)
        {
            if (rentWorldCurrentPos)
                RootView.rootView.poolManager.effects[keyIndex].RentWorld(null, transform.position);
            else
                RootView.rootView.poolManager.effects[keyIndex].Rent(parent, Vector3.zero);
        }
        else
            RootView.rootView.poolManager.effects[keyIndex].Rent(parent, Vector3.zero).SetIndex(index);
    }

    [MyButtonInt(nameof(Rent),BuiltInEnumType.EffectEnum)] [SerializeField]
    private int testRent;

  
    protected override void DoRent()
    {
        base.DoRent();
        var poolManager = RootView.rootView.poolManager;
        if (keyIndex >= poolManager.effects.CountPools())
        {
            Debug.Log("Key Index Out Of Range Pool");
            return;
        }
        var rent = rentWorldCurrentPos
            ? poolManager.effects[keyIndex].RentWorld(null, transform.position + GetOffset())
            : poolManager.effects[keyIndex].Rent(parent, actionOffset.Random(GetOffset()));
        if (actionIndex >= 0)
            rent.SetIndex(actionIndex);
    }
}