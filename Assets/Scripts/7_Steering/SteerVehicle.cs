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
    [field: SerializeField] private float sidesSensorLength { get; set; } = 3f;
    [field: SerializeField] private LayerMask obstacleLayer { get; set; } = 1 << 6;     // 6 = Obstacle
    [field: SerializeField] private Transform target { get; set; }
    [field: SerializeField] private float stopDistance { get; set; } = 0.05f;

    [field: Header("Config")]
    [field: SerializeField] private Rigidbody rb { get; set; }
    [field: SerializeField] private float speed { get; set; } = 0.1f;
    [field: SerializeField] private float rotationAmount { get; set; } = 1f;
    [field: SerializeField, Tooltip("N� por el que se dividir� la velocidad de rotaci�n" +
        " cuando tenga que girar mas despacio.")] private float rotationDivider { get; set; } = 2f;
    [field: SerializeField] private float timeToResetRot { get; set; } = 0.5f;
    [field: SerializeField] private float rotationSmoothing { get; set; } = 5f;

    private float desiredRot { get; set; }      // Rotacion original que tiene que mantener para ir hacia el objetivo
    private Vector3 desiredDirection;       // Direction to move towards
    private float steeredRot { get; set; }  // Rotation needed to avoid obstacle
    private float rotAmountDivided { get; set; }
    private float steerResetTimer { get; set; }

	
    void Start() {
        rotAmountDivided = rotationAmount / rotationDivider;
    }

    void Update() {
        CalculateDirection();
        CastSensors();
        if (steeredRot != 0)
            transform.eulerAngles = new Vector3(0f, steeredRot + desiredRot, 0f);

    }

    void FixedUpdate() {
        //rb.position += transform.forward * speed;
        
        if (target != null) {
            float distanceSquared = (target.position - transform.position).sqrMagnitude;
            if (distanceSquared < stopDistance * stopDistance) {
                rb.velocity = Vector3.zero;
                return;
            }
        }

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
            steerResetTimer = timeToResetRot;
            return;
        }

        // If the only available sensor is the left one with rotation -> rotate left
        if (!_leftRot && _left && _right && _rightRot) {
            steeredRot -= rotationAmount * Time.deltaTime;
            steerResetTimer = timeToResetRot;
            return;
        }

        // If the right sensor detects an obstacle -> rotate left
        if (_rightRot) {
            steeredRot -= rotationAmount * Time.deltaTime;
            steerResetTimer = timeToResetRot;
        }
        // If the right sensor doesn't detect an obstacle and the right with rotation sensor does -> rotate left
        else if (_rightRot) {
            steeredRot -= rotAmountDivided * Time.deltaTime;
            steerResetTimer = timeToResetRot;
        }
        // If the previous sensors don't detect an obstacle and the left sensor does -> rotate right
        else if (_leftRot) {
            steeredRot += rotationAmount * Time.deltaTime;
            steerResetTimer = timeToResetRot;
        }
        // If the previous sensors don't detect an obstacle and the left with rotation sensor does -> rotate right
        else if (_leftRot) {
            steeredRot += rotAmountDivided * Time.deltaTime;
            steerResetTimer = timeToResetRot;
        }
        else {

            if (steeredRot != 0) {
                bool _rightSide = Physics.Raycast(transform.position, transform.right, sidesSensorLength, obstacleLayer);
                bool _leftSide = Physics.Raycast(transform.position, -transform.right, sidesSensorLength, obstacleLayer);

                if (_rightSide || _leftSide) {
                    steerResetTimer = timeToResetRot;
                }
            }

            steerResetTimer -= Time.deltaTime;
            if (steerResetTimer <= 0f) {
                steeredRot = 0f;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, desiredRot, 0f), Time.deltaTime * rotationSmoothing);
            }
        }

    }

    void CalculateDirection() {
        if (target == null)
            return;

        desiredDirection = target.position - transform.position;
        desiredDirection.y = 0f;
        desiredDirection.Normalize();
        //desiredRot = Mathf.Atan2(desiredDirection.x, desiredDirection.z) * Mathf.Rad2Deg;
        desiredRot = Quaternion.LookRotation(desiredDirection).eulerAngles.y;
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

        // Sides sensors
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.right * sidesSensorLength);
        Gizmos.DrawRay(transform.position, -transform.right * sidesSensorLength);
    }
}
