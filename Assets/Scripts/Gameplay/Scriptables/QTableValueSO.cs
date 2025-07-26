using System;
using System.Collections.Generic;
using Enum;
using Model;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.Scriptables
{
    [CreateAssetMenu(fileName = "QTableValue", menuName = "ScriptableObjects/QTableValueScriptable", order = 4)]
    public class QTableValueSO : SerializedScriptableObject
    {
        public List<QTableInitValue>  QTableList = new List<QTableInitValue>();
    }
    
    [Serializable]
    public class QTableInitValue
    {
        public QGameplayState GameplayState;
        public float CardNumberIncreaseQValue;
        public float CardNumberMaintainQValue;
        public float CardNumberDecreaseQValue;
        public float ChangeGameDifficultQValue;
        public float ChangeGridModeQValue;
    }
}
