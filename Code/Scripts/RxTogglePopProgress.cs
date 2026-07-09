using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RxTogglePopProgress : BaseRxToggleAnim
{
    [SerializeField] private float delayStart;
    [SerializeField] private float delayAfterIn;
    [SerializeField] private float speed = 1;

    protected override void LoadComponent()
    {
    }
    

    private float _progress;
    private bool _turn;

    private void OnChangedProgress(bool reverse)
    {
        UpdateValue(reverse, _progress);
    }

    protected override void Trigger(bool val)
    {
        if(val == _currentDirection)  _turn = false;
        base.Trigger(val);
    }

    private bool _currentDirection;
    protected override IEnumerator ToggleAnim(bool reverse)
    {
        _currentDirection = reverse;
        IsTweening = true;
        _progress = 0;
        _turn = false;
        OnChangedProgress(reverse);

        if (delayStart > 0) yield return new WaitForSeconds(delayStart);

        while (IsTweening)
        {
            _progress += _turn ? Time.deltaTime * -speed : Time.deltaTime * speed;
            if (!_turn && _progress >= 1)
            {
                _progress = 1 - (_progress % 1);
                _turn = true;
                if (delayAfterIn > 0) yield return new WaitForSeconds(delayAfterIn);
            }

            if (_progress <= 0 && _turn)
            {
                _turn = false;
                _progress = 0;
                IsTweening = false;
                break;
            }

            OnChangedProgress(reverse);
            yield return null;
        }
    }
}