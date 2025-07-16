using System;
using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Core.Character.Player;
using _StoryGame.Core.Character.Player.Interfaces;
using _StoryGame.Core.Common.Interfaces;
using _StoryGame.Core.Messaging.Interfaces;
using _StoryGame.Core.WalletNew.Interfaces;
using _StoryGame.Data.Loot;
using _StoryGame.Data.SO.Abstract;
using _StoryGame.Game.Character.Player.Messages;
using _StoryGame.Game.Movement;
using Cysharp.Threading.Tasks;
using MessagePipe;
using R3;
using UnityEngine;
using UnityEngine.AI;
using VContainer.Unity;

namespace _StoryGame.Game.Character.Player.Impls
{
    public sealed class PlayerInteractor : IPlayer, IInitializable, IFixedTickable
    {
        public ReadOnlyReactiveProperty<Vector3> Position => _playerView.Position.ToReadOnlyReactiveProperty();
        public ReadOnlyReactiveProperty<ECharacterState> State => _state.ToReadOnlyReactiveProperty();
        public ReadOnlyReactiveProperty<Vector3> DestinationPoint => _destinationPoint.ToReadOnlyReactiveProperty();
        public ReadOnlyReactiveProperty<int> Energy => _energy.ToReadOnlyReactiveProperty();

        public string Id => _service.Id;
        public IWallet Wallet => _wallet;
        public string Name => _playerView.name;
        public string Description => _playerView.Description;
        public object Animator => _playerView.Animator;
        public int Health { get; set; }
        public int MaxHealth { get; set; }

        public ReadOnlyReactiveProperty<int> MaxEnergy => _maxEnergy.ToReadOnlyReactiveProperty();
        public NavMeshAgent NavMeshAgent => _playerView.NavMeshAgent;

        private readonly ReactiveProperty<int> _energy = new(0);
        private readonly ReactiveProperty<ECharacterState> _state = new(ECharacterState.Idle);
        private readonly ReactiveProperty<Vector3> _destinationPoint = new();
        private readonly ReactiveProperty<int> _maxEnergy = new(0);

        private readonly PlayerService _service;
        private readonly IWallet _wallet;
        private readonly PlayerView _playerView;
        private readonly IPublisher<IPlayerMsg> _selfMsgPub;
        private readonly IJLog _log;
        private readonly PlayerMessageHandler _messageHandler;
        private readonly IJPublisher _publisher;

        public PlayerInteractor(
            PlayerService service,
            PlayerView playerView,
            IJLog log,
            IWalletService walletService,
            IPublisher<IPlayerMsg> selfMsgPub,
            ISubscriber<IPlayerAnimatorMsg> playerAnimatorMsgSub,
            IJPublisher publisher)
        {
            _playerView = playerView;
            _service = service;
            _log = log;
            _wallet = walletService.GetOrCreate(Id);
            _selfMsgPub = selfMsgPub;
            _publisher = publisher;

            _messageHandler = new PlayerMessageHandler(this, playerAnimatorMsgSub);
        }

        public void Initialize()
        {
            SetState(ECharacterState.Idle);
            _maxEnergy.Value = _service.MaxEnergy;
        }

        public void SetState(ECharacterState state)
        {
            // _log.Debug("Player SetState > " + state);
            _state.Value = state;
            PublishState(state);
        }

        private void PublishState(ECharacterState state) =>
            _selfMsgPub.Publish(new PlayerStateMsg(state));

        public async UniTask MoveToPointAsync(Vector3 position, EDestinationPoint destinationPointType)
        {
            _destinationPoint.Value = position;

            switch (destinationPointType)
            {
                case EDestinationPoint.Ground:
                    SetState(ECharacterState.MovingToPoint);
                    break;
                case EDestinationPoint.Entrance:
                    SetState(ECharacterState.MovingToInteractable);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(destinationPointType), destinationPointType, null);
            }

            await _playerView.MoveToAsync(position);

            SetState(ECharacterState.Idle);
        }

        public void OnStartInteract()
        {
            _log.Debug("On Start Interact");

            SetState(ECharacterState.Interacting);
        }

        public void OnEndInteract()
        {
            _log.Debug("On End Interact");
            SetState(ECharacterState.Idle);
        }

        public bool HasEnoughEnergy(int energy)
        {
            _selfMsgPub.Publish(new NotEnoughEnergyMsg());
            return _energy.Value >= energy;
        }

