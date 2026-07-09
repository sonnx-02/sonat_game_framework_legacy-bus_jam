using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class BaseBooleanActionScript : BaseView
    {
        public BaseBooleanActionScript[] others;

        public virtual void StartAction(bool value)
        {
            foreach (var baseBooleanActionScript in others)
                baseBooleanActionScript.StartAction(value);
        }

        public abstract void Prepare();
    }
