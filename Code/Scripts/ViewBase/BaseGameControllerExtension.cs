using UnityEngine;

public static class BaseGameControllerExtension
{
    
    private static BuiltInEnumTypeValueList[] _builtInEnumTypeValueLists;
    
    public static string GetName(this BuiltInEnumType enumType, int key)
    {
        foreach (var valueList in _builtInEnumTypeValueLists)
            if(valueList.EnumType == enumType)
               return valueList.GetName(key);
        
        Debug.LogError("Not found "+enumType);
        return "not_found_"+enumType+"_"+key;
    }
    

    public static void SetEnumValueList(BuiltInEnumTypeValueList[] toArray)
    {
        _builtInEnumTypeValueLists = toArray;
    }
}