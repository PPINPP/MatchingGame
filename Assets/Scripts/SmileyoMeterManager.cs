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
        [SerializeField] private ToggleGroup enjoyableGrp;
        [SerializeField] private ToggleGroup fatigueGrp;
        public string targetScene;

        // Use this for initialization
        void Start()
        {
            enjoyableGrp.SetAllTogglesOff();
            fatigueGrp.SetAllTogglesOff();
        }

        void Update()
        {
            if (enjoyableGrp.AnyTogglesOn())
            {
                if (enjoyableGrp.allowSwitchOff)
                {
                    enjoyableGrp.allowSwitchOff = false;
                }
            }

            if (fatigueGrp.AnyTogglesOn())
            {
                if (fatigueGrp.allowSwitchOff)
                {
                    fatigueGrp.allowSwitchOff = false;
                }
            }
        }

        public void SubmitForm()
        {
            if (!enjoyableGrp.AnyTogglesOn() || !fatigueGrp.AnyTogglesOn())
            {
                Debug.Log("Please answer correctly");
                return;
            }
                
            int enjoyable = MapToggleNameToNumber(enjoyableGrp.ActiveToggles().FirstOrDefault().name);
            int fatigue = MapToggleNameToNumber(fatigueGrp.ActiveToggles().FirstOrDefault().name);
            
            SmileyoMeterResult smileyoMeterResult = new(enjoyable, fatigue);

            DataManager.Instance.SmileyoMeterResultList.Add(smileyoMeterResult);

            Debug.Log("Save result");

            SceneManager.LoadScene(targetScene);
        }

        private int MapToggleNameToNumber(string toggleName)
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