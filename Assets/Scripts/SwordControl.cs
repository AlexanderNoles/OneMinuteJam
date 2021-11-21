using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordControl : MonoBehaviour
{
    private Animator ani;
    private AudioSource audio0;
    public GameObject swordToCreate;
    public float timeBetweenSwords = 3f;
    private float timeLeft;

    void Start()
    {
        ani = GetComponent<Animator>();
        audio0 = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (ani.GetCurrentAnimatorStateInfo(0).IsName("SwordEmpty"))
        {
            if(timeLeft <= 0)
            {
                Instantiate(swordToCreate, transform.position + (Vector3.up * Random.Range(4f, 7f)) + (Vector3.right * Random.Range(-3f, 3f)), Quaternion.identity);
                timeLeft = timeBetweenSwords;
                audio0.Play();
            }
            else
            {
                timeLeft -= Time.deltaTime;
            }
        }
    }
}
