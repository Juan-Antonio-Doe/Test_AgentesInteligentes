using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AB_Wander : StateMachineBehaviour {

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

        Wander();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if ((agent.destination - npc.transform.position).sqrMagnitude <= (agent.stoppingDistance * agent.stoppingDistance) + 0.05f) {
            animator.SetBool("isWandering", false);
        }

        npc.DetectPlayer();

        if (npc.Player != null) {
            animator.SetBool("isRunning", true);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.SetBool("isWandering", false);
        agent.ResetPath();
    }

    void Wander() {
        int[] _distValues = new int[6] { -6, -5, -4, 4, 5, 6 };

        Vector3 _randomDir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));

        int _distance = _distValues[Random.Range(0, _distValues.Length)];

        /* Comprueba que la posición proporcionada esté dentro del NavMesh. Le pasamos la posición calculada al azar más la posición del agente
         * para que sea con respecto a donde se encuentra. El resto de valores puede dejarse siempre así.
         */
        if (NavMesh.SamplePosition(npc.transform.position + (_randomDir * _distance), out NavMeshHit _hit, agentDoubledHeight, NavMesh.AllAreas)) {
            agent.SetDestination(_hit.position);
        }
        else {
            Wander();
        }

    }
}
