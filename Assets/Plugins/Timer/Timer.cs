using System;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace Plugins.Timer
{
    public class Timer : ITimer
    {
        private readonly FloatReactiveProperty _progress = new FloatReactiveProperty(0f);
        private readonly FloatReactiveProperty _remainingProgress = new FloatReactiveProperty(0f);
        private readonly FloatReactiveProperty _time = new FloatReactiveProperty(0f);
        private readonly FloatReactiveProperty _remainingTime = new FloatReactiveProperty(0f);
        private readonly FloatReactiveProperty _targetTime = new FloatReactiveProperty(0f);

        private readonly Subject<Unit> _onStarted = new Subject<Unit>();
        private readonly Subject<Unit> _onCompleted = new Subject<Unit>();
        private readonly Subject<Unit> _onStopped = new Subject<Unit>();
        private readonly Subject<Unit> _onPaused = new Subject<Unit>();
        private readonly Subject<Unit> _onResumed = new Subject<Unit>();
        private readonly Subject<Unit> _onReset = new Subject<Unit>();

        public IReadOnlyReactiveProperty<float> Progress => _progress;
        public IReadOnlyReactiveProperty<float> RemainingProgress => _remainingProgress;
        public IReadOnlyReactiveProperty<float> Time => _time;
        public IReadOnlyReactiveProperty<float> RemainingTime => _remainingTime;
        public IReadOnlyReactiveProperty<float> TargetTime => _targetTime;

        public IObservable<Unit> OnStarted => _onStarted;
        public IObservable<Unit> OnCompleted => _onCompleted;
        public IObservable<Unit> OnStopped => _onStopped;
        public IObservable<Unit> OnPaused => _onPaused;
        public IObservable<Unit> OnResumed => _onResumed;
        public IObservable<Unit> OnReset => _onReset;

        private Tween _tween;

        private float _duration;

        public void Start(float duration)
        {
            Reset();
            
            _duration = Mathf.Max(0, duration);
            _targetTime.Value = _duration;

            _onStarted.OnNext(Unit.Default);

            _tween = DOTween
                .To(() => _progress.Value, x => _progress.Value = x, 1f, _duration)
                .OnUpdate(() =>
                {
                    _time.Value = _progress.Value * _targetTime.Value;
                    _remainingTime.Value = _targetTime.Value - _time.Value;
                    _remainingProgress.Value = 1f - _progress.Value;
                })
                .OnComplete(Complete)
                .SetEase(Ease.Linear)
                .Play();
        }

        public void Complete()
        {
            if (_tween == null)
                return;

            Stop();

            _progress.Value = 1f;
            _remainingProgress.Value = 0f;
            _time.Value = _targetTime.Value;
            _remainingTime.Value = 0f;
            _onCompleted.OnNext(Unit.Default);
        }

        public void Stop()
        {
            if (_tween == null)
                return;

            _tween.Kill();
            _tween = null;
            _onStopped.OnNext(Unit.Default);
        }

        public void Pause()
        {
            if (_tween == null)
                return;

            if (_tween.IsPlaying())
            {
                _tween.Pause();
                _onPaused.OnNext(Unit.Default);
            }
        }

        public void Resume()
        {
            if (_tween == null)
                return;

            if (_tween.IsPlaying() == false)
            {
                _tween.Play();
                _onResumed.OnNext(Unit.Default);
            }
        }

        public void Reset()
        {
            Stop();
            _progress.Value = 0f;
            _remainingProgress.Value = 0f;
            _time.Value = 0f;
            _remainingTime.Value = 0f;
            _targetTime.Value = 0f;
            _onReset.OnNext(Unit.Default);
        }

        public void SetTimeScale(float timescale)
        {
            if (_tween == null)
                return;

            _tween.timeScale = Mathf.Max(0, timescale);
        }
    }
}