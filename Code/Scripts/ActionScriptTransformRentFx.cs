using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionScriptTransformRentFx : BaseActionScriptTransform
{
    // Start is called before the first frame update
    [SerializeField] [IndexAsEnum(BuiltInEnumType.EffectEnum)]
    private IntFloat effect = new IntFloat(-1, 0);

    [SerializeField] private bool bindToHit;
    [SerializeField] private bool setParent;
    [SerializeField] private Vector3 offset;
    [SerializeField] private string typeNameOfHit;

    [SerializeField] [DisplayAsDisable] private Transform target;

    public override void SetTarget(Transform tf)
    {
        target = tf;
    }

    protected override void DoAction()
    {
        base.DoAction();
       if(effect.value2 <= 0)
           RentFx();
       else
           effect.value2.Timer(RentFx);
    }

    private void RentFx()
    {
        if(effect.value < 0)
            return;
        EffectView emoji;
        if (setParent)
            emoji = PoolManager.RentFx(effect.value, target, offset);
        else
            emoji = PoolManager.RentFx(effect.value, null, target.position + offset);
        
        if (bindToHit)
            emoji.BindToTransform(target);
        if (setIndex > 0)
            emoji.SetIndex(setIndex);
    }
    
    public void RentFxWorld(Vector3 pos)
    {
        if(effect.value < 0)
            return;
        EffectView emoji;
   
            emoji = PoolManager.RentFx(effect.value, null, pos + offset);
        
        if (bindToHit)
            emoji.BindToTransform(target);
        if (setIndex > 0)
            emoji.SetIndex(setIndex);
    }
}