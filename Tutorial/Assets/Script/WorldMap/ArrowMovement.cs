using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowMovement : MonoBehaviour
{
    void Update()
    {
        GetComponent<RectTransform>().anchoredPosition = new Vector2(GetComponent<RectTransform>().anchoredPosition.x, GetComponent<RectTransform>().anchoredPosition.y + Mathf.Sin(Time.timeSinceLevelLoad * 4f) * 0.15f);
    }
}
