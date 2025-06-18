using System;
using _StoryGame.Game.Interactables.Data;
using UnityEngine;

namespace _StoryGame.Game.Interactables.Abstract
{
    /// <summary>
    /// Объект, который может быть использован, в т.ч. залутан(т.е. взят в рюкзак). Дверь, выключатель, предмет на полу
    /// </summary>
    public abstract class AUsable : AInteractable
    {
        [SerializeField] private EUsableAction usableAction = EUsableAction.NotSet;
        public override EInteractableType InteractableType => EInteractableType.Use;

        public EUsableAction UsableAction => usableAction;
        public EUseState UseState => EUseState.NotUsed;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (usableAction == EUsableAction.NotSet)
                throw new Exception("UsableAction not set. " + name);
        }
#endif
    }

    public enum EUsableAction
    {
        NotSet = -1,
        RoomExit,
        Switch,
        PickUp
    }

    public enum EUseState
    {
        NotUsed,
        Used
    }
}
