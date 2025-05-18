using System;
using System.Collections.Generic;
using _StoryGame.Infrastructure.Bootstrap.Interfaces;
using _StoryGame.Infrastructure.Logging;
using Cysharp.Threading.Tasks;

namespace _StoryGame.Infrastructure.Bootstrap
{
    public sealed class BootstrapLoader
    {
        private readonly Queue<IBootable> _loadingQueue = new();
        private readonly IBootstrapUIController _controller;
        private readonly IJLog _log;

        public BootstrapLoader(IBootstrapUIController controller, IJLog log)
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public void EnqueueBootable(IBootable bootable)
        {
            if (bootable == null)
                throw new ArgumentNullException(nameof(bootable));

            _loadingQueue.Enqueue(bootable);
        }

        public async UniTask StartServicesInitializationAsync(int pseudoDelay = 0)
        {
            if (_loadingQueue.Count == 0)
                return;

            while (_loadingQueue.Count > 0)
            {
                var service = _loadingQueue.Dequeue();
                try
                {
                    _controller.SetLoadingText($"Loading: {service.Description}...");

                    await service.InitializeOnBoot();

                    if (pseudoDelay > 0)
                        await UniTask.Delay(pseudoDelay);
                }
                catch (Exception ex)
                {
                    _log.Error($"Failed to initialize {service.GetType().Name}: {ex.Message}");
                    throw;
                }
            }
        }
    }
}
