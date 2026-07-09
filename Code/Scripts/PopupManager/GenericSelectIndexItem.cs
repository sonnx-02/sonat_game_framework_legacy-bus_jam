using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class GenericSelectIndexItem : IndexBindingScript
{
   [SerializeField] private Button button;
   [SerializeField] [IndexAsEnum(BuiltInEnumType.SoundEnum)]
   private int clickSound;

   private int Index { get; set; }
   private IntReactiveProperty Current { get; set; }
   public ToggleScript[] onSelect;
   public ToggleScript[] init;
   
   public void SetIndex(IntReactiveProperty current,int index)
   {
      Index = index;
      Current = current;
      OnChanged(Index);
      init.OnChanged(Current.Value == Index);
      ChangeSelect(Current.Value);
   }

   public void ChangeSelect(int newSelect)
   {
      onSelect.OnChanged(newSelect == Index);
   }

   protected override void LoadComponent()
   {
      button = GetComponent<Button>();
      var list = GetComponents<IndexBindingScriptBase>().ToList();
      list.Remove(this);
      sharedSetting.othersComputed = list.ToArray();
   }

   protected override void OnValueChanged(int computedValue)
   {
      base.OnValueChanged(computedValue);
      onSelect.OnChanged(computedValue == Index);
   }

   protected override void Register()
   {
      base.Register();
      button.onClick.AddListener(() =>
      {
         clickSound.PlaySound();
         Current.Value = Index;
      });
   }
}
