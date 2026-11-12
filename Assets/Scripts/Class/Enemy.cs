using DG.Tweening;
using Interface;
using Managers;
using ScriptableObjectsScripts;
using Structs;
using UnityEngine;
using UnityEngine.Splines;
using Utils;
using Image = UnityEngine.UI.Image;
using Random = UnityEngine.Random;

namespace Class
{
    public abstract class Enemy : MonoBehaviour, IDamagable, IHealable
    {
        private GameObject _parentEmpty;
    
        public EnemyBaseData enemyBaseData;

        public EnemyClass EnemyClass;
    
        [SerializeField] private float _speed;
        private float _maxHealth;
        private float _health;
        
        [SerializeField] private Material _materials;
        [SerializeField] private Image _healthBar;
        [SerializeField] private SplineAnimate _splineAnimate;
        [SerializeField] private SplineContainer _splineContainer;

        
        public float Health
        {
            get => _health;
            set
            {
                _healthBar.fillAmount = value / _maxHealth;
                _health = value;
            
                if (_health <= 0)
                {
                    //SoundManager.Instance.PlayRandomSound(enemyBaseData.DeadSounds, gameObject);
                    SpawnManager.Instance.SpawnVfxInPosition(enemyBaseData.VFXPrefab, transform.position);
                    SoundManager.Instance.PlayRandomSound(enemyBaseData.DeadSounds, gameObject, true);
                    EventBus.OnEnemyDie?.Invoke(this);
                    Destroy(gameObject);
                }
            }
        }

        private void Awake()
        {
            _materials = gameObject.GetComponentInChildren<Renderer>().material;
        }

        public virtual void OnEnable()
        {
            EventBus.OnGamePaused += OnPause;
            EventBus.OnGameResume += OnResume;
            EventBus.OnAllEnemieGetHealth += GetHealth;
        }

        public virtual void OnDisable()
        {
            EventBus.OnGamePaused -= OnPause;
            EventBus.OnGameResume -= OnResume;
            EventBus.OnAllEnemieGetHealth -= GetHealth;
        }


        public virtual void Start()
        {
            SpawnInEmpty();
        
            _splineAnimate.Container = GameObject.FindGameObjectWithTag("Spline").GetComponent<SplineContainer>();
            _splineContainer = _splineAnimate.splineContainer;
            _splineAnimate.Play();
            SetUp();
            RandOffset();
        }

        private void Update()
        {
            float t = _splineAnimate.NormalizedTime;
            
            Vector3 tangent = _splineContainer.EvaluateTangent(t);
            transform.rotation = Quaternion.LookRotation(tangent);
        }

        public void SetUp()
        {
            _speed = EnemyClass.Speed / 10;
            _maxHealth = EnemyClass.MaxHealth;
            _health = _maxHealth;
        
            _splineAnimate.AnimationMethod = SplineAnimate.Method.Speed;
            _splineAnimate.MaxSpeed = _speed;
            _splineAnimate.Alignment = SplineAnimate.AlignmentMode.None;
            _splineAnimate.StartOffset = Random.Range(0, 0.01f);

        }

        public void SpawnInEmpty()
        {
            GameObject parent = GameObject.Find("EnemieParent");
            GameObject emptyParent = new GameObject("Enemy_" + gameObject.name + "_Parent");
            emptyParent.tag = "Enemy";
            emptyParent.transform.position = transform.position;
            emptyParent.transform.rotation = transform.rotation;
            emptyParent.transform.SetParent(parent.transform, worldPositionStays: true);
            transform.SetParent(emptyParent.transform, worldPositionStays: true);

            _splineAnimate = emptyParent.AddComponent<SplineAnimate>();
        }

        public void RandOffset()
        {
            float randX = Random.Range(enemyBaseData.OffsetX.x, enemyBaseData.OffsetX.y);
            float randZ = Random.Range(enemyBaseData.OffsetZ.x, enemyBaseData.OffsetZ.y);
            transform.position = new Vector3(transform.position.x + randX, transform.position.y + enemyBaseData.OffsetUp, transform.position.z + randZ);
        }

        public void TakeDamage(float damage)
        {
            Health -= damage;
            // Change material
            float targetValue = 0.5f;
            
            DOTween.To(
                () => 0f,
                value =>
                {
                    _materials.SetFloat("_HitStrength", value);
                },
                targetValue,
                0.1f
            ).SetLoops(2, LoopType.Yoyo);
            
            SpawnManager.Instance.SpawnTextInWorldPosition(damage.ToString(), new Color32(255,22,0,255), new Vector3(transform.position.x,transform.position.y + 2,transform.position.z));
            EventBus.OnEnemyTookDamage?.Invoke(damage, this);
        }

        public void GetHealth(float health)
        {
            _health = Mathf.Clamp(_health + health, 0, _maxHealth);
            _healthBar.fillAmount = _health / _maxHealth;
            float popRand = Random.Range(0, 100);
            float rand = Random.Range(-0.5f, 0.5f);
            if (popRand < 20) SpawnManager.Instance.SpawnTextInWorldPosition("+", new Color32(115,255,0,255), new Vector3(transform.position.x, transform.position.y + 2, transform.position.z + rand));
            
            float targetValue = 0.5f;
            
            DOTween.To(
                () => 0f,
                value =>
                {
                    _materials.SetFloat("_HealGlow", value);
                },
                targetValue,
                1f
            ).SetLoops(2, LoopType.Yoyo);
        }

        private void OnPause()
        {
            _splineAnimate.Pause();
            print("Speed 0");
        }

        private void OnResume()
        {
            _splineAnimate.Play();
            print("Speed = le speed de la class");
        }
    }
}
