using System.Collections;
using System.Collections.Generic;
using MatchingGame.Gameplay;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class SCButton : MonoBehaviour
{
    public Transform leftListContent;  // Assign Left List Content
    public Transform rightListContent; // Assign Right List Content
    public Button moveToRightButton;   // Assign ">" Button
    public Button moveToLeftButton;    // Assign "<" Button
    private GameObject selectedItem;
    [SerializeField] TMP_InputField stageID;
    [SerializeField] TMP_Dropdown theme;
    [SerializeField] TMP_Dropdown pair;
    [SerializeField] TMP_Dropdown diff;
    [SerializeField] TMP_Dropdown layout;
    [SerializeField] Toggle toggle;

    List<int> pairT = new List<int>(){2,3,4,6,8};


    public void Start()
    {

        moveToRightButton.onClick.AddListener(() => MoveItem(leftListContent, rightListContent));
        moveToLeftButton.onClick.AddListener(() => MoveItem(rightListContent, leftListContent));
        for (int i = 1; i <= 100; i++)
        {
            CreateButton("Home_0" + i.ToString("D3"), new Vector2(0, 0), new Vector2(400, 100), leftListContent);
        }

    }
    void Update()
    {

    }

    public void CreateButton(string buttonText, Vector2 position, Vector2 size, Transform parent)
    {
        // Create the Button GameObject
        GameObject buttonGO = new GameObject("Button", typeof(RectTransform), typeof(Button), typeof(Image));

        // Set the button's parent to the canvas
        buttonGO.transform.SetParent(parent, false);

        // Configure the RectTransform
        RectTransform rectTransform = buttonGO.GetComponent<RectTransform>();
        rectTransform.sizeDelta = size;
        rectTransform.anchoredPosition = position;

        // Set up the button's visuals (Image component)
        Image buttonImage = buttonGO.GetComponent<Image>();
        buttonImage.color = Color.white; // Set a default color

        // Create a child TextMeshPro GameObject for the button's label
        GameObject textGO = new GameObject("ButtonText", typeof(RectTransform), typeof(TextMeshProUGUI));
        textGO.transform.SetParent(buttonGO.transform, false);

        // Configure the TextMeshPro component
        TextMeshProUGUI text = textGO.GetComponent<TextMeshProUGUI>();
        text.text = buttonText;
        text.fontSize = 64;
        text.color = Color.black;
        text.alignment = TextAlignmentOptions.Center;

        // Adjust RectTransform for the Text
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero; // Stretch to fill the button
        textRect.anchoredPosition = Vector2.zero;

        // Optionally add an onClick listener to the button
        Button button = buttonGO.GetComponent<Button>();
        button.onClick.AddListener(() => SelectItem(buttonGO));
    }

    public void SelectItem(GameObject item)
    {
        // Highlight the selected item visually
        if (selectedItem != null)
        {
            DeselectItem(); // Deselect the previous selection
        }

        selectedItem = item;

        // Example of changing color to indicate selection
        Image itemImage = selectedItem.GetComponent<Image>();
        if (itemImage != null)
        {
            itemImage.color = Color.yellow;
        }
    }

    public void DeselectItem()
    {
        if (selectedItem != null)
        {
            // Reset the color or visual style
            Image itemImage = selectedItem.GetComponent<Image>();
            if (itemImage != null)
            {
                itemImage.color = Color.white;
            }
            selectedItem = null;
        }
    }

    private void MoveItem(Transform fromList, Transform toList)
    {
        if (selectedItem != null && selectedItem.transform.parent == fromList)
        {
            selectedItem.transform.SetParent(toList);
            DeselectItem();
        }
    }

    public void Mini()
    {
        SequenceCreator.Instance.MiniGame();
    }

    public void Daily()
    {
        SequenceCreator.Instance.DailyGame();
    }

    public void Smile()
    {
        SequenceCreator.Instance.SmileO();
    }

    public void Gameplay()
    {
        GameplaySequenceSetting gameplaySequenceSetting = new GameplaySequenceSetting();
        gameplaySequenceSetting.isTutorial = false;
        gameplaySequenceSetting.categoryTheme = (CategoryTheme)theme.value;
        gameplaySequenceSetting.pairType = (PairType)pairT[pair.value];
        gameplaySequenceSetting.GameDifficult = (GameDifficult)diff.value;
        gameplaySequenceSetting.layout = (GameLayout)layout.value;
        if (toggle.isOn)
        {
            gameplaySequenceSetting.isForceCardID = true;
            var all_card = new List<string>();
            foreach(Transform item in rightListContent){
                all_card.Add(item.GetChild(0).GetComponent<TextMeshProUGUI>().text);
            }
            gameplaySequenceSetting.cardIDList = all_card;
        }
        
        SequenceCreator.Instance.GamePlay(stageID.text.ToString(), gameplaySequenceSetting);
    }

    public void Tutorial()
    {
        GameplaySequenceSetting gameplaySequenceSetting = new GameplaySequenceSetting();
        gameplaySequenceSetting.isTutorial = true;
        gameplaySequenceSetting.categoryTheme = (CategoryTheme)theme.value;
        gameplaySequenceSetting.pairType = (PairType)pairT[pair.value];
        gameplaySequenceSetting.GameDifficult = (GameDifficult)diff.value;
        gameplaySequenceSetting.layout = (GameLayout)layout.value;
        if (toggle.isOn)
        {
            gameplaySequenceSetting.isForceCardID = true;
            var all_card = new List<string>();
            foreach(Transform item in rightListContent){
                all_card.Add(item.GetChild(0).GetComponent<TextMeshProUGUI>().text);
            }
            gameplaySequenceSetting.cardIDList = all_card;
        }
        SequenceCreator.Instance.Tutorial(stageID.text.ToString(), gameplaySequenceSetting);
    }

}
