using Manager;
using Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayResultManager : MonoInstance<GameplayResultManager>
{
    private GamePlayResult _gameplayResult = new GamePlayResult();
    private List<GameplayClickLog> _gameplayClickLogList = new List<GameplayClickLog>();
    private MinigameResult _minigameResult = new MinigameResult();
    private List<MinigameClickLog> _minigameClickLogList = new List<MinigameClickLog>();
    private FuzzyGameData _fuzzygameResult = new FuzzyGameData();
    private SpecialFuzzyData _specialgameResult = new SpecialFuzzyData();

    public GamePlayResult GamePlayResult { get { return _gameplayResult; } set => _gameplayResult = value; }
    public List<GameplayClickLog> GameplayClickLogList { get => _gameplayClickLogList; set => _gameplayClickLogList = value; }
    public MinigameResult MinigameResult
    {
        get { return _minigameResult; }
        set => _minigameResult = value;
    }
    public List<MinigameClickLog> MinigameClickLogList { get => _minigameClickLogList; set => _minigameClickLogList = value; }
    public FuzzyGameData FuzzyGameResult { get {return _fuzzygameResult; } set => _fuzzygameResult = value; }
    public SpecialFuzzyData SpecialFuzzyData { get {return _specialgameResult; } set => _specialgameResult = value; }
    
    
    public override void Init()
    {
        base.Init();
        _gameplayResult.CardPosLogList = new List<CardPosLog>();
        _gameplayResult.GameplayClickLogList = new List<GameplayClickLog>();
        _gameplayResult.PauseLogList = new List<PauseLog>();
        _gameplayResult.PassiveLogList = new List<PassiveLog>();
        _minigameResult.MinigameClickLogList = new List<MinigameClickLog>();
        _minigameResult.TargetPosX = new List<float>();
        _minigameResult.TargetPosY = new List<float>();
        _specialgameResult.CorrectSeq = new List<bool>();
        _specialgameResult.ClickTypeList = new List<int>();
        _specialgameResult.TimeClick = new List<float>();
        // _minigameResult.RandomIDLogList = new List<int>();

    }

    public void CreateCardPosLog(string cardId,float screenPosX,float screenPosY)
    {
        CardPosLog cardPosLog = new CardPosLog(cardId, screenPosX, screenPosY);
        _gameplayResult.CardPosLogList.Add(cardPosLog);

        //Debug.Log($"Id : {cardPosLog.ItemID} , Pos x:{cardPosLog.ScreenPosX},y:{cardPosLog.ScreenPosY}");
    }
    
    public void OnEndGame()
    {
        _gameplayResult.GameplayClickLogList = _gameplayClickLogList;
        DataManager.Instance.GamePlayResultList.Add(_gameplayResult);
        //FirebaseManagerV2 upload Data
        FirebaseManagerV2.Instance.UploadGamePlayResult(_gameplayResult);
        //FuzzyBrain
        FuzzyBrain.Instance.PostGameStage(_fuzzygameResult);
    }

    public void OnEndMiniGame()
    {
        _minigameResult.MinigameClickLogList = _minigameClickLogList;
        DataManager.Instance.MinigameResultList.Add(_minigameResult);
        //FirebaseManagerV2 upload Data
        FirebaseManagerV2.Instance.UploadMiniGameResult(_minigameResult,DataManager.Instance.MinigameResultList.Count-1);
        //FuzzyBrain
        FuzzyBrain.Instance.PostSpecialTaskStage(_specialgameResult);
        
    }
}
