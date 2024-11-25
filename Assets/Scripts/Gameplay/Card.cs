using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace MatchingGame.Gameplay
{
    public enum CardState
    {
        FACE_UP,
        FACE_DOWN
    }

    public enum CardImgType
    {
        BACK_CARD,
        FRONT_CARD,
        FRONT_CORRECT_CARD
    }

    public class Card : MonoBehaviour
    {
        [SerializeField] Image _itemCardImg;
        [SerializeField] Image _backgroundImg;

        private CardProperty _cardProperty;
        private bool _isFliping = false;
        private bool _canFlip = false;
        private CardState _curState;
        private Vector3 _curRotationY;
        private Vector3 _targetRotationY;
        private float lerpTimer;
        public Subject<bool> onFlipComplete = new Subject<bool>();
        private int _indexClick = -1;
        private int hintTimer = 0;
        private bool hintDirection = false;

        public CardProperty CardProperty { get => _cardProperty; }
        public bool IsFliping { get => _isFliping; }
        public int IndexClick { get => _indexClick; set => _indexClick = value; }

        private void Update()
        {
            if (_isFliping)
            {
                _backgroundImg.transform.eulerAngles = Vector3.Lerp(_curRotationY, _targetRotationY,
                    lerpTimer / GameplayResources.Instance.GameplayProperty.FlipDuration);
                lerpTimer += Time.deltaTime;

                if (lerpTimer / GameplayResources.Instance.GameplayProperty.FlipDuration > 1)
                    OnFlipComplete();
                else
                {
                    if (_curState == CardState.FACE_UP && _backgroundImg.transform.eulerAngles.y <= 90)
                    {
                        _backgroundImg.sprite = GameplayResources.Instance.CardImgDic[CardImgType.FRONT_CARD];
                        _itemCardImg.enabled = true;
                    }
                    else if (_curState == CardState.FACE_DOWN && _backgroundImg.transform.eulerAngles.y >= 90)
                    {
                        _backgroundImg.sprite = GameplayResources.Instance.BackCardImg[SettingGameplay.Instance.CategoryTheme];
                        _itemCardImg.enabled = false;
                    }
                }
            }
        }

        public void Init(CardProperty cardProperty)
        {
            _cardProperty = cardProperty;
            _curState = CardState.FACE_UP;
            _canFlip = true;
            SetupGraphic();

            Button bt = GetComponent<Button>();
            bt.onClick.AddListener(CardClick);
        }

        private void SetupGraphic()
        {
            _itemCardImg.sprite = _cardProperty.sprite;
        }

        public void FlipCard(CardState state)
        {
            if (state != _curState && _canFlip)
            {
                _isFliping = true;
                lerpTimer = 0;
                _curRotationY = _backgroundImg.transform.rotation.eulerAngles;
                _targetRotationY = _curRotationY;
                _targetRotationY.y = _targetRotationY.y + (_curState == CardState.FACE_UP ? 180f : -180f);

                _curState = state;
            }
        }

        private void OnFlipComplete()
        {
            _isFliping = false;
            _backgroundImg.transform.eulerAngles = _targetRotationY;

            onFlipComplete?.OnNext(true);
        }

        public void CardClick()
        {
            AudioController.SetnPlay("audio/SFX/Click");
            GameManager.Instance.OnCardClick();
            if(!_canFlip){
                GameManager.Instance.OnCardRepeat();
            }
            if (!GameManager.Instance.CheckCanFlipCard() || !_canFlip || _isFliping) return;

            GameManager.Instance.AddCardToCheck(this);
            FlipCard(CardState.FACE_UP);
        }

        public void SelectedCorrect()
        {
            _canFlip = false;
            _backgroundImg.sprite = GameplayResources.Instance.CardImgDic[CardImgType.FRONT_CORRECT_CARD];
        }

        public bool IsInComplete()
        {
            return _canFlip;
        }

        public void StartFading()
        {
            InvokeRepeating("Fade", 0f, 0.1f);
        }
        public void StopFading()
        {
            CancelInvoke();
            _backgroundImg.color = new Color(1.0f,1.0f,1.0f);;
        }

        void Fade()
        {
            if(hintDirection){
                hintTimer -=1;
                 if(hintTimer== 0){
                    hintDirection = false;
                }
            }
            else{
                hintTimer +=1;
                if(hintTimer== 5){
                    hintDirection = true;
                }
            }
            float hintVal = hintTimer*0.04f;
            Color nColor = new Color(1.0f-hintVal,1.0f,1.0f);
            _backgroundImg.color = nColor;
        }
    }
}