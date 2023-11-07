using System;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace Plugins.Timer
{
    public class Timer
    {
        private readonly FloatReactiveProperty _progress = new FloatReactiveProperty(0f);
        private readonly FloatReactiveProperty _time = new FloatReactiveProperty(0f);
        private readonly FloatReactiveProperty _targetTime = new FloatReactiveProperty(0f);

        private readonly Subject<Unit> _onStarted = new Subject<Unit>();
        private readonly Subject<Unit> _onCompleted = new Subject<Unit>();
        private readonly Subject<Unit> _onStopped = new Subject<Unit>();
        private readonly Subject<Unit> _onPaused = new Subject<Unit>();
        private readonly Subject<Unit> _onResumed = new Subject<Unit>();

        public IReadOnlyReactiveProperty<float> Progress => _progress;
        public IReadOnlyReactiveProperty<float> Time => _time;
        public IReadOnlyReactiveProperty<float> TargetTime => _targetTime;

        public IObservable<Unit> OnStarted => _onStarted;
        public IObservable<Unit> OnCompleted => _onCompleted;
        public IObservable<Unit> OnStopped => _onStopped;
        public IObservable<Unit> OnPaused => _onPaused;
        public IObservable<Unit> OnResumed => _onResumed;

        private Tween _tween;

        private float _duration;

        public void Start(float duration)
        {
            Stop();

            _duration = Mathf.Max(0, duration);

            _onStarted.OnNext(Unit.Default);
            _progress.Value = 0f;
            _time.Value = 0f;
            _targetTime.Value = _duration;

            _tween = DOTween
                .To(() => _progress.Value, x => _progress.Value = x, 1f, _duration)
                .OnUpdate(() => _time.Value = _progress.Value * _targetTime.Value)
                .OnComplete(() =>
                {
                    _onCompleted.OnNext(Unit.Default);
                    _onStopped.OnNext(Unit.Default);
                    _progress.Value = 1f;
                    _time.Value = _targetTime.Value;
                })
                .SetEase(Ease.Linear)
                .Play();
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
            _time.Value = 0f;
            _targetTime.Value = 0f;
        }
    }
}