using System.Collections;
 using System.Collections.Generic;
using UniRx;
using UnityEngine;
 
 public class ToggleEventTrigger : BaseView
 {
     // Start is called before the first frame update
     [IndexAsEnum(BuiltInEnumType.ToggleEventEnum)]
     [SerializeField] private int key;

     [SerializeField] private ToggleScript[] toggles; 
     protected override void Register()
     {
         base.Register();
         if (key > 0)
             SubjectController.ToggleEvent
                 .Where(x => x.key == key)
                 .Subscribe(data => toggles.OnChanged(data.value));
     }
 }