using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class MyCanvasCustomText : BaseMyCustomFontText
{
    [SerializeField] private List<Image> chars;
    [SerializeField] private float fontSize = 50;

    [SerializeField] private float paddingLeft;
    [SerializeField] private float posY;

    protected override void LoadComponent()
    {
        chars = GetComponentsInChildren<Image>().ToList();
    }

  

    protected override void SetText(byte[] asciiBytes)
    {
        float posIncre = paddingLeft;
        int length = currentText.Length;

        float center = 0;
        for (int i = 0; i < length; i++)
        {
            Image character;
            if (chars.Count > i)
                character = chars[i];
            else
            {
                var newGo = new GameObject {name = "char " + i};
                newGo.transform.SetParent(transform);
                character = newGo.AddComponent<Image>();
                chars.Add(character);
                newGo.transform.localScale = Vector3.one;
            }

            if (currentFont.CharactersData.ContainsKey(asciiBytes[i]))
            {
                character.gameObject.SetActive(true);
                var characterData = currentFont.CharactersData[asciiBytes[i]];
                character.sprite = characterData.Data.sprite;

                Vector2 size = new Vector2(characterData.GetWidth(fontSize), fontSize);

                var rectTransform1 = character.rectTransform;
                rectTransform1.sizeDelta = size;


                if (i == 0)
                {
                    rectTransform1.anchoredPosition = new Vector2(posIncre + characterData.Data.adjustSpacing, posY);
                    posIncre += size.x / 2 + gap + characterData.Data.adjustSpacing;
                    center += size.x / 2;
                }
                else
                {
                    rectTransform1.anchoredPosition =
                        new Vector2(posIncre + size.x / 2 + characterData.Data.adjustSpacing, posY);

                    if (i != length - 1)
                        posIncre += size.x + gap + characterData.Data.adjustSpacing;
                    else
                    {
                        center = rectTransform1.localPosition.x + size.x / 2;
                    }
                }
            }
            else
            { 
                currentFont.Reload();
                Debug.LogError("not contains key" + asciiBytes[i], currentFont);
                character.gameObject.SetActive(false);
            }
        }

        if (alignCenter)
            for (int i = 0; i < length; i++)
            {
                var character = chars[i];
                character.rectTransform.SetLocalX(character.rectTransform.localPosition.x - center / 2);
            }

        for (int i = length; i < chars.Count; i++)
            chars[i].gameObject.SetActive(false);
    }
}