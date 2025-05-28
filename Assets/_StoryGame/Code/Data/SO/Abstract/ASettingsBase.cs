using UnityEngine;

namespace _StoryGame.Data.SO.Abstract
{
    public abstract class ASettingsBase : ScriptableObject
    {
        public string ConfigName => GetType().Name;
    }
}
