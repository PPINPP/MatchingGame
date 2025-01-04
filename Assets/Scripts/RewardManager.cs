using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
    [SerializeField] TMP_Text coins;
    [SerializeField] GameObject MockReward;
    // Start is called before the first frame update
    public void SetScore(int score, int coin)
    {
        coins.text = coin.ToString();
        if (score == 3)
        {
            MockReward.transform.GetChild(0).gameObject.SetActive(true);
            MockReward.transform.GetChild(1).gameObject.SetActive(true);
            MockReward.transform.GetChild(2).gameObject.SetActive(true);
        }
        else if (score == 2)
        {
            MockReward.transform.GetChild(0).gameObject.SetActive(true);
            MockReward.transform.GetChild(1).gameObject.SetActive(true);
            MockReward.transform.GetChild(2).gameObject.SetActive(false);
        }
        else
        {
            MockReward.transform.GetChild(0).gameObject.SetActive(true);
            MockReward.transform.GetChild(1).gameObject.SetActive(false);
            MockReward.transform.GetChild(2).gameObject.SetActive(false);
        }
    }
}
