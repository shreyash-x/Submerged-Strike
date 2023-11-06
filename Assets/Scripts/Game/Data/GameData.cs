using System.Collections.Generic;
using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "Game Data", menuName = "_/Game Data", order = 0)]
    public class GameData : ScriptableObject
    {
        public List<Plane> planes;
        public List<Shield> shields;
    }
}