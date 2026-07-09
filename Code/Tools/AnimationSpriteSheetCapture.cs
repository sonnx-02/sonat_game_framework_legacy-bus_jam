using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class AnimationSpriteSheetCapture : BaseFrameworkView
{
    
    #if UNITY_EDITOR

    [SerializeField] private string path = "";
    [MyButton(nameof(BrowserPath),nameof(OpenPath))] public int openPath;

    public Camera myCamera;
    public Vector2Int picSize = new Vector2Int(128,128);
    
    [MyButtonInt(nameof(TakeScreenShot))] public int capture;

    [LabelHeader("camera must be solid color", 1, 0, 0.4f)]
    public int header;

    protected virtual void TakeScreenShot(bool reopen)
    {
        if (myCamera != null)
        {
            var filePath = DoTakeScreenShot();
            if(reopen)
                Application.OpenURL(filePath);
        }
    }

    protected virtual string DoTakeScreenShot()
    {
        Debug.Log("Camera must be solid color for transperant");
        RenderTexture rt = new RenderTexture(picSize.x, picSize.y, 24);
        myCamera.targetTexture = rt;
        TextureFormat tFormat = TextureFormat.ARGB32;
        Texture2D screenShot = new Texture2D(picSize.x, picSize.y, tFormat, false);
        myCamera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, picSize.x, picSize.y), 0, 0);
        myCamera.targetTexture = null;
        RenderTexture.active = null;
        byte[] bytes = screenShot.EncodeToPNG();
        string filename = ScreenShotName();
        System.IO.File.WriteAllBytes(filename, bytes);
        return filename;
    }

    protected void BrowserPath()
    {
        path = EditorUtility.SaveFolderPanel("Path to Save Images", path, Application.dataPath);
    }
    
    protected void OpenPath()
    {
        Application.OpenURL("file://" + path);
    }
    
    protected virtual string ScreenShotName()
    {
        return $"{path}/{GetFileName()}";
    }

    protected virtual string GetFileName()
    {
        return string.Format("screen_{0}x{1}_{2:yyyy-MM-dd_HH-mm-ss}.png",
            picSize.x, picSize.y, System.DateTime.Now);
    }

#endif
}
