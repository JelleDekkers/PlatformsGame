using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputPC : IInputSystem {

    public float GetAxisRawHorizontal() {
        return Input.GetAxisRaw("Horizontal");
    }

    public float GetAxisRawVertical() {
        return Input.GetAxisRaw("Vertical");
    }
}
