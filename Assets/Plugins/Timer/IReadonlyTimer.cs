using System;
using UniRx;

namespace Plugins.Timer
{
    public interface IReadonlyTimer
    {
        public IReadOnlyReactiveProperty<float> Progress { get; }
        public IReadOnlyReactiveProperty<float> RemainingProgress { get; }
        public IReadOnlyReactiveProperty<float> Time { get; }
        public IReadOnlyReactiveProperty<float> RemainingTime { get; }
        public IReadOnlyReactiveProperty<float> TargetTime { get; }
        public IReadOnlyReactiveProperty<float> TimeScale { get; }
        public IReadOnlyReactiveProperty<bool> IsPaused { get; }

        public IObservable<Unit> OnStarted { get; }
        public IObservable<Unit> OnCompleted { get; }
        public IObservable<Unit> OnStopped { get; }
    }
}