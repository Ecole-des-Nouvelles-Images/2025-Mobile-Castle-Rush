using System;
using Buttons;
using Class;
using Menu;
using Player;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using Utils;

public class CursorController : MonoBehaviour
{

    public GameObject SlectedCard;
    public bool HaveSpeel;
    public GameObject Cursor;
    
    public Vector2 Direction;
    public float Speed;

    [SerializeField] private GameObject _selectedCard;
    //[SerializeField] SpellClass SpellClass;
    [SerializeField] private EventSystem _eventSystem;
    [SerializeField] private PauseButton _pauseButton;

    [SerializeField] private Vector4 LimitZones;
    
    private Rigidbody _rigidbody;
    private PlayerInput _playerInput;
    private ControllerInputHandler _inputHandler;
    private InputSystemUIInputModule _uiInputModule;
    [NonSerialized] public bool GameInPause;
    private GameObject _lastCardSelected;
    
    /// <summary>
    /// Récupère les ref
    /// </summary>
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _playerInput = GetComponent<PlayerInput>();
        _inputHandler = GetComponent<ControllerInputHandler>();
        
        EventBus.OnPlayerSelectSpell += ActiveCursor;
        EventBus.OnGameResume += ResumeGame;
        EventBus.OnLevelFinished += Cancel;
        EventBus.OnInventoryAreUpdated += SelectCard;
    }

    private void OnDisable()
    {
        EventBus.OnPlayerSelectSpell -= ActiveCursor;
        EventBus.OnGameResume -= ResumeGame;
        EventBus.OnLevelFinished -= Cancel;
        EventBus.OnInventoryAreUpdated -= SelectCard;
    }

    /// <summary>
    /// Récupère la ref du InputSystemUiModule 
    /// </summary>
    private void Start()
    {
        _uiInputModule = _eventSystem.gameObject.GetComponent<InputSystemUIInputModule>();
        
        _playerInput.enabled = true;
        _inputHandler.enabled = true;
    }

    /// <summary>
    /// Activer le curseur et les input du joueur
    /// Desactiver l'inpute de l'UI afin d'éviter les probleme
    /// </summary>
    private void ActiveCursor()
    {
        HaveSpeel = true;
        Cursor.SetActive(true);
        _selectedCard = _eventSystem.currentSelectedGameObject;
        _playerInput.enabled = true;
        _inputHandler.enabled = true;
        _uiInputModule.enabled = false;
        
        // Set size of spell
        float size = InventoryHandler.Instance.EquipedSpell.AreaSize;
        Cursor.transform.localScale = new Vector3(size, size, size);
    }

    /// <summary>
    /// Re activer les input de l'ui et désactiver ceux du joueur
    /// Instantier le sort depuis le manager 
    /// </summary>
    public void ClickWithSpell()
    {
        if (!HaveSpeel || GameInPause) return;
        HaveSpeel = false;
        //_playerInput.enabled = false;
        //_inputHandler.enabled = false;
        _uiInputModule.enabled = true;
        Cursor.SetActive(false);
        
        ClickManager.Instance.LastPosition = transform.position;
        InventoryHandler.Instance.DropSpell();
        SlectedCard.GetComponent<SpellButton>().UnselectedVisuel();
        Debug.Log("Player place spell");
    }

    /// <summary>
    /// Desactiver les input du joueur et re activer ceux de l'UI
    /// </summary>
    public void Cancel()
    {
        Direction = new Vector2(0, 0);
        _rigidbody.linearVelocity = Vector3.zero;
        HaveSpeel = false;
       // _playerInput.enabled = false;
        //_inputHandler.enabled = false;
        _uiInputModule.enabled = true;
        Cursor.SetActive(false);
        
        if (SlectedCard.GetComponent<SpellButton>()) SlectedCard.GetComponent<SpellButton>().UnselectedVisuel();
    }

    private void Update()
    {
        Moving();
        LimiteZone();
        SlectedCard = _eventSystem.currentSelectedGameObject;
    }

    private void Moving()
    {
        if (!HaveSpeel || GameInPause) return;
        Vector3 moveDirection = new Vector3(- Direction.y, 0, Direction.x);
        _rigidbody.linearVelocity = moveDirection * Speed;
    }

    public void PauseGame()
    {
        _lastCardSelected =  _eventSystem.currentSelectedGameObject;
        _pauseButton.OnPause();
        GameInPause = true;
        Cancel();
    }

    public void ResumeGame()
    {
        GameInPause = false;
        _eventSystem.SetSelectedGameObject(_lastCardSelected);
        Debug.Log("On game Resume" + _lastCardSelected.name);
    }

    public void SelectCard()
    {
        if (InventoryHandler.Instance.EnemyButtonSpawns.Count == 0) return;
        
        _eventSystem.SetSelectedGameObject(InventoryHandler.Instance.EnemyButtonSpawns[0].gameObject);
        Debug.Log("Equipe card");
    }

    private void LimiteZone()
    {
        Vector3 position = transform.position;

        position.x = Mathf.Clamp(position.x, LimitZones.x, LimitZones.y);
        position.z = Mathf.Clamp(position.z, LimitZones.z, LimitZones.w);

        transform.position = position;
    }
}
