using System.Collections;
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
        public GameObject clickEffectPrefab; // Префаб Particle System
        public int poolSize = 5; // Размер пула (сколько эффектов держать готовыми)
        private Queue<ParticleSystem> effectPool = new(); // Пул объектов
        private int currentIndex = 0; // Индекс следующего свободного эффекта

        [Inject]
        private void Construct(ISubscriber<IMovementHandlerMsg> movementHandlerMsgSub)
        {
            movementHandlerMsgSub.Subscribe(ShowClickEffect);
        }

        void Start()
        {
            for (int i = 0; i < poolSize; i++)
            {
                GameObject effectObj = Instantiate(clickEffectPrefab, transform);
                ParticleSystem ps = effectObj.GetComponent<ParticleSystem>();
                effectObj.SetActive(false); // Отключаем объект при создании
                effectPool.Enqueue(ps);
            }
        }

        public async void ShowClickEffect(IMovementHandlerMsg msg)
        {
            if (msg is not MoveToPointHandlerMsg message)
                return;

            var position = message.Position;
            Debug.LogWarning(" ShowClickEffect position: " + position);
            Debug.LogWarning("PoolSize: " + effectPool.Count);


            // Берем следующий эффект из пула
            ParticleSystem effect = effectPool.Dequeue(); 
            effect.gameObject.SetActive(true); // Активируем объект

            // Перемещаем в точку клика
            effect.transform.position = position + Vector3.up * 0.01f; // Смещение над землей
            // effect.transform.rotation = Quaternion.Euler(90, 0, 0); // Ориентация на плоскости XZ

            // Запускаем воспроизведение
            effect.Play();

            // Планируем деактивацию после завершения эффекта (1 секунда)
            Debug.LogWarning("pre delay");
            var dur = effect.main.duration;
            Debug.LogWarning("duration: " + dur);
           // await UniTask.Delay((int)(dur*1000)+1000);
            await UniTask.WaitUntil(() => !effect.IsAlive(true));
            Debug.LogWarning("post delay");
            
            effectPool.Enqueue(effect);
            effect.gameObject.SetActive(false);
        }
    }
}
