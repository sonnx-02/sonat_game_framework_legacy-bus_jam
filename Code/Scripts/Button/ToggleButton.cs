using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButton : BaseButton
{
   [SerializeField] private BindingToStringTextChange bindText;
   [SerializeField] private ToggleScript[] setToggle;

   public void Toggle(bool value)
   {
      setToggle.OnChanged(value);
   }
   
   public void SetName(string n)
   {
      if (bindText != null)
      {
         bindText.OnChanged(n);
      }
   }

   private BoolReactiveProperty _property;
   [SerializeField] private bool reverse;
   public void SetProperty(BoolReactiveProperty property)
   {
      _property = property;
      _property.Subscribe(OnChanged);
      Toggle(reverse ? !_property.Value : property.Value);
   }

   protected override void OnClick()
   {
      base.OnClick();
      _property.Value = !_property.Value; 
   }

   private void OnChanged(bool val)
   {
      Toggle(reverse ? !val : val);
   }
}
