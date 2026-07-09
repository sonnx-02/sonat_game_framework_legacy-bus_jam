using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

[Serializable]
public class TypeReactiveProperty : ReactiveProperty<Type>
{
    public TypeReactiveProperty()
    {
    }

    public TypeReactiveProperty(Type initialValue) : base(initialValue)
    {
    }
}

[Serializable]
public class ArrayIntReactiveProperty : ReactiveProperty<ArrayInt>
{
    public ArrayIntReactiveProperty()
    {
    }

    public ArrayIntReactiveProperty(ArrayInt initialValue) : base(initialValue)
    {
    }

    public string GetString() => Value == null ? "null" : Value.toString();
}

[Serializable]
public class ProductReactiveProperty : ReactiveProperty<Product>
{
    public ProductReactiveProperty()
    {
    }

    public ProductReactiveProperty(Product initialValue) : base(initialValue)
    {
    }
}


