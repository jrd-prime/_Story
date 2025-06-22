using System;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Data.Interact;
using _StoryGame.Game.Interact.Systems.Use;
using UnityEngine;
using UnityEngine.Serialization;

namespace _StoryGame.Game.Interact.Abstract
{
    /// <summary>
    /// Объект, который может быть использован, в т.ч. залутан(т.е. взят в рюкзак). Дверь, выключатель, предмет на полу
    /// </summary>
    public abstract class AUsable : AInteractable<UseSystem>, IUsable
    {
        [FormerlySerializedAs("usableAction")] [SerializeField] private EUseAction useAction = EUseAction.NotSet;
        public override EInteractableType InteractableType => EInteractableType.Use;

        public EUseAction UseAction => useAction;
        public EUseState UseState => EUseState.NotUsed;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (useAction == EUseAction.NotSet)
                throw new Exception("UseAction not set. " + name);
        }
#endif
    }

    public enum EUseAction
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
