using System;
using Random = UnityEngine.Random;

namespace _StoryGame.Data.Interactable
{
    [Serializable]
    public struct InteractableSystemTipVo
    {
        public EInteractableSystemTip type;
        public int tipCount;
        public string localizationKeyBase;

        public string GetRandomLocalizationKey()
        {
            // localizationKeyBase_01 localizationKeyBase_02 etc
            var random = Random.Range(1, tipCount + 1);
            return $"{localizationKeyBase}_{random:D2}";
        }
    }
}
