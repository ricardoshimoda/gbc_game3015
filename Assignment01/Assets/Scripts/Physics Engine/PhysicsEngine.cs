using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsEngine : MonoBehaviour
{
    public float groundedTol = 0.1f;

    public struct CollisionPair{
        public PhysicsRBody rigidBodyA;
        public PhysicsRBody rigidBodyB;
    }

    public struct CollisionInfo{
        public Vector2 collisionNormal;
        public float penetration;
    }

    private Dictionary<CollisionPair, CollisionInfo> collisions = new Dictionary<CollisionPair, CollisionInfo>();
    private List<PhysicsRBody> rigidBodies = new List<PhysicsRBody>();

    public void AddRigidBody(PhysicsRBody rigidBody){
        rigidBodies.Add(rigidBody);
    }

    /*
     * Moves objects along
     */
    void IntegrateBodies(float dT){
        foreach (PhysicsRBody rb in rigidBodies){
            rb.Integrate(dT);
        }
    }

    /*
     * 
     */
    public bool IsGrounded(PhysicsRBody rigidBody){
        foreach (PhysicsRBody rb in rigidBodies){
            if (rb != rigidBody){
                if (rigidBody.aabb.bLeft.x < rb.aabb.tRight.x 
                    && rigidBody.aabb.tRight.x > rb.aabb.bLeft.x
                    && Mathf.Abs(rigidBody.aabb.bLeft.y - rb.aabb.tRight.y) <= groundedTol){
                    if (Mathf.Abs(rigidBody.currentVelocity.y) < groundedTol)
                        return true;
                }
            }
        }
        return false;
    }

    /*
     * Check collisions betweem all the objects in the scene
     */
    void CheckCollisions(){
        foreach (PhysicsRBody bodyA in rigidBodies.GetRange(0, rigidBodies.Count - 1)){
            foreach (PhysicsRBody bodyB in rigidBodies.GetRange(rigidBodies.IndexOf(bodyA), rigidBodies.Count - rigidBodies.IndexOf(bodyA))){
                if (bodyA != bodyB){

                    /*
                     * builds the collision pair just before analysing if collision really happened
                     */
                    CollisionPair pair = new CollisionPair();
                    pair.rigidBodyA = bodyA; pair.rigidBodyB = bodyB;

                    Vector2 distance = bodyB.transform.position - bodyA.transform.position;
                    Vector2 halfSizeA = (bodyA.aabb.tRight - bodyA.aabb.bLeft) / 2;
                    Vector2 halfSizeB = (bodyB.aabb.tRight - bodyB.aabb.bLeft) / 2;

                    Vector2 gap = new Vector2(Mathf.Abs(distance.x), Mathf.Abs(distance.y)) - (halfSizeA + halfSizeB);

                    // Seperating Axis Theorem test
                    if (gap.x < 0 && gap.y < 0){
                        //Debug.Log("Collided!!!");
                        //Debug.Log(bodyA.gameObject.name);
                        //Debug.Log(bodyB.gameObject.name);
                        /*
                         * Removes data from a "previous" collision between those two objects
                         */
                        if (collisions.ContainsKey(pair)){
                            collisions.Remove(pair);
                        }

                        CollisionInfo colInfo = new CollisionInfo();
                        if (gap.x > gap.y){
                            Debug.Log("GapX - collision is happening in the x Axis");
                            if (distance.x > 0){
                                /*
                                 * In this case the collision is happening like this:
                                 * B to the left of A
                                 */
                                colInfo.collisionNormal += new Vector2(1,0);
                            }
                            else{
                                /*
                                 * In this case the collision is happening like this:
                                 * A to the left of B
                                 */
                                colInfo.collisionNormal += new Vector2(-1,0);
                            }                                
                            colInfo.penetration = gap.x;
                            Debug.Log("Penetration: " + gap.x);
                        }
                        else{
                            Debug.Log("GapY");
                            if (distance.y > 0){
                                colInfo.collisionNormal += new Vector2(0,1);
                            }                              
                            else{
                                colInfo.collisionNormal += new Vector2(0,-1);
                            }
                            colInfo.penetration = gap.y; 
                        }                                 
                        collisions.Add(pair, colInfo);
                    }
                    else if (collisions.ContainsKey(pair)){
                        /* Removes data from a collision between those two objects that is happening no more */
                        //if(pair.rigidBodyA.IsGrounded() && pair.rigidBodyA.mass > 0){
                            //pair.rigidBodyA.grounded = false;
                        //}
                        //if(pair.rigidBodyB.IsGrounded() && pair.rigidBodyB.mass > 0){
                            //pair.rigidBodyB.grounded = false;
                        //}
                        Debug.Log("Removed - Collision happens no more");
                        collisions.Remove(pair);
                    }

                }
            }
        }
    }

    void ResolveCollisions(){
        foreach (CollisionPair pair in collisions.Keys){
            float minBounce = Mathf.Min(pair.rigidBodyA.bounciness, pair.rigidBodyB.bounciness);
            float velAlongNormal = Vector2.Dot(pair.rigidBodyB.currentVelocity - pair.rigidBodyA.currentVelocity, collisions[pair].collisionNormal);
            if (velAlongNormal > 0) continue;

            float j = -(1 + minBounce) * velAlongNormal;
            float invMassA, invMassB;
            if (pair.rigidBodyA.mass == 0)
                invMassA = 0;
            else
                invMassA = 1 / pair.rigidBodyA.mass;

            if (pair.rigidBodyB.mass == 0)
                invMassB = 0;
            else
                invMassB = 1 / pair.rigidBodyB.mass;

            j /= invMassA + invMassB;

            Vector2 impulse = j * collisions[pair].collisionNormal;

            // ... update velocities
            pair.rigidBodyA.currentVelocity -= impulse*invMassA;
            pair.rigidBodyB.currentVelocity += impulse*invMassB;

            //if(pair.rigidBodyA.mass == 0 || pair.rigidBodyB.mass == 0 || pair.rigidBodyA.IsGrounded() || pair.rigidBodyB.IsGrounded()){
                //pair.rigidBodyA.grounded = true;
                //pair.rigidBodyB.grounded = true;
            //}
            /*
            float totalWeight = pair.rigidBodyA.mass + pair.rigidBodyB.mass;

            if(impulse.x != 0)
            {
                if(pair.rigidBodyA.mass != 0)
                {
                    if(pair.rigidBodyB.mass != 0){
                        pair.rigidBodyA.currentVelocity = -pair.rigidBodyB.mass / totalWeight * impulse + new Vector2(0,pair.rigidBodyA.currentVelocity.y);
                    }
                    else{
                        pair.rigidBodyA.currentVelocity = -pair.rigidBodyA.mass / totalWeight * impulse + new Vector2(0,pair.rigidBodyA.currentVelocity.y);
                    }
                }
                if(pair.rigidBodyB.mass != 0)
                {
                    if(pair.rigidBodyA.mass != 0){
                        pair.rigidBodyB.currentVelocity = pair.rigidBodyA.mass / totalWeight * impulse+ new Vector2(0,pair.rigidBodyB.currentVelocity.y);
                    }
                    else{
                        pair.rigidBodyB.currentVelocity = pair.rigidBodyB.mass / totalWeight * impulse+ new Vector2(0,pair.rigidBodyB.currentVelocity.y);
                    }
                }

            }
            else{
                if(pair.rigidBodyA.mass != 0)
                {
                    if(pair.rigidBodyB.mass != 0)
                    {
                        if(pair.rigidBodyB.IsGrounded()){
                            pair.rigidBodyA.currentVelocity = new Vector2(pair.rigidBodyA.currentVelocity.x,0);
                            pair.rigidBodyA.grounded = true;
                        }
                        else{
                            pair.rigidBodyA.currentVelocity = -pair.rigidBodyB.mass / totalWeight * impulse + new Vector2(pair.rigidBodyA.currentVelocity.x,0);
                        }
                    }
                    else{
                        if(pair.rigidBodyA.obeysGravity){
                            pair.rigidBodyA.currentVelocity = new Vector2(pair.rigidBodyA.currentVelocity.x,0);
                            pair.rigidBodyA.grounded = true;
                        }
                        else{
                            pair.rigidBodyA.currentVelocity = -pair.rigidBodyA.mass / totalWeight * impulse + new Vector2(pair.rigidBodyA.currentVelocity.x,0);
                        }
                    }
                }
                if(pair.rigidBodyB.mass != 0)
                {
                    if(pair.rigidBodyA.mass != 0){
                        if(pair.rigidBodyA.IsGrounded()){
                            pair.rigidBodyB.currentVelocity = new Vector2(pair.rigidBodyB.currentVelocity.x,0);
                            pair.rigidBodyB.grounded = true;
                        }
                        else{
                            pair.rigidBodyB.currentVelocity = pair.rigidBodyA.mass / totalWeight * impulse+ new Vector2(pair.rigidBodyB.currentVelocity.x,0);
                        }
                    }
                    else{
                        if(pair.rigidBodyB.obeysGravity){
                            pair.rigidBodyB.currentVelocity = new Vector2(pair.rigidBodyB.currentVelocity.x,0);
                            pair.rigidBodyB.grounded = true;
                        }
                        else{
                            pair.rigidBodyB.currentVelocity = -pair.rigidBodyB.mass / totalWeight * impulse + new Vector2(pair.rigidBodyB.currentVelocity.x,0);
                        }

                    }
                }
            }*/
            
            if (Mathf.Abs(collisions[pair].penetration) > 0.01f){
                Debug.Log("Correction needed!");
                PositionalCorrection(pair);
            }
        }
    }

    /*
    * This function updates the position of solids upon collision using the inverse mass as weights (as well as a 
    * pre-defined fixed percentage) so that objects which collided are just a bit separated, instead of still penetrating
    * We need this function to give the effect that collision happens "instantly" - i.e.: in just one frame
    * otherwise, with velocities opposed, this could create the "illusion" that, in the second frame after dectecting collision,
    * despite having their velocities altered (and in the opposite direction), they are still colliding - which triggers 
    * changing the direction of their velocities - and so objects would collide several times during several frames
    * - only separating when their final velocity accumulated (wrongly) so that they can tear themselves appart 
    * <- Why do we need this function? 
    * Ok, will make a movie out of this and submit alongside the project
    * <- Try taking it out and see what happens
    */
    void PositionalCorrection(CollisionPair c){
        const float percent = 0.2f;
                            
        float invMassA, invMassB;
        if (c.rigidBodyA.mass == 0)
            invMassA = 0;
        else
            invMassA = 1 / c.rigidBodyA.mass;

        if (c.rigidBodyB.mass == 0)
            invMassB = 0;
        else
            invMassB = 1 / c.rigidBodyB.mass;

        Debug.Log("Correction: " + ((collisions[c].penetration / (invMassA + invMassB)) * percent));
        Vector2 correction = ((collisions[c].penetration / (invMassA + invMassB)) * percent) * -collisions[c].collisionNormal;
        Debug.Log("Correction: " + correction.x);

        Vector2 temp = c.rigidBodyA.transform.position;
        temp -= invMassA * correction;
        c.rigidBodyA.transform.position = temp;

        temp = c.rigidBodyB.transform.position;
        temp += invMassB * correction;
        c.rigidBodyB.transform.position = temp;
    }

    void UpdatePhysics(){
        IntegrateBodies(Time.fixedDeltaTime);
        CheckCollisions();
        ResolveCollisions();
    }

    // Update is called once per frame
    void FixedUpdate(){
        UpdatePhysics();
    }
}
