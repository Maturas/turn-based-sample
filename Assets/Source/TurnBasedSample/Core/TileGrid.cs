using TurnBasedSample.Gameplay;
using TurnBasedSample.Utils;
using UnityEngine;

namespace TurnBasedSample.Core
{
    public class TileGrid : MonoBehaviour
    {
        [SerializeField] private Tile tilePrefab;
        [SerializeField] private float tileSpacing = 1.5f;
        [SerializeField] private Vector3 tileRotation = new Vector3(90.0f, 0.0f, 0.0f);

        private ObjectPool<Tile> _tilePool;
        private Tile[,] _tiles;
        
        public Vector2Int Size { get; private set;}
        
        public void Init(int width, int height)
        {
            _tilePool = ObjectPool<Tile>.Create(tilePrefab, width * height, true);
            Size = new Vector2Int(width, height);
            GenerateGrid(width, height);
        }

        public Tile GetTile(Vector2Int position)
        {
            if (position.x < 0 || position.x >= Size.x || position.y < 0 || position.y >= Size.y)
                return null;
            
            return _tiles[position.x, position.y];
        }

        public void UnhighlightAllTiles()
        {
            foreach (var tile in _tiles)
            {
                tile.SwitchHighlight(false);
            }
        }

        private void GenerateGrid(int width, int height)
        {
             var gridCenter = transform.position;
             _tiles = new Tile[width, height];
             
             var totalWidth = width * tileSpacing;
             var totalHeight = height * tileSpacing;
             
             var startPos = gridCenter - new Vector3(totalWidth / 2 - tileSpacing / 2, 0, totalHeight / 2 - tileSpacing / 2);
             for (int i = 0; i < width; i++)
             {
                 for (int j = 0; j < height; j++)
                 {
                     var tile = _tilePool.Get();
                     
                     tile.transform.position = startPos + new Vector3(i * tileSpacing, 0, j * tileSpacing);
                     tile.transform.rotation = Quaternion.Euler(tileRotation);
                     tile.name = $"Tile [{i},{j}]";
                     
                     tile.Init(this, new Vector2Int(i, j));
                     _tiles[i, j] = tile;
                 }
             }
        }
    }
}