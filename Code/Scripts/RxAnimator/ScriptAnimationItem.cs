using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScriptAnimationItem : BaseFrameworkView
{
    public float Delay;
    public float ActiveDelay;
    public float ActiveTime = 999;
    
    public bool Move;
    public Vector3 StartMove = Vector3.one;
    public Vector3 EndMove = Vector3.one;
    public float MoveDelay;
    public float MoveTime = 1;
    public float MoveDelayAfter;
    
    public bool Scale;
    public Vector3 StartScale = Vector3.one;
    public Vector3 EndScale = Vector3.one;
    public float ScaleDelay;
    public float ScaleTime = 1;
    public float ScaleDelayAfter;
    
    public bool Fade;
    public float StartFade = 1;
    public float EndFade = 0;
    public float FadeDelay;
    public float FadeTime = 1;
    public float FadeAfter;

    private RectTransform _rectTransform;
    private Vector3 _startPos;
    private Vector3 _startScale;
    private CanvasGroup _canvasGroup;
    private Image _image;
    private SpriteRenderer _spriteRenderer;
    private Text _text;
    private TextMeshProUGUI _textMesh;
    private TextMeshPro _textMesho;
    
    protected override void Register()
    {
        base.Register();

        _rectTransform = GetComponent<RectTransform>();
        if (Fade)
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _image = GetComponent<Image>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _text = GetComponent<Text>();
            _textMesh = GetComponent<TextMeshProUGUI>();
            _textMesho = GetComponent<TextMeshPro>();
        }
    }

    
    public void OnChanged(float time)
    {
        time -= Delay;
        CheckRegister();
        if (time < ActiveDelay)
        {
            gameObject.SetActive(false);
        }
        else if (ActiveTime - time > 0)
        {
            gameObject.SetActive(true);
            
            if (Move)
            {
                if (time < MoveDelay)
                {
                    if (_rectTransform != null)
                        _rectTransform.anchoredPosition = StartMove;
                    else
                        transform.localPosition = StartMove;
                }
                else if (time > (MoveDelay + MoveTime))
                {
                    if (_rectTransform != null)
                        _rectTransform.localPosition = EndMove;
                    else
                        transform.localPosition = EndMove;
                }
                else
                {
                    var pos  = Vector3.Lerp(StartMove, EndMove, (time - MoveDelay) / MoveTime);
                    if (_rectTransform != null)
                        _rectTransform.anchoredPosition = pos;
                    else
                        transform.localPosition = pos;
                }
            }
            if (Scale)
            {
                if (time < ScaleDelay)
                {
                    transform.localScale = StartScale;
                }
                else if (time > (ScaleDelay + ScaleTime))
                    transform.localScale = EndScale;
                else
                    transform.localScale = Vector3.Lerp(StartScale, EndScale, (time - ScaleDelay) / ScaleTime);
            }
            if (Fade)
            {
                if (time < FadeDelay)
                   applyAlpha(StartFade);
                else if (time > (FadeDelay + FadeTime))
                    applyAlpha(EndFade);
                else
                    applyAlpha(Mathf.Lerp(StartFade, EndFade, (time - FadeDelay) / FadeTime));
            }
            
        }
        else
        {
            gameObject.SetActive(false); 
        }
    }

    void applyAlpha(float a)
    {
        if (_canvasGroup != null)
            _canvasGroup.alpha = a;
        else if (_image != null)
            _image.color = _image.color.Fade(a);
        else if (_spriteRenderer != null)
            _spriteRenderer.color = _spriteRenderer.color.Fade(a);
        else if (_textMesh != null)
            _textMesh.color = _textMesh.color.Fade(a);
        else if (_textMesho != null)
            _textMesho.color = _textMesho.color.Fade(a);
        else if (_text != null)
            _text.color = _text.color.Fade(a);
    }
    
    public float GetLargestTime()
    {
        float max = 0f;
        if (Move)
        {
            if (max < MoveDelay + MoveTime + MoveDelayAfter)
                max = MoveDelay + MoveTime + MoveDelayAfter;
        }
        if (Scale)
        {
            if (max < ScaleDelay + ScaleTime + ScaleDelayAfter)
                max = ScaleDelay + ScaleTime + ScaleDelayAfter;
        }
        if (Fade)
        {
            if (max < FadeDelay + FadeTime + FadeAfter)
                max = FadeDelay + FadeTime + FadeAfter;
        }

        return max + Delay;
    }
}
