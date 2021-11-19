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
        timeLeft = timeBetweenLetters;
        letterInString = 0;
    }

    private void Update()
    {
        if (playing)
        {
            if (text.text == targetText.Replace("|",""))
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
                        text.text = $"{text.text}{targetText[letterInString]}";
                        talkingSound.Play();
                    }
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
