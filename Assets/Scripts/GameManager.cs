using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] Sprite[] cardFace;
    [SerializeField] Sprite cardBack;
    [SerializeField] TextMeshProUGUI matchText;
    [SerializeField] GameObject cardParent;
    [SerializeField] GameObject cardPrefab;

    List<GameObject> cardObjs = new List<GameObject>();
    private bool _init = false;
    private int _matches;
    private List<Card> cards = new List<Card>();
    List<Card> selectedCards = new List<Card>();
    private object sceneName;
    private GridLayoutGroup gridLayout;
    private PairConfig pairConfig = new PairConfig();

    private void Awake()
    {
        
        gridLayout = cardParent.GetComponent<GridLayoutGroup>();
    }

    private void Start()
    {
        GameplayResources.Instance.Init();
        pairConfig = GameplayResources.Instance.GameplayPairSO.pairConfigs.Find(_ => (int)_.pairType == 8);

        _matches = (int)pairConfig.pairType;
        matchText.text = "Number of Matches: " + _matches;
        initializeCards();
    }

    void initializeCards()
    {
        gridLayout.cellSize = new Vector2(pairConfig.cellSize.x, pairConfig.cellSize.y);
        gridLayout.constraintCount = pairConfig.ConstraintRow;
        for (int i = 0; i < _matches * 2; i++)
        {
            GameObject cardObj = Instantiate(cardPrefab, cardParent.transform);
            cardObjs.Add(cardObj);
            Card card = cardObj.GetComponent<Card>();
            cards.Add(card);
        }

        //TODO : SHUFFLE CARD

        cards = ShuffleCard(cards,pairConfig.roundShuffle);

        int value = 0;
        for (int i = 0; i < cards.Count; i++)
        {
            Card card = cards[i];
            card.Init(value);
            card.setupGraphics(cardBack, getCardFace(card.CardValue));

            if ((i +1) % 2== 0)
            {
                value++;
            }
        }

        if (!_init)
            _init = true;
    }

    List<Card> ShuffleCard(List<Card> cardList, int roundShuffle = 1)
    {
        int lastIndex = cardList.Count - 1;

        for (int i = 0; i < roundShuffle;i++)
        {
            while (lastIndex > 0)
            {
                Card tempValue = cardList[lastIndex];
                int randomIndex = Random.Range(0, lastIndex);
                cardList[lastIndex] = cardList[randomIndex];
                cardList[randomIndex] = tempValue;
                lastIndex--;
            }
        }

        return cardList;
    }

    public Sprite getCardFace(int i)
    {
        return cardFace[i];
    }

    public bool CanFilpCard()
    {
        print(selectedCards.Count);
        if (selectedCards.Count >= 2)
        {
            return false;
        }
        else return true;
    }

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
            ClearCardList();
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
       
    }

    public void ClearCardList()
    {
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