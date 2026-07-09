using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardViewProduct : RewardView
{
   [SerializeField] public Product product;
   
   public override Product Product => product;

   [SerializeField] [MyButtonInt(nameof(SetDataItSelf))]
   private int testSetData;
   public void SetDataItSelf()
   {
      SetData(product);
      #if UNITY_EDITOR
      UnityEditor.EditorUtility.SetDirty(this);
      #endif
   }
}
