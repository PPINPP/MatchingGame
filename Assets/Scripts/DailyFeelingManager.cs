using Manager;
using MatchingGame.Gameplay;
using Model;
using System.Linq;
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

    private string _state;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnSelectedFeelingButton(string val)
    {
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
        fbm.UploadDailyFeelingResult(dailyFeelingResult, DataManager.Instance.SmileyoMeterResultList.Count - 1);

        Debug.Log("Save result");
        SequenceManager.Instance.NextSequence();
    }
}
