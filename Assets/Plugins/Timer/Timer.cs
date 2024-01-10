using System;
using System.Collections;
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
        private readonly FloatReactiveProperty _timeScale = new FloatReactiveProperty(1f);
        private readonly BoolReactiveProperty _isPaused = new BoolReactiveProperty(false);

        private readonly Subject<Unit> _onStarted = new Subject<Unit>();
        private readonly Subject<Unit> _onCompleted = new Subject<Unit>();
        private readonly Subject<Unit> _onStopped = new Subject<Unit>();

        public IReadOnlyReactiveProperty<float> Progress => _progress;
        public IReadOnlyReactiveProperty<float> RemainingProgress => _remainingProgress;
        public IReadOnlyReactiveProperty<float> Time => _time;
        public IReadOnlyReactiveProperty<float> RemainingTime => _remainingTime;
        public IReadOnlyReactiveProperty<float> TargetTime => _targetTime;
        public IReadOnlyReactiveProperty<float> TimeScale => _timeScale;
        public IReadOnlyReactiveProperty<bool> IsPaused => _isPaused;

        public IObservable<Unit> OnStarted => _onStarted;
        public IObservable<Unit> OnCompleted => _onCompleted;
        public IObservable<Unit> OnStopped => _onStopped;

        private Coroutine _coroutine;

        private float _duration;

        public void Start(float duration)
        {
            Reset();

            _duration = Mathf.Max(0, duration);
            _targetTime.Value = _duration;

            _onStarted.OnNext(Unit.Default);

            _coroutine = CoroutineRunner.Run(TimerRoutine());
        }

        private IEnumerator TimerRoutine()
        {
            while (true)
            {
                if (_isPaused.Value)
                {
                    yield return null;
                    continue;
                }

                _time.Value += UnityEngine.Time.deltaTime * _timeScale.Value;
                _remainingTime.Value = _targetTime.Value - _time.Value;
                _progress.Value = _time.Value / _targetTime.Value;
                _remainingProgress.Value = 1f - _progress.Value;

                if ((Mathf.Sign(_timeScale.Value) >= 0 && _time.Value >= _targetTime.Value) ||
                    (Mathf.Sign(_timeScale.Value) < 0 && _time.Value <= 0f))
                {
                    Complete();
                    yield break;
                }

                yield return null;
            }
        }

        public void Complete()
        {
            if (_coroutine == null)
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
            if (_coroutine == null)
                return;

            CoroutineRunner.Stop(_coroutine);
            _coroutine = null;
            _onStopped.OnNext(Unit.Default);
        }

        public void Pause()
        {
            if (_coroutine == null)
                return;

            if (_isPaused.Value == false)
                _isPaused.Value = true;
        }

        public void Resume()
        {
            if (_coroutine == null)
                return;

            if (_isPaused.Value)
                _isPaused.Value = false;
        }

        public void TogglePause()
        {
            if (_isPaused.Value)
                Resume();
            else
                Pause();
        }

        public void Reset()
        {
            Stop();
            _progress.Value = 0f;
            _remainingProgress.Value = 0f;
            _time.Value = 0f;
            _remainingTime.Value = 0f;
            _targetTime.Value = 0f;
            _isPaused.Value = false;
            _timeScale.Value = 1f;
        }

        public void SetTimeScale(float timescale)
        {
            if (_coroutine == null)
                return;

            _timeScale.Value = timescale;
        }

        public void SetTime(float time)
        {
            if (_coroutine == null)
                return;

            _time.Value = Mathf.Clamp(time, 0f, _targetTime.Value);
        }

        public void SetTargetTime(float targetTime)
        {
            if (_coroutine == null)
                return;

            _targetTime.Value = Mathf.Max(0, targetTime);
        }
    }
}