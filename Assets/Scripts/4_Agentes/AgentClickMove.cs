using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentClickMove : MonoBehaviour {

    [field: SerializeField] private NavMeshAgent agent { get; set; }
    [field: SerializeField] private LayerMask clickLayer { get; set; }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(_ray, out RaycastHit _hit, 1000f, clickLayer)) {
                agent.SetDestination(_hit.point);
            }
        }
    }
}
