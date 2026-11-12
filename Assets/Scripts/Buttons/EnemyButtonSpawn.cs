using Managers;
using Structs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Buttons
{
    public class EnemyButtonSpawn : MonoBehaviour
    {
        [SerializeField] private TMP_Text _priceText;
        [SerializeField] private Image _image;
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
    }
}
