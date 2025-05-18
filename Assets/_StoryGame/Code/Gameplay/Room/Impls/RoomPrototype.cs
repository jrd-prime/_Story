using System.Collections.Generic;
using UnityEngine;

namespace _StoryGame.Gameplay.Room.Impls
{
    public sealed class RoomPrototype : MonoBehaviour, IRoom
    {
        [SerializeField] private string roomName;
        [SerializeField] private float progress;
        [SerializeField] private List<Loot> lootPool;
        [SerializeField] private List<Transition> transitions;

        public string Name => roomName;
        public float Progress => progress;
        public List<Loot> LootPool => lootPool;
        public List<Transition> Transitions => transitions;

        public void Inspect() => Debug.Log($"Inspecting {name}");

        public void DeepSearch() => Debug.Log($"Deep searching {name}");

        public void UnlockObject() => Debug.Log($"Unlocking {name}");

        public bool CanTransition(Transition transition) => true;
    }
}
