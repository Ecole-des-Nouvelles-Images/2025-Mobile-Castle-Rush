using System;
using UnityEngine;

public class ForcePoX : MonoBehaviour
{
    public RectTransform RectTransform;

    private void Update()
    {
        RectTransform.anchoredPosition = Vector2.zero;
        RectTransform.anchorMin = RectTransform.anchorMax = new Vector2(0, 0); // bas gauche
        RectTransform.anchoredPosition = new Vector2(430, 100);
    }
}
