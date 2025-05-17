using System;
using System.Collections.Generic;
using _StoryGame.Infrastructure.Logging;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Infrastructure.Bootstrap
{
    public sealed class BootstrapLoader
    {
        private readonly Queue<IBootable> _loadingQueue = new();
        private readonly IBootstrapUIController _controller;
        private readonly IJLog _log;
        private int servicesCount;

        public BootstrapLoader(IBootstrapUIController controller, IJLog log)
        {
            _controller = controller;
            _log = log;
        }

        public void EnqueueBootable(IBootable bootable) => _loadingQueue.Enqueue(bootable);

        public async UniTask StartServicesInitializationAsync(int pseudoDelay = 0)
        {
            if (_loadingQueue.Count == 0)
                throw new Exception("No services to initialize! Use EnqueueBootable first.");

            foreach (var service in _loadingQueue)
            {
                try
                {
                    if (_controller != null)
                    {
                        _controller.SetLoadingText($"Loading: {service.Description}..");

                        await service.InitializeOnBoot();

                        if (pseudoDelay > 0)
                            await UniTask.Delay(pseudoDelay); // fake delay per service

                        _log.Info($"Service {service.GetType().Name} initialized.");
                        return;
                    }

                    _log.Error("BootstrapUIController is null. " + nameof(BootstrapLoader));
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to initialize {service.GetType().Name}: {ex.Message}");
                }
            }

            _log.Info("All services initialized.");
            await UniTask.CompletedTask;
        }
    }
}
