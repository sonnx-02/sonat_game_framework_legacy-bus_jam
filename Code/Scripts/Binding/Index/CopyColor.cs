using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CopyColor : BaseFrameworkView
{
   public Color[] averageColors;

   public override void LogCustom()
   {
      base.LogCustom();
      averageColors = GetComponent<IndexBindingSpriteChange>().Sprites.Select(x => AverageColorFromTexture(x.texture))
         .ToArray();
   }


   [MyButtonInt(nameof(SetAlpha))] public int testAlpha;
   public void SetAlpha(float al)
   {
      for (var i = 0; i < averageColors.Length; i++)
      {
         averageColors[i].a = al /  100f;
      }
   }

   Color AverageColorFromTexture(Texture2D tex)
   {
      
      Color[] texColors = tex.GetPixels();
 
      int total = texColors.Length;
 
      float r = 0;
      float g = 0;
      float b = 0;
 
      for(int i = 0; i < total; i++)
      {
         r += texColors[i].r;
         g += texColors[i].g;
         b += texColors[i].b;
      }

      var color = new Color((r / total),  (g / total),  (b / total), 1);
      return color;
 
   }

}
