using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadEndingEffectScript : MonoBehaviour
{
    public float speed = 10f;

    private void OnEnable()
    {
        transform.localScale = Vector3.zero;
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * 100, Time.deltaTime * speed);
    }
}
