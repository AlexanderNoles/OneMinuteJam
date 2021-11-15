using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayDialogue : MonoBehaviour
{
    private bool playing;
    public TextMeshProUGUI text;
    public float timeBetweenLetters = 0.1f;
    private float timeLeft;
    private string targetText;
    private int letterInString;

    public void Play(string newText)
    {
        playing = true;
        text.text = "";
        targetText = newText;
        timeLeft = timeBetweenLetters;
        letterInString = 0;
    }

    private void Update()
    {
        if (playing)
        {
            if (text.text == targetText)
            {
                playing = false;
                timeLeft = timeBetweenLetters;
            }
            else
            {
                //Add on a new letter 
                if (timeLeft < 0)
                {
                    text.text = $"{text.text}{targetText[letterInString]}";
                    ++letterInString;
                    timeLeft = timeBetweenLetters;
                }
                else
                {
                    timeLeft -= Time.deltaTime;
                }
            }
        }
        else
        {
            if(timeLeft < 0)
            {
                DialogueManagment.dialogueFinished = true;
            }
            else
            {
                timeLeft -= Time.deltaTime;
            }
        }
    }
}
