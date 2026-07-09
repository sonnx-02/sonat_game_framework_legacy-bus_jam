using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressScriptIntFill : ProgressScriptInt
{
	[SerializeField] private Image fill;
	
	protected override void Trigger(int val)
	{
		base.Trigger(val);
		fill.fillAmount = val * 1f / Target;
	}
}