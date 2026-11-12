using System.Collections.Generic;
using DG.Tweening;
using Managers;
using Player;
using ScriptableObjectsScripts.Upgrades;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using Utils;
using Image = UnityEngine.UI.Image;

namespace Class
{
    public class Card : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        public Upgrade Upgrade;
        private string _name;
        private Sprite _sprite;
        private string _description;
        public LocalizeStringEvent LocalizeStringEvent;
        
        [SerializeField] private Color32 _unselectedColor;

        [SerializeField] private GameObject _outLineParent;
        [SerializeField] private InventoryHandler _inventory;
    
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Image _image;
        [SerializeField] private Image _typeImage;
        
        [Header("Rarity")]
        [SerializeField] private Sprite _communCadre;
        [SerializeField] private Sprite _moyenCadre;
        [SerializeField] private Sprite _rareCadre;
        
        [Header("Type")]
        
        [SerializeField] private Sprite _enemyType;
        [SerializeField] private Sprite _spellType;
        [SerializeField] private Sprite _diversType;

        [Header("Audio")]
        
        public List<AudioClip> AudioClip;
        
        private Image _imageParent;
        private void Start()
        {
            SetUp(Upgrade);
        }

        public void SetUp(Upgrade upgrade)
        {
            _imageParent = GetComponent<Image>();
            Upgrade = upgrade;
            _name = upgrade.name;
            _sprite = upgrade.Srite;

            LocalizeStringEvent.StringReference = upgrade.LocalizedString;
            _image.sprite = _sprite;
        
            _inventory = GameObject.Find("InventoryHandler").GetComponent<InventoryHandler>();
            Image image = gameObject.GetComponent<Image>();
            
            if (Upgrade.Rarity is Rarity.Commun) image.sprite = _communCadre;
            if (Upgrade.Rarity is Rarity.Moyen) image.sprite = _moyenCadre;
            if (Upgrade.Rarity is Rarity.Rare) image.sprite = _rareCadre;
            
            if(upgrade is UpgradeEnemy) _typeImage.sprite = _enemyType;
            if(upgrade is UpgradeSpell) _typeImage.sprite = _spellType;
            if(upgrade is UpgradeDivert) _typeImage.sprite = _diversType;
            if(upgrade is UnlockNewTroup) _typeImage.sprite = _diversType;
        }
    
        public void OnClick()
        {
            Debug.Log("UPGRADE");
            Upgrade.Apply(_inventory);
            SoundManager.Instance.PlayRandomSound(AudioClip, gameObject);
            EventBus.OnPlayerTakedCard?.Invoke();
        }

        public void OnSelect(BaseEventData eventData)
        {
            for (int i = 0; i < _outLineParent.transform.childCount; i++)
            {
                var image = _outLineParent.transform.GetChild(i).GetComponent<Image>();
                image.color = Color.white;
            }
            _imageParent.DOBlendableColor(Color.white, 0.5f);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            for (int i = 0; i < _outLineParent.transform.childCount; i++)
            {
                var image = _outLineParent.transform.GetChild(i).GetComponent<Image>();
                image.color = Color.black;
            }
            _imageParent.DOBlendableColor(_unselectedColor, 0.5f);
        }
    }
}
