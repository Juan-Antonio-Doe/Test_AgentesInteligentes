using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AB_Idle : StateMachineBehaviour {

    private NPC_ASM npc;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (npc == null) {
            npc = animator.GetComponent<NPC_ASM>();
        }
        
        npc.StartCoroutine(BackToWanderCo(animator));
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        npc.DetectPlayer();

        if (npc.Player != null) {
            //animator.SetBool("isRunning", true);
            //animator.SetBool("isTakingCover", true);
            RunOrTakeCover(animator);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    IEnumerator BackToWanderCo(Animator _anim) {
        yield return new WaitForSeconds(npc.IdleTime);
        _anim.SetBool("isWandering", true);

    }

    void RunOrTakeCover(Animator _anim) {
        // Si el NPC ya está huyendo o escondiéndose, no se ejecuta la corrutina
        npc.StopAllCoroutines();

        // Para calcular un porcentaje exacto, se calcula un número aleatorio entre 1 y 100
        // Si el número es menor o igual al porcentaje, se ejecuta la acción
        int _random = Random.Range(1, 101); /// Otra vez todos preguntando lo mismo sobre el 1 extra...

        // En función del nº aleatorio, se esconde o huye
        if (_random <= npc.ChanceToTakeCover) {
            _anim.SetBool("isTakingCover", true);
        }
        else {
            _anim.SetBool("isRunning", true);
        }
    }
}
