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
        bool _right = Physics.CheckSphere(transform.position + transform.right * sideOffset, boundsSize, obstacleLayer);
        bool _left = Physics.CheckSphere(transform.position - transform.right * sideOffset, boundsSize, obstacleLayer);

        if (_right) {
            transform.eulerAngles -= new Vector3(0, 90, 0); // Rotate to the left
        } else if (_left) {
            transform.eulerAngles += new Vector3(0, 90, 0); // Rotate to the right
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * frontRayLength);
        Gizmos.DrawWireSphere(transform.position + transform.right * sideOffset, boundsSize);
    }
}