        /// <summary>
        /// Set directly energy 0+
        /// </summary>
        public void SetEnergy(int energy)
        {
            if (IsEnergyArgNegative(energy, "set"))
                return;

            _energy.Value = energy;
        }

        /// <summary>
        /// Add energy 0+
        /// </summary>
        /// <param name="energy"></param>
        public void AddEnergy(int energy)
        {
            if (IsEnergyArgNegative(energy, "add"))
                return;

            _energy.Value += energy;
        }

        /// <summary>
        /// Spend energy 0+
        /// </summary>
        /// <param name="energy"></param>
        public void SpendEnergy(int energy)
        {
            if (IsEnergyArgNegative(energy, "spend"))
                return;

            if (HasEnoughEnergy(energy))
                _energy.Value -= energy;

            if (_energy.CurrentValue == 0)
                _selfMsgPub.Publish(new OutOfEnergyMsg());
        }

        public void AddNote(PreparedLootVo preparedLootVo)
        {
            _wallet.Add(preparedLootVo.Currency.Id, preparedLootVo.Currency.Amount);
        }

        public void AddItemToWallet(ACurrencyData itemData, int amount)
        {
            _wallet.Add(itemData.Id, amount);
        }

        public void SetPosition(Vector3 value) => _playerView.SetPosition(value);

        /// <summary>
        /// Check if energy is negative
        /// </summary>
        private bool IsEnergyArgNegative(int energy, string operation)
        {
            if (energy >= 0)
                return false;

            _log.Warn($"Try to {operation} negative energy!");
            return true;
        }

// Новые константы для управления замедлением и анимацией
        // Расстояние до цели, на котором начинаем задумываться о торможении
        private const float BrakeAnimationStartDistance = 1.5f; // Настройте это! 1.0-1.5 метра обычно хорошо

        // Порог скорости, ниже которого считаем, что агент остановился для анимации
        private const float AnimatorStopSpeedThreshold = 0.05f;

// Новое поле для отслеживания состояния Root Motion
        private bool _isApplyingRootMotion = true;

        // Время сглаживания для параметра speed в аниматоре
        private const float AnimatorDampTime = 0.1f; // Настройте для плавности

        // Новое поле для отслеживания предыдущей скорости
        private float _previousPhysicalSpeed = 0f;
        private float _originalNavMeshAgentSpeed = 5f;

