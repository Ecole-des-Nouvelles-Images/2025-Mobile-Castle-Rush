using System.Collections.Generic;
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

        [SerializeField] public List<GameObject> _prefabsProps = new List<GameObject>();
        public bool DoRandomRotation = false;

        // Uniquement les props créés par CE composant
        private readonly List<GameObject> _spawned = new List<GameObject>();
        private readonly List<Vector2Int> _spawnedCells = new List<Vector2Int>();

        private void OnEnable()
        {
            EventBus.OnIaPlaceTower += Spawn;
        }

        private void OnDisable()
        {
            EventBus.OnIaPlaceTower -= Spawn;
        }

        [ContextMenu("Spawn")]
        public void Spawn()
        {
            Cell[,] cells = PathManager.Instance.CellsMatrix;
            if (cells == null) return;

            // 1. Nettoie SES props de l'ancien niveau (pas ceux de l'autre composant)
            DestroyAllProps();

            if (_prefabsProps == null || _prefabsProps.Count == 0)
            {
                Debug.LogWarning($"{name} SpawnProps : aucun prefab.");
                return;
            }

            // 2. Cases libres
            List<Vector2Int> positions = new List<Vector2Int>();
            for (int i = 0; i < cells.GetLength(0); i++)
            {
                for (int j = 0; j < cells.GetLength(1); j++)
                {
                    if (!cells[i, j].IsAPath && !cells[i, j].IsTower && !cells[i, j].IsProps)
                        positions.Add(new Vector2Int(i, j));
                }
            }

            // 3. Spawn
            for (int i = 0; i < NumberPropos && positions.Count > 0; i++)
            {
                int randIndex = Random.Range(0, positions.Count);
                Vector2Int cell = positions[randIndex];
                positions.RemoveAt(randIndex);

                GameObject prefab = _prefabsProps[Random.Range(0, _prefabsProps.Count)];

                Quaternion rot = prefab.transform.rotation;
                if (DoRandomRotation)
                    rot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f) * rot;

                GameObject props = Instantiate(prefab, new Vector3(cell.x, 0, cell.y), rot, transform);

                cells[cell.x, cell.y].IsProps = true;
                _spawned.Add(props);
                _spawnedCells.Add(cell);
            }

            Debug.Log($"Spawn décorations : {_spawned.Count} props");
        }

        [ContextMenu("Destroy")]
        public void DestroyAllProps()
        {
            Cell[,] cells = PathManager.Instance != null ? PathManager.Instance.CellsMatrix : null;

            for (int i = 0; i < _spawned.Count; i++)
            {
                Vector2Int c = _spawnedCells[i];
                if (cells != null && c.x < cells.GetLength(0) && c.y < cells.GetLength(1))
                    cells[c.x, c.y].IsProps = false;

                if (_spawned[i] != null)
                    Destroy(_spawned[i]);
            }

            _spawned.Clear();
            _spawnedCells.Clear();
        }
    }
}