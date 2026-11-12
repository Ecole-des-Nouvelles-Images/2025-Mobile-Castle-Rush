using DG.Tweening;
using Managers;
using Structs;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

namespace Buttons
{
    public class EnemyButtonSpawn : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        [SerializeField] private TMP_Text _priceText;
        [SerializeField] private Image _image;
        [SerializeField] private Image _outLine;
        [SerializeField] private AnimationCurve _animationCurveBounce;
        public EnemyClass EnemyClass;

        private void OnEnable()
        {
            EventBus.OnLevelFinished += DisableClick;
            EventBus.OnPlayerTakedCard += EnableClick;
        }

        private void OnDisable()
        {
            EventBus.OnLevelFinished -= DisableClick;
            EventBus.OnPlayerTakedCard -= EnableClick;
        }
        
        private void Start()
        {
            SetUp();
        }

        public void SetUp()
        {
            _priceText.text = EnemyClass.price.ToString();
            if(_image) _image.sprite = EnemyClass.baseData.Sprite;
        }

        public void OnClick()
        {
            transform.DOScale(1.2f, 0.05f).SetEase(_animationCurveBounce);
            SpawnManager.Instance.Spawn(EnemyClass);
        }
        
        private void EnableClick()
        {
            gameObject.GetComponent<Button>().interactable = true;
        }
        private void DisableClick()
        {
            gameObject.GetComponent<Button>().interactable = false;
        }

        public void OnSelect(BaseEventData eventData)
        {
            Debug.Log("OnSelect");
            _outLine.color = Color.white;
        }

        public void OnDeselect(BaseEventData eventData)
        {
            Debug.Log("OnCancel");
            _outLine.color = Color.black;
        }
    }
}