        public void FixedTick()
        {
            if (NavMeshAgent == null || !NavMeshAgent.enabled || _playerView.Animator == null) return;

            float currentPhysicalSpeed = NavMeshAgent.velocity.magnitude;
            float distanceToTarget = NavMeshAgent.hasPath ? NavMeshAgent.remainingDistance : 0f;

            // --- Логика замедления NavMeshAgent ---
            if (NavMeshAgent.hasPath && NavMeshAgent.remainingDistance > NavMeshAgent.stoppingDistance)
            {
                float targetNavMeshAgentSpeed;
                if (distanceToTarget <= BrakeAnimationStartDistance)
                {
                    float normalizedDistance = distanceToTarget / BrakeAnimationStartDistance;
                    targetNavMeshAgentSpeed = Mathf.Lerp(0.5f, _originalNavMeshAgentSpeed, normalizedDistance);
                    NavMeshAgent.speed = Mathf.Max(targetNavMeshAgentSpeed, 0.1f);
                }
                else
                {
                    NavMeshAgent.speed = _originalNavMeshAgentSpeed;
                }
            }
            else
            {
                NavMeshAgent.speed = _originalNavMeshAgentSpeed;
            }

            // --- Управление анимацией с нормализацией ---
            float animatorSpeed = currentPhysicalSpeed / _originalNavMeshAgentSpeed; // Нормализация: 0 до 1
            _playerView.Animator.SetFloat("speed", animatorSpeed, AnimatorDampTime, Time.fixedDeltaTime);

            // --- Логика состояния Root Motion и анимации торможения ---
            if (_playerView.IsApplyingRootMotion)
            {
                bool shouldExitBraking = false;

                // Проверяем, остановился ли агент или анимация торможения завершена
                if (currentPhysicalSpeed < AnimatorStopSpeedThreshold || NavMeshAgent.isStopped)
                {
                    shouldExitBraking = true;
                }
                // Проверяем, начал ли агент новое движение с высокой скоростью
                else if (!_playerView.Animator.GetCurrentAnimatorStateInfo(0).IsName("Braking") &&
                         currentPhysicalSpeed > AnimatorStopSpeedThreshold * 2)
                {
                    shouldExitBraking = true;
                }

                if (shouldExitBraking)
                {
                    _playerView.Animator.SetBool("IsBraking", false);
                    NavMeshAgent.updatePosition = true;
                    NavMeshAgent.nextPosition = _playerView.transform.position;
                    _playerView.IsApplyingRootMotion = false;
                    SetState(currentPhysicalSpeed < AnimatorStopSpeedThreshold
                        ? ECharacterState.Idle
                        : ECharacterState.MovingToPoint);
                    _log.Debug("Ended Braking Animation & Root Motion. Syncing NavMeshAgent.");
                }
                else
                {
                    _playerView.Animator.SetFloat("speed", animatorSpeed, AnimatorDampTime, Time.fixedDeltaTime);
                }
            }
            else
            {
                bool shouldEnterBraking = false;

                // Условия для входа в торможение с гистерезисом
                if (NavMeshAgent.hasPath &&
                    distanceToTarget <= BrakeAnimationStartDistance &&
                    currentPhysicalSpeed > AnimatorStopSpeedThreshold &&
                    currentPhysicalSpeed < _previousPhysicalSpeed - 0.5f && // Увеличен порог для стабильности
                    !_playerView.Animator.GetCurrentAnimatorStateInfo(0).IsName("Braking"))
                {
                    shouldEnterBraking = true;
                }
                else if (!NavMeshAgent.hasPath &&
                         currentPhysicalSpeed > AnimatorStopSpeedThreshold &&
                         currentPhysicalSpeed < _previousPhysicalSpeed - 0.5f &&
                         !_playerView.Animator.GetCurrentAnimatorStateInfo(0).IsName("Braking"))
                {
                    shouldEnterBraking = true;
                }

                if (shouldEnterBraking && !_playerView.IsApplyingRootMotion) // Проверяем, что не в процессе торможения
                {
                    _playerView.Animator.SetBool("IsBraking", true);
                    NavMeshAgent.updatePosition = false;
                    _playerView.IsApplyingRootMotion = true;
                    SetState(ECharacterState.Interacting);
                    _log.Debug("Started Braking Animation & Root Motion");
                }
                else
                {
                    _playerView.Animator.SetBool("IsBraking", false);
                    if (currentPhysicalSpeed < AnimatorStopSpeedThreshold)
                    {
                        _playerView.Animator.SetFloat("speed", 0f);
                        SetState(ECharacterState.Idle);
                    }
                    else
                    {
                        SetState(ECharacterState.MovingToPoint);
                    }
                }
            }

            _previousPhysicalSpeed = currentPhysicalSpeed;
        }


