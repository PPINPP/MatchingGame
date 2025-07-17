using System;
using System.Collections;
using System.Collections.Generic;
using Manager;
using MatchingGame.Gameplay;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class LevelSelectorManager : MonoSingleton<LevelSelectorManager>
{
    [SerializeField] List<Sprite> state_pics = new List<Sprite>(); //0-lock 1-unlock 2-minigame 3-played1 4-played2 5-played3

    List<int> game_state = new List<int>() { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }; //0-lock 1-unlock 2-played
    List<int> game_score = new List<int>() { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
    List<int> game_role = new List<int>() { 2, 4, 4, 4, 4, 2, 4, 4, 4, 4, 2 }; //2=minigame+smileo 4=gameplay
    List<string> rule_tiles = new List<string>() { "Home", "Home", "Clothes", "Clothes", "Market", "Market", "Store", "Store" };
    List<int> categoryThemes = new List<int>() { 0, 0, 1, 1, 2, 2, 3, 3 };
    List<int> game_pairType = new List<int>() { 0, 4, 4, 6, 8, 0, 4, 4, 6, 8, 0 };
    Dictionary<string, List<Sprite>> tile_image = new Dictionary<string, List<Sprite>>();
    [SerializeField] List<Sprite> home_tile = new List<Sprite>();
    [SerializeField] List<Sprite> clothes_tile = new List<Sprite>();
    [SerializeField] List<Sprite> market_tile = new List<Sprite>();
    [SerializeField] List<Sprite> store_tile = new List<Sprite>();
    int current_state = 0;
    public float save_curr_page = 0.0f;
    SequenceManager _so;
    DataManager _dm;
    public override void Init()
    {
        base.Init();
        // gameObject = new GameObject();
        // _so = gameObject.AddComponent<SequenceManager>();
        // _dm = gameObject.AddComponent<DataManager>();
        // game_state = FirebaseManagerV2.Instance.gameState["W" + FirebaseManagerV2.Instance.curr_week.ToString()];
        // game_score = FirebaseManagerV2.Instance.gameScore["W" + FirebaseManagerV2.Instance.curr_week.ToString()];
        tile_image["Home"] = home_tile;
        tile_image["Clothes"] = clothes_tile;
        tile_image["Market"] = market_tile;
        tile_image["Store"] = store_tile;
    }
    public void UpdateGameData(){
        game_state = FirebaseManagerV2.Instance.gameState["W" + FirebaseManagerV2.Instance.curr_week.ToString()+ "_" + DateTime.Now.ToString("yyyyMMdd")];
        game_score = FirebaseManagerV2.Instance.gameScore["W" + FirebaseManagerV2.Instance.curr_week.ToString()+ "_" + DateTime.Now.ToString("yyyyMMdd")];
    }
    public void GetGameConponent(){
        _so = (SequenceManager)FindObjectOfType(typeof(SequenceManager));
        _dm = (DataManager)FindObjectOfType(typeof(DataManager));
        _so._selectormode = true;
    }

    public void OnSuccessLevel(int game_no, int score = 1)
    {
        game_state[game_no] = 2;
        game_score[game_no] = score;
        current_state = game_no + 1;
        if (current_state >= game_state.Count)
        {
            current_state = game_state.Count - 1;
        }
        if (game_state[current_state] == 0)
        {
            game_state[current_state] = 1;
        }
        //UPDATE SCORE and STATE
        FirebaseManagerV2.Instance.UploadGameStateAndGameScore(game_state, game_score);
    }
    public void ResetGame()
    {
        game_state = new List<int>() { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        game_score = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    }
    public void StartLevel(int levelnum,float curr_page)
    {
        if(levelnum == 4){
            FuzzyBrain.Instance.HoldOutput();
        }
        else if(levelnum == 0){
            FuzzyBrain.Instance.FirstMinigame();
        }
        save_curr_page = curr_page;
        if (game_role[levelnum] == 2)
        {
            GameplaySequenceSO gameplaySequenceSO = ScriptableObject.CreateInstance<GameplaySequenceSO>();
            SequenceDetail sequenceDetail = new SequenceDetail()
            {
                isMinigame = true,
            };
            gameplaySequenceSO.sequences.Add(sequenceDetail);
            sequenceDetail = new SequenceDetail()
            {
                isSmileyOMeter = true,
            };
            gameplaySequenceSO.sequences.Add(sequenceDetail);
            _so.ReloadSequence(gameplaySequenceSO);
            _so.game_no = levelnum;
            _so.NextSequence();
        }
        else if (game_role[levelnum] == 3) //No use now
        {
            GameplaySequenceSO gameplaySequenceSO = ScriptableObject.CreateInstance<GameplaySequenceSO>();
            SequenceDetail sequenceDetail = new SequenceDetail()
            {
                isSmileyOMeter = true,
            };
            gameplaySequenceSO.sequences.Add(sequenceDetail);
            _so.ReloadSequence(gameplaySequenceSO);
            _so.game_no = levelnum;
            _so.NextSequence();
        }
        else if (game_role[levelnum] == 4)
        {
            // TODO : 4. Change To Call QBrain To Get Next Stage Difficulty, PairType, Layout
            var (pt,gl,gd) = FuzzyBrain.Instance.DLS.GetDifficulty();
            
            GameplaySequenceSetting gameplaySequenceSetting = new GameplaySequenceSetting();
            gameplaySequenceSetting.isTutorial = false;
            gameplaySequenceSetting.categoryTheme = (CategoryTheme)categoryThemes[FirebaseManagerV2.Instance.curr_week - 1];
            gameplaySequenceSetting.pairType = pt;
            gameplaySequenceSetting.GameDifficult = gd;
            gameplaySequenceSetting.layout = gl;
            GameplaySequenceSO gameplaySequenceSO = ScriptableObject.CreateInstance<GameplaySequenceSO>();
            SequenceDetail sequenceDetail = new SequenceDetail()
            {
                stageID = "Test",
                isGamePlay = true,
                gameplay = gameplaySequenceSetting
            };
            gameplaySequenceSO.sequences.Add(sequenceDetail);
            _so.ReloadSequence(gameplaySequenceSO);
            _so.game_no = levelnum;
            _so.NextSequence();
        }
    }
    // public void UpdateLevelState(GameObject levelButton)
    // {
    //     for (int i = 0; i < game_state.Count; i++)
    //     {
    //         if (game_state[i] == 0)
    //         {
    //             levelButton.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = state_pics[0];
    //             levelButton.transform.GetChild(i).gameObject.GetComponent<Button>().interactable = false;
    //         }
    //         else if (game_state[i] == 1)
    //         {
    //             levelButton.transform.GetChild(i).gameObject.GetComponent<Button>().interactable = true;
    //             if (false)//minigame
    //             {
    //                 levelButton.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = state_pics[2];
    //             }
    //             else
    //             {
    //                 levelButton.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = state_pics[1];
    //             }

    //         }
    //         else if (game_state[i] == 2)
    //         {
    //             levelButton.transform.GetChild(i).gameObject.GetComponent<Button>().interactable = true;
    //             levelButton.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = state_pics[2 + game_score[i]];
    //         }
    //     }
    // }
    public void UpdateTile(List<Image> BackgroudTile, List<GameObject> LevelButton)
    {
        for (int i = 0; i < 3; i++)
        {
            BackgroudTile[i].sprite = tile_image[rule_tiles[FirebaseManagerV2.Instance.curr_week - 1]][i];
        }
        //0-lock 1-unlock 2-minigame 3-played 4-playedmini
        for (int i = 0; i < 11; i++)
        {
            if (game_state[i] == 0)
            {
                LevelButton[i].GetComponent<Button>().interactable = false;
                LevelButton[i].GetComponent<Image>().sprite = state_pics[0];
            }
            else if (game_state[i] == 1)
            {
                LevelButton[i].GetComponent<Button>().interactable = true;
                if (game_role[i] == 4)
                {
                    LevelButton[i].GetComponent<Image>().sprite = state_pics[1];
                }
                else
                {
                    LevelButton[i].GetComponent<Image>().sprite = state_pics[2];
                }
            }
            else
            {
                if (game_role[i] == 2)
                {
                    LevelButton[i].GetComponent<Button>().interactable = false;
                    LevelButton[i].GetComponent<Image>().sprite = state_pics[4];
                }
                else
                {
                    LevelButton[i].GetComponent<Button>().interactable = false;
                    LevelButton[i].GetComponent<Image>().sprite = state_pics[3];
                    LevelButton[i].transform.GetChild(0).gameObject.SetActive(true);
                    if (game_score[i] == 3)
                    {
                        LevelButton[i].transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
                        LevelButton[i].transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
                    }
                    else if (game_score[i] == 2)
                    {
                        LevelButton[i].transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
                        LevelButton[i].transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
                    }
                    else
                    {
                        LevelButton[i].transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                        LevelButton[i].transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
                    }

                }
            }
        }
    }

    public bool AllCompleteCheck()
    {
        if (game_state.Count(n => n == 0) == 0 && game_state.Count(n => n == 1) == 0)
        {
            return true;
        }
        return false;
        
    }

    public void Reset()
    {
        _dm.ClearData();
        _so.ResetGame();
    }
}
