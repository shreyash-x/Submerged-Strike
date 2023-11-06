using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "Shield", menuName = "_/Shield", order = 0)]
    public class Shield : ScriptableObject
    {
        public Player.Shield shieldPrefab;
        public Sprite shieldSprite;
        public Material shieldMaterial;
        
        public new string name;
        [TextArea] public string description;
        public int cost;
    }
}