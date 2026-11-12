using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSelectionEvent : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private float scaleMultiplier = 1.1f;
    [SerializeField] private float duration = 0.15f;
    [SerializeField] private Ease selectEase = Ease.OutBack;
    [SerializeField] private Ease deselectEase = Ease.OutQuad;

    private GameObject currentSelection;
    private GameObject previousSelection;

    void Update()
    {
        currentSelection = EventSystem.current.currentSelectedGameObject;

        if (currentSelection != previousSelection)
        {
            // Remet l'ancien à sa taille normale
            if (previousSelection != null)
            {
                previousSelection.transform.DOKill();

                previousSelection.transform
                    .DOScale(Vector3.one, duration)
                    .SetEase(deselectEase);
            }

            // Agrandit le nouveau
            if (currentSelection != null)
            {
                currentSelection.transform.DOKill();

                currentSelection.transform
                    .DOScale(Vector3.one * scaleMultiplier, duration)
                    .SetEase(selectEase);
            }

            previousSelection = currentSelection;
        }
    }
}
