using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MatchingGame.Gameplay
{
    public class OldCardScripts : MonoBehaviour
    {
        [SerializeField] Image itemImage;

        public bool canFlip { get; private set; }

        private CardState _state;
        private bool _initialized = false;
        private int _cardValue;
        private Sprite _backCardSprite;
        private Sprite _faceUpCardSprite;
        private Sprite _itemSprite;
        private GameObject _manager;
        private OldGamemanager gameManager;
        private Image cardImage;

        public int CardValue
        {
            get { return _cardValue; }
        }

        public CardState State
        {
            get { return _state; }
        }

        public bool Initialized
        {
            get { return _initialized; }
        }

        void Awake()
        {
            _state = CardState.FACEUP;
            cardImage = GetComponent<Image>();
            _manager = GameObject.FindGameObjectWithTag("Manager");
            _faceUpCardSprite = cardImage.sprite;
        }

        public void Init(int cardValue)
        {
            canFlip = false;
            _cardValue = cardValue;
            _initialized = true;
        }

        public void setupGraphics(Sprite cardBack, Sprite itemSprite)
        {
            gameManager = _manager.GetComponent<OldGamemanager>();
            _backCardSprite = cardBack;
            _itemSprite = itemSprite;
            itemImage.sprite = _itemSprite;

            print(CardValue);
            print(_state);
            print(canFlip);

            StartCoroutine(DelayShowFaceUp(5.0f));
        }

        public void CardClick()
        {
            print(gameManager.CanFilpCard());
            if (!canFlip || !gameManager.CanFilpCard()) return;

            canFlip = false;
            flipCard();
            gameManager.checkCards(this);
        }

        private void flipCard()
        {

            if (_state == CardState.FACEDOWN)
            {
                _state = CardState.FACEUP;
                cardImage.sprite = _faceUpCardSprite;
                itemImage.enabled = true;
                itemImage.sprite = _itemSprite;
            }
            else if (_state == CardState.FACEUP)
            {
                _state = CardState.FACEDOWN;
                cardImage.sprite = _backCardSprite;
                itemImage.enabled = false;
                itemImage.sprite = null;
            }
        }


        public void falseCheck()
        {
            StartCoroutine(DelayForceFaceDown());
        }

        IEnumerator DelayForceFaceDown()
        {
            yield return new WaitForSeconds(1);
            flipCard();
            canFlip = true;
            gameManager.ClearCardList();
        }

        IEnumerator DelayShowFaceUp(float time)
        {
            yield return new WaitForSeconds(time);
            flipCard();
            canFlip = true;
        }
    }
}