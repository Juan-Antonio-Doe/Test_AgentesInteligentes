using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AB_RunAway : StateMachineBehaviour {

    private NPC_ASM npc;
    private NavMeshAgent agent;

    // Mitad de la altura del agente
    private float agentDoubledHeight;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (npc == null) {
            npc = animator.GetComponent<NPC_ASM>();
            agent = npc.agent;
            agentDoubledHeight = npc.agent.height * 2f;
        }

        agent.speed *= 1.5f;
        RunAway();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if ((agent.destination - npc.transform.position).sqrMagnitude <= (agent.stoppingDistance * agent.stoppingDistance) + 0.05f) {
            if (npc.Player != null) {
                // Huye del jugador
                RunAway();
            }
            else {
                animator.SetBool("isRunning", false);
            }
        }

        npc.DetectPlayer();
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        agent.ResetPath();
        agent.speed /= 1.5f;
    }

    // Huye del jugador
    void RunAway() {
        Vector3 _dirToPlayer = npc.transform.position - npc.Player.position;
        _dirToPlayer.y = 0; // Evitamos que no encuentre el punto de huida por estar en una posición más alta o más baja que el jugador

        int _rotation = Random.Range(-45, 46);

        _dirToPlayer = Quaternion.Euler(0, _rotation, 0) * _dirToPlayer.normalized;   // Rota el vector _dirToPlayer

        if (NavMesh.SamplePosition(npc.transform.position + (_dirToPlayer * npc.RunAwayDistance), 
            out NavMeshHit _hit, agentDoubledHeight, NavMesh.AllAreas)) {
            agent.SetDestination(_hit.position);
        }
        else {
            RunAway();
        }
    }
}