        //
        // public void FixedTick()
        // {
        //     if (NavMeshAgent == null || !NavMeshAgent.enabled || _playerView.Animator == null) return;
        //
        //     float currentPhysicalSpeed = NavMeshAgent.velocity.magnitude;
        //     float distanceToTarget = NavMeshAgent.hasPath ? NavMeshAgent.remainingDistance : 0f;
        //
        //     // --- Логика замедления NavMeshAgent ---
        //     if (NavMeshAgent.hasPath && NavMeshAgent.remainingDistance > NavMeshAgent.stoppingDistance)
        //     {
        //         float targetNavMeshAgentSpeed;
        //         if (distanceToTarget <= BrakeAnimationStartDistance)
        //         {
        //             float normalizedDistance = distanceToTarget / BrakeAnimationStartDistance;
        //             targetNavMeshAgentSpeed = Mathf.Lerp(0.5f, _originalNavMeshAgentSpeed, normalizedDistance);
        //             NavMeshAgent.speed = Mathf.Max(targetNavMeshAgentSpeed, 0.1f);
        //         }
        //         else
        //         {
        //             NavMeshAgent.speed = _originalNavMeshAgentSpeed;
        //         }
        //     }
        //     else
        //     {
        //         NavMeshAgent.speed = _originalNavMeshAgentSpeed;
        //     }
        //
        //     // --- Управление анимацией с нормализацией ---
        //     float animatorSpeed = currentPhysicalSpeed / _originalNavMeshAgentSpeed; // Нормализация: 0 до 1
        //     _playerView.Animator.SetFloat("speed", animatorSpeed, AnimatorDampTime, Time.fixedDeltaTime);
        //
        //     // --- Логика состояния Root Motion и анимации торможения ---
        //     if (_playerView.IsApplyingRootMotion)
        //     {
        //         bool shouldExitBraking = false;
        //
        //         if (currentPhysicalSpeed < AnimatorStopSpeedThreshold || NavMeshAgent.isStopped)
        //         {
        //             shouldExitBraking = true;
        //         }
        //         else if (!_playerView.Animator.GetCurrentAnimatorStateInfo(0).IsName("Braking") &&
        //                  currentPhysicalSpeed > AnimatorStopSpeedThreshold * 2)
        //         {
        //             shouldExitBraking = true;
        //         }
        //
        //         if (shouldExitBraking)
        //         {
        //             _playerView.Animator.SetBool("IsBraking", false);
        //             NavMeshAgent.updatePosition = true;
        //             NavMeshAgent.nextPosition = _playerView.transform.position;
        //             _playerView.IsApplyingRootMotion = false;
        //             SetState(currentPhysicalSpeed < AnimatorStopSpeedThreshold
        //                 ? ECharacterState.Idle
        //                 : ECharacterState.MovingToPoint);
        //             _log.Debug("Ended Braking Animation & Motion. Syncing NavMeshAgent.");
        //         }
        //         else
        //         {
        //             _playerView.Animator.SetFloat("speed", animatorSpeed, AnimatorDampTime, Time.fixedDeltaTime);
        //         }
        //     }
        //     else
        //     {
        //         bool shouldEnterBraking = false;
        //
        //         if (NavMeshAgent.hasPath &&
        //             distanceToTarget <= BrakeAnimationStartDistance &&
        //             currentPhysicalSpeed > AnimatorStopSpeedThreshold &&
        //             currentPhysicalSpeed < _previousPhysicalSpeed - 0.3f &&
        //             !_playerView.Animator.GetCurrentAnimatorStateInfo(0).IsName("Braking"))
        //         {
        //             shouldEnterBraking = true;
        //         }
        //         else if (!NavMeshAgent.hasPath &&
        //                  currentPhysicalSpeed > AnimatorStopSpeedThreshold &&
        //                  currentPhysicalSpeed < _previousPhysicalSpeed - 0.3f &&
        //                  !_playerView.Animator.GetCurrentAnimatorStateInfo(0).IsName("Braking"))
        //         {
        //             shouldEnterBraking = true;
        //         }
        //
        //         if (shouldEnterBraking)
        //         {
        //             _playerView.Animator.SetBool("IsBraking", true);
        //             NavMeshAgent.updatePosition = false;
        //             _playerView.IsApplyingRootMotion = true;
        //             SetState(ECharacterState.Interacting);
        //             _log.Debug("Started Braking Animation & Root Motion");
        //         }
        //         else
        //         {
        //             _playerView.Animator.SetBool("IsBraking", false);
        //             if (currentPhysicalSpeed < AnimatorStopSpeedThreshold)
        //             {
        //                 _playerView.Animator.SetFloat("speed", 0f);
        //                 SetState(ECharacterState.Idle);
        //             }
        //             else
        //             {
        //                 SetState(ECharacterState.MovingToPoint);
        //             }
        //         }
        //     }
        //
        //     _previousPhysicalSpeed = currentPhysicalSpeed;
        // }
        //

