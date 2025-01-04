using System.Collections;
using System.Collections.Generic;
using Manager;
using MatchingGame.Gameplay;
using Model;
using UnityEngine;

public class TutorialResultManager : MonoInstance<TutorialResultManager>
{
    
    private GamePlayResult _tutorialResult = new GamePlayResult();
    private List<GameplayClickLog> _tutorialClickLogList = new List<GameplayClickLog>();
    
    public GamePlayResult TutorialResult { get { return _tutorialResult; } set => _tutorialResult = value; }
    public List<GameplayClickLog> TutorialClickLogList { get => _tutorialClickLogList; set => _tutorialClickLogList = value; }
    FirebaseManagerV2 fbm;
    public override void Init()
    {
        base.Init();
        _tutorialResult.CardPosLogList = new List<CardPosLog>();
        _tutorialResult.GameplayClickLogList = new List<GameplayClickLog>();
        _tutorialResult.PauseLogList = new List<PauseLog>();
        _tutorialResult.PassiveLogList = new List<PassiveLog>();
    }
    
    public void CreateCardPosLog(string cardId,float screenPosX,float screenPosY)
    {
        CardPosLog cardPosLog = new CardPosLog(cardId, screenPosX, screenPosY);
        _tutorialResult.CardPosLogList.Add(cardPosLog);

        //Debug.Log($"Id : {cardPosLog.ItemID} , Pos x:{cardPosLog.ScreenPosX},y:{cardPosLog.ScreenPosY}");
    }
    
    public void OnEndTutorial()
    {
        _tutorialResult.GameplayClickLogList = _tutorialClickLogList;
        DataManager.Instance.TutorialResultList.Add(_tutorialResult);
        FirebaseManagerV2.Instance.UploadTutorialResult(_tutorialResult);
        var sequence = SequenceManager.Instance.GetSequenceDetail();
        FirebaseManagerV2.Instance.SaveTutorialUserGameData(sequence.stageID[sequence.stageID.Length -1].ToString(),true);

    }
}
