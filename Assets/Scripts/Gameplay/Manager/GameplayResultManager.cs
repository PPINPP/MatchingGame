using Manager;
using Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayResultManager : MonoInstance<GameplayResultManager>
{
    private GamePlayResult _gameplayResult = new GamePlayResult();
    private List<GameplayClickLog> _gameplayClickLogList = new List<GameplayClickLog>();

    public GamePlayResult GamePlayResult { get { return _gameplayResult; } set => _gameplayResult = value; }
    public List<GameplayClickLog> GameplayClickLogList { get => _gameplayClickLogList; set => _gameplayClickLogList = value; }

    public override void Init()
    {
        base.Init();
        _gameplayResult.CardPosLogList = new List<CardPosLog>();
        _gameplayResult.GameplayClickLogList = new List<GameplayClickLog>();
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
    }
}
