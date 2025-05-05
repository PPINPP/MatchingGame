using System;
using System.Collections;
using System.Collections.Generic;
using MatchingGame.Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectorUIManager : MonoBehaviour
{
    // Start is called before the first frame update
    float curr_page = 0.0f;
    [SerializeField] List<Image> BackgroudTile = new List<Image>();
    [SerializeField] List<GameObject> LevelButton = new List<GameObject>();
    [SerializeField] Transform BGTile;
    float checkTime;
    void Start()
    {
        curr_page = LevelSelectorManager.Instance.save_curr_page;
        BGTile.localPosition = new Vector3(curr_page, 0, 0);
        checkTime = Time.time;
        FirebaseManagerV2.Instance.checkTimeChange();
        LevelSelectorManager.Instance.UpdateTile(BackgroudTile, LevelButton);
        AudioController.SetnPlayBGM("audio/BGM/BGM_Main");
    }

    // Update is called once per frame
    public void OnButtonClickToStartGame(int levelnum)
    {
        AudioController.StopPlayBGM();
        curr_page = BGTile.localPosition.x;
        LevelSelectorManager.Instance.StartLevel(levelnum, curr_page);
    }

    public void SyncData()
    {
        FirebaseManagerV2.Instance.SyncData();
    }
    void Update()
    {
        if (Time.time - checkTime > 5.0f)
        {
            FirebaseManagerV2.Instance.checkTimeChange();
            checkTime = Time.time;
        }
    }

}
