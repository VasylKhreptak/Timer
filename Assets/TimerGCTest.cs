using System;
using Plugins.Timer;
using UnityEngine;

public class TimerGCTest : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private int _timerCount = 10000;

    private void Start()
    {
        for (int i = 0; i < _timerCount; i++)
        {
            ITimer timer = new Timer();
            timer.Start(TimeSpan.MaxValue);
        }
    }
}