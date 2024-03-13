using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MatchingGame.Gameplay
{
    public class SequenceManager : MonoSingleton<SequenceManager>
    {
        private int currentSequenceIndex;

        private GameplaySequenceSO _sequenceSO;

        //public GameplaySequenceSO SequenceSO { get { 
            
        //        if (this._sequenceSO == null)
        //        {
        //            this._sequenceSO =  Resources.Load<GameplaySequenceSO>("GameplaySequence");
        //            return this._sequenceSO;
        //        }
        //        else { return this._sequenceSO; }
        //}}

        public override void Init()
        {
            base.Init();
            if (this._sequenceSO == null)
                this._sequenceSO = Resources.Load<GameplaySequenceSO>("ScriptableObjects/GameplaySequence");
        }

        public SequenceDetail GetSequenceDetail()
        {
            return _sequenceSO.sequences[currentSequenceIndex];
        }

        public GameplaySequenceSetting GetGameplaySequenceSetting()
        {
            return _sequenceSO.sequences[currentSequenceIndex].gameplay;
        }
    }
}
