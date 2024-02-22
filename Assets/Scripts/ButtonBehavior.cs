using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Diagnostics;

public class ButtonBehavior : Button
{
    public override void OnSubmit(BaseEventData eventData)
    {
        UnityEngine.Debug.Log("Submitted");
    }
}
