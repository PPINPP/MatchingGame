using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MatchingGame.Gameplay
{
    public enum CardState
    {
        FACEUP,
        FACEDOWN
    }

    public enum CardImgType
    {
        BACK_CARD,
        FRONT_CARD,
        FRONT_CORRECT_CARD
    }

    public class Card : MonoBehaviour
    {
        [SerializeField] Image _FrontImg;

        private CardProperty _cardProperty;

        public CardProperty CardProperty { get { return _cardProperty; } }

        public void Init(CardProperty cardProperty)
        {
            _cardProperty = cardProperty;
            SetupGraphic();
        }

        private void SetupGraphic()
        {
            _FrontImg.sprite = _cardProperty.sprite;
        }
    }
}