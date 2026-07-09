using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexBindingMusicPlay : IndexBindingScript
{
   [IndexAsEnum(BuiltInEnumType.MusicEnum)]
   public int[] music;

   protected override void LoadComponent()
   {
   }

   protected override void OnValueChanged(int computedValue)
   {
      base.OnValueChanged(computedValue);
      if(RootView.rootView != null)
         RootView.rootView.audioManager.PlayMusic(music[computedValue%music.Length]);
   }
}
