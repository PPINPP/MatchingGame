using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
	public string target;

	public void ChangeScene()
	{
		SceneManager.LoadScene(target);
	}
}
