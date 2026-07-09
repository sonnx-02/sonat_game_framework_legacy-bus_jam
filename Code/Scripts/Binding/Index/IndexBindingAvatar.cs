using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexBindingAvatar : IndexBindingSpriteChange
{
   public override Sprite[] Sprites
   {
      get
      {
         if (!Kernel.IsReady())
         {
            Debug.LogError("out of range",gameObject);
            return null;
         }
         Debug.LogError("err",gameObject);
         return null;
//         return Kernel.GetDatabase<GraphicAssetDatabase>().avatarSprites;
      }
   }
}
