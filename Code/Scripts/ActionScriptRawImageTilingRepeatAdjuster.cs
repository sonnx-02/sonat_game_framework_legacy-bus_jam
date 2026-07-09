using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionScriptRawImageTilingRepeatAdjuster : BaseActionScript
{
    [SerializeField] private RawImage rawImage;

    public override void StartAction()
    {
        base.StartAction();
        var texture = rawImage.texture;
        var originalSize = texture.width * 1f / texture.height;
        var rect = rawImage.rectTransform.rect;
        var currentSize  = rect.width * 1f / rect.height;
        rawImage.uvRect  =  new Rect(0,0,currentSize / originalSize,1);
    }
}
