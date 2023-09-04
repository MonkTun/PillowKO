using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//TODO gameManager and appManager input enter for next

public class DialogueManager : MonoBehaviour
{
	[SerializeField] TMP_Text youngerSisterText, olderSisterText;
	public Dialogue[] dialogues;

	bool isPlayingDialogue;

	[SerializeField] Image youngerSisImg, olderSisImg;
	[SerializeField] Sprite youngerSisIdle, youngerSisTalk, olderSisIdle, olderSisTalk;

	Coroutine youngerSisCoroutine, olderSisCoroutine;


	private void OnEnable()
	{
		youngerSisterText.text = string.Empty;
		olderSisterText.text = string.Empty;
		olderSisImg.sprite = olderSisIdle;
		youngerSisImg.sprite = youngerSisIdle;
		StartCoroutine(PlayDialogue());
	}

	IEnumerator PlayDialogue()
	{
		if (isPlayingDialogue) yield break;

		isPlayingDialogue = true;
		bool waiting = true;

		Dialogue selectedDialogue =
			dialogues[GameManager.Instance.round >= dialogues.Length ? dialogues.Length - 1 : GameManager.Instance.round];

		if (!selectedDialogue.oldersistertalkFirst)
		{
			youngerSisCoroutine = StartCoroutine(AnimateTextRoutine(youngerSisterText, selectedDialogue.dialogueLittleSis,
			selectedDialogue.textIntervalLittleSis, true));

			waiting = true;

			while (waiting)
			{
				if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
				{
					if (youngerSisCoroutine != null)
					{
						print("GetKeyDown and younger skips");
						StopCoroutine(youngerSisCoroutine);
						youngerSisCoroutine = null;
						youngerSisterText.text = selectedDialogue.dialogueLittleSis;
					}
					else
					{
						print("GetKeyDown and younger stops");
						waiting = false;
					}
				}

				yield return null;
			}

			olderSisCoroutine = StartCoroutine(AnimateTextRoutine(olderSisterText, selectedDialogue.dialogueOlderSis,
				selectedDialogue.textIntervalOlderSis, false));

			waiting = true;

			while (waiting)
			{
				if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
				{
					if (olderSisCoroutine != null)
					{
						print("GetKeyDown and older skips");
						StopCoroutine(olderSisCoroutine);
						olderSisCoroutine = null;
						olderSisterText.text = selectedDialogue.dialogueOlderSis;
					}
					else
					{
						print("GetKeyDown and older stops");
						waiting = false;
					}
				}

				yield return null;
			}
		} else
		{
			olderSisCoroutine = StartCoroutine(AnimateTextRoutine(olderSisterText, selectedDialogue.dialogueOlderSis,
				selectedDialogue.textIntervalOlderSis, false));

			waiting = true;

			while (waiting)
			{
				if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
				{
					if (olderSisCoroutine != null)
					{
						print("GetKeyDown and older skips");
						StopCoroutine(olderSisCoroutine);
						olderSisCoroutine = null;
						olderSisterText.text = selectedDialogue.dialogueOlderSis;
					}
					else
					{
						print("GetKeyDown and older stops");
						waiting = false;
					}
				}

				yield return null;
			}

			youngerSisCoroutine = StartCoroutine(AnimateTextRoutine(youngerSisterText, selectedDialogue.dialogueLittleSis,
			selectedDialogue.textIntervalLittleSis, true));

			waiting = true;

			while (waiting)
			{
				if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
				{
					if (youngerSisCoroutine != null)
					{
						print("GetKeyDown and younger skips");
						StopCoroutine(youngerSisCoroutine);
						youngerSisCoroutine = null;
						youngerSisterText.text = selectedDialogue.dialogueLittleSis;
					}
					else
					{
						print("GetKeyDown and younger stops");
						waiting = false;
					}
				}

				yield return null;
			}
		}

		

		GameManager.Instance.StartRound();
		isPlayingDialogue = false;
		/*
		while (true)
		{
			if (Input.GetKeyDown(KeyCode.Return))
			{
				GameManager.Instance.StartRound();
				isPlayingDialogue = false;
				print("GetKeyDown and exit");
			}
			yield return null;
		}
		*/
		
	}


	IEnumerator AnimateTextRoutine(TMP_Text textTarget, string entireText, float textInterval, bool isYounger)
	{
		textTarget.text = "";

		foreach (char letter in entireText.ToCharArray())
		{
			textTarget.text += letter;

			if (isYounger)
				if (youngerSisImg.sprite == youngerSisIdle) youngerSisImg.sprite = youngerSisTalk;
				else youngerSisImg.sprite = youngerSisIdle;
			else
				if (olderSisImg.sprite == olderSisIdle) olderSisImg.sprite = olderSisTalk;
				else olderSisImg.sprite = olderSisIdle;

			AppManager.Instance.sfxManager.PlaySound("TextScroll", 0.2f);
			yield return new WaitForSeconds(textInterval);
		}

		if (isYounger) youngerSisCoroutine = null;
		else olderSisCoroutine = null;
		olderSisImg.sprite = olderSisIdle;
		youngerSisImg.sprite = youngerSisIdle;
	}
}

[System.Serializable]
public class Dialogue
{
	//TODO can add portrait too
	public string dialogueLittleSis;
	public float textIntervalLittleSis;
	public string dialogueOlderSis;
	public float textIntervalOlderSis;
	public bool oldersistertalkFirst;
}