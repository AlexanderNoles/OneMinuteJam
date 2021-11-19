using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOffAudioOnEnding : MonoBehaviour
{
    private AudioSource As;

    void Start()
    {
        As = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (CustomCode.endingActive)
        {
            As.enabled = false;
        }
    }
}
