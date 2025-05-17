using System;
using _StoryGame.Core.Character.Common.Interfaces;
using _StoryGame.Gameplay.Managers.Inerfaces;
using _StoryGame.Infrastructure.Logging;
using R3;
using UnityEngine;
using VContainer;

namespace _StoryGame.Gameplay.Managers.Impls
{
    namespace _Game._Scripts.Framework.Manager.JCamera
    {
        public sealed class CameraManager : MonoBehaviour, ICameraManager
        {
            [SerializeField] private Vector3 cameraOffset = new(5, 8, -5);
            private Camera _mainCamera;
            private readonly CompositeDisposable _disposables = new();
            private IFollowable _target;
            [Inject] private IJLog _log;

            private void Start()
            {
                _log.Info("CameraManager started.");
                _mainCamera = Camera.main;

                if (!_mainCamera)
                    throw new NullReferenceException($"MainCamera is null. {this}");

                _mainCamera.transform.position = cameraOffset;
            }

            private void SetCameraPosition(Vector3 position)
            {
                Vector3 newPosition = position + cameraOffset;
                if (_mainCamera.transform.position == newPosition) return;
                _mainCamera.transform.position = newPosition;
            }

            public void SetTarget(IFollowable target)
            {
                if (target == null)
                    throw new ArgumentNullException($"Target is null. {this}");

                if (_target == target)
                    return;

                _disposables.Clear();
                _target = target;
                _target.Position.Subscribe(SetCameraPosition).AddTo(_disposables);
            }

            public void RemoveTarget()
            {
                _target = null;
                _disposables?.Dispose();
            }

            public Camera GetMainCamera() => _mainCamera;


            public Vector3 GetCamEulerAngles()
            {
                if (!_mainCamera)
                    throw new NullReferenceException($"MainCamera is null. {this}");

                return _mainCamera.transform.eulerAngles;
            }

            public Quaternion GetCamRotation()
            {
                if (!_mainCamera)
                    throw new NullReferenceException($"MainCamera is null. {this}");

                return _mainCamera.transform.rotation;
            }

            private void OnDestroy() => _disposables?.Dispose();
        }
    }
}
