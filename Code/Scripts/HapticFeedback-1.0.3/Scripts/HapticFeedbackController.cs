// Copyright (c) Scott Doxey. All Rights Reserved. Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.UI;

namespace CandyCoded.HapticFeedback
{

    public class HapticFeedbackController : MonoBehaviour
    {

        public Button test;

        void Start()
        {
            if(test != null)
                test.onClick.AddListener(LightFeedback);
        }
        public void LightFeedback()
        {
            HapticFeedback.LightFeedback();
        }

        public void MediumFeedback()
        {
            HapticFeedback.MediumFeedback();
        }

        public void HeavyFeedback()
        {
            HapticFeedback.HeavyFeedback();
        }

    }

}
