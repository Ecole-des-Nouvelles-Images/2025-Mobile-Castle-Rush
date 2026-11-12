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
        public bool DoRandomRotation;

        private void OnEnable()
        {
            EventBus.OnIaPlaceTower += Spawn;
            EventBus.OnPlayerTakedCard += DestroyAllProps;
        }

        private void OnDisable()
        {
            EventBus.OnIaPlaceTower -= Spawn;
            EventBus.OnPlayerTakedCard -= DestroyAllProps;
        }

        private void Start()
        {
            _prefabsProps = Enumerable.ToList(Resources.LoadAll<GameObject>(_ressourcePath));
        }

        [ContextMenu("Spawn")]
        public void Spawn()
        {
            Cell[,] cells = PathManager.Instance.CellsMatrix;
            List<Vector2Int> positions = new List<Vector2Int>();
            for (int i = 0; i < cells.GetLength(0); i++)
            {
                for (int j = 0; j < cells.GetLongLength(1); j++)
                {
                    if (cells[i, j].IsAPath == false && cells[i, j].IsTower == false && cells[i, j].IsProps == false)
                    {
                        positions.Add(new Vector2Int(i, j));
                    }
                }
            }
        
            for (int i = 0; i < NumberPropos; i++)
            {
                int randProps = Random.Range(0, _prefabsProps.Count);
                Vector2Int vector2Int = positions[Random.Range(0, positions.Count)];
                
                GameObject props = Instantiate(_prefabsProps[randProps], new Vector3(vector2Int.x,0,vector2Int.y), Quaternion.identity, transform);

                if (DoRandomRotation)
                {
                    int randRotationY = Random.Range(0, 360);
                    props.transform.Rotate(props.transform.rotation.x, randRotationY, props.transform.rotation.z);
                }
                PathManager.Instance.CellsMatrix[vector2Int.x, vector2Int.y].IsProps = true;
            }
        }

        [ContextMenu("Destroy")]
        public void DestroyAllProps()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).CompareTag("Props"))
                {
                    GameObject props = transform.GetChild(i).gameObject;
                    PathManager.Instance.CellsMatrix[(int)props.transform.position.x, (int)props.transform.position.y].IsProps = false;
                    Destroy(props);
                }
            }
        }
    }
}
