using Manager;
using Model;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class SmileyoMeterManager : MonoBehaviour
    {
        [SerializeField] private ToggleGroup funnessToggleGrp;
        [SerializeField] private ToggleGroup fatiguenessToggleGrp;
        public string targetScene;

        // Use this for initialization
        void Start()
        {
            funnessToggleGrp.SetAllTogglesOff();
            fatiguenessToggleGrp.SetAllTogglesOff();
        }

        void Update()
        {
            if (funnessToggleGrp.AnyTogglesOn())
            {
                if (funnessToggleGrp.allowSwitchOff)
                {
                    funnessToggleGrp.allowSwitchOff = false;
                }

                Debug.Log(funnessToggleGrp.ActiveToggles().FirstOrDefault().name);
            }

            if (fatiguenessToggleGrp.AnyTogglesOn())
            {
                if (fatiguenessToggleGrp.allowSwitchOff)
                {
                    fatiguenessToggleGrp.allowSwitchOff = false;
                }

                Debug.Log(fatiguenessToggleGrp.ActiveToggles().FirstOrDefault().name);
            }
        }

        void SubmitForm()
        {
            if (!fatiguenessToggleGrp.AnyTogglesOn() || !funnessToggleGrp.AnyTogglesOn())
            {
                Debug.Log("Please answer correctly");
                return;
            }
                
            int enjoyable = MapToggleNameToNumber(fatiguenessToggleGrp.ActiveToggles().FirstOrDefault().name);
            int fatigue = MapToggleNameToNumber(fatiguenessToggleGrp.ActiveToggles().FirstOrDefault().name);
            
            SmileyoMeterResult smileyoMeterResult = new(enjoyable, fatigue);

            DataManager.Instance.SmileyoMeterResultList.Add(smileyoMeterResult);

            Debug.Log("Save result");

            SceneManager.LoadScene(targetScene);
        }

        int MapToggleNameToNumber(string toggleName)
        {
            switch (toggleName)
            {
                case "green_tgr":
                    return 1;
                case "l_green_tgr":
                    return 2;
                case "yellow_tgr":
                    return 3;
                case "orange_tgr":
                    return 4;
                case "red_tgr":
                    return 5;
                default:
                    return 0;
            }
        }

    }
}