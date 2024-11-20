using Manager;
using MatchingGame.Gameplay;
using Model;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DailyFeelingManager : MonoBehaviour
{
    [SerializeField] private Image bad;
    [SerializeField] private Image normal;
    [SerializeField] private Image good;
    [SerializeField] private GameObject bg;
    FirebaseManagerV2 fbm;
    private string current_select = "";
    [SerializeField] private List<Sprite> bad_gif = new List<Sprite>();
    [SerializeField] private List<Sprite> normal_gif = new List<Sprite>();
    [SerializeField] private List<Sprite> good_gif = new List<Sprite>();
    private List<int> imgIndex = new List<int>() { 0, 0, 0 };
    private float interval = 0.5f;
    private float current_time;

    private string _state;
    // Start is called before the first frame update

    // Update is called once per frame
    void Start(){
        current_time = interval;
    }
    void Update()
    {
        current_time -= Time.deltaTime;
        if (current_time <= 0.0f)
        {
            current_time = interval;
            switch (current_select)
            {
                case "bad":
                    imgIndex[0] = (imgIndex[0] + 1) % 3;
                    bad.sprite = bad_gif[imgIndex[0]];
                    break;

                case "normal":
                    imgIndex[1] = (imgIndex[1] + 1) % 3;
                    normal.sprite = normal_gif[imgIndex[1]];
                    break;

                case "good":
                    imgIndex[2] = (imgIndex[2] + 1) % 3;
                    good.sprite = good_gif[imgIndex[2]];
                    
                    break;
                case "":
                    imgIndex[0] = (imgIndex[0] + 1) % 3;
                    imgIndex[1] = (imgIndex[1] + 1) % 3;
                    imgIndex[2] = (imgIndex[2] + 1) % 3;
                    bad.sprite = bad_gif[imgIndex[0]];
                    normal.sprite = normal_gif[imgIndex[1]];
                    good.sprite = good_gif[imgIndex[2]];
                    break;
            }
        }

    }

    public void OnSelectedFeelingButton(string val)
    {
        current_select = val;
        switch (val)
        {
            case "bad":
                bad.color = ChangeAlpha(bad, 1f);
                normal.color = ChangeAlpha(normal, 0.39f);
                good.color = ChangeAlpha(good, 0.39f);
                bg.transform.position = bad.transform.position;
                break;
            case "normal":
                bad.color = ChangeAlpha(bad, 0.39f);
                normal.color = ChangeAlpha(normal, 1f);
                good.color = ChangeAlpha(good, 0.39f);
                bg.transform.position = normal.transform.position;
                break;
            case "good":
                bad.color = ChangeAlpha(bad, 0.39f);
                normal.color = ChangeAlpha(normal, 0.39f);
                good.color = ChangeAlpha(good, 1f);
                bg.transform.position = good.transform.position;
                break;
        }
        _state = val;
        bg.SetActive(true);
    }
    Color ChangeAlpha(Image img, float val)
    {
        Color newColor = img.color;
        newColor.a = val;
        return newColor;
    }

    public void SubmitForm()
    {
        if (!bg.activeSelf)
        {
            Debug.Log("Please answer correctly");
            return;
        }
        DailyFeelingResult dailyFeelingResult = new DailyFeelingResult(_state);
        DataManager.Instance.DailyFeelingResultList.Add(dailyFeelingResult);
        if (fbm == null)
        {
            fbm = (FirebaseManagerV2)GameObject.FindObjectOfType(typeof(FirebaseManagerV2));
        }
        FirebaseManagerV2.Instance.UploadDailyFeelingResult(dailyFeelingResult, DataManager.Instance.SmileyoMeterResultList.Count - 1);

        Debug.Log("Save result");
        SequenceManager.Instance.NextSequence();
    }
}
