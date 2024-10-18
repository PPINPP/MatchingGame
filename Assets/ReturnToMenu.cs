using System.Collections;
using System.Collections.Generic;
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
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > curr_time+delay){
            SceneManager.LoadScene("Main_P");
        }
    }
}
