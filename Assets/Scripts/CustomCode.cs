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

    [Header("BadEnding")]
    public GameObject BadEndingEmpty;

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
        backing.GetComponent<Image>().color = Color.black;
        backing.SetActive(true);
        BadEndingEmpty.SetActive(true);
        endingActive = true;
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
