using System;
using _StoryGame.Core.Character.Common.Interfaces;
using R3;
using UnityEngine;
using UnityEngine.AI;
using VContainer;

namespace _StoryGame.Game.Character.Player.Impls
{
    [RequireComponent(typeof(CapsuleCollider), typeof(Animator), typeof(NavMeshAgent))]
    public sealed class PlayerView : MonoBehaviour
    {
        [SerializeField] private PlayerFrontTriggerArea frontTriggerArea;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private float acceleration = 0f;
        public ReactiveProperty<Vector3> Position { get; } = new();
        public object Animator { get; private set; }
        public CharacterState State { get; private set; } = CharacterState.Idle;
        public NavMeshAgent NavMeshAgent { get; private set; }

        private IObjectResolver _resolver;

        private Rigidbody _rb;
        private Vector3 _currentVelocity;
        private Vector3 _previousPosition;

        private readonly CompositeDisposable _disposables = new();

        [Inject]
        private void Construct(IObjectResolver resolver) => _resolver = resolver;

        private void Awake()
        {
            NavMeshAgent = GetComponent<NavMeshAgent>();
            Animator = GetComponent<Animator>();
            _rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            if (!frontTriggerArea)
                throw new NullReferenceException($"{nameof(frontTriggerArea)} is null. {name}");

            _resolver.Inject(frontTriggerArea);
            frontTriggerArea.Init();
        }

        private void Update()
        {
            var position = NavMeshAgent.transform.position;

            if (_previousPosition == position)
                return;

            _previousPosition = position;
            Position.Value = position;
        }

        public void SetState(CharacterState state) =>
            State = state;

        public void MoveTo(Vector3 destination) => NavMeshAgent.SetDestination(destination);
    }
}
