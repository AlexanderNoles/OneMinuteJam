using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LowerAlphaOnStart : MonoBehaviour
{
    private Image image;
    public float speed = 10f;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    private void Update()
    {
        image.color = new Color(image.color.r,image.color.g,image.color.b,Mathf.Lerp(image.color.a,0,Time.deltaTime * speed));
        if(image.color.a < 0.05f)
        {
            Destroy(gameObject);
        }
    }
}
