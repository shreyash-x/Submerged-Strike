using Game.Player;
using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "Plane", menuName = "_/Plane", order = 0)]
    public class Plane : ScriptableObject
    {
        public PlayerController planePrefab;
        public Sprite planeSprite;
        
        public new string name;
        [TextArea] public string description;
        public int cost;
    }
}