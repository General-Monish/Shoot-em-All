using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(PlayerController))]
[RequireComponent (typeof(GunController))]
public class Player : MonoBehaviour
{
   [SerializeField] private float playerSpeed;
    PlayerController playerController;
    Camera viewCamera;
    GunController gunController;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
        viewCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {

        // Move Input
        Vector3 moveInputDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 moveVelocity = moveInputDir.normalized*playerSpeed;
        playerController.Move(moveVelocity);

        // Mouse Input
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 Point = ray.GetPoint(rayDistance);
            Debug.DrawLine(ray.origin, Point, Color.red);
            playerController.LookAt(Point);
        }

        //Waepon Input
        if (Input.GetMouseButton(0))
        {
            gunController.shoot();
        }
    }

}
