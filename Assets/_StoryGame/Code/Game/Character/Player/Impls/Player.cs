using System;
using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Core.Character.Player.Interfaces;
using R3;
using UnityEngine;
using UnityEngine.AI;
using VContainer;

namespace _StoryGame.Game.Character.Player.Impls
{
    [RequireComponent(typeof(CapsuleCollider), typeof(Animator), typeof(NavMeshAgent))]
    public sealed class Player : MonoBehaviour, IPlayer
    {
        [SerializeField] private PlayerFrontTriggerArea frontTriggerArea;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private float acceleration = 0f;

        public NavMeshAgent NavMeshAgent { get; private set; }
        public ReactiveProperty<Vector3> Position { get; } = new();
        public string Id => _interactor.Id;
        public string Name => _interactor.Name;
        public string Description => _interactor.Description;
        public object Animator { get; private set; }
        public int Health => _interactor.Health;
        public int MaxHealth => _interactor.MaxHealth;
        public CharacterState State { get; private set; } = CharacterState.Idle;

        private PlayerInteractor _interactor;
        private Rigidbody _rb;
        private Vector3 _currentVelocity;
        private Vector3 _previousPosition;

        [Inject]
        private void Construct(IObjectResolver resolver)
        {
            _interactor = resolver.Resolve<PlayerInteractor>();
            resolver.Inject(frontTriggerArea);
        }

        private void Awake()
        {
            Debug.Log("<color=green>Player awake</color>");
            NavMeshAgent = GetComponent<NavMeshAgent>();
            Animator = GetComponent<Animator>();
            _rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            Debug.Log("<color=green>Player started</color>");
            if (!frontTriggerArea)
                throw new NullReferenceException($"{nameof(frontTriggerArea)} is null. {name}");

            if (_interactor == null)
                throw new NullReferenceException($"{nameof(_interactor)} is null. {name}");

            frontTriggerArea.Init(this);
        }

        private void Update()
        {
            var position = NavMeshAgent.transform.position;

            if (_previousPosition == position)
                return;

            _previousPosition = position;
            Position.Value = position;
        }

        public ICharacterInteractor GetInteractor() => _interactor;
        public void SetState(CharacterState state) => State = state;
    }
}
