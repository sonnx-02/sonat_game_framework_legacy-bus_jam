using System.Collections;
using System.Collections.Generic;
using AssetKits.ParticleImage;
using UnityEngine;

public class PlayScriptPlayParticleImage : BaseActionScript
{
    [SerializeField] private ParticleImage[] particleImage;

    protected override void DoAction()
    {
        base.DoAction();
        foreach (var image in particleImage)
            image.Play();
    }
}
