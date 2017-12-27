using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : Tile {

    private static Goal instance;
    public static Goal Instance {
        get {
            if (instance == null)
                instance = FindObjectOfType<Goal>();
            return instance;
        }
    }

    public override void Enter(Block block) {
        Debug.Log("goal reached");
        
        if(GameEvents.OnGameOver != null)
            GameEvents.OnGameOver();
    }
}
