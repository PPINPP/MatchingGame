using System.Collections.Generic;
using System.Linq;
using Enum;
using Experiment.QModel;
using Model;
using MathNet.Numerics.Statistics;
using TMPro;
using UnityEngine;

namespace Experiment
{
    
    // TODO : Sequence Of Flow 
    // 1. Send GameData When End Every Game To Q Brain
    // 2. Calculate Q Result 
    // 3. Send Q Result To Firebase
    // 4. Set Q Result For Decision Next Stage Difficulty, PairType, Layout
    public class QBrain: MonoSingleton<QBrain>
    {
        public bool debugMode;
        public List<QLogResult> UserQLogCompleteData = new List<QLogResult>();
        public QLogResult LastUserQLogResult;
        public int gameCount;
        int gameCompleteCount;
        private int lastGameId;
        public TMP_Text vrbBox;
        List<int> CompleteGameID = new List<int>();
        
        public override void Init()
        {
            ClearParameter();
            if (debugMode)
            {
                StartRuntimeText();
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    this.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
        
        public void ClearParameter()
        {
            UserQLogCompleteData.Clear();
            gameCount = 0;
            gameCompleteCount = 0;
            lastGameId = 0;
            LastUserQLogResult = null;
            CompleteGameID = new List<int>();
        }
        
        public void SetGameProperties(List<int> qProperties, List<int> completeGameID, int dp)
        {
            var index = 0;
            foreach (var qProperty in qProperties)
            {
                Debug.Log($"Index: {index}, Property: {qProperty}");
                index++;
            }
            gameCount = qProperties[0];
            gameCompleteCount = qProperties[1];
            lastGameId = qProperties[2];
            if (completeGameID != null)
            {
                CompleteGameID = completeGameID;
                
                Debug.Log("==== game id ===");
                foreach (var i in completeGameID)
                {
                    
                Debug.Log($"Game ID: {i}");
                    
                }
            }
            else
            {
                CompleteGameID = new List<int>();
            }
        }
        
        public void StartRuntimeText()
        {
            if (IsInvoking("RuntimeText"))
            {
                Debug.Log("AlreadyRun");
            }
            else
            {
                InvokeRepeating("RuntimeText", 0f, 0.2f);
            }
        }
        
        public void RuntimeText()
        {
            vrbBox.text = $"Game Count: {gameCount}\n Test";
        }

        public void ComputeQResult(QLogResult _qlogResult)
        {
            _qlogResult.GameID = (gameCount).ToString();
            gameCount++;
            if (_qlogResult.Complete)
            {
                gameCompleteCount++;
                CompleteGameID.Add(int.Parse(_qlogResult.GameID));
            }
            
            if (gameCount > 2)
            {
                // TODO : Calculate
                //var state = CalState(_qlogResult);
                
            }



            if (_qlogResult.Complete)
            {
                UserQLogCompleteData.Add(_qlogResult);
            }

            LastUserQLogResult = _qlogResult;
            lastGameId = int.Parse(_qlogResult.GameID);
            FirebaseManagerV2.Instance.UpdateQPostGameStage(new List<int>() { gameCount , gameCompleteCount , lastGameId }, CompleteGameID);
            FirebaseManagerV2.Instance.UploadGameQLearningData(_qlogResult);
        }

        public QGameplaySate CalState(QLogResult _qlogResult)
        {
            var curGameIrmPhase = _qlogResult.PhaseDataList.Where(w => w.Phase == PhaseEnum.IRM).ToList();

            List<float> previousGameIrmMedianList = new List<float>();
            UserQLogCompleteData.OrderBy(o => o.GameID);
            

            for (int i = UserQLogCompleteData.Count - 3; i < UserQLogCompleteData.Count; i++)
            {
                var gameData = UserQLogCompleteData[i];
                var list = gameData.PhaseDataList.Where(w => w.Phase == PhaseEnum.IRM).ToList();
                var median = list.Select(s => s.TimeUsed).Median();
                previousGameIrmMedianList.Add(median);
            }

            var curGameIrmPhaseMedian = curGameIrmPhase.Select(s => s.TimeUsed).Median();
            var cat = CalPerformancePlaySpeedWithBound(curGameIrmPhaseMedian, previousGameIrmMedianList);

            // TODO : Here
            return QGameplaySate.None;
        }
        
        
        
        public SpeedCategoryEnum CalPerformancePlaySpeedWithBound(float curPhaseTimeUsed, List<float> pastPhaseTimeUsed)
        {
            if (curPhaseTimeUsed == 0)
                return SpeedCategoryEnum.None;
            
            float pastPhaseMedian = pastPhaseTimeUsed.Median();
            float bound = 0.1f * pastPhaseMedian;
            float upper = pastPhaseMedian + bound;
            float lower = pastPhaseMedian - bound;

            if (curPhaseTimeUsed > upper)
                return SpeedCategoryEnum.Slow;
            else if (curPhaseTimeUsed < lower)
                return SpeedCategoryEnum.Fast;
            else
                return SpeedCategoryEnum.Maintain;
        }
        
        public FailMatchResultEnum CalPerformanceFailMatchResultWithBound(FailMatchData curPhase,List<FailMatchData> pastPhase)
        {
            float curPhasePercent = (curPhase.FailMatch * 2 / curPhase.ClickCount) * 100;
            
            List<float> pastPhasePercent = new List<float>();
            foreach (FailMatchData failMatchData in pastPhase)
            {
                pastPhasePercent.Add((failMatchData.FailMatch * 2 / failMatchData.ClickCount) * 100);
            }
            
            float pastPhasePercentMedian = pastPhasePercent.Median();
            float bound = 0.5f * pastPhasePercentMedian;
            float upper = pastPhasePercentMedian + bound;
            float lower = pastPhasePercentMedian - bound;

            if (curPhasePercent > upper)
                return FailMatchResultEnum.High;
            else if (curPhasePercent < lower)
                return FailMatchResultEnum.Low;
            else
                return FailMatchResultEnum.Maintain;
        }
    }
}