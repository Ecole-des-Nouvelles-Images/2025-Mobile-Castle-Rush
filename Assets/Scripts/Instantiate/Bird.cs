using System.Collections.Generic;
using Managers;
using Player;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

public class Bird : MonoBehaviour, IClickable
{
    public int MinMoney;
    public int MaxMoney;

    public int MinElixir;
    public int MaxElixir;
    
    public int Speed;
    public bool LeftSide;

    public GameObject PrefabVFX;
    private bool _isPause;
    private float _timeBeforDie = 7f;

    [Header("SFX")] 
    public List<AudioClip> ScreemSFX;
    private void OnEnable()
    {
        EventBus.OnGamePaused += OnPause;
        EventBus.OnGameResume += OnResume;
    }

    private void OnDisable()
    {
        EventBus.OnGamePaused -= OnPause;
        EventBus.OnGameResume -= OnResume;
    }

    public void SetUp(bool leftSide)
    {
        LeftSide = leftSide;
        RotateBird();
    }
    
    private void RotateBird()
    {
        if (!LeftSide)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    public void OnClick()
    {
        int randMoney = Random.Range(MinMoney, MaxMoney + 1);
        int randElixir = Random.Range(MinElixir, MaxElixir + 1);
        InventoryHandler.Instance.Money += randMoney;
        InventoryHandler.Instance.Elixir += randElixir;
        SpawnManager.Instance.SpawnVfxInPosition(PrefabVFX, transform.position);
        SoundManager.Instance.PlayRandomSound(ScreemSFX, gameObject, true);
        SpawnManager.Instance.SpawnTextInWorldPosition("+" + randMoney, new Color32(255,185,42,255), new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z - 0.4f));
        SpawnManager.Instance.SpawnTextInWorldPosition("+" + randElixir, new Color32(233,90,255,255), new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z + 0.4f));
        Destroy(gameObject);
    }

    private void Update()
    {
        if(_isPause) return;
        transform.Translate(Vector3.forward * (Time.deltaTime * Speed));
        _timeBeforDie -= Time.deltaTime;
        if (_timeBeforDie <= 0) Destroy(gameObject);
    }
    
    
    private void OnPause()
    {
        _isPause = true;
    }

    private void OnResume()
    {
        _isPause = false;
    }
}
