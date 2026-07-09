using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class ColorCondition
{
    public int Below;
    public Color Color;
}

[CreateAssetMenu(fileName = "EditAssets.asset", menuName = "Sonat/EditAssets")]
public class EditorAssets : ScriptableObject
{
    public Vector2 tileSize = new Vector2(35f, 35f);
    public Vector2 spacing = new Vector2(3f, 3f);


    public Texture2D[] numberTextures;
    public Texture2D[] tileTextures;
    public Texture2D[] specialTextures;
    public ColorCondition[] colorConditions;

    public List<Color> colors = new List<Color>()
    {
        Color.red,
        Color.blue,
        Color.green,
        Color.cyan,
        Color.magenta,
        Color.yellow,
        new Color32(255, 69, 0, 255),
        new Color32(128, 0, 128, 255),
        new Color32(139, 69, 19, 255)
    };

    public Color blankColor = Color.gray;
    public Color blankColor2 = new Color(0.6f,.6f,.6f);

    public Color GetColor(int value)
    {
        int t = 0;
        for (var i = 0; i < colorConditions.Length; i++)
        {
            if (value >= colorConditions[i].Below)
            {
                t = i;
            }
            else
            {
                break;
            }
        }

        return colorConditions[t].Color;
    }

    public int GetIndex(int value)
    {
        int t = 0;
        for (var i = 0; i < colorConditions.Length; i++)
        {
            if (value >= colorConditions[i].Below)
            {
                t = i;
            }
            else
            {
                break;
            }
        }

        return t;
    }
}