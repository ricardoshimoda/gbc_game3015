using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsRBody : MonoBehaviour
{
    public float mass = 1f;                             // Mass of the RigidBody
    public float bounciness = 1;                        // The bounciness factor (value between 0 and 1, 0 being no bounce, and 1 being super bouncy!)
    public bool obeysGravity = true;                    // Whether or not this body obeys gravity
    public Vector2 gravity = new Vector2(0, -9.8f);     // The gravity vector applied to this body

    public Vector2 currentVelocity;                     // The current velocity the body is moving at
    public Vector2 maxVelocity = new Vector2(10f, 10f); // The maximum allowed velocity for this object

    public bool grounded;

    private Vector2 totalForces;                        
    private PhysicsEngine engine;

    public struct AABB
    {
        public Vector2 bLeft;
        public Vector2 tRight;
    }

    public AABB aabb;


    public void AddForce(Vector2 force)
    {
        totalForces += force;
    }

    public void Stop()
    {
        currentVelocity = Vector2.zero;
        totalForces = Vector2.zero;
    }

    public bool IsGrounded()
    {
        grounded = engine.IsGrounded(this);
        return grounded;
    }

    /*
     * This function sets the basic coordinates to apply the bounding function AABB
     * by using the bound component, the renderer component and  retrieve the exact 
     * coordinates to the Bottom Left and the Top Right positions of the solid
     * - just what is needed to calculate when collision is happening between two objects
     * <- What does this function do?
     */
    void SetAABB()
    {
        Bounds bound = new Bounds(new Vector2(0, 0), new Vector2(1, 1));
        Renderer renderer = GetComponent<Renderer>();

        if (renderer)
        {
            bound = renderer.bounds;
        }

        aabb.bLeft = new Vector2(bound.center.x - bound.extents.x, bound.center.y - bound.extents.y);
        aabb.tRight = new Vector2(bound.center.x + bound.extents.x, bound.center.y + bound.extents.y);
    }

    void Start(){
        SetAABB();
        engine = GameObject.FindWithTag("PhysicsEngine").GetComponent<PhysicsEngine>();
        engine.AddRigidBody(this);
    }

    /*
     * This function gets all the forces to which the game object is submitted (in the simulation),
     * includes (if necesssary) the effect of gravity and calculates the acceleration vector.
     * Using this vector and the DeltaTime (float dT) it calculates a new vector for its speed.
     * Finally, it updates the object's position transforming (integrating) the velocity - again using dT
     * This function should, however, be called upon update to make things happen 
     * <- Describe how this function works
     */
    public void Integrate(float dT){
        Vector2 acceleration = new Vector2();
        
        /*
         * This part of the code "implements" gravity, applying the force as the 
         * first force to be applied - unless the object floats in the air (i.e.: 
         * does not obey gravity) or is grounded (just so as not to use static physics 
         * when it is not necessary)
         * <- What is the purpose of this part of code?
         */ 
        if (obeysGravity && !IsGrounded()){ 
            Debug.Log(this.gameObject.name + "still applying gravity" );
            acceleration = gravity;
        }
        else{
            if (Mathf.Abs(currentVelocity.y) < 0.05f) currentVelocity.y = 0;
        }
        ///
        ///
        ///

        if (mass == 0)
            acceleration = Vector2.zero;
        else
            acceleration += totalForces / mass;
        
        currentVelocity += acceleration * dT;
        if(this.gameObject.name.StartsWith("B")){
            Debug.Log(this.gameObject.name);
            Debug.Log(acceleration);
            Debug.Log(currentVelocity);

        }
        Vector2 temp = transform.position;
        temp += currentVelocity * dT;

        transform.position = temp;
        SetAABB();

        totalForces = Vector2.zero;
    }
}
