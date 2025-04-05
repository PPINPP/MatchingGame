using System;
using System.Collections;
using System.Collections.Generic;
using MatchingGame.Gameplay;
using UnityEngine;

public class LevelSelectorUIManager : MonoBehaviour
{
    // Start is called before the first frame update
    public int curr_page = 0;
    [SerializeField] GameObject levelButton;
    [SerializeField] GameObject Tile;
    void Start()
    {
        // LevelSelectorManager.Instance.UpdateLevelState(levelButton);
        curr_page = LevelSelectorManager.Instance.save_curr_page;
        FirebaseManagerV2.Instance.checkTimeChange();
        LevelSelectorManager.Instance.UpdateTile(Tile, curr_page);
    }

    // Update is called once per frame
    public void OnButtonClickToStartGame(int levelnum)
    {
        LevelSelectorManager.Instance.StartLevel(levelnum+(curr_page*4));
    }
    
    public void Next()
    {
        curr_page++;
        if (curr_page > 2)
        {
            curr_page = 2;
        }
        LevelSelectorManager.Instance.UpdateTile(Tile, curr_page);
    }
    public void Previous()
    {
        curr_page--;
        if (curr_page < 0)
        {
            curr_page = 0;
        }
        LevelSelectorManager.Instance.UpdateTile(Tile, curr_page);
    }
}
