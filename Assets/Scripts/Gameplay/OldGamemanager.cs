using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OldGamemanager : MonoBehaviour
{
    [SerializeField] Sprite[] cardFace;
    [SerializeField] Sprite cardBack;
    [SerializeField] TextMeshProUGUI matchText;
    [SerializeField] GameObject cardParent;
    [SerializeField] GameObject cardPrefab;
    [SerializeField] GameDifficult gameMode;

    [SerializeField] string type;

    List<GameObject> cardObjs = new List<GameObject>();
    private bool _init = false;
    private int _targetPairMatchCount;
    private int _currentMatchCount;
    private Dictionary<string, CardDataConfig> currentConfigDict = new Dictionary<string, CardDataConfig>();
    private List<OldCardScripts> cards = new List<OldCardScripts>();
    List<OldCardScripts> selectedCards = new List<OldCardScripts>();
    private object sceneName;
    private GridLayoutGroup gridLayout;
    private PairConfig pairConfig = new PairConfig();

    private void Awake()
    {

        gridLayout = cardParent.GetComponent<GridLayoutGroup>();
    }

    private void Start()
    {
        //GameplayResources.Instance.Init();
        pairConfig = GameplayResources.Instance.PairConfigData.pairConfigs.Find(_ => _.pairType == PairType.SIX);

        _targetPairMatchCount = (int)pairConfig.pairType;
        matchText.text = $"Number of Matches : {_targetPairMatchCount}";
        initializeCards();
    }

    void initializeCards()
    {
        _currentMatchCount = 0;
        gridLayout.cellSize = new Vector2(pairConfig.cellSize.x, pairConfig.cellSize.y);
        gridLayout.constraintCount = pairConfig.ConstraintRow;
        for (int i = 0; i < _targetPairMatchCount * 2; i++)
        {
            GameObject cardObj = Instantiate(cardPrefab, cardParent.transform);
            cardObjs.Add(cardObj);
            OldCardScripts card = cardObj.GetComponent<OldCardScripts>();
            cards.Add(card);
        }

        cards = ShuffleCard(cards, pairConfig.roundShuffle);
        //SettingPair(GameplayResources.Instance.CardCategoryDataDic[type]);

        if (!_init)
            _init = true;
    }

    List<OldCardScripts> ShuffleCard(List<OldCardScripts> cardList, int roundShuffle = 1)
    {
        for (int i = 0; i < roundShuffle; i++)
        {
            int lastIndex = cardList.Count - 1;
            while (lastIndex > 0)
            {
                OldCardScripts tempValue = cardList[lastIndex];
                int randomIndex = Random.Range(0, lastIndex);
                cardList[lastIndex] = cardList[randomIndex];
                cardList[randomIndex] = tempValue;
                lastIndex--;
            }
        }

        return cardList;
    }

    public void SettingPair(CardCategoryDataSO dataDifficulty)
    {
        int value = 0;
        var list = GetStageConfigsFromGameMode(this.gameMode, dataDifficulty);
        var randomList = RandomCard(_targetPairMatchCount, list);
        currentConfigDict = randomList.ToDictionary(entry => entry.Key, entry => entry.Value);

        for (int i = 0; i < cards.Count; i++)
        {
            OldCardScripts card = cards[i];
            card.Init(value);
            var data = randomList.First();

            card.setupGraphics(cardBack, data.Value.easySprite);

            if ((i + 1) % 2 == 0)
            {
                value++;
                randomList.Remove(data.Key);
            }
        }
    }

    public Dictionary<string, CardDataConfig> GetStageConfigsFromGameMode(GameDifficult gameMode, CardCategoryDataSO data)
    {
        Dictionary<string, CardDataConfig> stageConfigs = new Dictionary<string, CardDataConfig>();
        var list = data.cardDataConfigDict.Where(_ =>
        {
            switch (gameMode)
            {
                case GameDifficult.EASY:
                    return _.Value.isEasy;
                case GameDifficult.NORMAL:
                    return _.Value.isNormal;
                case GameDifficult.HARD:
                    return _.Value.isHard;
                case GameDifficult.ADVANCE:
                    return _.Value.isAdvanced;
            }

            return false;
        });

        stageConfigs = list.ToDictionary(_ => _.Key, _ => _.Value);
        return stageConfigs;
    }

    public Dictionary<string, CardDataConfig> RandomCard(int targetAmount, Dictionary<string, CardDataConfig> data)
    {
        Dictionary<string, CardDataConfig> stageConfigs = new Dictionary<string, CardDataConfig>();

        for (int i = 0; i < targetAmount; i++)
        {
            int ranNum = Random.Range(0, data.Count);
            var ranConfig = data.ElementAt(ranNum);
            stageConfigs.Add(ranConfig.Key, ranConfig.Value);
            data.Remove(ranConfig.Key);
        }

        return stageConfigs;
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

    public void checkCards(OldCardScripts card)
    {
        selectedCards.Add(card);

        if (selectedCards.Count == 2)
            cardComparison();
    }

    void cardComparison()
    {
        if (selectedCards[0].CardValue == selectedCards[1].CardValue)
        {
            _currentMatchCount++;
            matchText.text = $"Number of Matches : {_targetPairMatchCount - _currentMatchCount}";
            ClearCardList();
            if (_targetPairMatchCount - _currentMatchCount == 0)
                //    //Invoke("LoadScene", 3f, "Menu");
                //    //SceneManager.LoadScene("Menu");
                LoadSceneWithDelay("Menu");
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
