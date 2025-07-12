using System;
using _StoryGame.Core.Interact.Interactables;
using _StoryGame.Game.Interact.Interactables.Unlock;
using UnityEngine;
using VContainer;

namespace _StoryGame.Game.Interact.Interactables.Condition
{
    public sealed class Effectable : MonoBehaviour
    {
        [SerializeField] private bool playIfConditionsMet = true;
        [SerializeField] private ConditionData conditionsData;
        [SerializeField] private ParticleSystem[] particleSystems;

        [Inject] private ConditionChecker _conditionChecker;

        [Inject]
        private void Construct(ConditionChecker conditionChecker)
        {
            Debug.LogWarning("Construct Effectable");
            // _conditionChecker = conditionChecker;
        }

        private void Awake()
        {
            if (particleSystems == null || particleSystems.Length == 0)
                throw new NullReferenceException("No one particle system not found.");
        }

        private bool _isInitialized;

        private void Start()
        {
            _isInitialized = true; // Флаг, что зависимости проинжектированы
            UpdateParticleState(); // Вызываем логику сразу после инъекции
        }

        private void OnEnable()
        {
            if (_isInitialized)
            {
                UpdateParticleState();
            }
        }

        private void UpdateParticleState()
        {
            // Проверяем условия
            bool conditionsMet = _conditionChecker.CheckConditions(conditionsData).Success;

            // Если playIfConditionsMet = true, то проигрываем частицы, когда условия выполнены (conditionsMet = true)
            // Если playIfConditionsMet = false, то проигрываем частицы, когда условия НЕ выполнены (conditionsMet = false)
            bool shouldPlay = playIfConditionsMet ? conditionsMet : !conditionsMet;

            foreach (var particleSystem in particleSystems)
            {
                if (shouldPlay)
                {
                    particleSystem.Play();
                }
                else
                {
                    particleSystem.Stop();
                }
            }
        }
    }
}
