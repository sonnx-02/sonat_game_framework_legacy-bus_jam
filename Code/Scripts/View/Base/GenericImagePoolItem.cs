using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenericImagePoolItem : PoolItem
{

    [SerializeField] private Button button;
    public Button Button => button;
    [SerializeField] private Image image;
    public Image Image => image;
    public RectTransform RectTransform => Image.rectTransform;

    public ToggleScript[] isCurrent;
}
