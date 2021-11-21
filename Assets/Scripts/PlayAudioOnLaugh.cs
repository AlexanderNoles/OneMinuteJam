using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudioOnLaugh : MonoBehaviour
{
    private AudioSource laugh;
    private Animator ani;

    void Start()
    {
        laugh = GetComponent<AudioSource>();
        ani = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(ani.GetCurrentAnimatorStateInfo(0).IsName("DKLaugh") && !laugh.isPlaying)
        {
            laugh.Play();
        }
    }
}
