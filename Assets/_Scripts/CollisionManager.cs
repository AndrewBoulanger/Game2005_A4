using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CollisionManager : MonoBehaviour
{
    public CubeBehaviour[] actors;
    public BulletManager bManager;
    private Queue<GameObject> activeBullets;
    [Range(0.0f, 1.0f)]
    public float restitution;
    public void setRestitution(float val) { restitution = val; }
    public float bulletMass { get; set; }
    public float SquareMass { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        actors = FindObjectsOfType<CubeBehaviour>();
        bManager = FindObjectOfType<BulletManager>();

        bulletMass = 1;
        SquareMass = 5;
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < actors.Length; i++)
        {
            // Cloning Queue (To iterate through for collision Checking)
            activeBullets = new Queue<GameObject>(bManager.activeBullets);

            // Box to Box Collisions
            for (int j = 0; j < actors.Length; j++)
            {
                if (i != j)
                {
                    CheckAABBs(actors[i], actors[j]);
                }
            }

            BulletBehaviour Bullet;
            // Box to Bullet Collisions
            for (; activeBullets.Count > 0;)
            {
                Bullet = activeBullets.Dequeue().GetComponent<BulletBehaviour>();
                CheckAABBCircle(Bullet, actors[i]);
            }
        }
    }

    // AABB check b/n Boxes
    public static void CheckAABBs(CubeBehaviour a, CubeBehaviour b)
    {
        if ((a.min.x <= b.max.x && a.max.x >= b.min.x) &&
            (a.min.y <= b.max.y && a.max.y >= b.min.y) &&
            (a.min.z <= b.max.z && a.max.z >= b.min.z))
        {
            if (!a.contacts.Contains(b))
            {
                a.contacts.Add(b);
                a.isColliding = true;
            }
        }
        else
        {
            if (a.contacts.Contains(b))
            {
                a.contacts.Remove(b);
                a.isColliding = false;
            }
           
        }
    }

    // AABBCircle check b/n Bullets and Boxes
    public static void CheckAABBCircle(BulletBehaviour a, CubeBehaviour b)
    {
        Vector3 spherePos = a.transform.position;
        var x = Mathf.Max(b.min.x, Mathf.Min(spherePos.x, b.max.x));
        var y = Mathf.Max(b.min.y, Mathf.Min(spherePos.y, b.max.y));
        var z = Mathf.Max(b.min.z, Mathf.Min(spherePos.z, b.max.z));

        var distance = Mathf.Sqrt((x - spherePos.x) * (x - spherePos.x) +
                                     (y - spherePos.y) * (y - spherePos.y) +
                                     (z - spherePos.z) * (z - spherePos.z));

        if (distance < a.radius)
        {
            // Collision Occurs (Call Collision Response!)
            b.isColliding = true;
            Debug.Log("Collision!");
            ResponseAABBCircle(a, b);
        }
        else
        {
            b.isColliding = false;
        }
    }

    // AABBCircle Response to Collision
    public static void ResponseAABBCircle(BulletBehaviour a, CubeBehaviour b)
    {
        Vector3 spherePos = a.transform.position;
        // Calculate Final Velocities of Box and Bullet
        CalculateVelocities(a, b);
        // Check which side the ball collided with the cube, and Reverse direction, and move out of cube
        if (spherePos.x > b.max.x || spherePos.x < b.min.x)
        {
            a.direction.x *= -1;
            if (spherePos.x > b.max.x)
                a._MoveOut(1);
            else
                a._MoveOut(-1);
        }
        if (spherePos.y > b.max.y || spherePos.y < b.min.y)
        {
            a.direction.y *= -1;
            if (spherePos.y > b.max.y)
                a._MoveOut(2);
            else
                a._MoveOut(-2);
        }
        if (spherePos.z > b.max.z || spherePos.z < b.min.z)
        {
            a.direction.z *= -1;
            if (spherePos.z > b.max.z)
                a._MoveOut(3);
            else
                a._MoveOut(-3);
        }
        // Check Final Velocities of Box and Bullet

        // Apply the Restitution Calculation
        a.currentSpeed *=  FindObjectOfType<CollisionManager>().restitution;
    }

    // Only for Bullet and Cube (For Now)
    public static void CalculateVelocities(BulletBehaviour a, CubeBehaviour b)
    {
        float M1 = FindObjectOfType<CollisionManager>().bulletMass;
        float M2 = 200000.0f;
        if (b.moveable)
        {
             M2 = FindObjectOfType<CollisionManager>().SquareMass;
        }
       

        // A's Velocity
        Vector3 vA = ((M2 - M1) / (M2 + M1)) * a._GetVelocity() +
                     ((2 * M2) / (M2 + M1)) * b._GetVelocity();
        // B's Velocity
        Vector3 vB = ((M2 - M1) / (M2 + M1)) * b._GetVelocity() +
                     ((2 * M1) / (M2 + M1)) * a._GetVelocity();

        a._SetVelocity(vA);
        b._SetVelocity(vB);
    }
}
