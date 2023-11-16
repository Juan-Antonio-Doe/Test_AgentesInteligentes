using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensors : MonoBehaviour {


	[SerializeField, Tooltip("Longitud del sensor delantero.")] private float rayLength = 1.0f;
	[SerializeField, Tooltip("Capas físicas que serán obstaculos")] private LayerMask obstacleLayer;

	public bool FrontSensor() {
        return Physics.Raycast(transform.position, transform.forward, rayLength, obstacleLayer);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * rayLength);
    }
}
