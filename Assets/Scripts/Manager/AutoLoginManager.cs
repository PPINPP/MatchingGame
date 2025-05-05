using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoLoginManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject loadbg;
    void Start()
    {
        if (PlayerPrefs.HasKey("autologinname"))
        {
            if(PlayerPrefs.GetString("autologinname") != ""){
                loadbg.SetActive(true);
                StartCoroutine(ExampleCoroutine());
                
            }
        }
        else
        {
            PlayerPrefs.SetString("autologinname", "");
            PlayerPrefs.SetString("autologinpassword", "");
        }
    }
    IEnumerator ExampleCoroutine()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("Login_P");
    }
}
