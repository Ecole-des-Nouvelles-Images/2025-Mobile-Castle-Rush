using UnityEngine;
using UnityEngine.EventSystems;

public class PanelWithController : MonoBehaviour
{
    [SerializeField] private GameObject _firstSelected;
    
    private void OnEnable()
    {
        EventSystem eventSystem = GameObject.FindWithTag("EventSystem").GetComponent<EventSystem>();
        eventSystem.SetSelectedGameObject(_firstSelected);
        Debug.Log("Set le bouton");
    }
}
