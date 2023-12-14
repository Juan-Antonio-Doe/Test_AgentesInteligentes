using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public abstract class State {

    /*
     * Para que todos los scripts que sean estados puedan acceder a su StateMachine,
     * lo añadimos al contructor de la clase.
     */
    /*protected StateMachine stateMachine;    

    // Constructor
    public State(StateMachine stateMachine) {
        this.stateMachine = stateMachine;
    }*/

    public virtual void OnEnter() {

    }

    public virtual void OnUpdate() {

    }

    public virtual void OnExit() {

    }
}
