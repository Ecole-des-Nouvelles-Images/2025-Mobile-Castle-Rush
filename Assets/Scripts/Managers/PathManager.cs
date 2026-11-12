using System.Collections.Generic;
using Instantiate;
using Structs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.Tilemaps;
using Utils;

namespace Managers
{
    public class PathManager : MonoBehaviourSingleton<PathManager>
    {
        public int Height;
        public int Width; 
    
        public Cell[,] CellsMatrix;
    
        [SerializeField] private Tilemap _tilemap;
    
        [SerializeField] private GameObject _wayGround;
        [SerializeField] private GameObject _ground;
    
        [SerializeField] private GameObject _castlePrefab;
        public Vector3 PositionCastleA;
        [SerializeField] private GameObject _castleEnemiePrefab;
    
        [SerializeField] private List<GameObject> _cellGameObjects = new List<GameObject>();

        [Header("Visuel tile")]
        [SerializeField] private RuleTile _tileGrass;    
        [SerializeField] private RuleTile _tileRoad;
        
        Vector2Int[] offsets = new Vector2Int[]
        {
            new Vector2Int( 1,  0),
            new Vector2Int(-1,  0),
            new Vector2Int( 0,  1),
            new Vector2Int( 0, -1),
            new Vector2Int( 1,  1),
            new Vector2Int(-1, -1),
            new Vector2Int( 1, -1),
            new Vector2Int(-1,  1),
            new Vector2Int(-2, 0),
            new Vector2Int(2, 0),
        };
        public void SetDataPath(List<Vector3Int> vector3Ints)
        {
            CellsMatrix =  new Cell[Width, Height];
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    CellsMatrix[i, j] = new Cell();
                }
            }
        
            foreach (Vector3Int vector3Int in vector3Ints)
            {
                int x = Mathf.Clamp(vector3Int.x, 0, Width - 1);
                int z = Mathf.Clamp(vector3Int.z, 0, Height - 1);
                CellsMatrix[vector3Int.x, vector3Int.z].IsAPath = true;
            }
            SetVisual();
        }

        public void SetVisual()
        {
            ResetVisual();
            int height = CellsMatrix.GetLength(0); 
            int width = CellsMatrix.GetLength(1); 

            for (int x = 0; x < height; x++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (!CellsMatrix[x, j].IsAPath)
                    {
                        //GameObject instance = Instantiate(_ground, new Vector3(x, 0, j), Quaternion.identity,transform);
                        //_cellGameObjects.Add(instance);
                        Vector3Int vector3Int = new Vector3Int(x, j, 0);
                        _tilemap.SetTile(vector3Int, _tileGrass);
                    }
                    else
                    {
                        // GameObject instance = Instantiate(_wayGround, new Vector3(x, 0, j), Quaternion.identity, transform);
                        // _cellGameObjects.Add(instance);
                        Vector3Int vector3Int = new Vector3Int(x, j, 0);
                        _tilemap.SetTile(vector3Int, _tileRoad);
                    }
                }
            }
        
            PlaceCastle();
        }

        public void PlaceCastle()
        {
            GameObject castleA = Instantiate(_castlePrefab, transform.position, quaternion.identity, transform);
            GameObject castleB = Instantiate(_castleEnemiePrefab, transform.position, quaternion.identity, transform);
            _cellGameObjects.Add(castleA);
            _cellGameObjects.Add(castleB);

            Spline spline = GameObject.FindGameObjectWithTag("Spline").GetComponent<SplineContainer>().Splines[0];

            BezierKnot firstKnot = spline[0];
            BezierKnot lastKnot = spline[spline.Count - 1];
        
            castleA.transform.position = new Vector3(firstKnot.Position.x, firstKnot.Position.y, firstKnot.Position.z);
            PositionCastleA = castleA.transform.position;
        
            castleB.transform.position = new Vector3(lastKnot.Position.x, lastKnot.Position.y, lastKnot.Position.z);
            castleB.transform.rotation = new quaternion(0,180,0,1);

            Vector3 posA = castleA.transform.position;
            Vector3 posB = castleB.transform.position;
            
            SetAroundCastle(posA);
            SetAroundCastle(posB);
            
            EventBus.OnTerrainGenerate?.Invoke();
        }
        
        void SetAroundCastle(Vector3 pos)
        {
            int width = CellsMatrix.GetLength(0);
            int height = CellsMatrix.GetLength(1);

            for (int i = 0; i < offsets.Length; i++)
            {
                Vector2Int newPos = new Vector2Int((int)pos.x + offsets[i].x, (int)pos.z + offsets[i].y);
                if (newPos.x >= 0 && newPos.x < width && newPos.y >= 0 && newPos.y < height)
                {
                    CellsMatrix[newPos.x, newPos.y].IsBlocked = true;
                }
            }
        }
        
        public void ResetVisual()
        {
            foreach (GameObject gm in _cellGameObjects)
            {
                if (!Application.isPlaying) DestroyImmediate(gm);
                else
                {
                    Destroy(gm);
                }
            }
            _cellGameObjects.Clear();
        }
    }
}