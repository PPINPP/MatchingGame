using System.Collections;
using System.Collections.Generic;
using Manager;
using MatchingGame.Gameplay;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMenu : MonoBehaviour
{

    private float delay = 10.0f;
    private float curr_time;
    // Start is called before the first frame update
    void Start()
    {
        curr_time = Time.time;
        DataManager.Instance.ClearData();
        SequenceManager.Instance.ResetGame();
        // DestroyObjectByName("DataManager");
        // DestroyObjectByName("SequenceManager");
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > curr_time+delay){
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
    }

    void DestroyObjectByName(string objectName)
{
    GameObject obj = GameObject.Find(objectName); // Find the object by its name
    if (obj != null) // Check if the object was found
    {
        Destroy(obj); // Destroy the object
    }
    else
    {
        Debug.LogWarning("Object with name " + objectName + " not found.");
    }
}
}
