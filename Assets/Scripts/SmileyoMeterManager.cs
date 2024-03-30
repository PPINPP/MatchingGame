using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class SmileyoMeterManager : MonoBehaviour
    {
        [SerializeField] private ToggleGroup funnessToggleGrp;
        [SerializeField] private ToggleGroup fatiguenessToggleGrp;

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


    }
}