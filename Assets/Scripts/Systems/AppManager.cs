using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppManager : MonoBehaviour
{
    public static AppManager Instance { get; private set; }

    public MusicManager musicManager; //so it can be both assigned and accessible by outside scripts.
    public SceneLoader sceneLoader; //subject for optimization
    public SFXManager sfxManager;
    public SettingsManager settingsManager;

    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject exitGameButton, LeaveLobbyButton;

    public int score { get; private set; }

    void Awake()
    {
        if (Instance == null)
		{
            Instance = this;
            DontDestroyOnLoad(gameObject);
		}
        else
		{
            Destroy(gameObject);
            return;
		}

        SavesManager.Load();

        settingsManager.LoadMasterVolume();
        settingsManager.LoadMusicVolume();
        settingsManager.LoadSFXVolume();
    }

	private void Update()
	{
        //new input system returns always true when its pressed not only first time so...
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            if (settingsPanel.activeInHierarchy)
			{
                settingsPanel.SetActive(false);
                musicManager.SetLowPassCutOff(0.2f, 0.5f);
                Time.timeScale = 1;

			} else
			{
                settingsPanel.SetActive(true);
                musicManager.SetLowPassCutOff(0.2f, -0.5f);
                Time.timeScale = 0;

                if (SceneManager.GetActiveScene().buildIndex != 0)
                {
                    print("in game scene");
                    exitGameButton.SetActive(false);
                    LeaveLobbyButton.SetActive(true);
                }
                else
                {
                    print("in lobby");
                    exitGameButton.SetActive(true);
                    LeaveLobbyButton.SetActive(false);
                }
            }
        }

        //screenshot https://gamedevbeginner.com/how-to-capture-the-screen-in-unity-3-methods/


        if (Input.GetKeyDown(KeyCode.Space))
		{
            ScreenCapture.CaptureScreenshot("screenshot " + System.DateTime.Now.ToString("MM-dd-yy (HH-mm-ss)") + ".png");
        }
    }

    public void LeaveLobby()
	{
        Time.timeScale = 1;
        print("leaveLobby");
        sceneLoader.LoadScene(0);
        settingsPanel.SetActive(false);
        score = 0;
    }

    public void ExitGame()
	{
        print("Exit Game");
        Time.timeScale = 1;
        Application.Quit();
        settingsPanel.SetActive(false);
        score = 0;
    }

    public void SaveTempSecond(int newScore)
	{
        score = newScore;

    }
}
