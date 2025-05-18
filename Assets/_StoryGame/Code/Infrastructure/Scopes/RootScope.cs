using System;
using _StoryGame.Data;
using _StoryGame.Infrastructure.Assets;
using _StoryGame.Infrastructure.Bootstrap;
using _StoryGame.Infrastructure.Input;
using _StoryGame.Infrastructure.Input.Interfaces;
using _StoryGame.Infrastructure.Localization;
using _StoryGame.Infrastructure.Logging;
using _StoryGame.Infrastructure.Settings;
using _StoryGame.Infrastructure.Tools;
using MessagePipe;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;
using VContainer.Unity;
#if UNITY_EDITOR
using UnityEngine.Profiling;
#endif

namespace _StoryGame.Infrastructure.Scopes
{
    public class RootScope : LifetimeScope
    {
        [SerializeField] private BootstrapSettings bootstrapSettings;
        [SerializeField] private MainSettings mainSettings;
        [SerializeField] private MobileInput input;
        [SerializeField] private EventSystem eventSystem;
        [SerializeField] private JLog log;

        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Log($"<color=cyan>{nameof(RootScope)}</color>");

            RegisterMessagePipe(builder);

            if (!bootstrapSettings)
                throw new NullReferenceException("BootstrapSettings is null.");
            builder.RegisterInstance(bootstrapSettings);

            if (!mainSettings)
                throw new NullReferenceException("MainSettings is null.");
            builder.RegisterInstance(mainSettings);

            builder.Register<SettingsProvider>(Lifetime.Singleton).As<ISettingsProvider>();
            builder.Register<AssetProvider>(Lifetime.Singleton).As<IAssetProvider>();
            builder.Register<LocalizationProvider>(Lifetime.Singleton).As<ILocalizationProvider>();
            builder.Register<FirstSceneProvider>(Lifetime.Singleton);

            if (!input)
                throw new NullReferenceException("input is null.");
            builder.RegisterComponent(input).As<IJInput>();

            if (!eventSystem)
                throw new NullReferenceException("eventSystem is null.");
            builder.RegisterComponent(eventSystem);

            if (!log)
                throw new NullReferenceException("log is null.");
            builder.RegisterComponentInNewPrefab(log, Lifetime.Singleton).As<IJLog>();

            builder.Register<FullScreenMovementProcessor>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();

            builder.Register<FPSCounter>(Lifetime.Singleton).AsSelf().As<ITickable>();
        }

        private void RegisterMessagePipe(IContainerBuilder builder)
        {
            var options = builder.RegisterMessagePipe();

            // Setup GlobalMessagePipe to enable diagnostics window and global function
            builder.RegisterBuildCallback(c => GlobalMessagePipe.SetProvider(c.AsServiceProvider()));

            // RegisterMessageBroker: Register for IPublisher<T>/ISubscriber<T>, includes async and buffered.
            // builder.RegisterMessageBroker<ChangeGameStateSignalVo>(options);
        }

#if UNITY_EDITOR
        private void OnApplicationQuit()
        {
            var rendTex = (RenderTexture[])Resources.FindObjectsOfTypeAll(typeof(RenderTexture));

            var rendTexCount = rendTex.Length;
            var i = 0;
            foreach (var t in rendTex)
                if (t.name.StartsWith("Device Simulator"))
                {
                    Destroy(t);
                    i++;
                }

            Debug.Log("<color=darkblue><b>===</b></color>");

            if (i > 0) Debug.Log($"Render Textures: {rendTexCount} / Destroyed: {i}");

            Debug.Log(
                $"Allocated: {Profiler.GetTotalAllocatedMemoryLong() / (1024 * 1024)} MB / " +
                $"Reserved: {Profiler.GetTotalReservedMemoryLong() / (1024 * 1024)} MB / " +
                $"Unused Reserved: {Profiler.GetTotalUnusedReservedMemoryLong() / (1024 * 1024)} MB");

            Debug.Log("<color=darkblue><b>===</b></color>");
        }
#endif
    }
}
