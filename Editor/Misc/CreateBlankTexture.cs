using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using System.IO;
using UnityEngine;


public class CreateBlankTexture : EditorWindow
{
    public Texture2D circleTexture;

    enum TextureType
    {
        Square,
        Circle
    }

    [MenuItem("Assets/DuongPham/ConvertTextureToBase64String")]
    public static void ConvertTextureToBase64String()
    {
        var sprite = Selection.activeObject as Texture2D;
        Debug.Log(sprite);
        System.IO.File.WriteAllText("D:/texture.txt", Convert.ToBase64String(sprite.EncodeToPNG()));
    }

    [MenuItem("Assets/DuongPham/Convert Sprite To Texture")]
    public static void CloneToStreaming()
    {
        var sprite = Selection.activeObject as Sprite;
        Debug.Log(sprite);

        var assetPath = AssetDatabase.GetAssetPath(sprite);
        var folder = AssetDatabase.IsValidFolder(assetPath) ? assetPath : System.IO.Path.GetDirectoryName(assetPath);

        if (sprite != null)
        {
            var texture2D = new Texture2D((int) sprite.rect.width, (int) sprite.rect.height, TextureFormat.RGBA32, false);
            var pixels = sprite.texture.GetPixels((int) sprite.textureRect.x,
                (int) sprite.textureRect.y,
                (int) sprite.textureRect.width,
                (int) sprite.textureRect.height);
            texture2D.SetPixels(pixels);
            texture2D.Apply();

            System.IO.File.WriteAllBytes(folder + "/" + sprite.name + ".png", texture2D.EncodeToPNG());
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        //string rootPath = AssetDatabase.GetAssetPath(selected);
        //FileUtil.CopyFileOrDirectory(rootPath, string.Format("Assets/StreamingAssets/{0}", System.IO.Path.GetFileName(rootPath)));
        //Debug.Log(rootPath);
        //Debug.Log(string.Format("Assets/StreamingAssets/{0}", System.IO.Path.GetFileName(rootPath)));
        ////if (Directory.Exists(selectionPath))
        ////{
        ////    // do something
        ////}
    }

    //private static string Path { get; set; }
    //{
    //    get => PlayerPrefs.GetString("TempPath");
    //    set => PlayerPrefs.SetString("TempPath",value);
    //}

    private static string Path
    {
        get => PlayerPrefs.GetString("TempPath");
        set => PlayerPrefs.SetString("TempPath", value);
    }
    
    [MenuItem("Assets/DuongPham/Log this path")]
    public static void LogThisTempPath()
    {
        foreach (var o in Selection.objects)
        {
            Debug.Log(AssetDatabase.GetAssetPath(o));
            break;
        }

        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/DuongPham/Set this temp path")]
    public static void SetThisTempPath()
    {
        foreach (var o in Selection.objects)
        {
            Debug.Log(AssetDatabase.GetAssetPath(o));
            Path = AssetDatabase.GetAssetPath(o);
            break;
        }

        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/DuongPham/Focus temp path")]
    public static void FocusTempPath()
    {
        UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(Path, typeof(UnityEngine.Object));
        Selection.activeObject = obj;
        EditorGUIUtility.PingObject(obj);
    }


    [MenuItem("Assets/DuongPham/Copy to temp path")]
    public static void CloneToStreaming2()
    {
        foreach (var o in Selection.objects)
        {
            var des = $"{Path}/{System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(o))}";
            if (!File.Exists(des))
            {
                AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(o), des);
            }
        }

        AssetDatabase.Refresh();
    }


    [MenuItem("Assets/DuongPham/Move to temp path")]
    public static void CloneToStreaming3()
    {
        foreach (var o in Selection.objects)
        {
            var des = $"{Path}/{System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(o))}";
            if (!File.Exists(des))
            {
                if(!AssetDatabase.GetAssetPath(o).Contains("ignore_"))
                    AssetDatabase.MoveAsset(AssetDatabase.GetAssetPath(o), des);
            }
        }

        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/DuongPham/Create Texture/Square")]
    private static void Init1()
    {
        _type = TextureType.Square;
        CreateBlankTexture window = CreateInstance<CreateBlankTexture>();
        window.position = new Rect(Screen.width, Screen.height, 250, 150);
        window.CenterOnMainWin();
        window.ShowPopup();
    }

    [MenuItem("Assets/DuongPham/Create Texture/Circle")]
    private static void Init2()
    {
        _type = TextureType.Circle;
        CreateBlankTexture window = CreateInstance<CreateBlankTexture>();
        window.position = new Rect(Screen.width, Screen.height, 250, 150);
        window.CenterOnMainWin();
        window.ShowPopup();
    }

    private static TextureType _type;
    private Color _color = Color.white;
    private int _width = 8;
    private int _height = 8;

    private int _circleSize = 100;
    private int _textureSize = 100;

    void OnGUI()
    {
        if (_type == TextureType.Square)
        {
            EditorGUILayout.LabelField("Create a texture", EditorStyles.wordWrappedLabel);

            _color = EditorGUILayout.ColorField("Color", _color);
            _width = EditorGUILayout.IntField("Width", _width);
            _height = EditorGUILayout.IntField("Height", _height);

            if (GUILayout.Button("Create"))
            {
                var selected = Selection.activeObject;
                var path = AssetDatabase.GetAssetPath(selected);
                var folder = AssetDatabase.IsValidFolder(path) ? path : System.IO.Path.GetDirectoryName(path);

                var texture = new Texture2D(_width, _height, TextureFormat.ARGB32, false);
                Color[] colors = new Color[texture.width * texture.height];
                for (var i = 0; i < colors.Length; i++)
                    colors[i] = _color;
                texture.SetPixels(colors);
                texture.Apply();

                File.WriteAllBytes($"{folder}/{_width}x{_height}_{ColorUtility.ToHtmlStringRGBA(_color)}.png",
                    texture.EncodeToPNG());
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
//                this.Close();
            }

            if (GUILayout.Button("Close"))
            {
                this.Close();
            }
        }

        if (_type == TextureType.Circle)
        {
            EditorGUILayout.LabelField("Create a texture", EditorStyles.wordWrappedLabel);

            _color = EditorGUILayout.ColorField("Color", _color);
            _circleSize = EditorGUILayout.IntField("Circle Size", _circleSize);
            _textureSize = EditorGUILayout.IntField("Texture Size", _textureSize);

            if (GUILayout.Button("Create"))
            {
                var selected = Selection.activeObject;
                var path = AssetDatabase.GetAssetPath(selected);
                var folder = AssetDatabase.IsValidFolder(path) ? path : System.IO.Path.GetDirectoryName(path);


                var clone = Instantiate(circleTexture);
                TextureScale.Bilinear(clone, _circleSize, _circleSize);
                var pixels = clone.GetPixels();
                for (var i = 0; i < pixels.Length; i++)
                    pixels[i] = Fade(_color, pixels[i].a);
                clone.SetPixels(pixels);

                var texture2D = new Texture2D(_textureSize, _textureSize, TextureFormat.ARGB32, false);
                Color[] fillPixels = new Color[texture2D.width * texture2D.height];
                for (int i = 0; i < fillPixels.Length; i++)
                    fillPixels[i] = Color.clear;
                texture2D.SetPixels(fillPixels);

                int x = (_textureSize - clone.width) / 2;
                int y = (_textureSize - clone.height) / 2;
                int blockWidth = clone.width;
                int blockHeight = clone.height;
                texture2D.SetPixels(x, y, blockWidth, blockHeight, clone.GetPixels());

                texture2D.Apply();

                File.WriteAllBytes(
                    folder + "/circle_" + ColorUtility.ToHtmlStringRGBA(_color) + "_" + _circleSize + "x" +
                    _textureSize + ".png", texture2D.EncodeToPNG());
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                this.Close();
            }

            if (GUILayout.Button("Close"))
            {
                this.Close();
            }
        }
    }

    private static Color Fade(Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }

    public void Circle(Texture2D tex, int cx, int cy, int r, int size, Color color)
    {
        int x, y, px, nx, py, ny, d;
        var tempArray = tex.GetPixels32();

        for (x = 0; x <= r; x++)
        {
            d = (int) Mathf.Ceil(Mathf.Sqrt(r * r - x * x));
            for (y = 0; y <= d; y++)
            {
                px = cx + x;
                nx = cx - x;
                py = cy + y;
                ny = cy - y;

                tempArray[py * size + px] = color;
                tempArray[py * size + nx] = color;
                tempArray[ny * size + px] = color;
                tempArray[ny * size + nx] = color;
            }
        }

        tex.SetPixels32(tempArray);
        tex.Apply();
    }
}