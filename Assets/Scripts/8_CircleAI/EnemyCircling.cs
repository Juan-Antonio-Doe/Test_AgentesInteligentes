using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCircling : MonoBehaviour {

    [field: Header("Enemy properties")]
    [field: SerializeField] private NavMeshAgent agent { get; set; }
    [field: SerializeField] private float detectRange { get; set; } = 3f;
    [field: SerializeField] private LayerMask targetLayer { get; set; } = 1 << 7;
    [field: SerializeField] private bool maintainDistWhenAlone { get; set; } = true;
    public bool MaintainDistance { get { return maintainDistWhenAlone; } }

    private CirclingTarget target { get; set; }
	
    void Start() {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
    }

    void Update() {
        if (target != null) {
            if (Input.GetKeyDown(KeyCode.F1)) {
                agent.SetDestination(target.CalculateRandomPositionAround(transform));
            }
            return;

            Vector3 calculatedPos = target.CalculatePositionAround(transform, maintainDistWhenAlone);

            if (calculatedPos == Vector3Int.one * 9999) {
                agent.stoppingDistance = target.Radius;
                agent.SetDestination(target.transform.position);
            }
            else {
                agent.stoppingDistance = 0f;
                agent.SetDestination(calculatedPos);
            }
        }

        Collider[] _targets = Physics.OverlapSphere(transform.position, detectRange, targetLayer);

        if (_targets.Length == 0) {
            if (target != null) {
                target.RemoveFromEnemyList(transform);
                target = null;
            }
            return;
        }

        for (int i = 0; i < _targets.Length; i++) {
            target = _targets[i].GetComponent<CirclingTarget>();
            if (target != null) {
                target.AddToEnemyList(transform);
                //agent.SetDestination(target.CalculatePositionAround(transform, maintainDistance));
                break;
            }
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }
}
