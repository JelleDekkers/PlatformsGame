using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMobile : IInputSystem {

    private static InputMobile instance;
    public static InputMobile Instance { get { return instance; } }

    [SerializeField]
    private float horizontalInput;
    [SerializeField]
    private float verticalInput;

    public InputMobile() {
        instance = this;
    }

    public float GetAxisRawHorizontal() {
        return horizontalInput;
    }

    public float GetAxisRawVertical() {
        return verticalInput;
    }

    public void AddInputValue(float x, float y) {
        Debug.Log("adding " + x + " " + y);
        horizontalInput += x;
        horizontalInput = Mathf.Clamp(horizontalInput, -1, 1);
        verticalInput += y;
        verticalInput = Mathf.Clamp(verticalInput, -1, 1);
    }
}
