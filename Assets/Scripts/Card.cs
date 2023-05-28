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
    [SerializeField] Image itemImage;

    public bool canFlip { get; private set; }
    
    private CardState _state;
    private bool _initialized = false;
    private int _cardValue;
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
        itemImage.sprite = _cardFace;

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

    private void flipCard() {

        if(_state == CardState.FACEDOWN)
        {
            _state = CardState.FACEUP;
            cardImage.sprite = null;
            itemImage.sprite = _cardFace;
        }
        else if (_state == CardState.FACEUP)
        {
            _state = CardState.FACEDOWN;
            cardImage.sprite = _cardBack;
            itemImage.sprite = null;
        }
    }


    public void falseCheck() {
        StartCoroutine(DelayForceFaceDown());
    }

    IEnumerator DelayForceFaceDown() {
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
