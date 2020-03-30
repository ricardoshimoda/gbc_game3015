using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
public class SceneReader
{
    public static Actor GetActor(GameObject whoa)
    {
        Actor currentAct = new Actor();
        var realName = whoa.ToString();
        realName = realName.Substring(0, realName.IndexOf("(")-1);
        currentAct.Name = realName;
        currentAct.Enabled = whoa.activeSelf;
        currentAct.Layer = whoa.layer;
        currentAct.Tag = whoa.tag;
        var wtp = whoa.transform.localPosition;
        var wtr = whoa.transform.localEulerAngles;
        var wts = whoa.transform.localScale;
        currentAct.Transform = new ActorTransform(){
            Position = new Triplet(wtp.x, wtp.y, wtp.z),
            Rotation = new Triplet(wtr.x, wtr.y, wtr.z),
            Scale = new Triplet(wts.x, wts.y, wts.z)
        };
        /*
         * now for the other components
         */
        foreach(var comp in whoa.GetComponents<Component>()){
            ActorComponent ac = new ActorComponent();
            var compName = comp.ToString();
            ac.Name = compName.Substring(0, compName.IndexOf("(")-1);
            ac.Type =  compName.Substring(compName.IndexOf("(") + 1).Replace(")","");
            ac.Asm = comp.GetType().Assembly.ToString();
            if(ac.Type.IndexOf(".Transform") >=0 ){
                // Skipping transform because that's one that exists in every game object
                continue;
            }
            ac.SerializedValues = EditorJsonUtility.ToJson(comp);;
            currentAct.Components.Add(ac);
        }
        currentAct.Children = GetChildren(whoa);
        return currentAct;
    }

    public static List<Actor> GetChildren(GameObject parent)
    {
        List<Actor> finalList = new List<Actor>();
        var chCount = parent.transform.childCount;
        for(int i = 0; i < chCount; i ++){
            var childTransform = parent.transform.GetChild(i);
            var currentActor = GetActor(childTransform.gameObject);
            finalList.Add(currentActor);
        }
        return finalList;
    }
}