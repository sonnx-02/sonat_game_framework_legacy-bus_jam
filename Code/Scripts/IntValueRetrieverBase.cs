using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UniRx;

[Serializable]
public class IntValueRetrieverOrDefault
{
    [SerializeField] public int defaultValue;
    [SerializeField] public IntValueRetrieverBase retriever;

    public int Value => retriever != null ? retriever[0] : defaultValue;
}


/// <summary>
/// to many integer properties from one source
/// </summary>
public abstract class IntValueRetrieverBase : BaseFrameworkView
{
    public abstract int this[int index] { get; }

    public abstract int Length { get; }
    public virtual int[] Values => Enumerable.Range(0, Length).Select(x => this[x]).ToArray();
    public int Value => Values[0];
    public virtual int[] GetArrayInt(int index) => Values;
    
    public Product Product => new Product((Quantity)this[0],this[2],this[1],this[3],this[4]);
}

