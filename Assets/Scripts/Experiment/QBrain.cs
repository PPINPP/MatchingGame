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

        [Header("Configuration Weight")] 
        public float IRMWeight = 0.3f;
        public float SPMWeight = 0.2f;
        public float ESMWeight = 0.5f;
        public float FMWeight = 0.1f;
        public float DiffWeight = 0.1f;
        public float PerformanceWeight = 0.8f;
        
        [Header("Configuration Data")] 
        public bool debugMode;
        public List<QLogResult> UserQLogCompleteData = new List<QLogResult>();
        public QLogResult LastUserQLogResult;
        public int gameCount;
        public TMP_Text vrbBox;
        
        
        private List<int> CompleteGameID = new List<int>();
        private int gameCompleteCount;
        private int lastGameId;
        
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

            if (gameCount > 1)
            {
                _qlogResult.Difficulty = CalDifficulty(_qlogResult);
                if (_qlogResult.Difficulty > 0)
                {
                    _qlogResult.DifficultyMeaning = "Harder";
                }
                else if (_qlogResult.Difficulty < 0)
                {
                    _qlogResult.DifficultyMeaning = "Easier";
                }
                else
                {
                    _qlogResult.DifficultyMeaning = "Maintain";
                }
            }
            
            if (gameCount > 2)
            {
                QGameplayState state = CalState(_qlogResult);
                _qlogResult.GameplayState = state;

                float rewardResult = CalReward(_qlogResult);
                _qlogResult.Reward = rewardResult;
                

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

        public float CalDifficulty(QLogResult _qlogResult)
        {
            float difficulty;
            QLogResult LastCompleteQLogResult = UserQLogCompleteData.OrderByDescending(o => o.GameID).First();
            int curGamePoint = 0;
            int lastGamePoint = 0;

            #region Calculate Point

            switch (_qlogResult.TotalMatch / 2)
            {
                case 4:
                    curGamePoint = _qlogResult.GridMode ? 1 : 2;
                    break;
                case 6:
                    curGamePoint = _qlogResult.GridMode ? 2 : 3;
                    break;
                case 8:
                    curGamePoint = _qlogResult.GridMode ? 3 : 4;
                    break;
            }
            
            switch (LastCompleteQLogResult.TotalMatch / 2)
            {
                case 4:
                    lastGamePoint = LastCompleteQLogResult.GridMode ? 1 : 2;
                    break;
                case 6:
                    lastGamePoint = LastCompleteQLogResult.GridMode ? 2 : 3;
                    break;
                case 8:
                    lastGamePoint = LastCompleteQLogResult.GridMode ? 3 : 4;
                    break;
            }

            #endregion

            if (curGamePoint > lastGamePoint)
            {
                difficulty = 1;
            }
            else if (curGamePoint < lastGamePoint)
            {
                difficulty = -1;
            }
            else 
            {
                difficulty = 0;
            }

            return difficulty;
        }
        

        public QGameplayState CalState(QLogResult _qlogResult)
        {
            #region Use Helper

            bool isAllHelperNotUsed = _qlogResult.Helper.All(a => a == false);
            bool isAddTimeOrFlipUsed = _qlogResult.Helper.Take(2).Any(a=> a == true);
            bool isPassiveUsed = _qlogResult.Helper[2];

            #endregion

            #region SpeedCategory
            GetPhaseMedian(_qlogResult,PhaseEnum.IRM,out float curGameIrmPhaseMedian,out List<float> previousGameIrmMedianList);
            GetPhaseMedian(_qlogResult,PhaseEnum.SPM,out float curGameSPMPhaseMedian,out List<float> previousGameSPMMedianList);
            #endregion
            
            SpeedCategoryEnum speedCatIRM = CalPerformancePlaySpeedWithBound(curGameIrmPhaseMedian, previousGameIrmMedianList);
            SpeedCategoryEnum speedCatSPM = CalPerformancePlaySpeedWithBound(curGameSPMPhaseMedian, previousGameSPMMedianList);

            #region FailMatch

            FalseMatchData curGameFMD = new FalseMatchData
            {
                FalseMatch = _qlogResult.FalseMatch,
                TotalMatch = _qlogResult.TotalMatch
            };
            List<FalseMatchData> previousGameFMD = new List<FalseMatchData>();
            var last3GameComplete = UserQLogCompleteData.OrderByDescending(o => o.GameID).Take(3).ToList();
            foreach (var qLogResult in last3GameComplete)
            {
                FalseMatchData fmd = new FalseMatchData
                {
                    FalseMatch = qLogResult.FalseMatch,
                    TotalMatch = qLogResult.TotalMatch
                };
                previousGameFMD.Add(fmd);
            }

            float curGameFalseMatchPercent;
            if (curGameFMD.FalseMatch == 0)
            {
                curGameFalseMatchPercent = 0;
            }
            else
            {
                curGameFalseMatchPercent = (curGameFMD.FalseMatch / (curGameFMD.FalseMatch + curGameFMD.TotalMatch / 2f)) * 100f;
            }
            
            #endregion
            FailMatchResultEnum failMatchResult = CalPerformanceFailMatchResultWithBound(curGameFalseMatchPercent, previousGameFMD);

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
            _qlogResult.FalseMatchPercent = curGameFalseMatchPercent;

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

        public float CalReward(QLogResult _qLogResult)
        {
            float reward = 0;
            QLogResult LastCompleteQLogResult = UserQLogCompleteData.OrderByDescending(o => o.GameID).First();

            #region Performance
            
            List<float> phasePercentDiffList = new List<float>();
            for (int i = 0; i < _qLogResult.PhaseSuccessPercent.Count; i++)
            {
                var diff = (_qLogResult.PhaseSuccessPercent[i] - LastUserQLogResult.PhaseSuccessPercent[i]) / 100;
                phasePercentDiffList.Add(diff);
            }
            
            float irm = (IRMWeight * phasePercentDiffList[0]);
            float spm = (SPMWeight* phasePercentDiffList[1]);
            float esm = (ESMWeight* phasePercentDiffList[2]);
            
            #endregion
            
            float performance = (irm + spm - esm) * PerformanceWeight;

            #region FalseMatch

            float falseMatchDiff = ((_qLogResult.FalseMatchPercent - LastCompleteQLogResult.FalseMatchPercent) / 100 ) * FMWeight;

            #endregion

            float difficulty = _qLogResult.Difficulty * DiffWeight;
            
            reward = performance + difficulty - falseMatchDiff;

            return reward;
        }

        public void GetPhaseMedian(QLogResult _qlogResult,PhaseEnum targetPhase, out float curGameMedian, out List<float> previousGameMedianList)
        {
            var curGameIrmPhase = _qlogResult.PhaseDataList.Where(w => w.Phase == targetPhase).ToList();
            
            if (curGameIrmPhase.Count == 0)
                curGameMedian = 0;
            else
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
        
        public FailMatchResultEnum CalPerformanceFailMatchResultWithBound(float curGameFalseMatchPercent,List<FalseMatchData> previousGameFalseData)
        {
            List<float> pastPhasePercent = new List<float>();
            foreach (FalseMatchData failMatchData in previousGameFalseData)
            {
                float percent;
                if (failMatchData.FalseMatch == 0)
                {
                    percent = 0;
                }
                else
                {
                    percent = failMatchData.FalseMatch / (failMatchData.FalseMatch + failMatchData.TotalMatch / 2f) *
                              100f;
                }
                pastPhasePercent.Add(percent);
            }
            
            float pastPhasePercentMedian = pastPhasePercent.Median();
            float bound = 0.5f * pastPhasePercentMedian;
            float upper = pastPhasePercentMedian + bound;
            float lower = pastPhasePercentMedian - bound;

            if (curGameFalseMatchPercent > upper)
                return FailMatchResultEnum.High;
            else if (curGameFalseMatchPercent < lower)
                return FailMatchResultEnum.Low;
            else
                return FailMatchResultEnum.Maintain;
        }
    }
}
