using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class SceneInst {
    public static void InstanceActor(Actor current, Transform parent){
        Scene scene = SceneManager.GetActiveScene();

        GameObject currentGO = new GameObject(current.Name);
        currentGO.transform.parent = parent;
        currentGO.tag = current.Tag;
        currentGO.layer = current.Layer;
        currentGO.transform.localPosition = new Vector3(current.Transform.Position.x, current.Transform.Position.y, current.Transform.Position.z);
        currentGO.transform.localScale =  new Vector3(current.Transform.Scale.x, current.Transform.Scale.y, current.Transform.Scale.z);
        currentGO.transform.localEulerAngles = new Vector3(current.Transform.Rotation.x, current.Transform.Rotation.y, current.Transform.Rotation.z);
        foreach(ActorComponent comp in current.Components){
            Component added = currentGO.AddComponentExt(comp.Type, comp.Asm);
            if(added != null)
            {
                EditorJsonUtility.FromJsonOverwrite(comp.SerializedValues, added);
            }
        }
        foreach(Actor act in current.Children){
            InstanceActor(act, currentGO.transform);
        }
    }
}