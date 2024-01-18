using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteerVehicle : MonoBehaviour {

    [field: Header("Sensors")]
    [field: SerializeField] private Transform rightOrigin { get; set; }
    [field: SerializeField] private Transform rightRotOrigin { get; set; }
    [field: SerializeField] private Transform leftOrigin { get; set; }
    [field: SerializeField] private Transform leftRotOrigin { get; set; }
    [field: SerializeField] private float sensorLength { get; set; } = 5f;
    [field: SerializeField] private float rotSensorLengthOffset { get; set; } = 0.06f;
    [field: SerializeField] private LayerMask obstacleLayer { get; set; } = 1 << 6;     // 6 = Obstacle

    [field: Header("Config")]
    [field: SerializeField] private Rigidbody rb { get; set; }
    [field: SerializeField] private float speed { get; set; } = 0.1f;
    [field: SerializeField] private float rotationAmount { get; set; } = 1f;

    private Vector3 desiredDirection;   // Direction to move towards
    private float steeredRot;           // Rotation needed to avoid obstacle


	
    void Start() {
        
    }

    void Update() {
        CastSensors();
        transform.eulerAngles = new Vector3(0f, steeredRot, 0f);
    }

    void FixedUpdate() {
        //rb.position += transform.forward * speed;
        rb.MovePosition(transform.position + transform.forward * speed);
    }

    void CastSensors() {
        bool _right = Physics.Raycast(rightOrigin.position, rightOrigin.forward, sensorLength, obstacleLayer);
        bool _rightRot = Physics.Raycast(rightRotOrigin.position, rightRotOrigin.forward, sensorLength + rotSensorLengthOffset, obstacleLayer);
        bool _left = Physics.Raycast(leftOrigin.position, leftOrigin.forward, sensorLength, obstacleLayer);
        bool _leftRot = Physics.Raycast(leftRotOrigin.position, leftRotOrigin.forward, sensorLength + rotSensorLengthOffset, obstacleLayer);

        // If the only available sensor is the right one with rotation -> rotate right
        if (!_rightRot && _right && _left && _leftRot) {
            steeredRot += rotationAmount * Time.deltaTime;
            return;
        }

        // If the only available sensor is the left one with rotation -> rotate left
        if (!_leftRot && _left && _right && _rightRot) {
            steeredRot -= rotationAmount * Time.deltaTime;
            return;
        }
    }

    void OnDrawGizmos() {
        // Right sensor
        Gizmos.color = Color.red;
        Gizmos.DrawRay(rightOrigin.position, rightOrigin.forward * sensorLength);
        Gizmos.DrawRay(rightRotOrigin.position, rightRotOrigin.forward * (sensorLength + rotSensorLengthOffset));

        // Left sensor
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(leftOrigin.position, leftOrigin.forward * sensorLength);
        Gizmos.DrawRay(leftRotOrigin.position, leftRotOrigin.forward * (sensorLength + rotSensorLengthOffset));
    }
}
