using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class MyCustomFontCharacter
{
    public int index;
    public Sprite sprite;
    public float adjustSpacing;
}

public class MyCustomFontCharacterData
{
    private readonly float _ratio;
    private readonly float _height;
    
    public readonly MyCustomFontCharacter Data;
    public MyCustomFontCharacterData(MyCustomFontCharacter data)
    {
        Data = data;
        _ratio = Data.sprite.rect.width/Data.sprite.rect.height;
        _height = Data.sprite.rect.height;
    }

    public float GetWidth(float fontHeight)
    {
        return _ratio * fontHeight;
    }

    public float GetAdjustSpacing(float fontHeight)
    {
        return _ratio * (fontHeight/ _height) * Data.adjustSpacing;
    }

    public float GetSpacing(float fontHeight,float spacing)
    {
        return _ratio * (fontHeight / _height) * spacing;
    }
}

[CreateAssetMenu(fileName = "Font.asset", menuName = "DuongExtensions/Font/Create")]
[System.Serializable]
public class MyCustomFont : ScriptableObject
{
    
    #if UNITY_EDITOR

    public List<Sprite> loadSprites;

    [MyButton(nameof(Test))] public int abc;

    public void Test()
    {
        for (var i = 0; i < characters.Length; i++)
        {
            characters[i].sprite = loadSprites[i];
        }
    }
    
    #endif
    
    public MyCustomFontCharacter[] characters;

    private Dictionary<int, MyCustomFontCharacterData> _charactersData;
    public Dictionary<int, MyCustomFontCharacterData> CharactersData
    {
        get
        {
            if (_charactersData == null)
            {
                _charactersData = new Dictionary<int, MyCustomFontCharacterData>();
                for (var i = 0; i < characters.Length; i++)
                    _charactersData.Add(characters[i].index,new MyCustomFontCharacterData(characters[i]));
            }

            return _charactersData;
        }
    }

    public void Reload()
    {
        if (_charactersData == null)
        {
            _charactersData = new Dictionary<int, MyCustomFontCharacterData>();
            for (var i = 0; i < characters.Length; i++)
                _charactersData.Add(characters[i].index,new MyCustomFontCharacterData(characters[i]));
        }
    }

    public void Init()
    {
        if (_charactersData == null)
        {
            _charactersData = new Dictionary<int, MyCustomFontCharacterData>();
            for (var i = 0; i < characters.Length; i++)
                _charactersData.Add(characters[i].index, new MyCustomFontCharacterData(characters[i]));
        }
    }
}
