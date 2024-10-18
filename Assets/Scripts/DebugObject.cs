using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugObject :  MonoSingleton<DebugObject>
{
    // Start is called before the first frame update
    public void Checker(){
        Debug.Log("This Work!");
    }
}
