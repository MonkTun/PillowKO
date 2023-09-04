using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LobbyManager : MonoBehaviour
{
	[SerializeField] AudioClip lobbyMusic;
	[SerializeField] TMP_Text highestScoreText; //TODO

	private void Start()
	{
		AppManager.Instance.musicManager.PlayMusic(lobbyMusic, true, 1, 0.3f, false);
		//AppManager.Instance.musicManager.SetLowPassCutOff(0.1f, -1);
		highestScoreText.text = AppManager.Instance.settingsManager.GetHighestScore().ToString();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Return))
		{
			StartGame();
		}
	}

	public void StartGame()
	{
		AppManager.Instance.sceneLoader.LoadScene(1);
		AppManager.Instance.sfxManager.PlaySound("MenuSelect2");
	}
}
