using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Refree is not going to have any game rule stuff, this is just animation
/// </summary>
public class Refree : MonoBehaviour
{
    [SerializeField] private TMP_Text refreeText;
	[SerializeField] private Animator AN;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnterScene() //this is also the setupOf refree
	{
        AN.SetTrigger("Enter");
     
    }

    public void ExitScene()
	{
        AN.SetTrigger("Exit");
    }

    /*
    public void StartCountDown()
	{
        //AN.SetTrigger(); TODO
        if (!isCountDowning) StartCoroutine(CountDown());
	}

    IEnumerator CountDown() //TODO optimize
    {
        isCountDowning = true;
        countDown = 0;

        while (true) 
        {
            yield return new WaitForSeconds(1);
            countDown++;
            refreeText.text = countDown.ToString();
        }
    }
    
    public void EndCountDown()
    {
        isCountDowning = false;
        StopCoroutine(CountDown());
        refreeText.text = "";
    }*/

    public void SpeakMessage(string message)
	{
        //StopCoroutine(CountDown());
        refreeText.text = message;
    }
}
