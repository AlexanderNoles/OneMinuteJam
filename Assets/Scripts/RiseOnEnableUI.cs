using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiseOnEnableUI : MonoBehaviour
{
    public float speed = 10f;
    private Vector2 cachedTargetPos;
    private RectTransform rt;

    private void OnEnable()
    {
        if(rt == null)
        {
            rt = GetComponent<RectTransform>();
            cachedTargetPos = rt.anchoredPosition;
        }
        rt.anchoredPosition = cachedTargetPos + (Vector2.down * 400f);
    }

    private void Update()
    {
        rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition,cachedTargetPos,speed * Time.deltaTime);
    }
}
