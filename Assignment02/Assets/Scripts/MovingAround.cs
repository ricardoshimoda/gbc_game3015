using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingAround : MonoBehaviour
{
    public float angularSpeed = 100;
    private float startingAngle = 0;

    private float radius = 4;
    // Start is called before the first frame update
    void Start()
    {
        var posComp = this.gameObject.transform.localPosition;
        if(posComp.x != 0) {
            if(posComp.x < 0) startingAngle = 180;
            else startingAngle = 0;
        }
        else {
            if(posComp.z < 0) startingAngle = 270;
            else startingAngle = 90;
        }
    }

    // Update is called once per frame
    void Update()
    {
        startingAngle += angularSpeed * Time.deltaTime;
        if (startingAngle > 360){
            startingAngle -= 360;
        }
        var radAngle = startingAngle * Mathf.PI / 180;
        // change the local position so that things spin around the local center
        this.gameObject.transform.localPosition = new Vector3(
            radius * Mathf.Cos(radAngle),
            0,
            radius * Mathf.Sin(radAngle)
        ); 
    }
}
