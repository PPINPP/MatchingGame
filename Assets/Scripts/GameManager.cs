using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class GameManager : MonoBehaviour
{
    public Sprite[] cardFace;
    public Sprite cardBack;
    public GameObject[] cardObjs;
    public TextMeshProUGUI matchText;

    private bool _init = false;
    private int _matches;
    private Card[] cards;
    private object sceneName;

    private void Awake()
    {
        _matches = cardObjs.Length / 2;
        matchText.text = "Number of Matches: " + _matches;
    }

    private void Start()
    {
        cards = new Card[cardObjs.Length];
        for (int i = 0; i < cardObjs.Length; i++)
        {
            cards[i] = cardObjs[i].GetComponent<Card>();
        }
        if (!_init)
            initializeCards();
    }

    void Update()
    {
        //if (Input.GetMouseButtonUp(0))
        //    checkCards();
    }

    void initializeCards()
    {
        for (int id = 0; id < 2; id++)
        {
            for (int i = 1; i <= cards.Length / 2; i++)
            {
                bool isCardInit = false;
                int choice = 0;

                choice = Random.Range(0, cards.Length);
                isCardInit = !(cards[choice].Initialized);
                while (!isCardInit)
                {
                    choice = Random.Range(0, cards.Length);
                    isCardInit = !(cards[choice].Initialized);
                }
                var card = cards[choice];
                card.Init(i);
                card.setupGraphics(cardBack, getCardFace(card.CardValue));
            }
        }

        //foreach (Card c in cards)
        //    c.setupGraphics(cardBack);

        if (!_init)
            _init = true;
    }

    public Sprite getCardFace(int i)
    {
        return cardFace[i - 1];
    }

    List<Card> selectedCards = new List<Card>();

    public  void checkCards(Card card)
    {
        selectedCards.Add(card);

        if (selectedCards.Count == 2)
            cardComparison();
    }

    void cardComparison()
    {

        if (selectedCards[0].CardValue == selectedCards[1].CardValue)
        {
            _matches--;
            matchText.text = "Number of Matches: " + _matches;

            //if (_matches == 0)
            //    //Invoke("LoadScene", 3f, "Menu");
            //    //SceneManager.LoadScene("Menu");
            //    LoadSceneWithDelay("Menu");
        }
        else
        {
            selectedCards.ForEach(card =>
            {
                card.falseCheck();
            });
        }
        selectedCards.Clear();
    }

    public void LoadSceneWithDelay(string sceneName)
    {
        StartCoroutine(LoadSceneDelayed(sceneName, 3.0f));
    }

    private void Invoke(string v1, float v2, string sceneName)
    {
        throw new System.NotImplementedException();
    }

    private IEnumerator LoadSceneDelayed(string sceneName, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        SceneManager.LoadScene(sceneName);
    }

}