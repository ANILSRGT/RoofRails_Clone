using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speedZ = 10f;
    [SerializeField] private float swerveLimitX = 2.1f;
    [SerializeField] private float swerveSpeed = 0.5f;
    [SerializeField] private CylinderController cylinderController;
    private PlayerAnimatorController playerAnimatorController;
    private float touchPositionX;
    private float speedZTemp;

    private void Start()
    {
        playerAnimatorController = GetComponent<PlayerAnimatorController>();
        speedZTemp = speedZ;
    }

    private void Update()
    {
        Move();

        if (Input.GetMouseButtonDown(0))
        {
            touchPositionX = Camera.main.ScreenToViewportPoint(Input.mousePosition).x;
        }
        else if (Input.GetMouseButton(0))
        {
            float deltaX = Mathf.Lerp(-swerveLimitX, swerveLimitX, touchPositionX);
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(deltaX, transform.position.y, transform.position.z), swerveSpeed);
            touchPositionX = Camera.main.ScreenToViewportPoint(Input.mousePosition).x;
        }
        else
        {
            touchPositionX = 0;
        }
    }

    private void Move()
    {
        transform.position += Vector3.forward * playerAnimatorController.deltaPos.z;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Cutter"))
        {
            cylinderController.CutCylinder(other.gameObject.transform);
        }

        if (other.gameObject.CompareTag("CylinderGround"))
        {
            speedZTemp = speedZ * 1.5f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("CylinderGround"))
        {
            speedZTemp = speedZ;
        }
    }
}
