using Plugins.Timer;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

public class TimerTest : MonoBehaviour
{
    [Header("Preferences")]
    [SerializeField] private float _tarrgetTime = 10f;

    [SerializeField] private float _progress;
    [SerializeField] private float _time;
    [SerializeField] private float _targetTime;

    private ITimer _timer;

    private void Awake()
    {
        _timer = new Timer();

        _timer.Progress.Subscribe(x => _progress = x).AddTo(this);
        _timer.Time.Subscribe(x => _time = x).AddTo(this);
        _timer.TargetTime.Subscribe(x => _targetTime = x).AddTo(this);

        _timer.OnStarted.Subscribe(_ => Debug.Log("Started")).AddTo(this);
        _timer.OnCompleted
            .Subscribe(_ =>
            {
                Debug.Log("Completed");
                StartTimer();
            })
            .AddTo(this);
        _timer.OnStopped.Subscribe(_ => Debug.Log("Stopped")).AddTo(this);
        _timer.OnPaused.Subscribe(_ => Debug.Log("Paused")).AddTo(this);
        _timer.OnResumed.Subscribe(_ => Debug.Log("Resumed")).AddTo(this);

        StartTimer();
    }

    [Button]
    private void StartTimer() => _timer.Start(_tarrgetTime);

    [Button]
    private void StopTimer() => _timer.Stop();

    [Button]
    private void PauseTimer() => _timer.Pause();

    [Button]
    private void ResumeTimer() => _timer.Resume();

    [Button]
    private void ResetTimer() => _timer.Reset();
}