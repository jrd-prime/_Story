using System;
using R3;
using UnityEngine;
using VContainer.Unity;

namespace _StoryGame.Infrastructure.Tools
{
    public sealed class FPSCounter : ITickable
    {
        public Observable<float> Fps => _fps.ThrottleFirstLast(TimeSpan.FromSeconds(0.5f));

        private readonly ReactiveProperty<float> _fps = new(0);
        private float _deltaTime = 0.0f;

        public void Tick()
        {
            _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
            _fps.Value = 1.0f / _deltaTime;
        }
    }
}
