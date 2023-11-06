using System.Collections.Generic;
using UnityEngine;

namespace Clouds
{
    [System.Serializable]
    public struct TileSettings
    {
        public int maxClouds;
        public float radius;
        public SpriteRenderer[] cloudPrefabs;
        public float minAlpha;
        public float maxAlpha;
        [HideInInspector] public Bounds bounds;
    }
    
    public class Tile
    {
        private readonly List<SpriteRenderer> _clouds;
        private readonly TileSettings _tileSettings;

        public Vector2Int Index;

        public Tile(TileSettings tileSettings)
        {
            _tileSettings = tileSettings;
            _clouds = new List<SpriteRenderer>();
            for (int i = 0; i < _tileSettings.maxClouds; i++)
            {
                var cloud = Object.Instantiate(tileSettings.cloudPrefabs.SelectRandom());
                cloud.gameObject.SetActive(false);
                _clouds.Add(cloud);
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
            while (selected < _tileSettings.maxClouds && points.Count > 0)
            {
                var point = points.SelectRandom();
                selectedPoints.Add(point);
                points.RemoveAll((e) => Vector2.Distance(point, e) < radius);
                selected++;
            }

            int cloudIndex = 0;
            foreach (var point in selectedPoints)
            {
                var cloud = _clouds[cloudIndex++];
                var color = cloud.color;
                color.a = Random.Range(_tileSettings.minAlpha, _tileSettings.maxAlpha);
                cloud.color = color;
                cloud.gameObject.SetActive(true);
                cloud.transform.position = point + offset;
            }
        }
    }
}