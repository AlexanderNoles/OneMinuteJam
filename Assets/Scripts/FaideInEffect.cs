using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FaideInEffect : MonoBehaviour
{
    private Image image;
    public float speed = 10f;

    private void OnEnable()
    {
        image = GetComponent<Image>();
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
    }

    private void Update()
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(image.color.a, 1, Time.deltaTime * speed));
    }
}
