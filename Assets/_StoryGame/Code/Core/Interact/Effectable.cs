using _StoryGame.Core.Common.Interfaces;
using _StoryGame.Game.Managers.Condition;
using UnityEngine;
using UnityEngine.VFX;
using VContainer;

namespace _StoryGame.Core.Interact
{
    /// <summary>
    /// ADD to room
    /// </summary>
    public sealed class Effectable : MonoBehaviour
    {
        [SerializeField] private bool playIfConditionsMet = true;
        [SerializeField] private ConditionData conditionsData;
        [SerializeField] private ParticleSystem[] particleSystems;
        [SerializeField] private VisualEffect[] visualEffects;
        [Inject] private ConditionChecker _conditionChecker;

        [Inject]
        private void Construct(ConditionChecker conditionChecker, IJLog log)
        {
            _log = log;
            Debug.LogWarning("Construct Effectable");
            // _conditionChecker = conditionChecker;
        }

        private void Awake()
        {
            // if (particleSystems == null || particleSystems.Length == 0)
            //     throw new NullReferenceException("No one particle system not found.");
            if (visualEffects == null || visualEffects.Length == 0)
                _log.Error("No VisualEffect components found.");
        }

        private bool _isInitialized;
        private IJLog _log;

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

            _log.Warn("conditionsMet: " + conditionsMet);


            // Если playIfConditionsMet = true, то проигрываем частицы, когда условия выполнены (conditionsMet = true)
            // Если playIfConditionsMet = false, то проигрываем частицы, когда условия НЕ выполнены (conditionsMet = false)
            bool shouldPlay = playIfConditionsMet ? conditionsMet : !conditionsMet;

            // foreach (var particleSystem in particleSystems)
            // {
            //     if (shouldPlay)
            //     {
            //         particleSystem.Play();
            //     }
            //     else
            //     {
            //         particleSystem.Stop();
            //     }
            // }

            foreach (var vfxEffect in visualEffects) // <--- ИЗМЕНЕНО: particleSystem на vfxEffect
            {
                if (shouldPlay)
                {
                    vfxEffect.SendEvent("OnPlay");
                }
                else
                {
                    vfxEffect.SendEvent("OnStop");
                }
            }
        }
    }
}
