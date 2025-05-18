using System;
using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Core.Interactables.Interfaces;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _StoryGame.Gameplay.Interactables
{
    public sealed class Interactable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string interactionTipNameId;
        [SerializeField] private string localizationKey;
        public bool CanInteract { get; }
        public string InteractionTipNameId => interactionTipNameId;
        public string LocalizationKey => localizationKey;

        public UniTask InteractAsync(ICharacter character)
        {
            throw new NotImplementedException();
        }
    }
}
