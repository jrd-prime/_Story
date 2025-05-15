using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Infrastructure.Bootstrap
{
    public sealed class BootstrapLoader
    {
        private readonly Queue<IBootable> _loadingQueue = new();
        private readonly IBootstrapUIController _controller;

        public BootstrapLoader(IBootstrapUIController controller) => _controller = controller;
        
        public void EnqueueBootable(IBootable bootable) => _loadingQueue.Enqueue(bootable);
        
        public async UniTask StartServicesInitializationAsync(int pseudoDelay = 0)
        {
            if (_loadingQueue.Count == 0)
                throw new Exception("No services to initialize! Use EnqueueBootable first.");

            foreach (var service in _loadingQueue)
            {
                try
                {
                    _controller.SetLoadingText($"Loading: {service.Description}..");
                    await service.InitializeOnBoot();

                    if (pseudoDelay > 0) await UniTask.Delay(pseudoDelay); // fake delay per service
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to initialize {service.GetType().Name}: {ex.Message}");
                }
            }

            await UniTask.CompletedTask;
        }
    }
}
