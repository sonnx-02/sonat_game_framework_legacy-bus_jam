using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialHand : BaseFrameworkView
{
    [SerializeField]
    private RectTransform rectTransform;
    private TutorialButton _button;
    public void FocusOn(TutorialButton button)
    {
        _button = button;
        if (button == null)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
            transform.position = button.transform.position;
            rectTransform.localPosition = rectTransform.localPosition.RemoveZ();
            rectTransform.anchoredPosition += _button.Offset;
        }
    }

    private void Action()
    {
    }
}
