using System.Collections.Generic;
using UnityEngine;

namespace Clouds
{
    public class CloudSpawner : MonoBehaviour
    {
        [SerializeField] private TileSettings tileSettings;
        [SerializeField] private int updateGap;

        private Bounds _bounds;
        private readonly List<Tile> _tiles = new List<Tile>();
        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;
            tileSettings.bounds = _bounds = Utility.GetScreenBounds(0, _camera);

            var indexes = GetTileIndexes(_bounds.center);
            foreach (var index in indexes)
            {
                var tile = new Tile(tileSettings)
                {
                    Index = index
                };
                tile.Generate(TileIndexToWorldPosition(index));
                _tiles.Add(tile);
            }
        }

        private int _frame = 0;
        private void Update()
        {
            _frame++;
            if (_frame != updateGap) return;
            _frame = 0;
            
            var bounds = Utility.GetScreenBounds(0, _camera);
            if (_bounds.Contains(bounds.center)) return;

            _bounds = bounds;
            UpdateTiles();
        }

        private void UpdateTiles()
        {
            var tiles = GetTileIndexes(Utility.GetScreenBounds(0, _camera).center);
            var indexes = new Queue<Vector2Int>();
            foreach (var index in tiles)
            {
                if(!_tiles.Exists(t => t.Index == index)) indexes.Enqueue(index);
            }
            
            var extents = _bounds.extents.xy();
            foreach (var tile in _tiles)
            {
                if (tiles.Contains(tile.Index)) continue;
                tile.Index = indexes.Dequeue();
                tile.Generate(TileIndexToWorldPosition(tile.Index) + extents); // no idea why i have to add extents but it works :(
            }
        }

        private Vector2Int WorldPositionToTileIndex(Vector2 position)
        {
            var x = Mathf.FloorToInt(position.x / _bounds.size.x);
            var y = Mathf.FloorToInt(position.y / _bounds.size.y);
            return new Vector2Int(x, y);
        }

        private Vector2 TileIndexToWorldPosition(Vector2Int tileIndex)
        {
            return new Vector2(tileIndex.x * _bounds.size.x, tileIndex.y * _bounds.size.y);
        }

        private List<Vector2Int> GetTileIndexes(Vector2 position)
        {
            var tile = WorldPositionToTileIndex(position);
            var res = new List<Vector2Int>();
            for (int x = -1; x <= 1; ++x)
            {
                for (int y = -1; y <= 1; ++y)
                {
                    res.Add(tile + new Vector2Int(x, y));
                }
            }
            return res;
        }

        // private void OnDrawGizmos()
        // {
        //     if (!Application.isPlaying) return;
        //     var bounds = Utility.GetScreenBounds(0);
        //     var tiles = GetTileIndexes(bounds.center);
        //     foreach (var tile in tiles)
        //     {
        //         var center = TileIndexToWorldPosition(tile) + bounds.extents.xy();
        //         Gizmos.DrawWireCube(center, bounds.size);
        //     }
        //     // Gizmos.DrawWireCube(_bounds.center, _bounds.size);
        // }
    }
}