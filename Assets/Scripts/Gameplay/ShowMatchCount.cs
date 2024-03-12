using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowMatchCount : MonoInstance<ShowMatchCount>
{
    [SerializeField] GameObject prefab;
    [SerializeField] Sprite completeSprite;
    [SerializeField] Sprite uncompleteSprite;


    List<Image> imageList = new List<Image>();

    public void Init(int Count)
    {
        for (int i = 0; i < Count; i++)
        {
            Image img = Instantiate(prefab, transform).GetComponent<Image>();
            imageList.Add(img);
            img.sprite = uncompleteSprite;
        }
    }

    public void OnMatch(int matchCount)
    {
        imageList[matchCount].sprite = completeSprite;
    }
}
