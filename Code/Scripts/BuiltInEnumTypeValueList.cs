
using System;
using UnityEngine;

public class BuiltInEnumTypeValueList
{
    public BuiltInEnumType EnumType { get; private set; }
    private string[] _names;
    private int[] _values;

    public static BuiltInEnumTypeValueList Create(BuiltInEnumType typeName,string nameSpace)
    {
        Type type =  ReflectionExtension.FindTypeInDomain(typeName.ToString(),nameSpace);
        if(type == null)
            type = ReflectionExtension.FindTypeInDomain("Base"+typeName,nameSpace);
        if (type == null)
        {
            Debug.LogError("not found Type " +nameSpace+"."+ typeName);
            
            return new BuiltInEnumTypeValueList()
            {
                _names = new []
                {
                    "Not found "+typeName + " 0",
                    "Not found "+typeName + " 1",
                    "Not found "+typeName + " 2",
                },
                _values = new [] {0,1,2},
                EnumType = typeName
            };
            
//            Debug.Break();
            return null;
        }
        return new BuiltInEnumTypeValueList()
        {
            _names = Enum.GetNames(type),
            _values = (int[]) Enum.GetValues(type),
            EnumType = typeName
        };
    }

    public string GetName(int value)
    {
        for (var index = 0; index < _values.Length; index++)
            if (value == _values[index])
                return _names[index];

        return value.ToString();
    }
}