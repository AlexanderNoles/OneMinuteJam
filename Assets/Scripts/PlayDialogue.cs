using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayDialogue : MonoBehaviour
{
    private bool playing;
    public TextMeshProUGUI text;
    public float timeBetweenLetters = 0.1f;
    private float currentTimeBetweenLetters;
    private float timeLeft;
    private string targetText;
    private int letterInString;
    private AudioSource talkingSound;
    public float timeBeforeClose = 0.2f;

    private void Start()
    {
        talkingSound = GetComponent<AudioSource>();
    }

    public void Play(string newText)
    {
        playing = true;
        text.text = "";
        targetText = newText;
        currentTimeBetweenLetters = timeBetweenLetters;
        timeLeft = currentTimeBetweenLetters;
        letterInString = 0;
    }

    private void Update()
    {
        if (playing)
        {
            if (text.text == targetText.Replace("|","").Replace("^",""))
            {
                playing = false;
                timeLeft = timeBeforeClose;
            }
            else
            {
                //Add on a new letter 
                if (timeLeft < 0)
                {

                    if(targetText[letterInString] != '|')
                    {
                        if (targetText[letterInString] == '^')
                        {
                            currentTimeBetweenLetters = 0.05f;
                        }
                        else
                        {
                            text.text = $"{text.text}{targetText[letterInString]}";
                            if (!CustomCode.endingActive)
                            {
                                talkingSound.Play();
                            }
                        }
                    }
                    ++letterInString;
                    timeLeft = currentTimeBetweenLetters;                
                }
                else
                {
                    timeLeft -= Time.deltaTime;
                }
            }
        }
        else
        {
            if(timeLeft < 0 && !DialogueManagment.dialogueFinished)
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
