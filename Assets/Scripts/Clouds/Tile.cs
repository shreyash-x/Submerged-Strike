using System.Collections.Generic;
using UnityEngine;

namespace Plants
{
    [System.Serializable]
    public struct TileSettings
    {
        public int maxPlants;
        public float radius;
        public float scale;
        public SpriteRenderer[] plantPrefabs;
        [HideInInspector] public Bounds bounds;
    }
    
    public class Tile
    {
        private readonly List<SpriteRenderer> _plants;
        private readonly TileSettings _tileSettings;

        public Vector2Int Index;

        public Tile(TileSettings tileSettings)
        {
            _tileSettings = tileSettings;
            _plants = new List<SpriteRenderer>();
            for (int i = 0; i < _tileSettings.maxPlants; i++)
            {
                var plant = Object.Instantiate(tileSettings.plantPrefabs.SelectRandom());
                plant.gameObject.SetActive(false);
                _plants.Add(plant);
            }
        }
        
        public void Generate(Vector2 offset)
        {
            float width = _tileSettings.bounds.extents.x;
            float height = _tileSettings.bounds.extents.y;
            
            var points = new List<Vector2>();
            for (int i = 0; i < 30; ++i)
            {
                for (int j = 0; j < 30; ++j)
                {
                    // points.Add(new Vector2(Random.Range(_tileSettings.bounds.min.x, _tileSettings.bounds.max.x),
                    //     Random.Range(_tileSettings.bounds.min.y, _tileSettings.bounds.max.y)));

                    i -= 15;
                    j -= 15;
                    points.Add(new Vector2(i * width / 15f, j * height / 15f));
                    i += 15;
                    j += 15;
                }
            }

            // return points;

            var selectedPoints = new List<Vector2>();
            var radius = _tileSettings.radius;
            var selected = 0;
            while (selected < _tileSettings.maxPlants && points.Count > 0)
            {
                var point = points.SelectRandom();
                selectedPoints.Add(point);
                points.RemoveAll((e) => Vector2.Distance(point, e) < radius);
                selected++;
            }

            int plantIndex = 0;
            foreach (var point in selectedPoints)
            {
                var plant = _plants[plantIndex++];
                // set scale
                var scale = _tileSettings.scale;
                plant.transform.localScale = new Vector3(scale, scale, 1);
                plant.gameObject.SetActive(true);
                plant.transform.position = point + offset;
            }
        }
    }
}