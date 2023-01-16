using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerScript
{
    private float _countdownTime;

    public float CountdownTime { get => _countdownTime; set { _countdownTime = Mathf.Abs(value); } }
    public float CurrentTime { get; set; }

    public TimerScript(){ }

    public TimerScript(float countDownTime)
    {
        this.CountdownTime = countDownTime;
        CurrentTime = CountdownTime;
    }

    public void Reset()
    {
        CurrentTime = CountdownTime;
    }

    public bool Tick(float time)
    {
        CurrentTime -= time;
        if (CurrentTime <= 0)
        {
            Reset();
            return true;
        }
        else return false;
    }
}
