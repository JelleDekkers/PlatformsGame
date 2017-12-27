using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

enum TimerState {
    Stopped,
    CountingUp,
    CountingDown
}

public class Timer : LogicObject {

    //[SerializeField]
    //private float interval = 1;
    //[SerializeField]
    //private float max = 10;

    //public UnityEvent OnReachedZero;

    protected override string IconTextireName { get { return "Timer_Icon"; } }

    //public void Toggle() {

    //}

    //public void Pause() {

    //}
}
