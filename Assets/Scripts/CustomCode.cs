using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomCode : MonoBehaviour
{
    private bool burningTime = false;
    private bool burningHalfTime = false;
    private float halfExtraTime;

    public static bool endingActive = false;

    public GameObject backing;
    public GameObject exitButton;
    public Animator ani;
    public Animator ani2;
    public GameObject PlayerFightEffect;

    [Header("Bad Ending")]
    public GameObject BadEndingEmpty;
    public GameObject BadEndingEffect;

    [Header("Fight Ending")]
    public GameObject FightEndingEmpty;

    [Header("Nothing Ending")]
    public GameObject NothingEndingEmpty;

    [Header("Pun Ending")]
    public GameObject PunEndingEmpty;

    [Header("Exectued Ending")]
    public GameObject ExecutedEndingEmpty;

    [Header("Good Ending")]
    public GameObject goodEndingSprite;
    public GameObject goodEndingEmpty;

    [Header("Dance Ending")]
    public GameObject DanceEndingEmpty;

    public void DrawSword()
    {
        PlayerFightEffect.SetActive(true);
        DialogueManagment.customCodeRunning = false;
    }

    public void DKLaugh()
    {
        ani.Play("DKLaugh");
        DialogueManagment.customCodeRunning = false;
    }

    public void DKAngry()
    {
        ani.Play("DKAngry");
        DialogueManagment.customCodeRunning = false;
    }

    public void FightPrep()
    {
        ani2.Play("FightPrep");
        DialogueManagment.customCodeRunning = false;
    }

    public void BurnExtraTime()
    {
        burningTime = true;
    }

    public void BurnHalfExtraTime()
    {
        burningHalfTime = true;
        burningTime = true;
        halfExtraTime = DialogueManagment.extraTime / 2;
        DialogueManagment.extraTime = halfExtraTime;
    }

    public void EndGame()
    {
        Application.Quit();
    }


    //ENDINGS
    public void BadEnding()
    {
        BadEndingEffect.SetActive(true);
        StartCoroutine(nameof(actualBadEnding));      
    }

    IEnumerator actualBadEnding()
    {
        yield return new WaitForSecondsRealtime(2);
        BadEndingEmpty.SetActive(true);
        EndingSetup();
    }

    public void DelayedBadEnding()
    {
        DialogueManagment.customCodeRunning = false;
        StartCoroutine(nameof(delayedBadEnding));
    }

    IEnumerator delayedBadEnding()
    {
        yield return new WaitForSecondsRealtime(6);
        BadEnding();
    }

    public void FightEnding()
    {
        EndingSetup();
        FightEndingEmpty.SetActive(true);
    }

    public void NothingEnding()
    {
        EndingSetup();
        NothingEndingEmpty.SetActive(true);
    }

    public void PunEnding()
    {
        EndingSetup();
        PunEndingEmpty.SetActive(true);
    }

    public void ExecutedEnding()
    {
        EndingSetup();
        ExecutedEndingEmpty.SetActive(true);
    }

    public void GoodEndingSetup()
    {    
        goodEndingSprite.SetActive(true);
        DialogueManagment.customCodeRunning = false;
    }

    public void ActualGoodEnding()
    {
        endingActive = true;
        goodEndingEmpty.SetActive(true);
        exitButton.SetActive(true);
    }

    public void DanceEnding()
    {
        EndingSetup();
        DanceEndingEmpty.SetActive(true);
    }

    private void EndingSetup()
    {
        backing.SetActive(true);
        exitButton.SetActive(true);
        endingActive = true;
    }

    private void Update()
    {
        if (burningTime)
        {
            if(DialogueManagment.extraTime <= 0)
            {
                if (burningHalfTime)
                {
                    DialogueManagment.extraTime = halfExtraTime;
                    burningHalfTime = false;
                }
                else
                {
                    DialogueManagment.extraTime = 0;
                }              
                burningTime = false;
                DialogueManagment.customCodeRunning = false;
            }
            else
            {
                DialogueManagment.extraTime -= Time.deltaTime;
            }
        }
    }
}
