using UnityEngine;

namespace _StoryGame.Data
{
    public abstract class ASettingsBase : ScriptableObject
    {
        public string ConfigName => GetType().Name;
    }
}
