using System;
using UniRx;

namespace Plugins.Timer
{
    public interface IReadonlyTimer
    {
        public IReadOnlyReactiveProperty<float> Progress { get; }
        public IReadOnlyReactiveProperty<float> Time { get; }
        public IReadOnlyReactiveProperty<float> TargetTime { get; }

        public IObservable<Unit> OnStarted { get; }
        public IObservable<Unit> OnCompleted { get; }
        public IObservable<Unit> OnStopped { get; }
        public IObservable<Unit> OnPaused { get; }
        public IObservable<Unit> OnResumed { get; }
    }
}