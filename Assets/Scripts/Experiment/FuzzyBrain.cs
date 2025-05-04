using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MatchingGame.Gameplay;
using TMPro;
using UnityEngine.Timeline;
using Model;
using System.Linq;
using Unity.VisualScripting;
using System.Drawing.Drawing2D;
using System;

public class FuzzyBrain : MonoSingleton<FuzzyBrain>
{
    // Start is called before the first frame update
    [SerializeField]
    public bool debugMode;
    public List<FuzzyGameData> UserFuzzyData = new List<FuzzyGameData>();
    public List<SpecialFuzzyData> UserSpecialData = new List<SpecialFuzzyData>();
    public TMP_Text vrbBox;
    public TMP_Text ruleBox;
    public DifficultyLevelSequence DLS;

    bool mtDiff;
    bool isFirstDay;
    public int gameCount;
    int gameComplete;
    int gameInComplete;
    public int minigameCount;
    int dayPassed;
    int ruleCount;
    List<float> difficultyState = new List<float>() { 0, 0, 0 };
    List<object> ShowList = new List<object>();
    List<string> RuleTextList = new List<string>();
    List<int> CompleteGameID = new List<int>();

    public override void Init()
    {
        DLS = new DifficultyLevelSequence();
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
        UserFuzzyData.Clear();
        UserSpecialData.Clear();
        gameCount = 0;
        gameComplete = 0;
        gameInComplete = 0;
        isFirstDay = true;
        mtDiff = false;
        dayPassed = 0;
        ruleCount = 0;
        difficultyState = new List<float>() { 0, 0, 0 };
        CompleteGameID = new List<int>();
        UserSpecialData.Clear();
    }
    public void PostGameStage(FuzzyGameData _fuzzyGameData)
    {
        ShowList.Clear();
        gameCount++;
        bool redPoint = false;
        difficultyState = new List<float>() { 0, 0, 0 };
        if (_fuzzyGameData.Complete)
        {
            gameComplete++;
            if (_fuzzyGameData.PauseUsed)
            {
                //N4
                SetRuleText("N4, " + _fuzzyGameData.PauseUsed.ToString());
                difficultyState[1] = 1.0f;
            }
            else
            {
                CompleteGameID.Add(int.Parse(_fuzzyGameData.GameID));
                if (gameComplete <= 2)
                {
                    //REDPOINT
                    redPoint = true;
                }
                else
                {
                    //N11
                    ShowList.Add("N11");
                    ShowList.Add(string.Join(", ", _fuzzyGameData.Helper));
                    if (_fuzzyGameData.HelperSeq.Count == 0)
                    {
                        difficultyState[2] += 0.7f;
                        difficultyState[1] += 0.3f;
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
                                difficultyState[1] += 0.7f;
                                difficultyState[0] += 0.3f;
                                ShowList.Add("Maintain");
                            }
                            else
                            {
                                difficultyState[2] += 0.7f;
                                difficultyState[1] += 0.3f;
                                ShowList.Add("Increase");
                            }
                        }
                        else
                        {
                            if (_fuzzyGameData.HelperSeq[0] == "Passive")
                            {
                                difficultyState[2] += 0.7f;
                                difficultyState[1] += 0.3f;
                                ShowList.Add("Increase");

                            }
                            else
                            {
                                difficultyState[1] += 0.7f;
                                difficultyState[0] += 0.3f;
                                ShowList.Add("Maintain");
                            }
                        }
                    }
                    SetRuleTextList(ShowList);
                    //N12
                    ShowList.Add("N12");
                    List<int> n12irm = new List<int>();
                    for (int i = 0; i < UserFuzzyData.Count; i++)
                    {
                        n12irm.Add(UserFuzzyData[i].Phase[0]);
                    }
                    n12irm.Add(_fuzzyGameData.Phase[0]);
                    ShowList.Add(string.Join(", ", n12irm));
                    var medIRM = CalculateMedian(n12irm);
                    ShowList.Add(medIRM);
                    if ((float)_fuzzyGameData.Phase[0] < medIRM)
                    {
                        if (_fuzzyGameData.GameLevel < UserFuzzyData[UserFuzzyData.Count - 1].GameLevel)
                        {
                            difficultyState[0] += 0.7f;
                            difficultyState[1] += 0.15f;
                            difficultyState[2] += 0.15f;
                            ShowList.Add("Easier");
                            ShowList.Add("Decrease");

                        }
                        else if (_fuzzyGameData.GameLevel == UserFuzzyData[UserFuzzyData.Count - 1].GameLevel)
                        {
                            difficultyState[0] += 0.15f;
                            difficultyState[1] += 0.7f;
                            difficultyState[2] += 0.15f;
                            ShowList.Add("Equal");
                            ShowList.Add("Maintain");
                        }
                        else
                        {
                            difficultyState[0] += 0.15f;
                            difficultyState[1] += 0.7f;
                            difficultyState[2] += 0.15f;
                            ShowList.Add("Harder");
                            ShowList.Add("Maintain");
                        }
                    }
                    else if ((float)_fuzzyGameData.Phase[0] == medIRM)
                    {
                        if (_fuzzyGameData.GameLevel < UserFuzzyData[UserFuzzyData.Count - 1].GameLevel)
                        {
                            difficultyState[0] += 0.15f;
                            difficultyState[1] += 0.7f;
                            difficultyState[2] += 0.15f;
                            ShowList.Add("Easier");
                            ShowList.Add("Maintain");
                        }
                        else if (_fuzzyGameData.GameLevel == UserFuzzyData[UserFuzzyData.Count - 1].GameLevel)
                        {
                            difficultyState[0] += 0.15f;
                            difficultyState[1] += 0.7f;
                            difficultyState[2] += 0.15f;
                            ShowList.Add("Equal");
                            ShowList.Add("Maintain");
                        }
                        else
                        {
                            difficultyState[0] += 0.15f;
                            difficultyState[1] += 0.15f;
                            difficultyState[2] += 0.7f;
                            ShowList.Add("Harder");
                            ShowList.Add("Increase");
                        }
                    }
                    else
                    {
                        if (_fuzzyGameData.GameLevel < UserFuzzyData[UserFuzzyData.Count - 1].GameLevel)
                        {
                            difficultyState[0] += 0.15f;
                            difficultyState[1] += 0.7f;
                            difficultyState[2] += 0.15f;
                            ShowList.Add("Easier");
                            ShowList.Add("Maintain");
                        }
                        else if (_fuzzyGameData.GameLevel == UserFuzzyData[UserFuzzyData.Count - 1].GameLevel)
                        {
                            difficultyState[0] += 0.15f;
                            difficultyState[1] += 0.15f;
                            difficultyState[2] += 0.7f;
                            ShowList.Add("Equal");
                            ShowList.Add("Increase");
                        }
                        else
                        {
                            difficultyState[0] += 0.15f;
                            difficultyState[1] += 0.15f;
                            difficultyState[2] += 0.7f;
                            ShowList.Add("Harder");
                            ShowList.Add("Increase");
                        }
                    }
                    SetRuleTextList(ShowList);
                    if (gameComplete == 3)
                    {
                        //N1
                        ShowList.Clear();
                        ShowList.Add("N1");
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
                            difficultyState[2] += 1.0f;
                        }
                        else if (spm > irm && spm > esm)//M Maintain
                        {
                            ShowList.Add("Maintain");
                            difficultyState[1] += 1.0f;
                        }
                        else if (esm > irm && esm > spm)//S Maintain
                        {
                            ShowList.Add("Maintain");
                            difficultyState[1] += 1.0f;
                        }
                        else //EQUAL
                        {
                            ShowList.Add("Maintain");
                            difficultyState[1] += 1.0f;
                        }
                        SetRuleTextList(ShowList);
                    }
                    else
                    {
                        //Fifth Rule
                        ShowList.Add("N5");
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
                            difficultyState[2] += 0.7f;
                            difficultyState[1] += 0.15f;
                            difficultyState[0] += 0.15f;
                        }
                        else if (spm > irm && spm > esm)
                        { //M
                          //Maintain
                            ShowList.Add("Maintain");
                            difficultyState[2] += 0.15f;
                            difficultyState[1] += 0.7f;
                            difficultyState[0] += 0.15f;
                        }
                        else if (esm > irm && esm > spm)
                        { //S
                          //Maintain
                            ShowList.Add("Decrease");
                            difficultyState[2] += 0.15f;
                            difficultyState[1] += 0.15f;
                            difficultyState[0] += 0.7f;
                        }
                        else
                        {
                            //Equal
                            ShowList.Add("Maintain");
                            difficultyState[2] += 0.15f;
                            difficultyState[1] += 0.7f;
                            difficultyState[0] += 0.15f;
                        }
                        SetRuleTextList(ShowList);
                        // End Fifth Rule
                        /////////////////////////////////
                        // Sixth Rule
                        ShowList.Add("N6");
                        ShowList.Add(UserFuzzyData[UserFuzzyData.Count - 2].FalseMatch);
                        ShowList.Add(UserFuzzyData[UserFuzzyData.Count - 2].TotalMatch);
                        ShowList.Add(UserFuzzyData[UserFuzzyData.Count - 1].FalseMatch);
                        ShowList.Add(UserFuzzyData[UserFuzzyData.Count - 1].TotalMatch);
                        ShowList.Add(_fuzzyGameData.FalseMatch);
                        ShowList.Add(_fuzzyGameData.TotalMatch);
                        List<float> _fmr = new List<float>() { (float)UserFuzzyData[UserFuzzyData.Count - 2].FalseMatch / UserFuzzyData[UserFuzzyData.Count - 2].TotalMatch, (float)UserFuzzyData[UserFuzzyData.Count - 1].FalseMatch / UserFuzzyData[UserFuzzyData.Count - 1].TotalMatch };
                        var (rfmr, rmadr) = CalculateFuzzyFMR(_fmr, (float)_fuzzyGameData.FalseMatch / _fuzzyGameData.TotalMatch);
                        ShowList.Add(rfmr);
                        ShowList.Add(rmadr);
                        // string fmrPerformance = GetPerformanceLevel(rfmr, rmadr);
                        float lowerr = 1f - 2f * rmadr;
                        float upperr = 1f + 2f * rmadr;
                        ShowList.Add(lowerr);
                        ShowList.Add(upperr);

                        float afmr = (float)_fuzzyGameData.FalseMatch / _fuzzyGameData.TotalMatch;
                        float inc_val = 0f;
                        float man_val = 0f;
                        float dec_val = 0f;
                        if (lowerr < 0f)
                        {
                            lowerr = 0f;
                        }
                        if (afmr == 0)
                        {
                            inc_val = 1f;
                            man_val = 0f;
                            dec_val = 0f;
                        }
                        else if ((rfmr == 0 || (upperr == 0 && lowerr == 0)) && afmr > CalculateMedian(_fmr))
                        {
                            inc_val = 0f;
                            man_val = 1f;
                            dec_val = 0f;
                        }
                        else if (rfmr > (upperr + lowerr))
                        {
                            inc_val = 0f;
                            man_val = 0f;
                            dec_val = 1f;
                        }
                        else if (rfmr == upperr && rfmr == lowerr)
                        {
                            inc_val = 0f;
                            man_val = 1f;
                            dec_val = 0f;
                        }
                        else
                        {
                            float[] x_range = Linspace(0f, 2f, 100);
                            float[] imf = new float[4] { 0f, 0f, lowerr, (lowerr + upperr) / 2 };
                            float[] mmf = new float[3] { lowerr, (lowerr + upperr) / 2, upperr };
                            float[] dmf = new float[4] { (lowerr + upperr) / 2, upperr, 100, 100 };
                            float[] increase_mf = Trapmf(x_range, imf);
                            float[] maintain_mf = Trimf(x_range, mmf);
                            float[] Decrease_mf = Trapmf(x_range, dmf);

                            inc_val = InterpMembership(x_range, increase_mf, rfmr);
                            man_val = InterpMembership(x_range, maintain_mf, rfmr);
                            dec_val = InterpMembership(x_range, Decrease_mf, rfmr);

                        }
                        ShowList.Add(inc_val);
                        ShowList.Add(man_val);
                        ShowList.Add(dec_val);
                        difficultyState[0] += dec_val;
                        difficultyState[1] += man_val;
                        difficultyState[2] += inc_val;
                        SetRuleTextList(ShowList);
                        // End Sixth Rule
                        /////////////////////////////////
                        // Seventh Rule
                        ShowList.Add("N7");
                        ShowList.Add(UserFuzzyData[UserFuzzyData.Count - 2].FirstMatchTime);
                        ShowList.Add(UserFuzzyData[UserFuzzyData.Count - 1].FirstMatchTime);
                        ShowList.Add(_fuzzyGameData.FirstMatchTime);
                        List<float> _fmt = new List<float>() { UserFuzzyData[UserFuzzyData.Count - 2].FirstMatchTime, UserFuzzyData[UserFuzzyData.Count - 1].FirstMatchTime };
                        var (rmt, rmadt) = CalculateFuzzyMatchTime(_fmt, _fuzzyGameData.FirstMatchTime);
                        ShowList.Add(rmt);
                        ShowList.Add(rmadt);
                        float lowert = 1 - rmadt;
                        float uppert = 1 + rmadt;

                        ShowList.Add(lowert);
                        ShowList.Add(uppert);
                        float afmt = (float)_fuzzyGameData.FirstMatchTime;
                        inc_val = 0f;
                        man_val = 0f;
                        dec_val = 0f;

                        if (lowert < 0f)
                        {
                            lowert = 0f;
                        }
                        if (afmt == 0)
                        {
                            inc_val = 1f;
                            man_val = 0f;
                            dec_val = 0f;
                        }
                        else if ((rmt == 0 || (uppert == 0 && lowert == 0)) && afmt > CalculateMedian(_fmt))
                        {
                            inc_val = 0f;
                            man_val = 1f;
                            dec_val = 0f;
                        }
                        else if (rmt > (uppert + lowert))
                        {
                            inc_val = 0f;
                            man_val = 0f;
                            dec_val = 1f;
                        }
                        else if (rmt == uppert && rmt == lowert)
                        {
                            inc_val = 0f;
                            man_val = 1f;
                            dec_val = 0f;
                        }
                        else
                        {
                            float[] x_range = Linspace(0f, 2f, 100);
                            float[] imf = new float[4] { 0f, 0f, lowert, (lowert + uppert) / 2 };
                            float[] mmf = new float[3] { lowert, (lowert + uppert) / 2, uppert };
                            float[] dmf = new float[4] { (lowert + uppert) / 2, uppert, 100, 100 };
                            float[] increase_mf = Trapmf(x_range, imf);
                            float[] maintain_mf = Trimf(x_range, mmf);
                            float[] Decrease_mf = Trapmf(x_range, dmf);
                            inc_val = InterpMembership(x_range, increase_mf, rmt);
                            man_val = InterpMembership(x_range, maintain_mf, rmt);
                            dec_val = InterpMembership(x_range, Decrease_mf, rmt);

                        }
                        ShowList.Add(inc_val);
                        ShowList.Add(man_val);
                        ShowList.Add(dec_val);
                        difficultyState[0] += dec_val;
                        difficultyState[1] += man_val;
                        difficultyState[2] += inc_val;
                        SetRuleTextList(ShowList);
                        // End Seventh Rule
                    }
                }
                if (UserFuzzyData.Count >= 4)
                {
                    UserFuzzyData.RemoveAt(0);
                }
                UserFuzzyData.Add(_fuzzyGameData);
            }
        }
        else
        {
            //N3
            gameInComplete++;
            difficultyState[0] += 1.0f;
            SetRuleText("N3, " + _fuzzyGameData.Complete.ToString());

        }
        if (gameCount > 2)
        {
            if (redPoint)
            {
                if (mtDiff)
                {
                    //N13
                    DLS.ChangeImageType();
                    SetRuleText("N13,CIT");
                    mtDiff = false;
                }
                else
                {
                    mtDiff = true;
                    SetRuleText("N15, Maintain");
                }
            }
            else
            {
                OutputCalculate(difficultyState);
            }
        }
        else
        {
            SetRuleText("gameCount, " + gameCount.ToString() + ", Rule not activate");
            difficultyState[0] = 0f;
            difficultyState[1] = 0f;
            difficultyState[2] = 0f;
        }


        List<int> _upTemp = DLS.GetUploadProperties();
        FirebaseManagerV2.Instance.UpdateFuzzyPostGameStage(new List<int>() { gameCount, gameComplete, gameInComplete, mtDiff ? 1 : 0, _upTemp[1], _upTemp[0], _upTemp[2], minigameCount }, CompleteGameID);
        _fuzzyGameData.GameID = (gameCount - 1).ToString();
        FirebaseManagerV2.Instance.UploadFuzzyGameData(_fuzzyGameData);
        ShowList.Clear();
        ShowRuleText();
    }
    private void OutputCalculate(List<float> dival)
    {
        int temp_diff = 0;
        if (dival[0] == dival[1] && dival[0] == dival[2])
        {
            temp_diff = 0;
        }
        else if (dival[0] == dival[1] && dival[0] > dival[2])
        {
            temp_diff = -1;
        }
        else if (dival[0] == dival[2] && dival[0] > dival[1])
        {
            temp_diff = -1;
        }
        else if (dival[1] == dival[2] && dival[1] > dival[0])
        {
            temp_diff = 0;
        }
        else if (dival[0] > dival[1] && dival[0] > dival[2])
        {
            temp_diff = -1;
        }
        else if (dival[1] > dival[0] && dival[1] > dival[2])
        {
            temp_diff = 0;
        }
        else if (dival[2] > dival[0] && dival[2] > dival[1])
        {
            temp_diff = 1;
        }
        if (temp_diff != 0)
        {
            if (temp_diff > 0)
            {
                if (DLS.IncreaseDifficulty())
                {
                    SetRuleText("N14,max,CIT");
                }
            }
            else
            {
                if (DLS.DecreaseDifficulty())
                {
                    SetRuleText("N14,min,CIT");
                }
            }
        }
        else
        {
            if (mtDiff)
            {
                DLS.ChangeImageType();
                SetRuleText("N13,CIT");
                mtDiff = false;
            }
            else
            {
                mtDiff = true;
            }
        }
        SetRuleText("Increase:" + dival[2].ToString() + ", Maintain:" + dival[1].ToString() + ", Decrease:" + dival[0].ToString());
        difficultyState[0] = 0f;
        difficultyState[1] = 0f;
        difficultyState[2] = 0f;
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
    private float CalculateMedian(List<int> values)
    {
        if (values == null || values.Count == 0) return 0;

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
        float RFMR;
        float RMAD;
        //หา Median โดยใช้แค่ 3 ด่านก่อนหน้า จะได้ (0.33+0.42+0.46)/3 = 20
        if (medianFMR == 0)
        {
            RFMR = 0;
            RMAD = 0;
            return (RFMR, RMAD);
        }
        else
        {
            float madFMR = CalculateMAD(pastLevelsFMR, medianFMR);
            //นำค่า Median ที่ได้คำนวณหา MAD ของ 3 ด่านก่อนหน้า
            //โดย นำค่าของสามด่านแต่ละค่า มาลบด้วย Median แล้วทำ Absolute จะได้ = [0.07,0.02,0.06]
            //จากนั้นนำค่าที่ได้หา Median อีกครั้งนึง จะได้เป็น MADFMR = 0.06

            RFMR = currentFMR / medianFMR; //RFMR จะได้ค่าเท่ากับ ค่า FMR ด่านปัจจุบัน หารด้วย Median 0.25/ 0.40 = 0.625
            RMAD = madFMR / medianFMR; //RMAD จะได้ค่าเท่ากับ ค่า MADFMR หารด้วย Median  0.06 / 0.40  = 0.15
        }

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
    private float[] Trimf(float[] x, float[] abc)
    {
        if (abc.Length != 3)
            throw new ArgumentException("abc parameter must have exactly three elements.");

        float a = abc[0];
        float b = abc[1];
        float c = abc[2];

        if (!(a <= b && b <= c))
            throw new ArgumentException("abc requires the three elements a <= b <= c.");

        float[] y = new float[x.Length];

        // Left side: a < x < b
        if (a != b)
        {
            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] > a && x[i] < b)
                {
                    y[i] = (x[i] - a) / (b - a);
                }
            }
        }

        // Right side: b < x < c
        if (b != c)
        {
            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] > b && x[i] < c)
                {
                    y[i] = (c - x[i]) / (c - b);
                }
            }
        }

        // Peak: x == b
        for (int i = 0; i < x.Length; i++)
        {
            if (Mathf.Approximately(x[i], b))
            {
                y[i] = 1f;
            }
        }

        return y;
    }
    private float[] Trapmf(float[] x, float[] abcd)
    {
        if (abcd.Length != 4)
            throw new ArgumentException("abcd parameter must have exactly four elements.");

        float a = abcd[0];
        float b = abcd[1];
        float c = abcd[2];
        float d = abcd[3];

        if (!(a <= b && b <= c && c <= d))
            throw new ArgumentException("abcd requires the four elements a <= b <= c <= d.");

        float[] y = new float[x.Length];
        for (int i = 0; i < y.Length; i++)
            y[i] = 1f;

        // Left slope: x <= b
        for (int i = 0; i < x.Length; i++)
        {
            if (x[i] <= b)
            {
                float[] slice = new float[] { x[i] };
                float[] res = Trimf(slice, new float[] { a, b, b });
                y[i] = res[0];
            }
        }

        // Right slope: x >= c
        for (int i = 0; i < x.Length; i++)
        {
            if (x[i] >= c)
            {
                float[] slice = new float[] { x[i] };
                float[] res = Trimf(slice, new float[] { c, c, d });
                y[i] = res[0];
            }
        }

        // Out of bounds: x < a or x > d
        for (int i = 0; i < x.Length; i++)
        {
            if (x[i] < a || x[i] > d)
            {
                y[i] = 0f;
            }
        }

        return y;
    }
    float[] InterpMembership(float[] x, float[] xmf, float[] xx, bool zeroOutsideX = true)
    {
        float[] result = new float[xx.Length];

        for (int i = 0; i < xx.Length; i++)
        {
            float val = xx[i];

            // Check for extrapolation
            if (val <= x[0])
            {
                result[i] = zeroOutsideX ? 0f : xmf[0];
            }
            else if (val >= x[x.Length - 1])
            {
                result[i] = zeroOutsideX ? 0f : xmf[xmf.Length - 1];
            }
            else
            {
                // Find the interval
                for (int j = 0; j < x.Length - 1; j++)
                {
                    if (val >= x[j] && val <= x[j + 1])
                    {
                        // Linear interpolation
                        float t = (val - x[j]) / (x[j + 1] - x[j]);
                        result[i] = xmf[j] + t * (xmf[j + 1] - xmf[j]);
                        break;
                    }
                }
            }
        }

        return result;
    }

    float InterpMembership(float[] x, float[] xmf, float xx, bool zeroOutsideX = true)
    {
        return InterpMembership(x, xmf, new float[] { xx }, zeroOutsideX)[0];
    }

    float[] Linspace(float start, float stop, int num)
    {
        float[] result = new float[num];
        float step = (stop - start) / (num - 1);

        for (int i = 0; i < num; i++)
        {
            result[i] = start + step * i;
        }

        return result;
    }

    public void PostSpecialTaskStage(SpecialFuzzyData _specialgameData)
    {
        difficultyState = new List<float>() { 0, 0, 0 };
        minigameCount++;
        if (minigameCount >= 2)
        {
            //N8
            ShowList.Add("N8");
            List<float> _mpt = new List<float>();
            var ampt = _specialgameData.TimeUsed.Sum()/_specialgameData.TimeUsed.Count();
            ShowList.Add(ampt);
            foreach (var item in UserSpecialData)
            {
                _mpt.Add(item.TimeUsed.Sum()/item.TimeUsed.Count());
                ShowList.Add(_mpt[^1]);
            }
            
            var (rmpt, rmadpt) = CalculateFuzzyFMR(_mpt, ampt);
            ShowList.Add(rmpt);
            ShowList.Add(rmadpt);
            float lowerpt = 1f - rmadpt;
            float upperpt = 1f + rmadpt;
            ShowList.Add(lowerpt);
            ShowList.Add(upperpt);
            float inc_val = 0f;
            float man_val = 0f;
            float dec_val = 0f;
            if (lowerpt < 0f)
            {
                lowerpt = 0f;
            }
            if (rmpt == 0)
            {
                inc_val = 1f;
                man_val = 0f;
                dec_val = 0f;
            }
            else if ((rmpt == 0 || (upperpt == 0 && lowerpt == 0)) && rmpt > CalculateMedian(_mpt))
            {
                inc_val = 0f;
                man_val = 1f;
                dec_val = 0f;
            }
            else if (rmpt > (upperpt + lowerpt))
            {
                inc_val = 0f;
                man_val = 0f;
                dec_val = 1f;
            }
            else if (rmpt == upperpt && rmpt == lowerpt)
            {
                inc_val = 0f;
                man_val = 1f;
                dec_val = 0f;
            }
            else
            {
                float[] x_range = Linspace(0f, 2f, 100);
                float[] imf = new float[4] { 0f, 0f, lowerpt, (lowerpt + upperpt) / 2 };
                float[] mmf = new float[3] { lowerpt, (lowerpt + upperpt) / 2, upperpt };
                float[] dmf = new float[4] { (lowerpt + upperpt) / 2, upperpt, 100, 100 };
                float[] increase_mf = Trapmf(x_range, imf);
                float[] maintain_mf = Trimf(x_range, mmf);
                float[] Decrease_mf = Trapmf(x_range, dmf);

                inc_val = InterpMembership(x_range, increase_mf, rmpt);
                man_val = InterpMembership(x_range, maintain_mf, rmpt);
                dec_val = InterpMembership(x_range, Decrease_mf, rmpt);

            }
            ShowList.Add(inc_val);
            ShowList.Add(man_val);
            ShowList.Add(dec_val);
            difficultyState[0] += dec_val;
            difficultyState[1] += man_val;
            difficultyState[2] += inc_val;
            SetRuleTextList(ShowList);

            //N9
            ShowList.Add("N9");
            List<float> _cc = new List<float>();
            var acc = 1f - _specialgameData.GameScore.Count(n => n == 1) / 20.0f;
            foreach (var item in UserSpecialData)
            {
                _cc.Add(1f - item.GameScore.Count(n => n == 1) / 20.0f);
            }
            var (rcc, rmadc) = CalculateFuzzyFMR(_cc, acc);
            ShowList.Add(rcc);
            ShowList.Add(rmadc);
            float lowercc = 1f - rmadc;
            float uppercc = 1f + rmadc;
            ShowList.Add(lowercc);
            ShowList.Add(uppercc);
            inc_val = 0f;
            man_val = 0f;
            dec_val = 0f;
            if (lowercc < 0f)
            {
                lowercc = 0f;
            }
            if (rcc == 0)
            {
                inc_val = 1f;
                man_val = 0f;
                dec_val = 0f;
            }
            else if ((rcc == 0 || (uppercc == 0 && lowercc == 0)) && rcc > CalculateMedian(_cc))
            {
                inc_val = 0f;
                man_val = 1f;
                dec_val = 0f;
            }
            else if (rcc > (uppercc + lowercc))
            {
                inc_val = 0f;
                man_val = 0f;
                dec_val = 1f;
            }
            else if (rcc == uppercc && rcc == lowercc)
            {
                inc_val = 0f;
                man_val = 1f;
                dec_val = 0f;
            }
            else
            {
                float[] x_range = Linspace(0f, 2f, 100);
                float[] imf = new float[4] { 0f, 0f, lowercc, (lowercc + uppercc) / 2 };
                float[] mmf = new float[3] { lowercc, (lowercc + uppercc) / 2, uppercc };
                float[] dmf = new float[4] { (lowercc + lowercc) / 2, uppercc, 100, 100 };
                float[] increase_mf = Trapmf(x_range, imf);
                float[] maintain_mf = Trimf(x_range, mmf);
                float[] Decrease_mf = Trapmf(x_range, dmf);

                inc_val = InterpMembership(x_range, increase_mf, rcc);
                man_val = InterpMembership(x_range, maintain_mf, rcc);
                dec_val = InterpMembership(x_range, Decrease_mf, rcc);

            }
            ShowList.Add(inc_val);
            ShowList.Add(man_val);
            ShowList.Add(dec_val);
            difficultyState[0] += dec_val;
            difficultyState[1] += man_val;
            difficultyState[2] += inc_val;
            SetRuleTextList(ShowList);

            //N10
            ShowList.Add("N10");
            foreach (var ctl in _specialgameData.ClickTypeList)
            {
                ShowList.Add(ctl);
            }
            int mcount = 0;
            int dcount = 0;
            if (_specialgameData.ClickTypeList[0] > 0)
            {
                mcount++;
            }
            if (_specialgameData.ClickTypeList[1] > 0)
            {
                dcount++;
            }
            if (_specialgameData.ClickTypeList[2] > 0)
            {
                dcount++;
            }
            if (_specialgameData.ClickTypeList[3] > 0)
            {
                dcount++;
            }
            if (mcount == dcount)
            {
                bool compare_check = true;
                if (_specialgameData.ClickTypeList[0] < _specialgameData.ClickTypeList[1])
                {
                    compare_check = false;
                }
                if (_specialgameData.ClickTypeList[0] < _specialgameData.ClickTypeList[2])
                {
                    compare_check = false;
                }
                if (_specialgameData.ClickTypeList[0] < _specialgameData.ClickTypeList[3])
                {
                    compare_check = false;
                }
                if (compare_check)
                {
                    difficultyState[2] = 0f;
                    difficultyState[1] = 0.7f;
                    difficultyState[0] = 0.3f;
                    ShowList.Add("Maintain");
                }
                else
                {
                    difficultyState[2] = 0f;
                    difficultyState[1] = 0.3f;
                    difficultyState[0] = 0.7f;
                    ShowList.Add("Decrease");
                }

            }
            else
            {
                if (mcount > dcount)
                {
                    difficultyState[2] = 0f;
                    difficultyState[1] = 0.7f;
                    difficultyState[0] = 0.3f;
                    ShowList.Add("Maintain");

                }
                else
                {
                    difficultyState[2] = 0f;
                    difficultyState[1] = 0.3f;
                    difficultyState[0] = 0.7f;
                    ShowList.Add("Decrease");
                }
            }

            SetRuleTextList(ShowList);
            OutputCalculate(difficultyState);
        }
        UserSpecialData.Add(_specialgameData);
        FirebaseManagerV2.Instance.UploadSpecialGameData(_specialgameData);
        List<int> _upTemp = DLS.GetUploadProperties();
        FirebaseManagerV2.Instance.UpdateFuzzyPostGameStage(new List<int>() { gameCount, gameComplete, gameInComplete, mtDiff ? 1 : 0, _upTemp[1], _upTemp[0], _upTemp[2], minigameCount }, CompleteGameID);
        ShowList.Clear();
        ShowRuleText();
    }
    public void PostDailyStage()
    {
        dayPassed++;
        FirebaseManagerV2.Instance.UpdateDayPassed(dayPassed);
        if (dayPassed > 1) // Second Rule
        {
            SetRuleText("N2, " + dayPassed.ToString());
            difficultyState[0] = 1f;
            difficultyState[1] = 0f;
            difficultyState[2] = 0f;
            OutputCalculate(difficultyState);
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
        if (completeGameID != null)
        {
            CompleteGameID = completeGameID;
        }
        else
        {
            CompleteGameID = new List<int>();
        }

        dayPassed = dp;
        if (dayPassed > 1)
        {
            isFirstDay = false;
        }
    }
    public void RuntimeText()
    {

        vrbBox.text = string.Format("{0}mtDiff: {1}\nisFirstDay: {2}\ngameCount: {3}\ngameComplete: {4}\nminigameCount: {5}\nTimeFactor: {6}", DLS.GetParameterInfo(), mtDiff, isFirstDay, gameCount, gameComplete, minigameCount, Time.timeScale.ToString());

    }

    public void ChangeTimeFactor(bool inc)
    {
        if (inc)
        {
            Time.timeScale += 1f;
        }
        else
        {
            Time.timeScale -= 1f;
            if (Time.timeScale < 1f)
            {
                Time.timeScale = 1f;
            }
        }
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
        RuleTextList.Add(ruleText + "\n");
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
            // ruleBox.text += "\n";
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
        new List<object> { PairType.EIGHT, GameLayout.RANDOM }
    };
    bool gridMode = true;
    int currentDiff = 0;
    int currLevel = 0;
    public bool IncreaseDifficulty()
    {
        currLevel++;
        if (gridMode)
        {
            if (currLevel == 3)
            {
                gridMode = false;
                currLevel = 2;
            }
        }
        else
        {
            if (currLevel == 3)
            {
                currLevel = 2;
                ChangeImageType();
                return true;

            }
        }
        return false;

    }
    public void ChangeImageType()
    {
        currentDiff = currentDiff + 1;
        if (currentDiff == 4)
        {
            currentDiff = 0;
        }
    }
    public bool DecreaseDifficulty()
    {
        currLevel--;
        if (gridMode)
        {
            if (currLevel == -1)
            {
                currLevel = 0;
                ChangeImageType();
                return true;
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
        return false;
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

    public (PairType, GameLayout, GameDifficult) GetDifficulty()
    {
        if (gridMode)
        {
            PairType pt = (PairType)gridVal[currLevel][0];
            GameLayout gl = (GameLayout)gridVal[currLevel][1];
            GameDifficult gd = (GameDifficult)currentDiff;

            return (pt, gl, gd);
        }
        else
        {
            PairType pt = (PairType)randVal[currLevel][0];
            GameLayout gl = (GameLayout)randVal[currLevel][1];
            GameDifficult gd = (GameDifficult)currentDiff;
            return (pt, gl, gd);
        }
    }

    public (bool, int, int) GetLevelData()
    {
        return (gridMode, currLevel, currentDiff);
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