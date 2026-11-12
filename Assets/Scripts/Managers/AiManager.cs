using System.Collections.Generic;
using System.Linq;
using Class;
using Structs;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Managers
{
    public class AiManager : MonoBehaviourSingleton<AiManager>
    {
        [Header("IA choice")]
        public Vector2Int ChoiceAi;
        public Vector4 LimiteZoneTower;
        
        [Header(" Money ")]
        public int MoneyToAddParLevel;
    
        public int MaxMoney;
        public int Money;

        public List<DefenseBaseData> DefenseBaseDatas = new List<DefenseBaseData>();
        [SerializeField] private List<DefenseBaseData> _defenseCanUse = new List<DefenseBaseData>();
        
        [SerializeField] private List<DefenseBaseData> _defenseBaseDatasToSpawn = new List<DefenseBaseData>();
        private int[,] _matrixInt;
        private List<HeuristicResult> _heuristicResults = new List<HeuristicResult>();
        
        private void Awake()
        {
            Money = MaxMoney;
            // foreach (GameObject tower in Towers)
            // {
            //     DefenseBaseDatas.Add(tower.GetComponent<TowerBase>().BaseData);
            // }
            EventBus.OnTerrainGenerate += OnNextLevel;
        }
    
        private void OnDisable()
        {
            EventBus.OnTerrainGenerate -= OnNextLevel;
        }
        
        [ContextMenu("PlaceAITowers")]
        public void PlaceIaTower()
        {
            
            _matrixInt = AiUtils.ConvertMatrixCellToInt(PathManager.Instance.CellsMatrix);
            _heuristicResults = AiUtils.SetHeuristicResult(_matrixInt, _defenseCanUse);
            
            _heuristicResults = _heuristicResults.OrderByDescending(heuristic => heuristic.HeuristicValue).ToList();
            for (int i = 0; i < _heuristicResults.Count; i++)
            {
                if (_heuristicResults[i].HeuristicValue > 0)
                {
                    //Debug.Log(_heuristicResults[i].DefenseBaseData.name + " HEURISTIC " + _heuristicResults[i].HeuristicValue);
                }
                else
                {
                    _heuristicResults.RemoveAt(i);
                }
            }
            SetRandomDefenseToSpawn();
            BuyTower();
            Debug.Log(_matrixInt);
        }

        public void SetRandomDefenseToSpawn()
        {
            int moneyTest = Money;
            while (moneyTest > 0)
            {
                DefenseBaseData defenseBaseData = _defenseCanUse[Random.Range(0, _defenseCanUse.Count)];
                _defenseBaseDatasToSpawn.Add(defenseBaseData);
                moneyTest -= defenseBaseData.Price;
            }
        }

        public void BuyTower()
        {
            foreach (DefenseBaseData defense in _defenseBaseDatasToSpawn)
            {
                // je créer une list et je récup que les élément qui sont égal = defense 
                List<HeuristicResult> heuristicResults = _heuristicResults.Where(result => result.DefenseBaseData == defense).ToList();
                
                // je classe dans l'ordre décroissant par rapport à l'heuristiqueValue
                heuristicResults = heuristicResults.OrderByDescending(result => result.HeuristicValue).ToList();
                
                
                if (heuristicResults.Count == 0) continue;
                
                // Faire un random de choix
                int inteligeanceChoice = Random.Range(ChoiceAi.x, ChoiceAi.y);
                HeuristicResult finalResult = heuristicResults[inteligeanceChoice];
                
                
                // ensuite je l'enleve de la list
                _heuristicResults.Remove(finalResult);

                PathManager.Instance.CellsMatrix[Mathf.FloorToInt(finalResult.position.x), Mathf.FloorToInt(finalResult.position.y)].IsTower = true;
                GameObject turel = Instantiate(finalResult.DefenseBaseData.Prefab, new Vector3(finalResult.position.x, 0, finalResult.position.y), Quaternion.identity, transform);
                if (finalResult.position.x < LimiteZoneTower.x || finalResult.position.x > LimiteZoneTower.y ||  finalResult.position.y < LimiteZoneTower.z || finalResult.position.y > LimiteZoneTower.w)
                {
                    Destroy(turel);
                    continue;
                }
                Money -= finalResult.DefenseBaseData.Price;
                
                
            }
            Debug.Log("AVANT IA placed tower");
            EventBus.OnIaPlaceTower?.Invoke();
            Debug.Log("APRES IA placed tower");
        }
        
        
        private bool CheckIfHeCanBuy(DefenseBaseData data, int tempMoney)
        {
            if (tempMoney - data.Price < 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void AddTower()
        {
            foreach (DefenseBaseData data in DefenseBaseDatas)
            {
                if (data.WaveHeCanSpawn == DifficultyManager.Instance.CurrentLevel)
                {
                    if (!_defenseCanUse.Contains(data))
                    {
                        _defenseCanUse.Add(data);
                    }
                }
            }
        } 
        
        [ContextMenu("RemoveAllTowers")]
        public void RemoveAllTowers()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Vector3 pos = transform.GetChild(i).transform.position;
                PathManager.Instance.CellsMatrix[Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.z)].IsTower = false;
                Destroy(transform.GetChild(i).gameObject);
            }
            _defenseBaseDatasToSpawn.Clear();
            _heuristicResults.Clear();
        }

        public void OnNextLevel()
        {
            RemoveAllTowers();
            MaxMoney += MoneyToAddParLevel;
            Money = MaxMoney;
            AddTower();
            PlaceIaTower();
        }
    }
}
