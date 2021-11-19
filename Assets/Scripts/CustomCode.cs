using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCode : MonoBehaviour
{
    private bool burningTime = false;
    private bool burningHalfTime = false;
    private float halfExtraTime;

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
        Debug.Log("Bad Ending");
    }

    public void FightEnding()
    {
        Debug.Log("FightEnding");
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
