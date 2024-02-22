using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MatchingGame.Gameplay
{
	public class MenuBehavior : MonoBehaviour
	{
		public void triggerMenuBehavior(int i)
		{
			switch (i)
			{
				default:
				case (0):
					SceneManager.LoadScene("Level");
					break;
				case (1):
					Application.Quit();
					break;
			}
		}
	}
}