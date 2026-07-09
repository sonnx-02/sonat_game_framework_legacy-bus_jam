using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ActiveOnTutStep : BaseFrameworkView
{
    [FormerlySerializedAs("activeOn")] [SerializeField]
    private int[] activeOnIndex;

    [FormerlySerializedAs("activeOnType")] [IndexAsEnum(BuiltInEnumType.TutorialActionEnum)] [SerializeField]
    private int[] activeOnAction;

    [IndexAsEnumFlag(BuiltInEnumType.TutorialEnableFlag)]
    public int tutEnables;

    [SerializeField] private ToggleScript[] toggles;
    [SerializeField] private IndexBindingScript[] indexToggles;
    [SerializeField] private bool activeThis;
    [SerializeField] private bool reverse;

    public void SetActive(int stepIndex, int stepType, int enables)
    {
        bool found = false;
        foreach (var val in activeOnIndex)
            if (val == stepIndex)
            {
                found = true;
                break;
            }

        if (!found)
            foreach (var val in activeOnAction)
                if (val == stepType)
                {
                    found = true;
                    break;
                }

        if (!found && ((tutEnables & enables) != 0))
            found = true;

        if (reverse)
            found = !found;

        foreach (var indexBindingScript in indexToggles)
            indexBindingScript.OnChangedBoolean(found);

        toggles.OnChanged(found);
        if (activeThis)
            gameObject.SetActive(found);
    }
}