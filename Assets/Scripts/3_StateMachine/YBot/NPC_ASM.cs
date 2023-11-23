using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC_ASM : MonoBehaviour {

    [field: Header("General")]
    [field: SerializeField] public NavMeshAgent agent { get; private set; }

    [SerializeField] private float idleTime = 3f;
    public float IdleTime { get { return idleTime; } }
    
    [SerializeField] private Transform player;
    public Transform Player { get { return player; } }

    [SerializeField] private float detectRange = 5f;

    [SerializeField] private LayerMask detectLayer;

    // Campo de visión del NPC
    [SerializeField] private float VisionAngle = 90f;
    [SerializeField] private Transform angleHelperR, angleHelperL;

    [Header("Run Away")]
    [SerializeField] private float runAwayDistance = 7f;
    public float RunAwayDistance { get { return runAwayDistance; } }

    [SerializeField] private Transform checkFrontOrigin;
    public Transform CheckFrontOrigin { get { return checkFrontOrigin; } }
    [SerializeField] private float checkFrontRadius = 1f;
    public float CheckFrontRadius { get { return checkFrontRadius; } }
    [SerializeField] private float checkFrontDistance = 3.5f;
    public float CheckFrontDistance { get { return checkFrontDistance; } }

    [Header("Take Cover")]
    [SerializeField] private float takeCoverDistance = 15f;
    public float TakeCoverDistance { get { return takeCoverDistance; } }

    [SerializeField] private LayerMask coverLayer;
    public LayerMask CoverLayer { get { return coverLayer; } }

    [SerializeField] private float coverFactor = -0.1f;
    public float CoverFactor { get { return coverFactor; } }

    [SerializeField] private float timeTakingCover = 5f;
    public float TimeTakingCover { get { return timeTakingCover; } }


    void Start() {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
    }

    void Update() {
        
    }

    public void DetectPlayer() {
        Collider[] _targets = Physics.OverlapSphere(transform.position, detectRange, detectLayer);

        if (_targets.Length > 0) {
            for (int i = 0; i < _targets.Length; i++) {
                if (_targets[i].CompareTag("Player")) {

                    Vector3 _dirToPlayer = _targets[i].transform.position - transform.position;

                    if (Vector3.Angle(transform.forward, _dirToPlayer) <= VisionAngle / 2f) {
                        // Huye del jugador
                        player = _targets[i].transform;
                    }
                    
                    return;
                }
            }
        }
        else {
            player = null;
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, runAwayDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, takeCoverDistance);

        float auxAngle = VisionAngle / 2f;

        angleHelperR.localEulerAngles = new Vector3(0, auxAngle, 0);
        angleHelperL.localEulerAngles = new Vector3(0, -auxAngle, 0);
        Gizmos.DrawRay(angleHelperR.position, angleHelperR.forward * detectRange);
        Gizmos.DrawRay(angleHelperL.position, angleHelperL.forward * detectRange);

        // Dibujar el SphereCast que comprueba obstaculos frontales
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(checkFrontOrigin.position, checkFrontOrigin.forward * checkFrontDistance);
        Gizmos.DrawWireSphere(checkFrontOrigin.position + (checkFrontOrigin.forward * checkFrontDistance), checkFrontRadius);
    }
}
