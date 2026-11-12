using System.Linq;
using Class;
using Player;
using Structs;
using Unity.Mathematics;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Managers
{
    public class SpawnManager : MonoBehaviourSingleton<SpawnManager>
    {
        
        [SerializeField] private InventoryHandler _inventory;
        [SerializeField] private GameObject _birdPrefab;
        
        [Header("Prefab")]
        public GameObject VfxSpawner;
        public GameObject TextSpawner;

        [Header("Spawning rate")] 
        public Vector2 BirdRandSpawn;
        [SerializeField] private float _birdSpawningTime;
        
        private bool _isPause;
        
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

        private void Start()
        {
            _birdSpawningTime = Random.Range(BirdRandSpawn.x, BirdRandSpawn.y);
        }

        private void Update()
        {
            if (_isPause) return;
            _birdSpawningTime -= Time.deltaTime;
            if (_birdSpawningTime <= 0) SpawnBird();
        }

        [ContextMenu("Spawn")]
        public void Spawn(EnemyClass enemyclass)
        {
            float testMoney = InventoryHandler.Instance.Money - enemyclass.baseData.price;
            if (testMoney < 0) return;
        
            var newClass = _inventory.EnemyClass.FirstOrDefault(struc => struc.baseData == enemyclass.baseData);
        
            for (int i = 0; i < newClass.NumberSpawn; i++)
            {
                GameObject instantite = Instantiate(newClass.baseData.Prefab);
                instantite.GetComponent<Enemy>().EnemyClass = newClass;
            }

            if (enemyclass.baseData.SpawnSounds[0]) SoundManager.Instance.PlayRandomSound(enemyclass.baseData.SpawnSounds, gameObject);
            InventoryHandler.Instance.Money -= newClass.price;

            float randZ = Random.Range(-0.5f, 0.5f);
            SpawnTextInWorldPosition("-" + enemyclass.price, new Color32(255,185,42,255), new Vector3(PathManager.Instance.PositionCastleA.x, PathManager.Instance.PositionCastleA.y + 3f, PathManager.Instance.PositionCastleA.z + randZ));
            EventBus.OnplayerPlaceTroup?.Invoke();
        }
        
        [ContextMenu("SpawnBird")]
        public void SpawnBird()
        {
            _birdSpawningTime = Random.Range(BirdRandSpawn.x, BirdRandSpawn.y);
            bool leftSide = Random.value < 0.5f;
            float randXPos = Random.Range(6f,16f);
            float posZ;
            _ = leftSide ? posZ = -2f : posZ = 24f;
            
            GameObject bird = Instantiate(_birdPrefab, new Vector3(randXPos, 3.60f, posZ), Quaternion.identity);
            bird.GetComponent<Bird>().SetUp(leftSide);
        }

        
        
        public void SpawnVfxInPosition(GameObject particleSystem, Vector3 position)
        {
            GameObject vfx = Instantiate(VfxSpawner,  position, Quaternion.identity);
            vfx.GetComponent<VfxSpawner>().SetUp(particleSystem);
        }

        
        
        public enum TextSpawnMode
        {
            World,
            UI
        }
        
        public void SpawnTextInWorldPosition(string text, Color color,Vector3 position)
        {
            GameObject textGm = Instantiate(TextSpawner, position, quaternion.identity);
            textGm.GetComponent<TextPrefab>().SetUp(text, color);
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
}
