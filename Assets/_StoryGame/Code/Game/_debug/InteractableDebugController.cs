using _StoryGame.Core.Interactables.Interfaces;
using TMPro;
using UnityEngine;

namespace _StoryGame.Game._debug
{
    public sealed class InteractableDebugController : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text interactableTypeText;

        public void SetNameText(string s) => nameText.text = s;

        public void SetInteractableType(EInteractableType interactableType) =>
            interactableTypeText.text = interactableType.ToString();
    }
}
