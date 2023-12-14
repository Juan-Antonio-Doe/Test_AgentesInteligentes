using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCState : State {
	
	/*
	 * Todos los estados que usen los NPC heredaran de esta clase. Para que todos puedan acceder
	 * al controlador CustomNPC, lo añadimos al contructor de la clase.
	 */
	protected CustomNPC npc;

	public NPCState(CustomNPC npc) {
        this.npc = npc;
    }
}
