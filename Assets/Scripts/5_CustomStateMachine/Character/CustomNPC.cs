using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CustomNPC : StateMachine {
	
	[field: SerializeField] private NavMeshAgent agent { get; set; }
    public NavMeshAgent Agent { get => agent; }
	
    void Start() {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        currentState = new IdleState(this) { timeInIdle = 3f };
        currentState.OnEnter();
    }

    protected override void Update() {
        base.Update();
    }
}
