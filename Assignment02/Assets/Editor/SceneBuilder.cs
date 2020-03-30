using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class SceneBuilder : MonoBehaviour
{
    [MenuItem("Scene Loader/LoadScene", false, 1)]
    private static void LoadScene()
    {
        var path = EditorUtility.OpenFilePanel(
            "Select the Scene File",
            Application.dataPath,
            "json"
        );
        if(string.IsNullOrWhiteSpace(path)) {
            EditorUtility.DisplayDialog("Load Scene cancelled",
                "Loading scene from json cancelled", "Ok");
            return;
        }

        string sceneDescription = System.IO.File.ReadAllText(path, System.Text.Encoding.UTF8);
        SceneDescriptor desc = new SceneDescriptor();
        try{
            desc = JsonUtility.FromJson<SceneDescriptor>(sceneDescription);
        } catch (Exception ex) {
            Debug.Log("Error: " + ex.ToString());
            EditorUtility.DisplayDialog("Load Scene error",
                "Error: " + ex.ToString(), "Ok");
            return;
        }

        Scene scene = SceneManager.GetActiveScene();
        List<GameObject> rootObjects = new List<GameObject>();
        scene.GetRootGameObjects(rootObjects);
        if (rootObjects.Count > 0) {
            if (!EditorUtility.DisplayDialog("Confirm Scene Reset",
                "Do you really want to do this? All Scene objects will be erased and replaced", "Ok","Cancel")){
                EditorUtility.DisplayDialog("Load Scene cancelled",
                    "Loading scene from json cancelled", "Ok");
                return;
            }
        }

        scene.GetRootGameObjects(rootObjects);
        foreach(GameObject whoa in rootObjects){
            DestroyImmediate(whoa);
        }

        foreach(Actor ac in desc.RootActors ){
            SceneInst.InstanceActor(ac, null);
        }
        // Rebuild the scene
        // Says it's okay
        // Here we'll use FromJsonOverwrite 
    }
    [MenuItem("Scene Loader/SaveScene", false, 2)]
    private static void SaveScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        var path = EditorUtility.SaveFilePanel(
            "Save scene as json",
            Application.dataPath,
            scene.name + ".json",
            "json"
        );
        if(string.IsNullOrWhiteSpace(path)){
            EditorUtility.DisplayDialog("Save Scene cancelled",
                "Saving scene to json cancelled", "Ok");
            return;
        }
        SceneDescriptor desc = new SceneDescriptor();

        List<GameObject> rootObjects = new List<GameObject>();
        scene.GetRootGameObjects(rootObjects);
        foreach(GameObject whoa in rootObjects){
            Actor currentAct = SceneReader.GetActor(whoa);
            desc.RootActors.Add(currentAct);
        }

        string sceneAsAWhole = JsonUtility.ToJson(desc);
        System.IO.File.WriteAllText(path, sceneAsAWhole, System.Text.Encoding.UTF8);

        EditorUtility.DisplayDialog("Success",
                "Finished saving scene to json file", "Ok");

    }

}
