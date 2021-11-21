using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamEffect : MonoBehaviour
{
    public int afterSeconds;
    public float speed = 5f;
    public float lightSpeed = 1f;
    private float cachedScaleY;
    private float cachedScaleX;
    private List<Transform> lightEffect = new List<Transform>();
    private List<Vector3> cachedLightEffectSize = new List<Vector3>();
    private AudioSource beamSound;
    private bool soundPlayed;

    private void Start()
    {
        beamSound = GetComponent<AudioSource>();
        cachedScaleY = transform.localScale.y;
        cachedScaleX = transform.localScale.x;
        transform.localScale = new Vector3(0,cachedScaleY);
        foreach(Transform child in transform)
        {
            lightEffect.Add(child);
            cachedLightEffectSize.Add(child.localScale);
        }
    }

    private void Update()
    {
        if(Time.timeSinceLevelLoad > afterSeconds)
        {
            transform.localScale = new Vector3(Mathf.Lerp(transform.localScale.x,cachedScaleX,Time.deltaTime * speed), cachedScaleY);
            for(int i = 0; i < transform.childCount; i++)
            {
                lightEffect[i].localScale = cachedLightEffectSize[i] + (Vector3.right * (Mathf.Cos(Time.time * lightSpeed) / 10));
            }
            if (!soundPlayed && !CustomCode.endingActive)
            {
                beamSound.Play();
                soundPlayed = true;
            }         
        }
    }
}
