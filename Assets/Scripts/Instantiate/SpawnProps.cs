using System.Collections.Generic;
using System.Linq;
using Managers;
using Structs;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Instantiate
{
    public class SpawnProps : MonoBehaviour
    {
        public int NumberPropos;
    
        [SerializeField] private string _ressourcePath;
        [SerializeField] public List<GameObject> _prefabsProps = new List<GameObject>();
        public bool DoRandomRotation = false;

        private void OnEnable()
        {
            EventBus.OnIaPlaceTower += Spawn;
            EventBus.OnNextLevel += DestroyAllProps;
        }

        private void OnDisable()
        {
            EventBus.OnIaPlaceTower -= Spawn;
            EventBus.OnNextLevel -= DestroyAllProps;
        }

        private void Start()
        {
            //_prefabsProps = Enumerable.ToList(Resources.LoadAll<GameObject>(_ressourcePath));
        }

        [ContextMenu("Spawn")]
        public void Spawn()
        {
            Debug.Log("Spawn décorations");
            Cell[,] cells = PathManager.Instance.CellsMatrix;
            if (cells == null) return;

            List<Vector2Int> positions = new List<Vector2Int>();
            for (int i = 0; i < cells.GetLength(0); i++)
            {
                for (int j = 0; j < cells.GetLength(1); j++)
                {
                    if (cells[i, j].IsAPath == false && cells[i, j].IsTower == false && cells[i, j].IsProps == false)
                    {
                        positions.Add(new Vector2Int(i, j));
                    }
                }
            }
            if (positions.Count == 0)
            {
                Debug.LogWarning("SpawnProps : Aucune position libre sur la grille.");
                return;
            }
            if (_prefabsProps == null || _prefabsProps.Count == 0)
            {
                Debug.LogWarning("SpawnProps : Aucun prefab trouvé dans_prefabsProps.");
                return;
            }

            for (int i = 0; i < NumberPropos; i++)
            {
                if (positions.Count == 0) break;

                int randProps = Random.Range(0, _prefabsProps.Count);
                int randIndex = Random.Range(0, positions.Count);
                Vector2Int vector2Int = positions[randIndex];
                positions.RemoveAt(randIndex);
                
                // Définition de la rotation avant l'instanciation
                Quaternion finalRotation = Quaternion.identity;
                if (DoRandomRotation)
                {
                    Debug.Log("DO RANDOM ROTATION");
                    int randRotationY = Random.Range(0, 360);
                    finalRotation = Quaternion.Euler(0, randRotationY, 0);
                }
                
                // Instanciation directe avec la bonne rotation
                GameObject props = Instantiate(_prefabsProps[randProps], new Vector3(vector2Int.x, 0, vector2Int.y), finalRotation, transform);
        
                PathManager.Instance.CellsMatrix[vector2Int.x, vector2Int.y].IsProps = true;
            }
        }

        [ContextMenu("Destroy")]
        public void DestroyAllProps()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                GameObject props = transform.GetChild(i).gameObject;
                if (props.CompareTag("Props"))
                {
                    Vector3 pos = props.transform.position;
                    PathManager.Instance.CellsMatrix[Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.z)].IsProps = false;
                    Destroy(props);
                }
            }
        }
    }
}