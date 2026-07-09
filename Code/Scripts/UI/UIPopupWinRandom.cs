using UnityEngine;
using DG.Tweening;

public class UIPopupWinRandom : MonoBehaviour
{
    public RectTransform randomArrow;
    public float randomWidth = 830.0f;

    public void StartRamdomRun()
    {
        randomArrow.anchoredPosition = new Vector2(-randomWidth / 2, randomArrow.anchoredPosition.y);
        randomArrow.DOAnchorPosX(randomWidth / 2, 1.5f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

    public void StopRandomRun()
    {
        randomArrow.DOKill();
    }
}
