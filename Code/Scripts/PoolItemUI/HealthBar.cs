using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 
public class HealthBar : BindFromWorldToCanvasComponent
{
    [SerializeField] private TweenDisplayScript hpBar;

    public TweenDisplayScript HpBar => hpBar;
}
