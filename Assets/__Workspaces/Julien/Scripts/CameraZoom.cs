using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraScaler : MonoBehaviour
{
    // Ta résolution de référence (Tablette)
    private float refWidth = 2304f;
    private float refHeight = 1440f;
    
    // La taille orthographique que tu as réglée sur ta tablette (ex: 5 ou 7)
    public float baseOrthographicSize = 5.4f; 

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        AdjustCamera();
    }

    private void Update()
    {
        AdjustCamera();
    }

    void AdjustCamera()
    {
        float targetAspect = refWidth / refHeight; // 1.6
        float currentAspect = (float)Screen.width / Screen.height;

        // Si l'écran est plus large ou plus étroit, on adapte la taille 
        // pour que la LARGEUR visible reste constante.
        float horizontalScale = targetAspect / currentAspect;
        cam.orthographicSize = baseOrthographicSize * horizontalScale;
    }
}