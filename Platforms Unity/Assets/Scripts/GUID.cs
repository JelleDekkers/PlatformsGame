using System;

[Serializable]
public class MyGUID {

    public int ID { get; private set; }

    public MyGUID(int id) {
        ID = id;
    }
}
