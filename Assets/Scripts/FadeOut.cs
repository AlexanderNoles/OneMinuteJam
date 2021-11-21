using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOut : MonoBehaviour
{
    private SpriteRenderer image;
    public float speed = 10f;

    void Start()
    {
        image = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        GetComponent<AudioSource>().Play();
    }

    void Update()
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(image.color.a, 0, Time.deltaTime * speed));
    }
}
