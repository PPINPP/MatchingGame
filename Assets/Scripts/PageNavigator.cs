using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageNavigator : MonoBehaviour
{
    int activePage;
    int maxPage;

    void Start()
    {
        maxPage = this.transform.childCount;
        activePage = 0;
    }
    public void NextPage()
    {
        transform.GetChild(activePage).gameObject.SetActive(false);
        activePage++;
        activePage = activePage % maxPage;
        transform.GetChild(activePage).gameObject.SetActive(true);

    }
    public void PreviosPage()
    {
        transform.GetChild(activePage).gameObject.SetActive(false);
        activePage--;
        if (activePage == -1){
            activePage = maxPage-1;
        }
        transform.GetChild(activePage).gameObject.SetActive(true);
    }
}
