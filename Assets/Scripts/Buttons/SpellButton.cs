using System;
using Class;
using DG.Tweening;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

namespace Buttons
{
    public class SpellButton : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        public bool AreSelected;
        public SpellClass SpellClass;
        
        [SerializeField] private TMP_Text _priceText;
        [SerializeField] private Image _image;
        [SerializeField] private Image _outLine;

        private Vector3 _basePosition;
        [SerializeField] private CursorController _controlleManager;
        
        private void OnEnable()
        {
            EventBus.OnLevelFinished += DisableClick;
            EventBus.OnPlayerTakedCard += EnableClick;
            //EventBus.OnPlayerPlaceSpell += UnselectedVisuel;
            EventBus.OnLevelFinished += ConfirmUnselectedVisuel;
        }

        private void OnDisable()
        {
            EventBus.OnLevelFinished -= DisableClick;
            EventBus.OnPlayerTakedCard -= EnableClick;
            //EventBus.OnPlayerPlaceSpell -= UnselectedVisuel;
            EventBus.OnLevelFinished -= ConfirmUnselectedVisuel;
        }
        
        private void Start()
        {
            if (GameObject.FindWithTag("Controller"))_controlleManager = GameObject.FindWithTag("Controller").GetComponent<CursorController>();
            
            _basePosition = transform.position;
            SetUp();
        }

        public void SetUp()
        {
            if(_image) _image.sprite = SpellClass.SpellData.Sprite;
            _priceText.text = SpellClass.SpellData.Price.ToString();
        }

        public void OnClick()
        {
            if (SpellClass == InventoryHandler.Instance.EquipedSpell)
            {
                //InventoryHandler.Instance.UnEquipSpell();
                ConfirmUnselectedVisuel();
                return;
            }
            ConfirmUnselectedVisuel();
            ConfirmSelectedVisual();
            //_controlleManager.EnableCursorSpell();
            InventoryHandler.Instance.EquipedSpell = SpellClass;
            EventBus.OnPlayerSelectSpell?.Invoke();
        }

        public void ConfirmSelectedVisual()
        {
            transform.DOLocalMoveY(_basePosition.y + 30, 0.2f);
            AreSelected = true;
        }
        
        [ContextMenu("Reset visual")]
        public void ConfirmUnselectedVisuel()
        {
            GameObject parent = transform.parent.gameObject;
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                parent.transform.GetChild(i).GetComponent<SpellButton>().AreSelected = false;
                parent.transform.GetChild(i).gameObject.transform.DOLocalMoveY(_basePosition.y, 0.2f);
            }
            //_controlleManager.PlaceSpell();
            InventoryHandler.Instance.EquipedSpell = null;
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
