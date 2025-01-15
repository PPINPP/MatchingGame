using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingPanelSetter : MonoBehaviour
{
    [SerializeField] Transform Music;
    [SerializeField] Transform Effect;
    void Start(){
        Effect.GetChild(0).gameObject.SetActive(AudioController.effect);
        Effect.GetChild(1).gameObject.SetActive(!AudioController.effect);
        Music.GetChild(0).gameObject.SetActive(AudioController.bgm);
        Music.GetChild(1).gameObject.SetActive(!AudioController.bgm);
    }
    public void Toggle(string mode){
        if(mode == "music")
            AudioController.ToggleBGM();
        else
            AudioController.ToggleEffect();
    }
    public void Logout(){
        //Need Fix for Google Login
        FirebaseManagerV2.Instance.UserLogout();
    }
    public void DataSync(){

    }
}
