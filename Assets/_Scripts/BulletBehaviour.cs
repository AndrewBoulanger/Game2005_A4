using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BulletBehaviour : MonoBehaviour
{
    public float speed;
    public float currentSpeed;
    public Vector3 direction;
    public float range;
    public float radius;
    [Range(0.0f, 1.0f)]
    public float restitution;

    // Start is called before the first frame update
    void Start()
    {
        radius = transform.localScale.x;
        currentSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        _Move();
        _CheckBounds();
    }

    private void _Move()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void _CheckBounds()
    {
        if (Vector3.Distance(transform.position, Vector3.zero) > range)
        {
            gameObject.SetActive(false);
        }
    }

    // Despawn bullet if moving too slowly
    public void _CheckSpeed()
    {
        if(currentSpeed <= 0.1f)
        {
            gameObject.SetActive(false);
            Reset();
        }
    }

    // Moving bullet out of object
    public void _MoveOut(int axis = 0)
    {
        Vector3 moveVec = new Vector3(0.0f, 0.0f, 0.0f);
        switch (axis)
        {
            case 1:
                moveVec.x = 1.0f;
                break;
            case -1:
                moveVec.x = -1.0f;
                break;
            case 2:
                moveVec.y = 1.0f;
                break;
            case -2:
                moveVec.y = -1.0f;
                break;
            case 3:
                moveVec.z = 1.0f;
                break;
            case -3:
                moveVec.z = -1.0f;
                break;
        }

        transform.position += moveVec * radius / 2;
    }

    public void Reset()
    {
        currentSpeed = speed;
    }
}
