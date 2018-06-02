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
        if (block != Player.Instance)
            return;
        
        if(GameEvents.OnLevelFinished != null)
            GameEvents.OnLevelFinished();
    }
}
