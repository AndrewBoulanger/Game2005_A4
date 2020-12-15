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
    // public float SquareMass { get; set; }
    public Vector3 gravity;
    public void setGravity(float val) { gravity.y = val; }

    public static float friction { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        actors = FindObjectsOfType<CubeBehaviour>();
        bManager = FindObjectOfType<BulletManager>();

        bulletMass = 1;
        SquareMass = 5;
        friction = 0.6f;
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < actors.Length; i++)
        {
            // Cloning Queue (To iterate through for collision Checking)
            activeBullets = new Queue<GameObject>(bManager.activeBullets);

            BulletBehaviour Bullet;
            // Box to Bullet Collisions
            for (; activeBullets.Count > 0;)
            {
                Bullet = activeBullets.Dequeue().GetComponent<BulletBehaviour>();
                CheckAABBCircle(Bullet, actors[i]);
            }

            // Box to Box Collisions
            for (int j = 0; j < actors.Length; j++)
            {
                if (i != j)
                {
                    CheckAABBs(actors[i], actors[j]);
                }
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
            if (a.moveable)
                ResponseAABB(a, b);
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
            b.active = true;
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
            a.velocity.x *= -1;
            if (spherePos.x > b.max.x)
                a._MoveOut(1);
            else
                a._MoveOut(-1);
        }
        if (spherePos.y > b.max.y || spherePos.y < b.min.y)
        {
            a.velocity.y *= -1;
            if (spherePos.y > b.max.y)
                a._MoveOut(2);
            else
                a._MoveOut(-2);
        }
        if (spherePos.z > b.max.z || spherePos.z < b.min.z)
        {
            a.velocity.z *= -1;
            if (spherePos.z > b.max.z)
                a._MoveOut(3);
            else
                a._MoveOut(-3);
        }
        // Check Final Velocities of Box and Bullet

        // Apply the Restitution Calculation
        a._SetVelocity(a.velocity* FindObjectOfType<CollisionManager>().restitution);

    }

    // AABB Response to Collision
    public static void ResponseAABB(CubeBehaviour a, CubeBehaviour b)
    {
        Vector3 aPos = a.transform.position;
        Vector3 bPos = b.transform.position;
        Vector3 relativePos = bPos - aPos;
        relativePos.x = Mathf.Abs(relativePos.x);
        relativePos.y = Mathf.Abs(relativePos.y);
        relativePos.z = Mathf.Abs(relativePos.z);
        float dist = 0.01f;

        // Calculate Final Velocities of Boxes
        CalculateVelocities(a, b);

        // Get The direction normal of collision, reverse direction and calculate distance into the other object
        Vector3 normal = new Vector3(0.0f, 0.0f, 0.0f);
        float Xcollision = Mathf.Min(a.max.x - b.min.x, b.max.x - a.min.x);
        float Ycollision = Mathf.Min(a.max.y - b.min.y, b.max.y - a.min.y);
        float Zcollision = Mathf.Min(a.max.z - b.min.z, b.max.z - a.min.z);

        int choice = 0;
        choice = (Xcollision < Ycollision ? 0 : 1);
        choice = (Mathf.Min(Xcollision, Ycollision) < Zcollision ? choice : 2);

        switch (choice)
        {
            case 0:
                // Collision is on the X axis;
                normal = new Vector3(1.0f, 0.0f, 0.0f);

                a.velocity.x *= -1;
                dist = (a._GetHalfWidth() + b._GetHalfWidth()) - relativePos.x;

                if (!(aPos.x > bPos.x))
                {
                    normal *= -1;
                    choice *= -1;
                }
                break;
            case 1:
                // Collision is on the Y axis;
                normal = new Vector3(0.0f, 1.0f, 0.0f);

                a.velocity.y *= -1;
                dist = (a._GetHalfHeight() + b._GetHalfHeight()) - relativePos.y;

                if (!(aPos.y > bPos.y))
                {
                    normal *= -1;
                    choice *= -1;
                }
                break;
            case 2:
                // Collision is on the Z axis;
                normal = new Vector3(0.0f, 0.0f, 1.0f);

                a.velocity.z *= -1;
                dist = (a._GetHalfLength() + b._GetHalfLength()) - relativePos.z;

                if (!(aPos.z > bPos.z))
                {
                    normal *= -1;
                    choice *= -1;
                }
                break;
        }

        a._MoveOut(normal, dist);

        // Calculating bounciness of collision
        a.velocity = a.velocity + normal * Vector3.Dot(a.velocity, normal) * -(1 - a.restitution) / ((1 / a.mass) + (1 / b.mass)) * Time.deltaTime;
        // Friction
        Vector3 tangent = Vector3.Cross(normal, Vector3.Cross(a.velocity, normal)).normalized;
        a.velocity = a.velocity + tangent * Vector3.Dot(a.velocity, tangent) * -friction / ((1 / a.mass) + (1 / b.mass)) * Time.deltaTime;
    }

    // Only for Bullet and Cube (For Now)
    public static void CalculateVelocities(BulletBehaviour a, CubeBehaviour b)
    {
        float ballMass = FindObjectOfType<CollisionManager>().bulletMass;
        float cubeMass = b.moveable ? FindObjectOfType<CollisionManager>().SquareMass : 200000.0f;

        // A's Velocity
        Vector3 vA = ((b.mass - ballMass) / (b.mass + ballMass)) * a._GetVelocity() +
                     ((2 * b.mass) / (b.mass + ballMass)) * b._GetVelocity();
        // B's Velocity
        Vector3 vB = ((b.mass - ballMass) / (b.mass + ballMass)) * b._GetVelocity() +
                     ((2 * ballMass) / (b.mass + ballMass)) * a._GetVelocity();

        a._SetVelocity(vA);
        b._SetVelocity(vB);
    }

    public static void CalculateVelocities(CubeBehaviour a, CubeBehaviour b)
    {
        if(a.moveable)
        {
            a.mass = FindObjectOfType<CollisionManager>().SquareMass;
        }
        if(b.moveable)
        {
            b.mass = FindObjectOfType<CollisionManager>().SquareMass;
        }
        // A's Velocity
        Vector3 vA = ((b.mass - a.mass) / (b.mass + a.mass)) * a._GetVelocity() +
                     ((2 * b.mass) / (b.mass + a.mass)) * b._GetVelocity();
        // B's Velocity
        Vector3 vB = ((b.mass - a.mass) / (b.mass + a.mass)) * b._GetVelocity() +
                     ((2 * a.mass) / (b.mass + a.mass)) * a._GetVelocity();

        a._SetVelocity(vA);
        b._SetVelocity(vB);
    }
}
