using UnityEngine;

namespace MatchingGame.Gameplay
{
    public class CardProperty
    {
        public string key;
        public Sprite sprite;

        public CardProperty(string key, Sprite sprite)
        {
            this.key = key;
            this.sprite = sprite;
        }
    }
}