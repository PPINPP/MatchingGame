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
    void Start()
    {
        curr_page = LevelSelectorManager.Instance.save_curr_page;
        BGTile.localPosition = new Vector3(curr_page,0,0);
        FirebaseManagerV2.Instance.checkTimeChange();
        LevelSelectorManager.Instance.UpdateTile(BackgroudTile,LevelButton);
    }

    // Update is called once per frame
    public void OnButtonClickToStartGame(int levelnum)
    {
        curr_page = BGTile.localPosition.x;
        LevelSelectorManager.Instance.StartLevel(levelnum,curr_page);
    }

    public void SyncData(){
        FirebaseManagerV2.Instance.SyncData();
    }

}
