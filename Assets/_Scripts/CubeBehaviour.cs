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
    public float mass;
    public bool moveable;
    public Vector3 gravity;

    // Start is called before the first frame update
    void Start()
    {
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
        gravity = FindObjectOfType<CollisionManager>().gravity;
        max = Vector3.Scale(bounds.max, transform.localScale) + transform.position;
        min = Vector3.Scale(bounds.min, transform.localScale) + transform.position;

        if (moveable)
        {
            _Move();
        }
    }

    void FixedUpdate()
    {
        // physics related calculations
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
        transform.position += direction * currentSpeed * Time.deltaTime;
    }

    public Vector3 _GetVelocity()
    {
        return direction * currentSpeed;
    }

    public void _SetVelocity(Vector3 vel)
    {
        direction = vel.normalized;
        currentSpeed = vel.magnitude;
    }
}
