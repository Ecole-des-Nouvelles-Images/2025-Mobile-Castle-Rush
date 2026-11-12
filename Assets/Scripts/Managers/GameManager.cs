using System.Collections;
using ScriptableObjectsScripts.Difficulty;
using UnityEngine;
using UnityEngine.Rendering;
using Utils;

namespace Managers
{
    public class GameManager : MonoBehaviourSingleton<GameManager>
    {
        [SerializeField] private SplineManager _splineManager;
        [SerializeField] private GameObject _cardPropositionPanel;

        [Header("Card")] 
        public int NumberCard;

        [SerializeField] private float TimeEndingRound;


        public SoDifficultySpline DifficultySplineTest;
        
        private void OnEnable()
        {
            EventBus.OnLevelFinished += LoadSelectionCard;
            EventBus.OnPlayerTakedCard += LoadNewLevel;
        }
    
        private void OnDisable()
        {
            EventBus.OnLevelFinished -= LoadSelectionCard;
            EventBus.OnPlayerTakedCard -= LoadNewLevel;
        }

        private void Start()
        {
            EventBus.OnGameStart?.Invoke();
        }

        public void LoadNewLevel()
        {
            EventBus.OnNextLevel?.Invoke();
        }

        public void LoadSelectionCard()
        {
            StartCoroutine("LoadSelectionCardDelay");
        }

        public IEnumerator LoadSelectionCardDelay()
        {
            yield return new WaitForSeconds(TimeEndingRound);
            _cardPropositionPanel.SetActive(true);
            _cardPropositionPanel.GetComponent<CardManager>().LoadCardProposition();
        }
    }
}
