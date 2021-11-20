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

    [Header("BadEnding")]
    public GameObject BadEndingEmpty;

    [Header("FightEnding")]
    public GameObject FightEndingEmpty;

    public void DrawSword()
    {
        Debug.Log("Sword Drawn");
        DialogueManagment.customCodeRunning = false;
    }

    public void DKLaugh()
    {
        Debug.Log("Demon King Laughing");
        DialogueManagment.customCodeRunning = false;
    }

    public void FightPrep()
    {
        Debug.Log("Fight Prep");
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


    //ENDINGS
    public void BadEnding()
    {
        EndingSetup();
        BadEndingEmpty.SetActive(true);
    }

    public void FightEnding()
    {
        EndingSetup();
        FightEndingEmpty.SetActive(true);
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
