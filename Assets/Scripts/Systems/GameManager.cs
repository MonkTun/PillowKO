using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;
//using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    #region fields
    public static GameManager Instance { get; private set; } //I like to capitalize Instance...

    public enum GameState //TODO
    {
        preRound, Round, KnockedDown
    }

    public GameState gameState;

    public float timeRemaining { get; private set; }

    public int round { get; private set; }
    public int countDown { get; private set; }
    public int maxCountDown { get; private set; }
    public bool isCountDowning { get; private set; }

    [Header("references")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private GameObject playPanel, breakPanel, resultPanel, messagePanel;
    [SerializeField] private Image playerHealthBarFill, enemyHealthBarFill;
    [SerializeField] private AudioClip matchMusic;
    [SerializeField] private TMP_Text resultText, roundNumText;
    [SerializeField] private TMP_Text staminaText, starText, messagePanelText;
    [SerializeField] private GameObject getUpText;
    [SerializeField] private TMP_Text scoreText, scoreTextEnd;
    [SerializeField] private GameObject nextButton, returnHomeButton, restartButton;
    [SerializeField] private TMP_Text breakText;
	//[SerializeField] TMP_Text timerText2;


	[Header("LOCAL")]
    [SerializeField] private Refree refree;
    [SerializeField] private Animator backgroundAN;
	//


    //private Keyboard KB;
    private Coroutine countdownCoroutine;
    private Coroutine enemyHealthBarFillCoroutine, playerHealthBarFillCoroutine;

    [Header("game rules")]
    [SerializeField] private float timePerRound = 60;

    float roundTimer;

    int score;

    #endregion

    #region init

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //KB = Keyboard.current;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //StartRound(); //TODO: later call this after cutscene.

        if (SceneManager.GetActiveScene().buildIndex != 1)
		{
            AppManager.Instance.musicManager.UpdatePitch(1, 2);
        }
        AppManager.Instance.musicManager.PlayMusic(matchMusic, true, 1, 0.4f, false);
        /*
        if (!AppManager.Instance.musicManager.PlayMusic(matchMusic, true, 1, 0.4f, true))
        {
            //AppManager.Instance.musicManager.UpdatePitch(1,1);
            AppManager.Instance.musicManager.UpdateVolume(2, 0.4f);
        }

        AppManager.Instance.musicManager.SetLowPassCutOff(3, 1);*/

        if (AppManager.Instance.score > 0)
		{
            score = AppManager.Instance.score;
            scoreText.text = score.ToString();
        }

        PanelManage(breakPanel);
        breakText.text = "PRE ROUND";
    }

    #endregion

    #region Game System

    public void StartRound() //TODO fix
    {
        StartCoroutine(StartRoundRoutine());
    }

    IEnumerator StartRoundRoutine()
	{
        isRoundRoutinePlaying = true;
        backgroundAN.SetBool("Cheer", true);
        AppManager.Instance.musicManager.SetLowPassCutOff(3, 1);
        round++;
        messagePanelText.text = "ROUND " + round;
        roundNumText.text = "ROUND " + round;
        PanelManage(messagePanel);
        //TODO sound effect
        AppManager.Instance.sfxManager.PlaySound("CheerShort");
        yield return new WaitForSeconds(2);
        AppManager.Instance.sfxManager.PlaySound("CheerShort");
        PanelManage(playPanel);
        roundTimer = timePerRound;
        gameState = GameState.Round;
        yield return new WaitForSeconds(0.1f);
        AppManager.Instance.sfxManager.PlaySound("Ding", 1);
        //isPaused = false;
        backgroundAN.SetBool("Cheer", false);
        isRoundRoutinePlaying = false;
    }

    bool isRoundRoutinePlaying;

    public void EndRound()
	{
        StopAllCoroutines();
        StartCoroutine(EndRoundRoutine());
    }

    IEnumerator EndRoundRoutine()
    {
        isRoundRoutinePlaying = true;
        backgroundAN.SetBool("Cheer", true);
        AppManager.Instance.sfxManager.PlaySound("Ding3", 1);
        //TODO refree goes TKO
        gameState = GameState.KnockedDown;
        messagePanelText.text = "";
        PanelManage(messagePanel);

        Time.timeScale = 0.5f;


        yield return new WaitForSeconds(1);

        Time.timeScale = 1;
        PanelManage(breakPanel);
        breakText.text = "Round Break";
        backgroundAN.SetBool("Cheer", false);
        //isPaused = true;
        //TODO sound
        AppManager.Instance.musicManager.SetLowPassCutOff(3, -1);
        isRoundRoutinePlaying = false;
    }

    public void StartCountDown(bool isEnemyDown, int countDownMax)
    {
        gameState = GameState.KnockedDown;
        countDown = 0;
        isCountDowning = true;
        maxCountDown = countDownMax;
        refree.EnterScene();
        refree.SpeakMessage("");
        countdownCoroutine = StartCoroutine(CountDown());
        if (!isEnemyDown) getUpText.SetActive(true);
        backgroundAN.SetBool("Cheer", true);
    }
    public void EndCountDown(bool isEnemyDown, bool ableToGetUp)
    {
        isCountDowning = false;
        backgroundAN.SetBool("Cheer", false);
        if (countdownCoroutine != null) StopCoroutine(countdownCoroutine);
        StartCoroutine(RefreeDecision(isEnemyDown, ableToGetUp));
    }
    IEnumerator CountDown()
	{
        while (countDown < maxCountDown)
		{
            countDown++;

            if (isRoundRoutinePlaying)
			{
                yield break;
			}

            yield return new WaitForSeconds(1);
            refree.SpeakMessage(countDown.ToString());
        }
        Lose();
        yield return null;
	}
    IEnumerator RefreeDecision(bool isEnemyDown, bool ableToGetUp) //TODO fix
    {
        yield return new WaitForSeconds(1);

        if (!isEnemyDown) getUpText.SetActive(false);

        if (ableToGetUp)
		{
            refree.SpeakMessage("RESUME");
            refree.ExitScene();
        } else
		{
            refree.SpeakMessage("KO");
        }

        yield return new WaitForSeconds(1);

        if (!ableToGetUp)
		{
            if (isEnemyDown)
		    {
                Win();
		    } else
		    {
                Lose();
		    }
            
            yield break;
		}

        gameState = GameState.Round;

        yield return null;
    }

    public void Win()
    {
        scoreTextEnd.text = scoreText.text;

        if (AppManager.Instance.settingsManager.GetHighestScore() < score)
		{
            AppManager.Instance.settingsManager.UpdateHighestScore(score);

        }

        if (SceneManager.GetActiveScene().buildIndex == 1)
		{
            //TODO update state
            PanelManage(resultPanel);
            resultText.text = "WIN!\nReady for the next challege";
            nextButton.SetActive(true);
            returnHomeButton.SetActive(false);
            restartButton.SetActive(false);
            AppManager.Instance.SaveTempSecond(score);
        } else
		{
            PanelManage(resultPanel);
            resultText.text = "WIN!\nYou made your sister finally sleep!";
            nextButton.SetActive(false);
            returnHomeButton.SetActive(true);
            restartButton.SetActive(false);
        }
    }

    public void Lose()
	{
        scoreTextEnd.text = scoreText.text;

        if (AppManager.Instance.settingsManager.GetHighestScore() < score)
        {
            AppManager.Instance.settingsManager.UpdateHighestScore(score);

        }

        PanelManage(resultPanel);
        resultText.text = "Lose!\nYou got this! try again";
        nextButton.SetActive(false);
        returnHomeButton.SetActive(false);
        restartButton.SetActive(true);
    }

    public void Restart() //placeholder
	{
        AppManager.Instance.sceneLoader.LoadScene(1);
        AppManager.Instance.SaveTempSecond(0);
	}
    public void NextGame() //placeholder
    {
        AppManager.Instance.sceneLoader.LoadScene(2);
    }
    public void GoHome() //placeholder
    {
        AppManager.Instance.sceneLoader.LoadScene(0);
        AppManager.Instance.SaveTempSecond(0);
    }

    public void StarPunchCheer()
	{
        StartCoroutine(StarPunchCheerRoutine());
    }

    IEnumerator StarPunchCheerRoutine()
	{
        backgroundAN.SetBool("Cheer", true);
        yield return new WaitForSeconds(1);
        backgroundAN.SetBool("Cheer", false);
    }

    #endregion

    #region Update

    // Update is called once per frame
    void Update()
    {
        if (gameState == GameState.Round)
            if (roundTimer > 0)
		    {
                roundTimer -= Time.deltaTime;
                timerText.text = String.Format("{0:D1}:{1:D2}",
                (int)(roundTimer / 60),
                (int)(roundTimer % 60));
            } 
            else
		    {
                EndRound();
            }

        if (resultPanel.activeInHierarchy)
		{
            if (restartButton.activeInHierarchy && (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0)))
			{
                Restart();
			} else if (nextButton.activeInHierarchy && (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0)))
            {
                NextGame();
			}
            else if (returnHomeButton.activeInHierarchy && (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0)))
            {
                GoHome();
            }
        }
    }

	#endregion

	#region Update UI

	public void PlayerHealthUpdate(int health, int maxHealth)
	{
        if (playerHealthBarFillCoroutine != null) 
            StopCoroutine(playerHealthBarFillCoroutine);

        playerHealthBarFillCoroutine = StartCoroutine(UpdatehealthbarRoutine(playerHealthBarFill, 0.25f, (float)health / (float)maxHealth));
    }

    public void OpponentHealthUpdate(int health, int maxHealth)
    {
        if (enemyHealthBarFillCoroutine != null)
            StopCoroutine(enemyHealthBarFillCoroutine);

        enemyHealthBarFillCoroutine = StartCoroutine(UpdatehealthbarRoutine(enemyHealthBarFill, 0.25f, (float)health / (float)maxHealth));
    }

    IEnumerator UpdatehealthbarRoutine(Image targetGFX, float duration, float targetAmount)
	{
        float initialAmount = targetGFX.fillAmount;
        float count = duration;

        while (count > 0)
		{
            count -= Time.deltaTime;
            targetGFX.fillAmount = Mathf.Lerp(initialAmount, targetAmount, (duration - count) / duration);
            yield return null;
		}
	}

    public void UpdateStamina(int num)
	{
        staminaText.text = num.ToString();
	}

    public void UpdateStar(int num)
    {
        Debug.Log($"{num}");
        starText.text = num.ToString();
    }

    public void AddScore(int amount)
	{
        score += amount;
        scoreText.text = score.ToString();

    }

	void PanelManage(GameObject targetOpen)
	{
        //pausedPanel.SetActive(targetOpen == pausedPanel);
        playPanel.SetActive(targetOpen == playPanel);
        breakPanel.SetActive(targetOpen == breakPanel);
        resultPanel.SetActive(targetOpen == resultPanel);
        messagePanel.SetActive(targetOpen == messagePanel);
    }

	#endregion
}


