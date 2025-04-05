using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    string objName;
    void Start()
    {
        objName = gameObject.name;
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        int count = allObjects.Count(obj => obj.name == objName);
        if(count > 1){
            Destroy(this.gameObject);
        }
    }

}
