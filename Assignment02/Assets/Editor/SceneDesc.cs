using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class SceneDescriptor
{
    public List<Actor> RootActors = new List<Actor>();
}

[Serializable]
public class Actor{
    public string Name;
    public bool Enabled;
    public string Tag;
    public int Layer;
    public List<Actor> Children = new List<Actor>();
    public ActorTransform Transform;
    public List<ActorComponent> Components = new List<ActorComponent>();
}

[Serializable]
public class ActorComponent {
    public string Name;
    public string Type;
    public string Asm;
    public string SerializedValues;
}

[Serializable]
public class ActorTransform{
    public Triplet Position;
    public Triplet Scale;
    public Triplet Rotation;
}

[Serializable]
public class Triplet{
    public Triplet(){}
    public Triplet(float _x, float _y, float _z){
        x = _x;
        y = _y;
        z = _z;
    }
    public float x,y,z;
} 