        //
        // public void FixedTick()
        // {
        //     if (NavMeshAgent == null || !NavMeshAgent.enabled || _playerView.Animator == null) return;
        //
        //     float currentPhysicalSpeed = NavMeshAgent.velocity.magnitude;
        //     float distanceToTarget = NavMeshAgent.hasPath ? NavMeshAgent.remainingDistance : 0f;
        //
        //     // --- Логика замедления NavMeshAgent ---
        //     if (NavMeshAgent.hasPath && NavMeshAgent.remainingDistance > NavMeshAgent.stoppingDistance)
        //     {
        //         float targetNavMeshAgentSpeed;
        //         if (distanceToTarget <= BrakeAnimationStartDistance)
        //         {
        //             float normalizedDistance = distanceToTarget / BrakeAnimationStartDistance;
        //             targetNavMeshAgentSpeed = Mathf.Lerp(0.5f, _originalNavMeshAgentSpeed, normalizedDistance);
        //             NavMeshAgent.speed = Mathf.Max(targetNavMeshAgentSpeed, 0.1f);
        //         }
        //         else
        //         {
        //             NavMeshAgent.speed = _originalNavMeshAgentSpeed;
        //         }
        //     }
        //     else
        //     {
        //         NavMeshAgent.speed = _originalNavMeshAgentSpeed;
        //     }
        //
        //     // --- Улучшенное управление состоянием Root Motion и аниматором ---
        //     if (_playerView.IsApplyingRootMotion)
        //     {
        //         bool shouldExitBraking = false;
        //
        //         // Проверяем, остановился ли агент или анимация торможения завершена
        //         if (currentPhysicalSpeed < AnimatorStopSpeedThreshold || NavMeshAgent.isStopped)
        //         {
        //             shouldExitBraking = true;
        //         }
        //         // Проверяем, начал ли агент новое движение с высокой скоростью
        //         else if (!_playerView.Animator.GetCurrentAnimatorStateInfo(0).IsName("Braking") &&
        //                  currentPhysicalSpeed > AnimatorStopSpeedThreshold * 2)
        //         {
        //             shouldExitBraking = true;
        //         }
        //
        //         if (shouldExitBraking)
        //         {
        //             _playerView.Animator.SetBool("IsBraking", false);
        //             NavMeshAgent.updatePosition = true;
        //             NavMeshAgent.nextPosition = _playerView.transform.position;
        //             _playerView.IsApplyingRootMotion = false;
        //             SetState(currentPhysicalSpeed < AnimatorStopSpeedThreshold
        //                 ? ECharacterState.Idle
        //                 : ECharacterState.MovingToPoint);
        //             _log.Debug("Ended Braking Animation & Root Motion. Syncing NavMeshAgent.");
        //         }
        //         else
        //         {
        //             _playerView.Animator.SetFloat("speed", currentPhysicalSpeed, AnimatorDampTime, Time.fixedDeltaTime);
        //         }
        //     }
        //     else
        //     {
        //         bool shouldEnterBraking = false;
        //
        //         // Условия для входа в торможение
        //         if (NavMeshAgent.hasPath &&
        //             distanceToTarget <= BrakeAnimationStartDistance &&
        //             currentPhysicalSpeed > AnimatorStopSpeedThreshold &&
        //             currentPhysicalSpeed < _previousPhysicalSpeed - 0.3f && // Увеличиваем порог
        //             !_playerView.Animator.GetCurrentAnimatorStateInfo(0).IsName("Braking"))
        //         {
        //             shouldEnterBraking = true;
        //         }
        //         else if (!NavMeshAgent.hasPath &&
        //                  currentPhysicalSpeed > AnimatorStopSpeedThreshold &&
        //                  currentPhysicalSpeed < _previousPhysicalSpeed - 0.3f &&
        //                  !_playerView.Animator.GetCurrentAnimatorStateInfo(0).IsName("Braking"))
        //         {
        //             shouldEnterBraking = true;
        //         }
        //
        //         if (shouldEnterBraking)
        //         {
        //             _playerView.Animator.SetBool("IsBraking", true);
        //             NavMeshAgent.updatePosition = false;
        //             _playerView.IsApplyingRootMotion = true;
        //             SetState(ECharacterState.Interacting);
        //             _log.Debug("Started Braking Animation & Root Motion");
        //         }
        //         else
        //         {
        //             _playerView.Animator.SetBool("IsBraking", false);
        //             if (currentPhysicalSpeed < AnimatorStopSpeedThreshold)
        //             {
        //                 _playerView.Animator.SetFloat("speed", 0f);
        //                 SetState(ECharacterState.Idle);
        //             }
        //             else
        //             {
        //                 _playerView.Animator.SetFloat("speed", currentPhysicalSpeed, AnimatorDampTime,
        //                     Time.fixedDeltaTime);
        //                 SetState(ECharacterState.MovingToPoint);
        //             }
        //         }
        //     }
        //
        //     _previousPhysicalSpeed = currentPhysicalSpeed;
        // }

        // OnAnimatorMove остается тем же
        public void OnAnimatorMove()
        {
            if (_isApplyingRootMotion && NavMeshAgent != null)
            {
                // Применяем смещение из Root Motion к NavMeshAgent
                Vector3 rootMovement = _playerView.Animator.deltaPosition;
                NavMeshAgent.velocity =
                    rootMovement / Time.deltaTime; // Это для того, чтобы агент "почувствовал" движение
                // Или если хотите, чтобы трансформ NavMeshAgent напрямую следовал аниматору:
                // NavMeshAgent.transform.position = _playerView.Animator.rootPosition;
            }
        }
    }

    public record NotEnoughEnergyMsg : IPlayerMsg;

    public record OutOfEnergyMsg : IPlayerMsg;
}
