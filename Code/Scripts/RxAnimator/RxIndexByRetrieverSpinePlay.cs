using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpineSequenceList
{
    public SpineSequence[] spineSequences;
}

public class RxIndexByRetrieverSpinePlay : RxSpinePlay2
{
    [SerializeField] private BaseIndexPropertySourceRetriever retriever;
    [SerializeField] private SpineSequenceList[] others;

#if USE_SPINE
    protected override SpineSequence[] SpineSequences
    {
        get
        {
            var i = retriever.GetProperty().Value;
            if (i == 0)
                return base.SpineSequences;
            return others[i - 1].spineSequences;
        }
    }
#endif
}
