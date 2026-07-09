using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class LocalizeItem
{
    public SystemLanguage language;
    public RectCopy rectCopy;
    public Sprite sprite;
}

[RequireComponent(typeof(Image))]
public class LocalizeImage : MonoBehaviour
{
    public Image target;
    public List<LocalizeItem> localizeItems;
    public LocalizeItem defaultItem;

    private void Start()
    {
        Apply(Application.systemLanguage);
    }
    
    public void Apply(SystemLanguage language)
    {
        Apply(defaultItem);
        foreach (var localizeItem in localizeItems)
            if (localizeItem.language == language)
            {
                Apply(localizeItem);
            }
    }

    public void Apply(LocalizeItem data)
    {
        data.rectCopy.Apply(target.rectTransform);
        target.sprite = data.sprite;
    }
    
#if UNITY_EDITOR
    [MyButton(nameof(Load))] [SerializeField]
    private int testLoad1;

    public void Load()
    {
        target = GetComponent<Image>();
    }
#endif
}
