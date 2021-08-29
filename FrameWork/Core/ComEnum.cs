using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EResourceGroup
{
    public static readonly EResourceType Object      = new EResourceType(1, "Object");
    public static readonly EResourceType UIPage      = new EResourceType(2, "Ui/Page");
    public static readonly EResourceType UIPopup     = new EResourceType(3, "Ui/Popup");
    public static readonly EResourceType UIComponent = new EResourceType(4, "Ui/Component");
    public static readonly EResourceType Sfx         = new EResourceType(5, "Effect");
}