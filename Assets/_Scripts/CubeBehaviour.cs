using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Color = UnityEngine.Color;


[System.Serializable]
public class CubeBehaviour : MonoBehaviour
{
    public Vector3 size;
    public Vector3 max;
    public Vector3 min;
    public bool isColliding;
    public bool debug;
    public List<CubeBehaviour> contacts;

    private MeshFilter meshFilter;
    private Bounds bounds;

    // Movement
    public float speed;
    public float currentSpeed;
    public Vector3 direction;
    public Vector3 velocity;
    public Vector3 gravity;
    public float mass;
    public bool moveable;
    [Range(0.0f, 1.0f)]
    public float restitution;
    public float friction;

    public bool active;

    // Start is called before the first frame update
    void Start()
    {
        active = false;
        debug = false;
        meshFilter = GetComponent<MeshFilter>();

        bounds = meshFilter.mesh.bounds;
        size = bounds.size;

        speed = 0.0f;
        currentSpeed = speed;
        if (!moveable)
            mass = 20000.0f;
    }

    // Update is called once per frame
    void Update()
    {
        max = Vector3.Scale(bounds.max, transform.localScale) + transform.position;
        min = Vector3.Scale(bounds.min, transform.localScale) + transform.position;
    }

    void FixedUpdate()
    {
        // physics related calculations
        if (moveable && active)
        {
            _Move();
        }
    }

    private void OnDrawGizmos()
    {
        if (debug)
        {
            Gizmos.color = Color.magenta;

            Gizmos.DrawWireCube(transform.position, Vector3.Scale(new Vector3(1.0f, 1.0f, 1.0f), transform.localScale));
        }
    }

    private void _Move()
    {
        _SetVelocity(velocity - gravity * Time.deltaTime);

        transform.position += velocity * Time.deltaTime;
    }

    public Vector3 _GetVelocity()
    {
        if (moveable && active)
            return velocity;
        return new Vector3(0.0f, 0.0f, 0.0f);
    }

    public void _SetVelocity(Vector3 vel)
    { 
        if(moveable)
        {
            direction = vel.normalized;
            currentSpeed = vel.magnitude;
            velocity = vel;
        }
    }

    public void _MoveOut(Vector3 normal, float dist)
    {
        transform.position += normal * dist;
    }

    public float _GetHalfWidth()
    {
        return (max.x - min.x) / 2;
    }

    public float _GetHalfHeight()
    {
        return (max.y - min.y) / 2;
    }

    public float _GetHalfLength()
    {
        return (max.z - min.z) / 2;
    }
}
