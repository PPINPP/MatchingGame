using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MatchingGame.Gameplay
{
    [Serializable]
    public class GameplayProperty
    {
        [SerializeField] float _firstTimeShowDuration;
        [SerializeField] float _flipDuration;
        [SerializeField] float _wrongPairShowDuration;

        public float FirstTimeShowDuration { get => _firstTimeShowDuration; } 
        public float FlipDuration { get => _flipDuration; }
        public float WrongPairShowDuration { get => _wrongPairShowDuration; }
    }
}