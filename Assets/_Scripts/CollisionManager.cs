using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CollisionManager : MonoBehaviour
{
    public CubeBehaviour[] actors;
    public BulletManager bManager;
    private Queue<GameObject> activeBullets;

    // Start is called before the first frame update
    void Start()
    {
        actors = FindObjectsOfType<CubeBehaviour>();
        bManager = FindObjectOfType<BulletManager>();
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
        }
        else
        {
            b.isColliding = false;
        }
    }
}
