using System.Collections.Generic;

namespace Game.Data
{
    [System.Serializable]
    public struct SaveData
    {
        public int coins;
        
        public int planeIndex;
        public int shieldIndex;

        public List<int> planesBought;
        public List<int> shieldsBought;

        // last / current play data
        public int coinsCollected;
        public int timeCoins;

        public InputMode inputMode;
        public float effectsVolume;
        public bool playTutorial;

        public bool iconsEnabled;
        public bool radarEnabled;

        public int minesDestroyed;

        // Levels completed
        public List<int> levelsCompleted;

        public int difficulty;
    }
}