using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(PlayerController))]
[RequireComponent (typeof(GunController))]
public class Player : LivingEntity
{
   [SerializeField] private float playerSpeed;
    PlayerController playerController;
    Camera viewCamera;
    public CrossHair crossHairs;
    GunController gunController;

    // Start is called before the first frame update
   protected override void Start()
    {
        base.Start();
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

        // Look Input
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.up*gunController.GunHeight);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 Point = ray.GetPoint(rayDistance);
            //Debug.DrawLine(ray.origin, Point, Color.red);
            playerController.LookAt(Point);
            crossHairs.transform.position = Point;
            crossHairs.detectTargets(ray);
        }

        //Waepon Input
        if (Input.GetMouseButton(0))
        {
            gunController.OntriggerHold();
        }
        
        if (Input.GetMouseButtonUp(0)) // up means when releasing the mouse button
        {
            gunController.OntriggerReleased();
        }

      
        
    }

}
