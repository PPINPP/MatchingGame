using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CardState
{
    FACEUP,
    FACEDOWN
}

public class Card : MonoBehaviour
{
    public bool canFlip = false;

    [SerializeField]
    private CardState _state;
    [SerializeField]
    private int _cardValue;
    [SerializeField]
    private bool _initialized = false;
    
    private Sprite _cardBack;
    private Sprite _cardFace;

    private GameObject _manager;
    private GameManager gameManager;
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

    void Awake() {
        _state = CardState.FACEUP;
        cardImage = GetComponent<Image>();
        _manager = GameObject.FindGameObjectWithTag("Manager");
    }

    public void Init(int cardValue)
    {
        canFlip = false;
        _cardValue = cardValue;
        _initialized = true;
    }

    public void setupGraphics(Sprite cardBack,Sprite CardFace) {
        gameManager = _manager.GetComponent<GameManager>();
        _cardBack = cardBack;
        _cardFace = CardFace;
        cardImage.sprite = _cardFace;

        print(CardValue);
        print(_state);
        print(canFlip);

        StartCoroutine(DelayShowFaceUp(5.0f));
    }

    public void CardClick()
    {
        if (!canFlip) return;

        canFlip = false;
        flipCard();
        gameManager.checkCards(this);
    }

    public void flipCard() {

        if(_state == CardState.FACEDOWN)
        {
            _state = CardState.FACEUP;
            cardImage.sprite = _cardFace;
        }
        else if (_state == CardState.FACEUP)
        {
            _state = CardState.FACEDOWN;
            cardImage.sprite = _cardBack;
        }
    }


    public void falseCheck() {
        StartCoroutine(DelayForceFaceDown());
    }

    IEnumerator DelayForceFaceDown() {
        yield return new WaitForSeconds(1);
        flipCard();
        canFlip = true;
    }   

    IEnumerator DelayShowFaceUp(float time)
    {
        yield return new WaitForSeconds(time);
        flipCard();
        canFlip = true;
    }
}
