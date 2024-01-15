using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBomber : MonoBehaviour {

    [field: Header("Enemy properties")]
    [field: SerializeField] private MeshRenderer render { get; set; }
    [field: SerializeField] private float speed { get; set; } = 2f;

    [field: Header("Front")]
    [field: SerializeField] private float frontRayLength { get; set; } = 0.5f;
    [field: SerializeField] private LayerMask obstacleLayer { get; set; } = 1 << 6; // 6 = Obstacle

    [field: Header("Sides")]
    //[field: SerializeField] private float sideSpehereRadiusOffset { get; set; } = 0f;
    [field: SerializeField] private float sideOffset { get; set; } = 1f;
    private float boundsSize { get; set; }
    //private float sideSphereRadius { get; set; }
    [field: SerializeField, Range(0, 100)] private int rightChance { get; set; } = 33;
    [field: SerializeField, Range(0, 100)] private int leftChance { get; set; } = 33;
    [field: SerializeField] private float turnCooldown { get; set; } = 0.5f;
    private bool canTurn { get; set; } = true;
	
    void Start() {
        boundsSize = render.bounds.extents.x;
        //sideSphereRadius = boundsSize + sideSpehereRadiusOffset;
    }

    void Update() {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    void FixedUpdate() {
        DetectFront();
        DetectSides();
    }

    void DetectFront() {
        if (Physics.Raycast(transform.position, transform.forward, frontRayLength, obstacleLayer)) {
            transform.eulerAngles -= new Vector3(0, 90, 0); // Rotate to the left
        }
    }

    void DetectSides() {
        if (!canTurn)
            return;

        bool _right = Physics.CheckSphere(transform.position + transform.right * sideOffset, boundsSize, obstacleLayer);
        bool _left = Physics.CheckSphere(transform.position - transform.right * sideOffset, boundsSize, obstacleLayer);

        int _random = Random.Range(1, 101);

        if (!_right && !_left) {
            canTurn = false;
            if (_random <= rightChance) {
                transform.eulerAngles += new Vector3(0, 90, 0); // Rotate to the right
            } else if (_random <= rightChance + leftChance) {
                transform.eulerAngles -= new Vector3(0, 90, 0); // Rotate to the left
            }
            StartCoroutine(TurnCooldownCo());
        }
        else if (!_right) {
            TurnRightSimple(_random);
            //TurnRightProfesor();
        } else if (!_left) {
            TurnLeftSimple(_random);
            //TurnLeftProfesor();
        }
    }

    void TurnRightProfesor() {
        canTurn = false;
        int _random = Random.Range(1, 101);
        if (_random <= rightChance) {
            transform.eulerAngles += new Vector3(0, 90, 0); // Rotate to the right
        }
        StartCoroutine(TurnCooldownCo());
    }

    void TurnLeftProfesor() {
        canTurn = false;
        int _random = Random.Range(1, 101);
        if (_random <= leftChance) {
            transform.eulerAngles -= new Vector3(0, 90, 0); // Rotate to the left
        }
        StartCoroutine(TurnCooldownCo());
    }

    void TurnRightSimple(int _random) {
        canTurn = false;
        if (_random <= rightChance) {
            transform.eulerAngles += new Vector3(0, 90, 0); // Rotate to the right
        }
        StartCoroutine(TurnCooldownCo());
    }

    void TurnLeftSimple(int _random) {
        canTurn = false;
        if (_random <= rightChance + leftChance) {
            transform.eulerAngles -= new Vector3(0, 90, 0); // Rotate to the left
        }
        StartCoroutine(TurnCooldownCo());
    }

    IEnumerator TurnCooldownCo() {
        yield return new WaitForSeconds(turnCooldown);
        canTurn = true;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * frontRayLength);
        Gizmos.DrawWireSphere(transform.position + transform.right * sideOffset, boundsSize);
        Gizmos.DrawWireSphere(transform.position - transform.right * sideOffset, boundsSize);
    }
}
