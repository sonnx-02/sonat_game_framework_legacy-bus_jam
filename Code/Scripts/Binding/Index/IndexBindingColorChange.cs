using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IndexBindingColorChange : IndexBindingScript
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Image target2;
    [SerializeField] private TMP_Text tmpText;
    public ParticleSystem[] targetParticles;

    [SerializeField] private Color[] colors;
    [SerializeField] private PresetColorDatabase colors2;

    Color[] Colors
    {
        get
        {
            if (colors2 == null) return colors;
            return colors2.Colors;
        }
    }

    [SerializeField] private Color whenOutRange = Color.gray;
    protected override void OnValueChanged(int computedValue)
    {
        var c = !Colors.InRange(computedValue)  ? whenOutRange : Colors[Mathf.Clamp(computedValue,0,Colors.Length-1)];
        if (spriteRenderer != null)
            spriteRenderer.color = c;
        if (target2 != null)
            target2.color = c;
        if (tmpText != null)
            tmpText.color = c;
        foreach (var target in targetParticles)
        {
            ParticleSystem.MainModule settings = target.main;
            settings.startColor = new ParticleSystem.MinMaxGradient(c);
        }
    }

    protected override void LoadComponent()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        target2 = GetComponent<Image>();
        tmpText = GetComponent<TMP_Text>();
    }

    protected override bool IsNoComponent()
    {
        return spriteRenderer == null && target2 == null && tmpText == null;
    }
}