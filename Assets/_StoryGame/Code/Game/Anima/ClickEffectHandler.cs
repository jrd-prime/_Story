using System.Collections.Generic;
using _StoryGame.Core.Messaging.Interfaces;
using _StoryGame.Game.Movement.Messages;
using Cysharp.Threading.Tasks;
using MessagePipe;
using UnityEngine;
using VContainer;

namespace _StoryGame.Game.Anima
{
    public class ClickEffectHandler : MonoBehaviour
    {
        public GameObject clickEffectPrefab;
        public int poolSize = 5;
        private readonly Queue<ParticleSystem> _effectPool = new();

        [Inject]
        private void Construct(ISubscriber<IMovementHandlerMsg> movementHandlerMsgSub) =>
            movementHandlerMsgSub.Subscribe(ShowClickEffect);

        private void Start()
        {
            for (var i = 0; i < poolSize; i++)
            {
                var effectObj = Instantiate(clickEffectPrefab, transform);
                var ps = effectObj.GetComponent<ParticleSystem>();
                effectObj.SetActive(false);
                _effectPool.Enqueue(ps);
            }
        }

        private async void ShowClickEffect(IMovementHandlerMsg msg)
        {
            if (msg is not MoveToPointHandlerMsg message)
                return;

            var position = message.Position;
            var effect = _effectPool.Dequeue();

            effect.gameObject.SetActive(true);
            effect.transform.position = position + Vector3.up * 0.01f;
            effect.Play();

            var dur = effect.main.duration;

            await UniTask.Delay((int)(dur * 1000));

            _effectPool.Enqueue(effect);
            effect.gameObject.SetActive(false);
        }
    }
}
