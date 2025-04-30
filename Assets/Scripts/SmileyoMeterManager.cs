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
        [SerializeField] private List<Image> bgfade = new List<Image>();
        [SerializeField] private Button confirm;

        private float timer = 0.2f;
        private bool isShowing = true;

        void Start()
        {
            enjoyableGrp.SetAllTogglesOff();
            fatigueGrp.SetAllTogglesOff();
            SetAllImagesVisible(true);
            confirm.interactable = false;
        }

        void Update()
        {
            bool enjoyableSelected = enjoyableGrp.AnyTogglesOn();
            bool fatigueSelected = fatigueGrp.AnyTogglesOn();

            if (enjoyableSelected)
            {
                if (enjoyableGrp.allowSwitchOff)
                    enjoyableGrp.allowSwitchOff = false;

                for (int i = 0; i < 5; i++)
                {
                    SetImageAlpha(bgfade[i], 0.1f);
                }
            }

            if (fatigueSelected)
            {
                if (fatigueGrp.allowSwitchOff)
                    fatigueGrp.allowSwitchOff = false;

                for (int i = 5; i < 10; i++)
                {
                    SetImageAlpha(bgfade[i], 0.1f);
                }
            }

            if (enjoyableSelected && fatigueSelected)
            {
                confirm.interactable = true;
                enabled = false;
                return;
            }

            timer -= Time.deltaTime;

            if (isShowing && timer <= 0f)
            {
                ShowOrHideBackground(false);
                isShowing = false;
                timer = 5f;
            }
            else if (!isShowing && timer <= 0f)
            {
                ShowOrHideBackground(true);
                isShowing = true;
                timer = 0.2f;
            }
        }

        private void ShowOrHideBackground(bool show)
        {
            for (int i = 0; i < bgfade.Count; i++)
            {
                if (i < 5 && !enjoyableGrp.AnyTogglesOn())
                {
                    SetImageAlpha(bgfade[i], show ? 1f : 0.1f);
                }
                else if (i >= 5 && !fatigueGrp.AnyTogglesOn())
                {
                    SetImageAlpha(bgfade[i], show ? 1f : 0.1f);
                }
            }
        }

        private void SetAllImagesVisible(bool visible)
        {
            float alpha = visible ? 1f : 0.1f;
            foreach (var img in bgfade)
            {
                if (img != null)
                {
                    SetImageAlpha(img, alpha);
                }
            }
        }

        private void SetImageAlpha(Image img, float alpha)
        {
            if (img != null)
            {
                var tempColor = img.color;
                tempColor.a = alpha;
                img.color = tempColor;
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

            SequenceManager.Instance.NextSequence();
        }

        private int MapToggleNameToNumber(string toggleName)
        {
            switch (toggleName)
            {
                case "green_tgr": return 1;
                case "l_green_tgr": return 2;
                case "yellow_tgr": return 3;
                case "orange_tgr": return 4;
                case "red_tgr": return 5;
                default: return 0;
            }
        }
    }
}

