using Manager;
using MatchingGame.Gameplay;
using Model;
using System.Collections.Generic;
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
        FirebaseManagerV2 fbm;
        [SerializeField] List<Image> bgfade = new List<Image>();
        float colorVal = 0.1f;
        bool direction = false;

        // Use this for initialization
        void Start()
        {
            enjoyableGrp.SetAllTogglesOff();
            fatigueGrp.SetAllTogglesOff();
            InvokeRepeating("Fade", 0f, 0.1f);

        }

        void Fade()
        {
            if (direction)
            {
                colorVal -= 0.1f;
                if (colorVal <= 0.1f)
                {
                    direction = !direction;
                }
            }
            else
            {
                colorVal += 0.1f;
                if (colorVal >= 1.0f)
                {
                    direction = !direction;
                }
            }

            if (!enjoyableGrp.AnyTogglesOn())
            {
                for (int i = 0; i < 5; i++)
                {
                    var temp_color = bgfade[i].color;
                    temp_color.a = colorVal;
                    bgfade[i].color = temp_color;
                }
            }
            else{
                for (int i = 0; i < 5; i++)
                {
                    var temp_color = bgfade[i].color;
                    temp_color.a = 0.1f;
                    bgfade[i].color = temp_color;
                }
            }
            if (!fatigueGrp.AnyTogglesOn())
            {
                for (int i = 5; i < 10; i++)
                {
                    var temp_color = bgfade[i].color;
                    temp_color.a = colorVal;
                    bgfade[i].color = temp_color;
                }
            }
            else{
                for (int i = 5; i < 10; i++)
                {
                    var temp_color = bgfade[i].color;
                    temp_color.a = 0.1f;
                    bgfade[i].color = temp_color;
                }
                
            }
            if(fatigueGrp.AnyTogglesOn()&&enjoyableGrp.AnyTogglesOn()){
                CancelInvoke();
            }
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
            if (fbm == null)
            {
                fbm = (FirebaseManagerV2)GameObject.FindObjectOfType(typeof(FirebaseManagerV2));
            }
            FirebaseManagerV2.Instance.UploadSmileyoMeterResult(smileyoMeterResult, DataManager.Instance.SmileyoMeterResultList.Count - 1);

            Debug.Log("Save result");

            //SceneManager.LoadScene(targetScene);
            SequenceManager.Instance.NextSequence();
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