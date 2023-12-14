using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WanderState : NPCState {

    private NavMeshAgent agent;
    private float agentDoubledHeight;

    public WanderState(CustomNPC npc) : base(npc) {
        agent = this.npc.Agent;
        agentDoubledHeight = this.npc.Agent.height * 2f;
    }

    public override void OnEnter() {
        base.OnEnter();

        Wander();
    }

    void Wander() {
        int[] _distValues = new int[6] { -6, -5, -4, 4, 5, 6 };

        Vector3 _randomDir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));

        int _distance = _distValues[Random.Range(0, _distValues.Length)];

        /* Comprueba que la posici�n proporcionada est� dentro del NavMesh. Le pasamos la posici�n calculada al azar m�s la posici�n del agente
         * para que sea con respecto a donde se encuentra. El resto de valores puede dejarse siempre as�.
         */
        if (NavMesh.SamplePosition(npc.transform.position + (_randomDir * _distance), out NavMeshHit _hit, agentDoubledHeight, NavMesh.AllAreas)) {
            agent.SetDestination(_hit.position);
        }
        else {
            Wander();
        }
    }
}
