using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressScriptTextureOffset : ProgressScript
{
   [SerializeField] private Renderer rdr;
   [SerializeField] private Vector2 fromToX;
   [SerializeField] private Vector2 fromToY;
   [SerializeField] private bool shareMaterial;
   
   protected override void Trigger(float val)
   {
      base.Trigger(val);
      if (shareMaterial)
         rdr.sharedMaterial.mainTextureOffset = new Vector2(fromToX.Lerp(val), fromToY.Lerp(val));
      else
         rdr.material.mainTextureOffset = new Vector2(fromToX.Lerp(val), fromToY.Lerp(val));
   }

   private void Reset()
   {
      rdr = GetComponent<Renderer>();
   }
}
