using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonKingControl : MonoBehaviour
{
    private AudioSource laugh;
    private SpriteRenderer sr;
    private Animator ani;
    public Sprite laughIndicatorSprite;
    private Sprite spriteLastFrame;
    private int childLastLaugh = -1;
    private bool laughPlayed;

    void Start()
    {
        laugh = GetComponent<AudioSource>();
        sr = GetComponent<SpriteRenderer>();
        ani = GetComponent<Animator>();
    }

    void Update()
    {
        if(sr.sprite == laughIndicatorSprite && spriteLastFrame != laughIndicatorSprite)
        {
            laugh.Play();
            RandomLaughEffect();
            laughPlayed = true;
        }

        if (laughPlayed && ani.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            HideAllChildren();
            laughPlayed = false;
        }

        spriteLastFrame = sr.sprite;
    }

    private void RandomLaughEffect()
    {
        HideAllChildren();
        //Show one child at random
        int childIndex;
        while(true)
        {
            childIndex = Random.Range(0, transform.childCount);
            if(childIndex != childLastLaugh)
            {
                break;
            }
        }
        transform.GetChild(childIndex).gameObject.SetActive(true);
        childLastLaugh = childIndex;
    }

    private void HideAllChildren()
    {
        //Hide all children
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}
