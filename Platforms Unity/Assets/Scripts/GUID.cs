using System;
using UnityEngine;

[Serializable]
public class MyGUID {

    [SerializeField]
    private int id;
    public int ID {
        get { return id; } 
        private set {id = value; }
    }

    public MyGUID(int id) {
        ID = id;
    }
    
    public override string ToString() {
        return ID.ToString();
    }
}
