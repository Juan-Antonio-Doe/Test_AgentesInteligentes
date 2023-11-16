using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AB_TakeCover : StateMachineBehaviour {

    private NPC_ASM npc;
    private NavMeshAgent agent;

    // Mitad de la altura del agente
    private float agentHalfHeight;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (npc == null) {
            npc = animator.GetComponent<NPC_ASM>();
            agent = npc.agent;
            agentHalfHeight = npc.agent.height / 2f;
        }

        TakeCover();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        agent.ResetPath();
    }

    void TakeCover() {
        Collider[] _covers = Physics.OverlapSphere(npc.transform.position, npc.TakeCoverDistance, npc.CoverLayer);

        for (int i = 0; i < _covers.Length; i++) {
            if (_covers[i].bounds.size.y < agent.height) {
                continue;
            }

            Vector3 _coverPos = _covers[i].transform.position;
            Vector3 _direction = npc.Player.transform.position - npc.transform.position;
            _direction.y = 0;

            // Buscar primero que la posición este dentro del NavMesh
            if (NavMesh.SamplePosition(_coverPos, out NavMeshHit _hit , agentHalfHeight, NavMesh.AllAreas)) {
                // Bucar el borde del NavMesh lo más cercano posible a la posición del objeto de cobertura
                if (NavMesh.FindClosestEdge(_hit.position, out NavMeshHit _edge, NavMesh.AllAreas)) {
                    /*
                     * Calculamos el producto escalar entre la normal del borde con respecto a la dirección del jugador.
                     * Cuando ese valor es > 0, ambos vectores estan uno en frente del otro.
                     * Cuando ese valor es < 0, los vectores miran en direcciones opuestas.
                     */
                    float _dot = Vector3.Dot(_edge.normal, _direction.normalized);

                    if (_dot <= npc.CoverFactor) {  // Si el producto escalar es <= que el factor, es una cobertura válida
                        agent.SetDestination(_edge.position);
                        break;
                    }
                    else { // Cuando el producto escalar > que el factor, hay que buscar otra cobertura
                        /* Comprueba si al lado contrario de la cobertura se podría esconder
                         * Calcula la dirección del punto de cobertura encontrado con respecto a la posición del jugador
                         */
                        _direction = _edge.position - npc.Player.transform.position;

                        /*
                         * Para buscar la posición contraria a la que hemos encontrado en la primera busqueda de cobertura,
                         * desplazamos esa posición en la misma dirección en la que se encuentra ese punto respecto al jugador.
                         */
                        if (NavMesh.FindClosestEdge(_hit.position + (_direction.normalized * 2f), out _edge, NavMesh.AllAreas)) {
                            // Hay que realizar la misma comprobación que con el primer punto encontrado
                            _dot = Vector3.Dot(_edge.normal, _direction.normalized);

                            // Si el producto escalar es <= que el factor, es una cobertura válida
                            if (_dot <= npc.CoverFactor) {
                                agent.SetDestination(_edge.position);
                                break;
                            }
                        }
                    }

                }
            }
        }
    }
}
