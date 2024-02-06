using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(PlayerController))]
public class Player : MonoBehaviour
{
   [SerializeField] private float playerSpeed;
    PlayerController playerController;
    Camera viewCamera;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        viewCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveInputDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 moveVelocity = moveInputDir.normalized*playerSpeed;
        playerController.Move(moveVelocity);

        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 Point = ray.GetPoint(rayDistance);
            Debug.DrawLine(ray.origin, Point, Color.red);
            playerController.LookAt(Point);
        }
    }

}
