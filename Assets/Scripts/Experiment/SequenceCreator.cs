using System.Collections;
using System.Collections.Generic;
using Manager;
using MatchingGame.Gameplay;
using Unity.VisualScripting;
using UnityEngine;

public class SequenceCreator : MonoSingleton<SequenceCreator>
{

    // hfelab.come

    // Start is called before the first frame update
    GameObject gameObject;
    SequenceManager _so;
    DataManager _dm;
    public bool _testmode = true;
    public override void Init()
    {
        base.Init();
        gameObject = new GameObject();
        _so = gameObject.AddComponent<SequenceManager>();
        _dm = gameObject.AddComponent<DataManager>();
        _so._testmode = true;
    }

    public void MiniGame()
    {
        GameplaySequenceSO gameplaySequenceSO = ScriptableObject.CreateInstance<GameplaySequenceSO>();
        SequenceDetail sequenceDetail = new SequenceDetail()
        {
            isMinigame = true,
        };
        gameplaySequenceSO.sequences.Add(sequenceDetail);
        _so.ReloadSequence(gameplaySequenceSO);
        _so.NextSequence();
    }

    public void DailyGame()
    {
        GameplaySequenceSO gameplaySequenceSO = ScriptableObject.CreateInstance<GameplaySequenceSO>();
        SequenceDetail sequenceDetail = new SequenceDetail()
        {
            isDailyFeeling = true,
        };
        gameplaySequenceSO.sequences.Add(sequenceDetail);
        _so.ReloadSequence(gameplaySequenceSO);
        _so.NextSequence();
    }

    public void SmileO()
    {
        GameplaySequenceSO gameplaySequenceSO = ScriptableObject.CreateInstance<GameplaySequenceSO>();
        SequenceDetail sequenceDetail = new SequenceDetail()
        {
            isSmileyOMeter = true,
        };
        gameplaySequenceSO.sequences.Add(sequenceDetail);
        _so.ReloadSequence(gameplaySequenceSO);
        _so.NextSequence();
    }

    public void GamePlay(string stageID,GameplaySequenceSetting gameplaySequenceSetting){
         GameplaySequenceSO gameplaySequenceSO = ScriptableObject.CreateInstance<GameplaySequenceSO>();
        SequenceDetail sequenceDetail = new SequenceDetail()
        {
            stageID = stageID,
            isGamePlay = true,
            gameplay = gameplaySequenceSetting
        };
        gameplaySequenceSO.sequences.Add(sequenceDetail);
        _so.ReloadSequence(gameplaySequenceSO);
        _so.NextSequence();
    }

    public void Tutorial(string stageID,GameplaySequenceSetting gameplaySequenceSetting){
         GameplaySequenceSO gameplaySequenceSO = ScriptableObject.CreateInstance<GameplaySequenceSO>();
        SequenceDetail sequenceDetail = new SequenceDetail()
        {
            stageID = stageID,
            isGamePlay = true,
            gameplay = gameplaySequenceSetting
        };
        gameplaySequenceSO.sequences.Add(sequenceDetail);
        _so.ReloadSequence(gameplaySequenceSO);
        _so.NextSequence();
    }

    public void Reset(){
        _dm.ClearData();
        _so.ResetGame();
    }
}
