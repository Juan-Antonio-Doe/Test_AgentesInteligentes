using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actions : MonoBehaviour {

    public void MoveForward() {
        transform.position += transform.forward;
    }

    public void RotateRight() {
        transform.Rotate(0, 90, 0);
    }

    public void RotateLeft() {
        transform.Rotate(0, -90, 0);
    }

    public void RotateFull() {
        transform.Rotate(0, 180, 0);
    }
}
