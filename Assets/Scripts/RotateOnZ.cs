using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateOnZ : MonoBehaviour
{
    public float speed = 5f;
    const float globalSpeedModifer = 100f;

    void Update()
    {
        transform.Rotate(Vector3.forward * speed * globalSpeedModifer * Time.deltaTime);
    }
}
