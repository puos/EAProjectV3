using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;

// Implementation of multilingual support method for UI

[ExecuteInEditMode]
public class MultiLanguageText : Text
{
    [HideInInspector] public LANGUAGE_TYPE langType;
    [HideInInspector] public UI_TEXT_TYPE uiType;
    [HideInInspector] public string uiID;
    

}

//
[SerializeField]
public class ChangeRectTransform
{

}