using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UniRx;


public class OnEventGameMainTriggerIndex : OnEventGameMainTriggerIndexBase
{
    [SerializeField] private IndexPropertySourceRetriever source;
    [SerializeField] private IntRetrieverItem source2;

    protected override int Value => source != null ? source.GetProperty().Value : source2.GetValue();
}