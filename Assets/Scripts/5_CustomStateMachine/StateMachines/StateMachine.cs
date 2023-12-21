using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class StateMachine : MonoBehaviour {
	
	protected State currentState;

	public void ChangeState(State _newState) {
        /*
         * Salir del estado actual y entrar en el nuevo. 
         * "?" significa que si el estado actual no es nulo, ejecuta OnExit() (sustituye a un if)
         */
        currentState?.OnExit();

        currentState = _newState;
        currentState.OnEnter();
    }

    protected virtual void Update() {
        currentState?.OnUpdate();
    }

    private void OnDrawGizmos() {
        currentState?.OnDrawGizmos();

#if UNITY_EDITOR
        GUI.color = Color.blue;
        Handles.Label(transform.position + Vector3.up * 2, $"{gameObject.name} | Current state: {currentState}");

#endif
    }
}
