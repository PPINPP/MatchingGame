using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] PairScriptable pairScriptable;
    [SerializeField] Sprite[] cardFace;
    [SerializeField] Sprite cardBack;
    [SerializeField] TextMeshProUGUI matchText;
    [SerializeField] GameObject cardParent;
    [SerializeField] GameObject cardPrefab;

    List<GameObject> cardObjs = new List<GameObject>();
    private bool _init = false;
    private int _matches;
    private List<Card> cards = new List<Card>();
    private object sceneName;
    private GridLayoutGroup gridLayout;
    private PairConfig pairConfig = new PairConfig();

    private void Awake()
    {
        pairConfig = pairScriptable.pairConfigs.Find(_ => (int)_.pairType == 3);

        _matches = (int)pairConfig.pairType;
        matchText.text = "Number of Matches: " + _matches;
        gridLayout = cardParent.GetComponent<GridLayoutGroup>();
    }

    private void Start()
    {
        initializeCards();
    }

    void initializeCards()
    {
        gridLayout.cellSize = new Vector2(pairConfig.cellSize.x, pairConfig.cellSize.y);    
        for (int i = 0; i < _matches * 2; i++)
        {
            print("d");
            GameObject cardObj = Instantiate(cardPrefab, cardParent.transform);
            cardObjs.Add(cardObj);
            Card card = cardObj.GetComponent<Card>();
            cards.Add(card);
        }

        //TODO : SHUFFLE CARD

        //for (int id = 0; id < 2; id++)
        //{
        //    for (int i = 1; i <= cards.Length / 2; i++)
        //    {
        //        bool isCardInit = false;
        //        int choice = 0;

        //        choice = Random.Range(0, cards.Length);
        //        isCardInit = !(cards[choice].Initialized);
        //        while (!isCardInit)
        //        {
        //            choice = Random.Range(0, cards.Length);
        //            isCardInit = !(cards[choice].Initialized);
        //        }
        //        var card = cards[choice];
        //        card.Init(i);
        //        card.setupGraphics(cardBack, getCardFace(card.CardValue));
        //    }
        //}

        if (!_init)
            _init = true;
    }

    public Sprite getCardFace(int i)
    {
        return cardFace[i - 1];
    }

    List<Card> selectedCards = new List<Card>();

    public void checkCards(Card card)
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