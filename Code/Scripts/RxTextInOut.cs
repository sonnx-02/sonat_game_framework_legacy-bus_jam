using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RxTextInOut : LoadMono
{
   [SerializeField] private BaseRxAnimator[] anims;
   
   [SerializeField] private TMP_Text text;
   [SerializeField] private  BaseMyCustomFontText customFontText;
   
   public override void Load()
   {
      base.Load();
      text = GetComponent<TMP_Text>();
      customFontText = GetComponent<BaseMyCustomFontText>();
   }
   
   public RxTextInOut SetText(string mess)
   {
      if (text != null)
         text.text = mess;
      if(customFontText != null)
         customFontText.SetText(mess);

      return this;
   }

   public void AnimIn()
   {
      gameObject.SetActive(true);
      foreach (var anim in anims)
         anim.Animator_In();
   }
   
   public void AnimOut()
   {
//      gameObject.SetActive(false);
      foreach (var anim in anims)
         anim.Animator_Out();
   }
   
   public void AnimInThenOut()
   {
      foreach (var anim in anims)
         anim.Animator_InThenOut();
   }
}
