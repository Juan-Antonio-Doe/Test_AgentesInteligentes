using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IdleState : NPCState {

    public float timeInIdle = 3f;
    private float timer = 0f;

    public IdleState(CustomNPC npc) : base(npc) {

    }

    public override void OnEnter() {
        base.OnEnter();

        timer = timeInIdle;
    }

    public override void OnUpdate() {
        base.OnUpdate();

        timer -= Time.deltaTime;

        if (timer <= 0f) {
            npc.ChangeState(new WanderState(npc));
        }
    }
}
