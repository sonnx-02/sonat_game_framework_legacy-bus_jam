using UnityEngine;

public class IndexBindingBombEffect : IndexBindingScriptBase
{
    [SerializeField] private ParticleSystem bombOpen;
    [SerializeField] private ParticleSystem bombMinus;
    [SerializeField] private ParticleSystem bombReadyExplode;
    [SerializeField] private ParticleSystem bombExplode;
    private bool _isReadyExplode = false;
    private bool isOpen;
    public override void OnChanged(int value)
    {
        if(value == 0)
        {
            bombExplode.Play();
            return;
        }
        if (isOpen == false)
        {
            bombOpen.Play();
            isOpen = true;
        }
        if (value <= 3 && _isReadyExplode == false)
        {
            _isReadyExplode = true;
            bombReadyExplode.Play();
        }
        bombMinus.Play();
    }
}
