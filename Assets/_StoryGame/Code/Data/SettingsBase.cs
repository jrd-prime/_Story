using UnityEngine;

namespace _StoryGame.Data
{
    public abstract class SettingsBase : ScriptableObject
    {
        public abstract string ConfigName { get; }
    }
}
