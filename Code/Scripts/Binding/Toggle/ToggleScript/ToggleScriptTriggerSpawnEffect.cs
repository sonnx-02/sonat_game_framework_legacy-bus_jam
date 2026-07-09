using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleScriptTriggerSpawnEffect : ToggleScript
{
   [SerializeField] [IndexAsEnum(BuiltInEnumType.EffectEnum)] private int effectIndex;

   protected override void LoadComponent()
   {
   }

   protected override void Trigger(bool val)
   {
      RootView.rootView.poolManager.effects[effectIndex].RentWorld(null,transform.position);
   }
}
