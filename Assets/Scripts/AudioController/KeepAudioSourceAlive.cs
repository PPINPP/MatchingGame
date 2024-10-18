using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepAudioSourceAlive : MonoBehaviour
{
    public AudioSource audioSourceClip;
    public AudioSource audioSourceBGM;

    private static KeepAudioSourceAlive kasa_instance = null;
    // Start is called before the first frame update
    void Awake(){
        if (kasa_instance == null)
            {
                kasa_instance = this;
                DontDestroyOnLoad(this.gameObject);
                return;
            }
            Destroy(this.gameObject);
    }
    void Start()
    {
        
        AudioController.InitialLoadAudioClip();
        AudioController.SetAudioSource(audioSourceClip,audioSourceBGM);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
