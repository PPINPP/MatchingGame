using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportPad : MonoBehaviour
{
	public void ChangeScene(string target)
	{
		SceneManager.LoadScene(target);
	}
}
