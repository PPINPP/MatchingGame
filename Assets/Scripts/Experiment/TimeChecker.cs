using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class TimeChecker : MonoBehaviour
{
    // Start is called before the first frame update
    private DateTime checkpointDateTime;
    public void checktime()
    {
        // FirebaseManagerV2.Instance.GameRuleTimeChecker(compareTime);
    }

    public void compareTime(List<string> timeRule)
    {
        DateTime currentDateTime = DateTime.Now;
        List<int> tempTimeNow;
        foreach (var item in timeRule)
        {
            tempTimeNow = item.Split(',').Select(int.Parse).ToList();
            checkpointDateTime = new DateTime(tempTimeNow[0], tempTimeNow[1], tempTimeNow[2], tempTimeNow[3], tempTimeNow[4], tempTimeNow[5]);
            if (currentDateTime > checkpointDateTime)
            {
                Debug.Log("The current datetime has passed the checkpoint datetime.");
                Debug.Log(checkpointDateTime);
                Debug.Log(currentDateTime);
            }
            else
            {
                Debug.Log("The current datetime has NOT yet passed the checkpoint datetime.");
                Debug.Log(checkpointDateTime);
            }
        }
    }
}
