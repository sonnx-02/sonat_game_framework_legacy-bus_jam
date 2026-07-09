using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardViewExtended : RewardView
{
    public override int Index => _index;
    private int _index;

   [SerializeField] private IndexBindingScriptBase[] indexBinds;
   [SerializeField] private ToggleScript[] onReady;
   [SerializeField] private Button button;

   public Button Button => button;
   
   
   
   public void SetIndex(int i )
   {
       _index = i;
       indexBinds.OnChanged(i);
   }

   public void SetReady(bool ready)
   {
       onReady.OnChanged(ready);
   }
}