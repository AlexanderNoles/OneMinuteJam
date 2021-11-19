using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCode : MonoBehaviour
{
    private bool burningTime = false;

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

    public void BadEnding()
    {
        Debug.Log("Bad Ending");
    }

    public void BurnExtraTime()
    {
        burningTime = true;
    }

    private void Update()
    {
        if (burningTime)
        {
            if(DialogueManagment.extraTime <= 0)
            {
                DialogueManagment.extraTime = 0;
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
