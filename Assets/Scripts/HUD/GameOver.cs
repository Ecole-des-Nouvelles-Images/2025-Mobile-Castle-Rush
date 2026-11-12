using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace HUD
{
    public class GameOver : MonoBehaviour
    {
        [SerializeField] private GameObject _endPanel;
        [SerializeField] private string _sceneToLoad;
    
        private void OnEnable()
        {
            EventBus.OnChronoAreFinished += OpenPanel;
        }

        private void OnDisable()
        {
            EventBus.OnChronoAreFinished -= OpenPanel;
        }

        private void OpenPanel()
        {
            _endPanel.SetActive(true);
            StartCoroutine("Delay");
            EventBus.OnGamePaused.Invoke();
        }
        
        private IEnumerator Delay()
        {
            yield return new WaitForSeconds(2f);
            transform.DOScale(0f, 1f).OnComplete(() =>
            {
                new WaitForSeconds(2f);
                SceneManager.LoadScene(_sceneToLoad);
            });
        }
    }
}
