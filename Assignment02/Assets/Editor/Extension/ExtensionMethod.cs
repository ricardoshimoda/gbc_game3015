using UnityEngine;

using System;
using System.Reflection;

/*
-- Extension taken from [Programmer]'s answer on stackoverflow:
https://stackoverflow.com/questions/42347327/unity-how-to-dynamically-attach-an-unknown-script-to-a-gameobject-custom-edito

Usage:

gameObject.AddComponentExt("YourScriptOrComponentName");
It is important to understand how I did it so that you can add support for new components in any future Unity updates.

For any script created by users:

1.Find out what needs to be in the ??? in the Assembly.Load function.

Assembly asm = Assembly.Load("???");
You can do that by putting this in your script:

Debug.Log("Info: " + this.GetType().Assembly);
I got: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null

We should now replace ??? with that.

Assembly asm = Assembly.Load("Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
2.Find out what needs to be in the ??? in the asm.GetType function.

Assembly asm = Assembly.Load("Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
Type type = asm.GetType(???); 
In this case, it is simply the name of the script you want to add to the GameObject.

Let's say that your script name is NathanScript:

Assembly asm = Assembly.Load("Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
Type type = asm.GetType("NathanScript"); 
gameObject.AddComponent(type);
For Unity built in scripts/components scripts not created by users:

Example of this is the Rigidbody, Linerenderer, Image components. Just any component not created by the user.

1.Find out what needs to be in the ??? in the Assembly.Load function.

Assembly asm = Assembly.Load("???");
You can do that by putting this in your script:

ParticleSystem pt = gameObject.AddComponent<ParticleSystem>();
Debug.Log("Info11: " + pt.GetType().Assembly);
I got: UnityEngine, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null

We should now replace ??? with that.

Assembly asm = Assembly.Load("UnityEngine, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
2.Find out what needs to be in the ??? in the asm.GetType function.

Assembly asm = Assembly.Load("UnityEngine, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
Type type = asm.GetType(???); 
You can do that by putting this in your script:

ParticleSystem pt = gameObject.AddComponent<ParticleSystem>();
Debug.Log("Info: " + pt.GetType());
I got: UnityEngine.ParticleSystem

Remember that ParticleSystem is used as an example here. So the final string that will go to the asm.GetType function will be calculated like this:

string typeString = "UnityEngine." + componentName;
Let's say that the Component you want to add is LineRenderer:

Assembly asm = Assembly.Load("UnityEngine, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
string typeString = "UnityEngine." + "LineRenderer";
Type type = asm.GetType(typeString); 
gameObject.AddComponent(type);
Putting it together in an extension method:

As you can see, adding scripts you created and the script/components that comes with Unity requires totally different process. You can fix this by checking if the type if null. If the type is null, perform the other step. If the other step is null too then the script simply does not exit.

*/

public static class ExtensionMethod
{
    public static Component AddComponentExt(this GameObject obj, string scriptName, string assembly)
    {
        Component cmpnt = null;
        for (int i = 0; i < 10; i++)
        {
            //If call is null, make another call
            cmpnt = _AddComponentExt(obj, scriptName, i, assembly);

            //Exit if we are successful
            if (cmpnt != null)
            {
                break;
            }
        }


        //If still null then let user know an exception
        if (cmpnt == null)
        {
            Debug.LogError("Failed to Add Component");
            return null;
        }
        return cmpnt;
    }

    private static Component _AddComponentExt(GameObject obj, string className, int trials, string assembly)
    {
        //Any script created by user(you)
        const string userMadeScript = "Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";

        //Any script/component that comes with Unity such as "Rigidbody"
        const string builtInScript = "UnityEngine, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";

        //Any script/component that comes with Unity such as "Image"
        const string builtInScriptUI = "UnityEngine.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";

        //Any script/component that comes with Unity such as "Networking"
        const string builtInScriptNetwork = "UnityEngine.Networking, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";

        //Any script/component that comes with Unity such as "AnalyticsTracker"
        const string builtInScriptAnalytics = "UnityEngine.Analytics, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";

        //Any script/component that comes with Unity such as "AnalyticsTracker"
        const string builtInScriptHoloLens = "UnityEngine.HoloLens, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";

        Assembly asm = Assembly.Load(assembly);
        /*
        if(className.StartsWith("UnityEngine.VR.WSA.")){
            asm = Assembly.Load(builtInScriptHoloLens);
        } else if(className.StartsWith("UnityEngine.AI.")){
            asm = Assembly.Load(builtInScript);
        } else if(className.StartsWith("UnityEngine.Audio.")){
            asm = Assembly.Load(builtInScriptHoloLens);
        } else if(className.StartsWith("UnityEngine.EventSystems.")){
            asm = Assembly.Load(builtInScriptUI);
        } else if(className.StartsWith("UnityEngine.Analytics.")){
            asm = Assembly.Load(builtInScriptAnalytics);
        } else if(className.StartsWith("UnityEngine.UI.")){
            asm = Assembly.Load(builtInScriptUI);
        } else if(className.StartsWith("UnityEngine.Video.")){
            asm = Assembly.Load(builtInScript);
        } else if(className.StartsWith("UnityEngine.Networking.")){
            asm = Assembly.Load(builtInScriptNetwork);
        } else if(className.StartsWith("UnityEngine.")){
            asm = Assembly.Load(builtInScript);
        } else {
            asm = Assembly.Load(userMadeScript);
        } */

        //Return if Assembly is null
        if (asm == null)
        {
            Debug.Log("Could not find assembly");
            return null;
        }

        //Get type then return if it is null
        Type type = asm.GetType(className);
        if (type == null){
            Debug.Log("Could not resolve type");
            return null;
        }

        //Finally Add component since nothing is null
        Component cmpnt = obj.AddComponent(type);
        return cmpnt;
    }
}