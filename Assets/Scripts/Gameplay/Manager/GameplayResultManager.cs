using Manager;
using Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayResultManager : MonoInstance<GameplayResultManager>
{
    private GamePlayResult _gameplayResult = new GamePlayResult();

    public GamePlayResult GamePlayResult { get { return _gameplayResult; } set => _gameplayResult = value; }

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
    }

    public void OnEndGame()
    {
        DataManager.Instance.GamePlayResultList.Add(_gameplayResult);
    }
}
