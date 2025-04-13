using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MatchingGame.Gameplay;
using TMPro;
using UnityEngine.Timeline;
using Model;
using System.Linq;
using Unity.VisualScripting;

public class FuzzyBrain : MonoSingleton<FuzzyBrain>
{
    // Start is called before the first frame update
    [SerializeField]
    public bool debugMode;
    public List<FuzzyGameData> UserFuzzyData = new List<FuzzyGameData>();
    public TMP_Text vrbBox;
    public TMP_Text ruleBox;
    DifficultyLevelSequence DLS;

    bool mtDiff;
    bool isFirstDay;
    public int gameCount;
    int gameComplete;
    int gameInComplete;
    int minigameCount;
    int dayPassed;
    int ruleCount;
    int difficultyState;
    List<object> ShowList = new List<object>();
    List<string> RuleTextList = new List<string>();
    List<int> CompleteGameID = new List<int>();

    public override void Init()
    {
        DLS = new DifficultyLevelSequence();
        ClearParameter();
        if(debugMode){
            StartRuntimeText();
        }
        else{
            for(int i=0;i<3;i++){
                this.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
    public void ClearParameter()
    {
        UserFuzzyData.Clear();
        gameCount = 0;
        gameComplete = 0;
        gameInComplete = 0;
        isFirstDay = true;
        mtDiff = false;
        dayPassed = 0;
        ruleCount = 0;
        difficultyState = 0;
    }
    public void PostGameStage(FuzzyGameData _fuzzyGameData)
    {
        ShowList.Clear();
        gameCount++;
        bool redPoint = false;
        difficultyState = 0;
        if (_fuzzyGameData.Complete)
        {
            gameComplete++;
            CompleteGameID.Add(int.Parse(_fuzzyGameData.GameID));
            if (_fuzzyGameData.PauseUsed)
            {
                //N4
                SetRuleText("Rule:\nN4, " + _fuzzyGameData.PauseUsed.ToString());
            }
            else
            {
                if (gameCount <= 2)
                {
                    //REDPOINT
                    redPoint = true;
                }
                else
                {
                    //N11
                    ShowList.Add("Rule:\nN11");
                    ShowList.Add(string.Join(", ", _fuzzyGameData.Helper));
                    if (_fuzzyGameData.HelperSeq.Count == 0)
                    {
                        difficultyState++;
                        ShowList.Add("Increase");
                    }
                    else
                    {
                        foreach (var hps in _fuzzyGameData.HelperSeq)
                        {
                            ShowList.Add(hps);
                        }
                        if (_fuzzyGameData.HelperSeq.Count == 1)
                        {
                            if (_fuzzyGameData.HelperSeq[0] == "Passive")
                            {
                                ShowList.Add("Maintain");
                            }
                            else
                            {
                                difficultyState++;
                                ShowList.Add("Increase");
                            }
                        }
                        else
                        {
                            if (_fuzzyGameData.HelperSeq[0] == "Passive")
                            {
                                difficultyState++;
                                ShowList.Add("Increase");

                            }
                            else
                            {
                                ShowList.Add("Maintain");
                            }
                        }
                    }
                    //N12
                    ShowList.Add("Rule:\nN12");
                    SetRuleTextList(ShowList);
                    // List<int> _fgm = new List<int>() { UserFuzzyData[0].Phase[0], UserFuzzyData[1].Phase[0], UserFuzzyData[2].Phase[0], UserFuzzyData[3].Phase[0], _fuzzyGameData.Phase[0] };
                    // _fgm.Sort();
                    // if (_fuzzyGameData.Phase[0] < _fgm[2])
                    // {
                    //     if (true)//easier
                    //     {
                    //         difficultyState--;
                    //         ShowList.Add("Decrease");
                    //     }
                    //     else
                    //     {
                    //         ShowList.Add("Maintain");
                    //     }
                    // }
                    // else if (_fuzzyGameData.Phase[0] == _fgm[2])
                    // {
                    //     if (true)//harder
                    //     {
                    //         difficultyState++;
                    //         ShowList.Add("Increase");

                    //     }
                    //     else
                    //     {
                    //         ShowList.Add("Maintain");
                    //     }
                    // }
                    // else
                    // {
                    //     if (true)//easier
                    //     {
                    //         ShowList.Add("Maintain");
                    //     }
                    //     else
                    //     {
                    //         difficultyState++;
                    //         ShowList.Add("Increase");
                    //     }
                    // }

                    if (gameCount == 3)
                    {
                        //N1
                        ShowList.Clear();
                        ShowList.Add("Rule:\nN1");
                        int irm = 0;
                        int spm = 0;
                        int esm = 0;
                        for (int i = 0; i < 2; i++)
                        {
                            irm += UserFuzzyData[i].Phase[0];
                            spm += UserFuzzyData[i].Phase[1];
                            esm += UserFuzzyData[i].Phase[2];
                            ShowList.Add(string.Join(", ", UserFuzzyData[i].Phase));
                        }
                        irm += _fuzzyGameData.Phase[0];
                        spm += _fuzzyGameData.Phase[1];
                        esm += _fuzzyGameData.Phase[2];
                        ShowList.Add(string.Join(", ", _fuzzyGameData.Phase));
                        ShowList.Add(irm);
                        ShowList.Add(spm);
                        ShowList.Add(esm);
                        if (irm > spm && irm > esm)//F Increase
                        {
                            ShowList.Add("Increase");
                            difficultyState++;
                        }
                        else if (spm > irm && spm > esm)//M Maintain
                        {
                            ShowList.Add("Maintain");
                        }
                        else if (esm > irm && esm > spm)//S Maintain
                        {
                            ShowList.Add("Maintain");
                        }
                        else //EQUAL
                        {
                            ShowList.Add("Maintain");
                        }
                        SetRuleTextList(ShowList);
                    }
                    else
                    {
                        //Fifth Rule
                        ShowList.Add("Rule:\nN5");
                        int irm = 0;
                        int spm = 0;
                        int esm = 0;
                        irm += _fuzzyGameData.Phase[0];
                        spm += _fuzzyGameData.Phase[1];
                        esm += _fuzzyGameData.Phase[2];
                        ShowList.Add(string.Join(", ", _fuzzyGameData.Phase));
                        // ShowList.Add(irm);
                        // ShowList.Add(spm);
                        // ShowList.Add(esm);
                        if (irm > spm && irm > esm)
                        { //F
                          //Increase
                            ShowList.Add("Increase");
                            difficultyState++;
                        }
                        else if (spm > irm && spm > esm)
                        { //M
                          //Maintain
                            ShowList.Add("Maintain");
                        }
                        else if (esm > irm && esm > spm)
                        { //S
                          //Maintain
                            ShowList.Add("Maintain");
                        }
                        else
                        {
                            //Equal
                            ShowList.Add("Maintain");
                        }
                        SetRuleTextList(ShowList);
                        // End Fifth Rule
                        /////////////////////////////////
                        // Sixth Rule
                        Debug.Log(UserFuzzyData[UserFuzzyData.Count - 2].FalseMatch);
                        Debug.Log(UserFuzzyData[UserFuzzyData.Count - 2].TotalMatch);
                        Debug.Log(UserFuzzyData[UserFuzzyData.Count - 1].FalseMatch);
                        Debug.Log(UserFuzzyData[UserFuzzyData.Count - 1].TotalMatch);
                        Debug.Log(_fuzzyGameData.FalseMatch);
                        Debug.Log(_fuzzyGameData.TotalMatch);
                        List<float> _fmr = new List<float>() { (float)UserFuzzyData[UserFuzzyData.Count - 2].FalseMatch / UserFuzzyData[UserFuzzyData.Count - 2].TotalMatch, (float)UserFuzzyData[UserFuzzyData.Count - 1].FalseMatch / UserFuzzyData[UserFuzzyData.Count - 1].TotalMatch };
                        var (rfmr, rmadr) = CalculateFuzzyFMR(_fmr, (float)_fuzzyGameData.FirstMatchTime / _fuzzyGameData.TotalMatch);
                        ShowList.Add("Rule:\nN6");
                        ShowList.Add(rfmr);
                        ShowList.Add(rmadr);
                        string fmrPerformance = GetPerformanceLevel(rfmr, rmadr);
                        if (fmrPerformance == "Good")
                        {
                            difficultyState++;
                            ShowList.Add("Increase");
                        }
                        else if (fmrPerformance == "Maintain")
                        {
                            ShowList.Add("Maintain");
                        }
                        else
                        {
                            difficultyState--;
                            ShowList.Add("Decrease");
                        }
                        SetRuleTextList(ShowList);
                        // End Sixth Rule
                        /////////////////////////////////
                        // Seventh Rule
                        Debug.Log(UserFuzzyData[UserFuzzyData.Count - 2].FirstMatchTime);
                        Debug.Log(UserFuzzyData[UserFuzzyData.Count - 1].FirstMatchTime);
                        Debug.Log(_fuzzyGameData.FirstMatchTime);
                        List<float> _fmt = new List<float>() { UserFuzzyData[UserFuzzyData.Count - 2].FirstMatchTime, UserFuzzyData[UserFuzzyData.Count - 1].FirstMatchTime };
                        var (rmt, rmadt) = CalculateFuzzyMatchTime(_fmt, _fuzzyGameData.FirstMatchTime);
                        ShowList.Add("Rule:\nN7");
                        ShowList.Add(rmt);
                        ShowList.Add(rmadt);
                        string fmtPerformance = GetPerformanceLevel(rmt, rmadt);
                        if (fmtPerformance == "Good")
                        {
                            difficultyState++;
                            ShowList.Add("Increase");
                        }
                        else if (fmtPerformance == "Maintain")
                        {
                            ShowList.Add("Maintain");
                        }
                        else
                        {
                            difficultyState--;
                            ShowList.Add("Decrease");
                        }
                        SetRuleTextList(ShowList);
                        // End Seventh Rule
                    }
                }

            }
        }
        else
        {
            //N3
            gameInComplete++;
            difficultyState--;
            SetRuleText("Rule:\nN3, " + _fuzzyGameData.Complete.ToString());
        }
        if (redPoint)
        {
            if(mtDiff){
                //N13
                //CIT
                Debug.Log("CIT");
                mtDiff = false;
            }
            else{
                mtDiff = true;
            }
        }
        else
        {
            OutputCalculate(difficultyState);
        }

        if (UserFuzzyData.Count == 4)
        {
            UserFuzzyData.RemoveAt(0);
        }
        UserFuzzyData.Add(_fuzzyGameData);

        List<int> _upTemp = DLS.GetUploadProperties();
        FirebaseManagerV2.Instance.UpdateFuzzyPostGameStage(new List<int>() { gameCount, gameComplete, gameInComplete, mtDiff ? 1 : 0, _upTemp[0], _upTemp[1], _upTemp[2] ,minigameCount}, CompleteGameID);
        _fuzzyGameData.GameID = gameCount.ToString();
        FirebaseManagerV2.Instance.UploadFuzzyGameData(_fuzzyGameData);

        ShowList.Clear();
        ShowRuleText();
    }
    private void OutputCalculate(int dival){
        if(dival!=0){
            if(dival > 0){
                    DLS.IncreaseDifficulty();
            }
            else{
                DLS.DecreaseDifficulty();
            }
        }
        else{
            if(mtDiff){
                //CIT
                Debug.Log("CIT");
                mtDiff = false;
                //N13
            }
            else{
                mtDiff = true;
            }
        }
    }
    private float CalculateMedian(List<float> values)
    {
        if (values == null || values.Count == 0) return 0f;

        values.Sort();
        int mid = values.Count / 2;

        if (values.Count % 2 == 0)
            return (values[mid - 1] + values[mid]) / 2.0f;
        else
            return values[mid];
    }

    // Calculate Median Absolute Deviation (MAD)
    private float CalculateMAD(List<float> values, float median)
    {
        List<float> deviations = values.Select(x => Mathf.Abs(x - median)).ToList();
        return CalculateMedian(deviations);
    }

    // Compute Fuzzy Membership Function for False Match Rate (FMR)


    //ตัวอย่างค่า fmr(false match rate) = [0.33,0.42,0.46,0.25] #ค่าสุดท้ายคือด่านปัจจุบัน 0.25
    public (float RFMR, float RMAD) CalculateFuzzyFMR(List<float> pastLevelsFMR, float currentFMR)
    {
        float medianFMR = CalculateMedian(pastLevelsFMR);
        //หา Median โดยใช้แค่ 3 ด่านก่อนหน้า จะได้ (0.33+0.42+0.46)/3 = 20

        float madFMR = CalculateMAD(pastLevelsFMR, medianFMR);
        //นำค่า Median ที่ได้คำนวณหา MAD ของ 3 ด่านก่อนหน้า
        //โดย นำค่าของสามด่านแต่ละค่า มาลบด้วย Median แล้วทำ Absolute จะได้ = [0.07,0.02,0.06]
        //จากนั้นนำค่าที่ได้หา Median อีกครั้งนึง จะได้เป็น MADFMR = 0.06

        float RFMR = currentFMR / medianFMR; //RFMR จะได้ค่าเท่ากับ ค่า FMR ด่านปัจจุบัน หารด้วย Median 0.25/ 0.40 = 0.625
        float RMAD = madFMR / medianFMR; //RMAD จะได้ค่าเท่ากับ ค่า MADFMR หารด้วย Median  0.06 / 0.40  = 0.15
        return (RFMR, RMAD);
    }

    // Compute Fuzzy Membership Function for Match Time (RMT)
    public (float RMT, float RMAD) CalculateFuzzyMatchTime(List<float> pastLevelsMatchTime, float currentMatchTime)
    {
        float medianMatchTime = CalculateMedian(pastLevelsMatchTime);
        float madMatchTime = CalculateMAD(pastLevelsMatchTime, medianMatchTime);

        float RMT = currentMatchTime / medianMatchTime;
        float RMAD = madMatchTime / medianMatchTime;

        return (RMT, RMAD);
    }

    // Determine the performance level based on the given thresholds
    private string GetPerformanceLevel(float ratio, float deviation)
    {
        //ratio คือ RFMR = 0.625
        //deviation คือ RMAD = 0.15
        float lowThreshold = ratio - deviation;
        float highThreshold = ratio + deviation;
        //หา Threshold
        //low = 0.475
        //high = 0.775

        if (lowThreshold >= 1.0f) return "Low";
        if (highThreshold <= 1.0f) return "Good";
        return "Maintain";
        //เมื่อนำมาเทียบจะได้ Performance ในระดับที่ดี

    }
    public void PostSpecialTaskStage()
    {
        if(minigameCount >= 2){
            //N8
            //N9
            //N10
            //ITS BROKEN, NEED FIX!!!!!!!!!
        }
    }
    public void PostDailyStage()
    {
        dayPassed++;
        FirebaseManagerV2.Instance.UpdateDayPassed(dayPassed);
        if (dayPassed > 1) // Second Rule
        {
            SetRuleText("Rule:\nN2, " + dayPassed.ToString());
        }
        ShowRuleText();
    }
    public void SetDifficulty(int val, bool gmode, int cdiff)
    {
        DLS.SetDifficulty(val, gmode, cdiff);
    }
    public void SetGameProperties(List<int> FuzzyProperties, List<int> completeGameID, int dp)
    {
        gameCount = FuzzyProperties[0];
        gameComplete = FuzzyProperties[1];
        gameInComplete = FuzzyProperties[2];
        mtDiff = FuzzyProperties[3] == 1 ? true : false;
        DLS.SetDifficulty(FuzzyProperties[4], FuzzyProperties[5] == 1 ? true : false, FuzzyProperties[6]);
        minigameCount = FuzzyProperties[7];
        CompleteGameID = completeGameID;
        dayPassed = dp;
        if (dayPassed > 1)
        {
            isFirstDay = false;
        }
    }
    public void RuntimeText()
    {
        vrbBox.text = string.Format("{0}mtDiff: {1}\nisFirstDay: {2}", DLS.GetParameterInfo(), mtDiff, isFirstDay);

    }
    public void StopRuntimeText()
    {
        if (IsInvoking("RuntimeText"))
        {
            CancelInvoke("RuntimeText");
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
    public void SetRuleText(string ruleText)
    {
        RuleTextList.Add(ruleText);
        ShowList.Clear();
    }
    public void SetRuleTextList(List<object> textList)
    {
        string rtext = "";
        foreach (var item in textList)
        {
            rtext += item.ToString();
            rtext += ", ";
        }
        rtext = rtext.Substring(0, rtext.Length - 2);
        rtext += "\n";
        SetRuleText(rtext);
        ShowList.Clear();
    }
    public void ShowRuleText()
    {
        ruleBox.text = "";
        foreach (var item in RuleTextList)
        {
            ruleBox.text += item;
            ruleBox.text += "\n";
        }
        RuleTextList.Clear();
    }
}

public class DifficultyLevelSequence
{
    List<List<object>> gridVal = new List<List<object>> {
        new List<object> { PairType.FOUR, GameLayout.GRID },
        new List<object> { PairType.SIX, GameLayout.GRID },
        new List<object> { PairType.EIGHT, GameLayout.GRID }
    };
    List<List<object>> randVal = new List<List<object>>{
        new List<object> { PairType.FOUR, GameLayout.RANDOM },
        new List<object> { PairType.SIX, GameLayout.RANDOM },
        new List<object> { PairType.EIGHT, GameLayout.RANDOM },
        new List<object> { PairType.EIGHT, GameLayout.RANDOM }
    };
    bool gridMode = true;
    int currentDiff = 0;
    int currLevel = 0;
    public void IncreaseDifficulty()
    {
        currLevel++;
        if (gridMode)
        {
            if (currLevel == 3)
            {
                gridMode = false;
            }
        }
        else
        {
            if (currLevel == 4)
            {
                currLevel = 3;
                //NEED TO CHANGE currentDiff (Image Type)
                Debug.Log("CIT");
                
            }
        }

    }
    public void DecreaseDifficulty()
    {
        currLevel--;
        if (gridMode)
        {
            if (currLevel == -1)
            {
                currLevel = 0;
                //NEED TO CHANGE currentDiff (Image Type)
                Debug.Log("CIT");
            }
        }
        else
        {
            if (currLevel == -1)
            {
                currLevel = 0;
                gridMode = true;
            }
        }
    }
    public void CompareDifficulty(int cd, bool ig)
    { //SHOWCASE
        if (!ig)
        {
            cd++;
        }
        if (cd > 3)
        {
            cd = 3;
        }
    }

    public List<object> GetDifficulty()
    {
        return new List<object> { gridMode, gridMode ? gridVal[currLevel] : randVal[currLevel] };
    }
    public void SetDifficulty(int val, bool gmode, int cdiff)
    {
        currLevel = val;
        gridMode = gmode;
        currentDiff = cdiff;
    }
    public List<int> GetUploadProperties()
    {
        return new List<int>() { gridMode ? 1 : 0, currLevel, currentDiff };
    }
    public string GetParameterInfo()
    {
        return string.Format("currentDiff: {0} \ncurrLevel: {1}\ngridMode: {2}\n", currentDiff, currLevel, gridMode);
    }

}