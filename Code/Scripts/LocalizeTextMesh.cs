using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


[System.Serializable]
public class LocalizeTextItem
{
    public SystemLanguage language;
    public RectCopy rectCopy;
    public TMP_FontAsset font;
    public float fontSize;
    public string message;
    public string[] otherMessages;
}


public class LocalizeTextMesh : MonoBehaviour
{
    public TextMeshProUGUI target;
    public List<LocalizeTextItem> localizeItems;
    public LocalizeTextItem defaultItem;

    private void Start()
    {
        Apply(Application.systemLanguage);
    }

    public void ApplyCurrentLanguage(int message)
    {
        Apply(Application.systemLanguage, message);
    }

    public void Apply(SystemLanguage language, int mess = 0)
    {
        Apply(defaultItem, mess);
        foreach (var localizeItem in localizeItems)
            if (localizeItem.language == language)
                Apply(localizeItem, mess);
    }

    public void Apply(LocalizeTextItem data, int mess = 0)
    {
        target.font = data.font;
        data.rectCopy.Apply(target.rectTransform);
        target.fontSize = data.fontSize;

        target.text = mess == 0 ? data.message : data.otherMessages[mess - 1];
    }
}