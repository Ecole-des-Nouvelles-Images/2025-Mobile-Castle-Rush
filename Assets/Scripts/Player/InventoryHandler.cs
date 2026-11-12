using System;
using System.Collections.Generic;
using Buttons;
using Class;
using Managers;
using ScriptableObjectsScripts.Spells;
using ScriptableObjectsScripts.Upgrades;
using Structs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace Player
{
    public class InventoryHandler : MonoBehaviourSingleton<InventoryHandler>
    {
        [Header("-------------Money")]
        [Header("-----StartMoney")]
        public int MaxMoney;
        public int MaxElixir;
        
        [Header("-----Money")]
        [SerializeField] private float _money;
        public float MaxTimeBeforeGetMoney;
        private float _timeBeforeGetMoney;
        public float MoneyParHit;
        
        [Header("-----Elixir")]
        [SerializeField] private float _elixir;
        public float MaxTimeBeforeGetElixir;
        private float _timeBeforeGetElixir;
        public float ElixirParHit;

        [Header("Inventory")] 
        [SerializeField] private Vector3 _elixirSpawnTextPosition;
        [SerializeField] private Vector3 _moneySpawnTextPosition;
        public float Money
        {
            get => _money;
            set
            {
                _money = value;
                EventBus.OnPlayerUseMoney?.Invoke();
            }
        }
        
        public float Elixir
        {
            get => _elixir;
            set
            {
                _elixir = value;
                EventBus.OnPlayerUseElixir?.Invoke();
            }
        }

        public SpellClass EquipedSpell;
    
        public List<EnemyClass> EnemyClass = new List<EnemyClass>();
        public List<SpellClass> SpellClasses = new List<SpellClass>();

        public List<EnemyButtonSpawn> EnemyButtonSpawns = new List<EnemyButtonSpawn>();
        private List<SpellButton> _spellButtonSpawn = new List<SpellButton>();
        
        [SerializeField] private GameObject PanelInventoryEnemy;
        [SerializeField] private GameObject PanelInventorySpell;
        [SerializeField] private GameObject prefabEnemyButton;
        [SerializeField] private GameObject prefabSpellButton;

        private EventSystem _eventSystem;
        
        public Upgrade UpgradeTest;

        private void Awake()
        {
            EventBus.OnNextLevel += UpdateInventoryData;
            EventBus.OnPlayerClicked += DropSpell;
            EventBus.OnGamePaused += OnPause;
            EventBus.OnGameResume += OnResume;
        }

        private bool _inPause;
        private void OnEnable()
        {
            //EventBus.OnNextLevel += UpdateInventoryData;
            //EventBus.OnPlayerClicked += DropSpell;
            //EventBus.OnGamePaused += OnPause;
            //EventBus.OnGameResume += OnResume;
            //EventBus.OnLevelStart += SelectCard;
        }
        

        private void OnDestroy()
        {
            EventBus.OnNextLevel -= UpdateInventoryData;
            EventBus.OnPlayerClicked -= DropSpell;
        }
        private void OnDisable()
        {
            EventBus.OnGamePaused -= OnPause;
            EventBus.OnGameResume -= OnResume;
        }

        private void Start()
        {
            _eventSystem = GameObject.FindWithTag("EventSystem").GetComponent<EventSystem>();
            _timeBeforeGetElixir = MaxTimeBeforeGetElixir;
            _timeBeforeGetMoney = MaxTimeBeforeGetMoney;
            UpdateInventoryData();
            foreach (EnemyClass c in EnemyClass)
            {
                c.SetUpData();
            }
        
            foreach (EnemyClass c in EnemyClass)
            {
                SetVisualEnemy(c);
            }
        
            foreach (SpellClass c in SpellClasses)
            {
                c.SetData();
            }
        
            foreach (SpellClass c in SpellClasses)
            {
                SetVisuelSpell(c);
            }
            
            _eventSystem.SetSelectedGameObject(EnemyButtonSpawns[0].gameObject);
        }

        private void Update()
        {
            if (_inPause) return;
            _timeBeforeGetElixir -= Time.deltaTime;
            _timeBeforeGetMoney -= Time.deltaTime;

            if (_timeBeforeGetElixir <= 0 && Elixir < MaxElixir)
            {
                _timeBeforeGetElixir = MaxTimeBeforeGetElixir;
                Elixir += ElixirParHit;
                Elixir = math.clamp(Elixir, 0, MaxElixir);
                SpawnManager.Instance.SpawnTextInWorldPosition("+" + ElixirParHit, new Color32(233,90,255,255), _elixirSpawnTextPosition);
            }
            
            if (_timeBeforeGetMoney <= 0 && Money < MaxMoney)
            {
                _timeBeforeGetMoney = MaxTimeBeforeGetMoney;
                Money += MoneyParHit;
                Money = math.clamp(Money, 0, MaxMoney);
                SpawnManager.Instance.SpawnTextInWorldPosition("+" + MoneyParHit, new Color32(255,185,42,255), _moneySpawnTextPosition);
            }
        }

        // Enemy
        public void AddEnemy(EnemyClass classToAdd)
        {
            EnemyClass.Add(classToAdd);
            classToAdd.SetUpData();
            SetVisualEnemy(classToAdd);
        }
        public void SetVisualEnemy(EnemyClass enemyClass)
        {
            GameObject instanciate = Instantiate(prefabEnemyButton, transform.position, quaternion.identity, PanelInventoryEnemy.transform);
            instanciate.GetComponent<EnemyButtonSpawn>().EnemyClass = enemyClass;
            EnemyButtonSpawns.Add(instanciate.GetComponent<EnemyButtonSpawn>());
        }
    
        // Sort
        public void EquipeSpell(SpellClass spellClass)
        {
            EquipedSpell = spellClass;
        }
        public void DropSpell()
        {
            if (EquipedSpell.SpellData == null) return;
            float testPrice = Elixir - EquipedSpell.Price;
            if (testPrice < 0 || ClickManager.Instance.LastPosition.x > 15.3f || ClickManager.Instance.LastPosition.y > 1) return;
            
            GameObject spell = Instantiate(EquipedSpell.SpellData.Prefab, ClickManager.Instance.LastPosition, Quaternion.identity);
            spell.GetComponent<Spell>().SpellClass = EquipedSpell;
            Elixir -= EquipedSpell.Price;
            SpawnManager.Instance.SpawnTextInWorldPosition("-" + EquipedSpell.Price, new Color32(233,90,255,255), new Vector3(spell.transform.position.x, spell.transform.position.y + 1.5f, spell.transform.position.z));
            EventBus.OnPlayerPlaceSpell?.Invoke();
        }
       
        public void SetVisuelSpell(SpellClass spellClass)
        {
            if (spellClass == null) return;
            GameObject instanciate = Instantiate(prefabSpellButton, transform.position, quaternion.identity, PanelInventorySpell.transform);
            instanciate.GetComponent<SpellButton>().SpellClass = spellClass;
            _spellButtonSpawn.Add(instanciate.GetComponent<SpellButton>());
        }
        public void AddSpell(SpellClass classToAdd)
        {
            SpellClasses.Add(classToAdd);
            classToAdd.SetData();
            SetVisuelSpell(classToAdd);
        }


        public void UpdateAllPrice()
        {
            
        }
    
        [ContextMenu("Update")]
        public void UpdateInventoryData()
        {
            Money = MaxMoney;
            Elixir = MaxElixir;
            EventBus.OnInventoryAreUpdated?.Invoke();
        }
        
        
        [ContextMenu("Upgrade")]
        public void Upgrade()
        {
            UpgradeTest.Apply(this);
            EventBus.OnInventoryAreUpdated?.Invoke();
        }
    
        private void OnPause()
        {
            _inPause = true;
        }

        private void OnResume()
        {
            _inPause = false;
        }
    }
}