using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    Rigidbody rb;
    Vector3 velocity;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

  public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }

    public void FixedUpdate()
    {
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
    }

    public void LookAt(Vector3 Lookpoint)
    {
        Vector3 heightCorrectedPoint = new Vector3(Lookpoint.x, transform.position.y, Lookpoint.z);
        transform.LookAt(heightCorrectedPoint);
    }
}
