using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class DivideDataCondition
{
    public int condition;
    public DivideData[] data;

    public DivideDataCondition(int condition, DivideData[] data)
    {
        this.condition = condition;
        this.data = data;
    }
}

public class DivideData
{
    public readonly int Weight;
    public readonly string PropertyName;
    public readonly string Label = "";
    public readonly float LabelWidth;
    public readonly bool Disable;
    public readonly EnumList EnumList;

    public DivideData(int weight, string name,string enumTypeName = null)
    {
        Weight = weight;
        PropertyName = name;
        if (enumTypeName != null)
            EnumList = new EnumList(enumTypeName);
    }

    public DivideData(int weight, string propertyName, string label, float labelWidth,bool disable = false)
    {
        Weight = weight;
        PropertyName = propertyName;
        Label = label;
        LabelWidth = labelWidth;
        Disable = disable;
    }
}

[AttributeUsage(AttributeTargets.Field)]
public class EditorDisplayNameAttribute : PropertyAttribute
{
    public readonly DivideData[] Data;

    public EditorDisplayNameAttribute(int[] weights, string[] propertyNames, string[] labels, int[] widths)
    {
        Data = new DivideData[weights.Length];
        for (var i = 0; i < weights.Length; i++)
            Data[i] = new DivideData(weights[i], propertyNames[i], labels[i], widths[i]);
    }
}

[AttributeUsage(AttributeTargets.Field)]
public class ConditionalDivideDataAttribute : PropertyAttribute
{
    public readonly string EnumPropertyName;
    public DivideDataCondition[] Conditions;

    public  ConditionalDivideDataAttribute(string _enumPropertyName, object[] _conditions)
    {
        EnumPropertyName = _enumPropertyName;
        Conditions = _conditions.Cast<DivideDataCondition>().ToArray();
    }
}


[AttributeUsage(AttributeTargets.Field)]
public class ListPointNameAttribute : PropertyAttribute
{
    public readonly DivideData[] Data;
    public readonly int Col;
    public readonly int Row;
    public readonly EnumList[] EnumLists;
    public ListPointType ListPointType { get; set; }
    
    public ListPointNameAttribute(int[] weights, string[] propertyNames, string[] labels, int[] widths,
        ListPointType listPointType, int col,int row,string[] enumTypes = null)
    {
        Data = new DivideData[weights.Length];
        for (var i = 0; i < weights.Length; i++)
            Data[i] = new DivideData(weights[i], propertyNames[i], labels[i], widths[i]);
        Col = col;
        Row = row;
        ListPointType = listPointType;

        if (enumTypes != null)
        {
            EnumLists = new EnumList[enumTypes.Length];
            for (var i = 0; i < enumTypes.Length; i++)
                if(enumTypes[i] != null)
                    EnumLists[i] = new EnumList(enumTypes[i]);
        }
    }
}

[AttributeUsage(AttributeTargets.Field)]
public class ListPointPointNameAttribute : PropertyAttribute
{
    public readonly DivideData[] Data;
    public readonly int Col;
    public readonly int Row;
    public readonly EnumList[] EnumLists;
    public ListPointType ListPointType { get; set; }
    public readonly string SizePropertyName;
    
    public ListPointPointNameAttribute(int[] weights, string[] propertyNames, string[] labels, int[] widths,
        ListPointType listPointType, int col,int row,string[] enumTypes = null,string sizePropertyName = null)
    {
        Data = new DivideData[weights.Length];
        for (var i = 0; i < weights.Length; i++)
            Data[i] = new DivideData(weights[i], propertyNames[i], labels[i], widths[i]);
        Col = col;
        Row = row;
        ListPointType = listPointType;
        SizePropertyName = sizePropertyName;
        if (enumTypes != null)
        {
            EnumLists = new EnumList[enumTypes.Length];
            for (var i = 0; i < enumTypes.Length; i++)
                if(enumTypes[i] != null)
                    EnumLists[i] = new EnumList(enumTypes[i]);
        }
    }
}

[AttributeUsage(AttributeTargets.Field)]
public class MyMatrixAttribute : PropertyAttribute
{
    public MyMatrixAttribute()
    {
        
    }
}



[AttributeUsage(AttributeTargets.Field)]
public class PopUpAttribute : PropertyAttribute
{
    public string[] items;

    public PopUpAttribute(params string[] args)
    {
        items = args;
    }
}


public class ArrayLabelSquare2Attribute : PropertyAttribute
{
    public readonly string VarName;

    public ArrayLabelSquare2Attribute(string elementTitleVar)
    {
        VarName = elementTitleVar;
    }
}

public class ArrayLabelIndexAsEnumAttribute : PropertyAttribute
{
    public readonly EnumList EnumList;

    public ArrayLabelIndexAsEnumAttribute(BuiltInEnumType enumType)
    {
        EnumList = new EnumList(enumType.ToString());
    }
}

[AttributeUsage(AttributeTargets.Field)]
public class DisablePropertyAttribute : PropertyAttribute
{
    public readonly string PropertyToCheck;

    public readonly object[] CompareValues;

    public readonly string[] Names;
    public readonly int[] Values;

    public DisablePropertyAttribute(Type enumType, string propertyToCheck, object[] compareValue = null)
    {
        this.PropertyToCheck = propertyToCheck;
        CompareValues = compareValue;

        //var names = Enum.GetNames(enumType);
        //Names = new string[names.Length];
        //Array.Copy(names, 0, Names, 0, names.Length);
        //Names[names.Length] = "";

        Names = Enum.GetNames(enumType);
        Values = (int[]) Enum.GetValues(enumType);
    }
}


[AttributeUsage(AttributeTargets.Field)]
public class ConditionalEnumPopUpAttribute : PropertyAttribute
{
    public readonly string PropertyToCheck;
    public readonly object[] CompareValues;
    public readonly string[] Names;
    public readonly int[] Values;

    public ConditionalEnumPopUpAttribute(string propertyToCheck, Type enumType, object[] compareValue = null)
    {
        this.PropertyToCheck = propertyToCheck;
        CompareValues = compareValue;
        Names = Enum.GetNames(enumType);
        Values = (int[]) Enum.GetValues(enumType);
    }
}