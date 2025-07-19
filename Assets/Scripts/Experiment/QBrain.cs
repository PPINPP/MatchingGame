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
            gameCount = qProperties[0];
            gameCompleteCount = qProperties[1];
            lastGameId = qProperties[2];
            if (completeGameID != null)
            {
                CompleteGameID = completeGameID;
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
                var state = CalState(_qlogResult);
                _qlogResult.GameplayState = state;
                // TODO : Calculate
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

        public QGameplayState CalState(QLogResult _qlogResult)
        {
            #region Use Helper

            bool isAllHelperNotUsed = _qlogResult.Helper.All(a => a == false);
            bool isAddTimeOrFlipUsed = _qlogResult.Helper.Take(2).Any(a=> a == true);
            bool isPassiveUsed = _qlogResult.Helper[2];

            #endregion

            
            UserQLogCompleteData.OrderBy(o => o.GameID);

            #region SpeedCategory
            GetPhaseMedian(_qlogResult,PhaseEnum.IRM,out float curGameIrmPhaseMedian,out List<float> previousGameIrmMedianList);
            GetPhaseMedian(_qlogResult,PhaseEnum.SPM,out float curGameSPMPhaseMedian,out List<float> previousGameSPMMedianList);
            #endregion
            
            SpeedCategoryEnum speedCatIRM = CalPerformancePlaySpeedWithBound(curGameIrmPhaseMedian, previousGameIrmMedianList);
            SpeedCategoryEnum speedCatSPM = CalPerformancePlaySpeedWithBound(curGameSPMPhaseMedian, previousGameSPMMedianList);

            #region FailMatch

            FailMatchData curGameFMD = new FailMatchData
            {
                FalseMatch = _qlogResult.FalseMatch,
                TotalMatch = _qlogResult.TotalMatch
            };
            List<FailMatchData> previousGameFMD = new List<FailMatchData>();
            for (int i = UserQLogCompleteData.Count - 3 < 0 ? 0 : UserQLogCompleteData.Count - 3;
                 i < UserQLogCompleteData.Count; i++)
            {
                FailMatchData fmd = new FailMatchData
                {
                    FalseMatch = UserQLogCompleteData[i].FalseMatch,
                    TotalMatch = UserQLogCompleteData[i].TotalMatch
                };
                previousGameFMD.Add(fmd);
            }

            #endregion
            FailMatchResultEnum failMatchResult = CalPerformanceFailMatchResultWithBound(curGameFMD, previousGameFMD);

            #region MemoryPhase
            List<MemoryPhase> memoryPhaseList = new List<MemoryPhase>();
            PhaseEnum phaseMax;
            Dictionary<PhaseEnum,int> phaseCounts = _qlogResult.PhaseDataList
                .GroupBy(pd => pd.Phase)
                .ToDictionary(g => g.Key, g => g.Count());

            int totalCount = _qlogResult.PhaseDataList.Count;

            #region Find Highest Count In Phase
            int maxCount = phaseCounts.Values.Max();
            List<PhaseEnum> phasesWithMaxCount = phaseCounts
                .Where(pair => pair.Value == maxCount)
                .Select(pair => pair.Key)
                .OrderByDescending(phase => (int)phase)
                .ToList();
            if (maxCount * 3 == totalCount)
            {
                phaseMax = PhaseEnum.SPM;
            }
            else
            {
                phaseMax = phasesWithMaxCount.First();
            }

            #endregion
            
            foreach (PhaseEnum phase in System.Enum.GetValues(typeof(PhaseEnum)))
            {
                phaseCounts.TryGetValue(phase, out int countForPhase);
                float percentage = ((float)countForPhase / totalCount) * 100.0f;
            
                memoryPhaseList.Add(new MemoryPhase
                {
                    Phase = phase,
                    InPhasePercentage = percentage
                });
            }

            memoryPhaseList = memoryPhaseList.OrderBy(p => p.Phase).ToList();
            _qlogResult.PhaseSuccessPercent = memoryPhaseList.Select(s=> s.InPhasePercentage).ToList();
            
            
            #endregion
            MemoryPhase selectMemoryPhase = memoryPhaseList.First(f => f.Phase == phaseMax);
            _qlogResult.SelectMemoryPhase = selectMemoryPhase;
            _qlogResult.SpeedCatIRM = speedCatIRM;
            _qlogResult.SpeedCatSPM = speedCatSPM;
            _qlogResult.FailMatchResult = failMatchResult;

            #region Condition Return State

            if (isPassiveUsed || _qlogResult.Complete == false)
                return QGameplayState.State7;

            if (_qlogResult.PauseUsed)
                return QGameplayState.State6;
            
            #region State 1

            if ((speedCatIRM == SpeedCategoryEnum.Maintain || speedCatIRM == SpeedCategoryEnum.Slow ||
                 speedCatIRM == SpeedCategoryEnum.None) 
                && selectMemoryPhase.Phase == PhaseEnum.SPM 
                && failMatchResult != FailMatchResultEnum.High)
                return QGameplayState.State1;

            #endregion
            
            #region State 2

            if (speedCatIRM == SpeedCategoryEnum.Fast
                && selectMemoryPhase.Phase != PhaseEnum.ESM
                && (failMatchResult == FailMatchResultEnum.Low || failMatchResult == FailMatchResultEnum.Maintain))
            {
                if (selectMemoryPhase.Phase == PhaseEnum.SPM)
                    return QGameplayState.State2;
                
                if (selectMemoryPhase.Phase == PhaseEnum.IRM 
                    && (failMatchResult == FailMatchResultEnum.Maintain 
                    || isAddTimeOrFlipUsed == true
                    || speedCatSPM != SpeedCategoryEnum.None))
                    return QGameplayState.State2;
            }

            #endregion

            #region State 3

            if (speedCatIRM == SpeedCategoryEnum.Fast
                && selectMemoryPhase.Phase == PhaseEnum.IRM
                && speedCatSPM == SpeedCategoryEnum.None
                && failMatchResult == FailMatchResultEnum.Low
                && isAllHelperNotUsed == true)
                return QGameplayState.State3;

            #endregion
            
            #region State 4

            if ((speedCatIRM == SpeedCategoryEnum.Maintain ||  speedCatIRM == SpeedCategoryEnum.Slow)
                && selectMemoryPhase.Phase == PhaseEnum.IRM
                && failMatchResult != FailMatchResultEnum.High)
                return QGameplayState.State4;

            #endregion
            
            #region State 5

            bool case1 = selectMemoryPhase.Phase == PhaseEnum.ESM
                         && speedCatSPM != SpeedCategoryEnum.Slow
                         && failMatchResult != FailMatchResultEnum.None;
            bool case2 = speedCatSPM != SpeedCategoryEnum.Slow
                         && failMatchResult == FailMatchResultEnum.High;
            bool case3 = selectMemoryPhase.Phase == PhaseEnum.ESM
                         && speedCatSPM == SpeedCategoryEnum.Slow
                         && (failMatchResult == FailMatchResultEnum.Low 
                             || failMatchResult == FailMatchResultEnum.Maintain);
            
            if (case1 || case2 || case3)
                return QGameplayState.State5;
            
          
            #endregion
            
            #region State 6
            
            if (speedCatSPM == SpeedCategoryEnum.Slow && failMatchResult == FailMatchResultEnum.High)
                return QGameplayState.State6;
          
            #endregion
            
            #endregion
            
            return QGameplayState.None;
        }

        public void GetPhaseMedian(QLogResult _qlogResult,PhaseEnum targetPhase, out float curGameMedian, out List<float> previousGameMedianList)
        {
            var curGameIrmPhase = _qlogResult.PhaseDataList.Where(w => w.Phase == targetPhase).ToList();
            curGameMedian = curGameIrmPhase.Select(s => s.TimeUsed).Median();

            previousGameMedianList = new List<float>();
            for (int i = UserQLogCompleteData.Count - 3 < 0 ? 0 : UserQLogCompleteData.Count - 3;
                 i < UserQLogCompleteData.Count; i++)
            {
                var gameData = UserQLogCompleteData[i];
                var list = gameData.PhaseDataList.Where(w => w.Phase == targetPhase).ToList();
                var median = list.Select(s => s.TimeUsed).Median();
                previousGameMedianList.Add(median);
            }
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
            float curPhasePercent = (curPhase.FalseMatch / (curPhase.FalseMatch + curPhase.TotalMatch / 2)) * 100;
            
            List<float> pastPhasePercent = new List<float>();
            foreach (FailMatchData failMatchData in pastPhase)
            {
                pastPhasePercent.Add((curPhase.FalseMatch / (curPhase.FalseMatch + curPhase.TotalMatch / 2)) * 100);
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