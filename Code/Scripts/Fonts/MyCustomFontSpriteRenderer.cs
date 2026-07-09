using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class BaseMyCustomFontText  : IndexBindingScript
{
    [SerializeField] private string prefix = "";

    [SerializeField] private MyCustomFont fontData;
    [SerializeField] private MyCustomFont[] otherFonts;
    [SerializeField] protected float gap = 0;
    [SerializeField] protected bool alignCenter;
    
    protected MyCustomFont currentFont;
    [SerializeField]
    [DisplayStringAsLabel]
    protected string currentText;

    public bool setTextNotFont;

    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);
        if (!setTextNotFont)
        {
            currentFont = computedValue == 0 ? fontData : otherFonts[computedValue - 1];
            currentFont.Init();
            SetText(currentText);
        }
        else
        {
            SetText(computedValue.ToString());
        }
    }

    public void SetFont(int font)
    {
        currentFont = font == 0 ? fontData : otherFonts[font - 1];
            currentFont.Init();
            SetText(currentText);
    }

    protected override void Register()
    {
        base.Register();
        currentFont = fontData;
        fontData.Init();
    }

    public virtual void SetText(string text)
    {
        CheckRegister();
        currentText = prefix+text;
        byte[] asciiBytes = Encoding.ASCII.GetBytes(currentText);
        
        SetText(asciiBytes);
    }

    protected abstract void SetText(byte[] asciiBytes);
    
    
    [MyButtonString(nameof(TestSetText))] public string testNumber;
   
    public void TestSetText(string number)
    {
        SetText(number);
    }
}



public class MyCustomFontSpriteRenderer : BaseMyCustomFontText
{
    [SerializeField] private List<SpriteRenderer> chars;

    protected override void LoadComponent()
    {
        chars = GetComponentsInChildren<SpriteRenderer>().ToList();
    }

    protected override bool IsNoComponent()
    {
        return chars == null || chars.Count == 0;
    }

    protected override void SetText(byte[] asciiBytes)
    {
        float spacing = 0;
        int length = asciiBytes.Length;
        
        for (int i = 0; i < length; i++)
        {
            SpriteRenderer character;
            if (chars.Count > i)
                character = chars[i];
            else
            {
                var newGo = new GameObject {name = "char " + i};
                newGo.transform.SetParent(transform);
                character = newGo.AddComponent<SpriteRenderer>();
                chars.Add(character);
            }

            var rectTransform = character.transform;
            rectTransform.localPosition = new Vector3(spacing, 0);

            if (currentFont.CharactersData.ContainsKey(asciiBytes[i]))
            {
                character.gameObject.SetActive(true);
                var characterData = currentFont.CharactersData[asciiBytes[i]];
                character.sprite = characterData.Data.sprite;
                if(i < length-1)
                    spacing += characterData.Data.adjustSpacing + gap;
            }
            else
            {
                Debug.Log($"not found char {currentText[i]}, code={asciiBytes[i]}");
                character.gameObject.SetActive(false);
            }
        }

        if (alignCenter)
            for (int i = 0; i < length; i++)
            {
                var character = chars[i];
                character.transform.SetLocalX(character.transform.localPosition.x - spacing / 2);
            }

        for (int i = length; i < chars.Count; i++)
            chars[i].gameObject.SetActive(false);
    }
}