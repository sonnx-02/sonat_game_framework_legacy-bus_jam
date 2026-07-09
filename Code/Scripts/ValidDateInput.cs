using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ValidDateInput : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<TMP_InputField>().onValidateInput += delegate(string input, int charIndex, char addedChar) { return MyValidate(addedChar); };
    }

    private char MyValidate(char charToValidate)
    {
        
        //Checks if a dollar sign is entered....
        if (Char.IsLetterOrDigit(charToValidate) || charToValidate == ' ')
        {
            return charToValidate;
            // ... if it is change it to an empty character.
        }
        return '\0';
    }
}
