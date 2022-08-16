using System.Collections;
using System.Collections.Generic;
using PlayerAnimatorControllerNamespace;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speedZ = 10f;
    [SerializeField] private float swerveLimitX = 2.1f;
    [SerializeField] private float swerveSpeed = 0.5f;
    [SerializeField] private CylinderController cylinderController;
    [SerializeField] private Rig handRig;

    private RigBuilder rigBuilder;
    private PlayerAnimatorController playerAnimatorController;
    private float touchPositionX;
    private float speedZTemp;

    private List<Transform> cylinderGrounds = new List<Transform>();

    private void Start()
    {
        rigBuilder = GetComponent<RigBuilder>();
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
        if (playerAnimatorController.currentPlayerAnimStatus == PlayerAnimLoopingStatus.None) return;

        if (playerAnimatorController.currentPlayerAnimStatus == PlayerAnimLoopingStatus.Sprint)
        {
            transform.position += Vector3.forward * playerAnimatorController.deltaPos.z;
        }
        else
        {
            transform.position += Vector3.forward * speedZTemp * Time.deltaTime;
        }
    }

    private void ActiveRigLayer(Rig rig, bool isActive)
    {
        playerAnimatorController.SetRigged(isActive);
        rigBuilder.layers.Find(x => x.rig == rig).active = isActive;
    }

    private void CheckAllCylinderGround()
    {
        if (cylinderGrounds.Count == 0)
        {
            return;
        }

        CheckFinish();
    }

    private void CheckFinish()
    {
        if (!cylinderGrounds.Exists(x => x.position.x < transform.position.x) || !cylinderGrounds.Exists(x => x.position.x > transform.position.x) || cylinderGrounds.Count == 0)
        {
            ActiveRigLayer(handRig, false);
            cylinderController.transform.SetParent(null);
            cylinderController.gameObject.AddComponent<Rigidbody>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Cutter"))
        {
            ActiveRigLayer(handRig, false);
            cylinderController.CutCylinder(other.gameObject.transform, () => ActiveRigLayer(handRig, true));
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Finish"))
        {
            playerAnimatorController.SetLoopingAnimMode(PlayerAnimLoopingStatus.None);
            playerAnimatorController.ActiveLanding();
            FinishGroundItem finishItem = other.gameObject.GetComponentsInChildren<FinishGroundItem>().OrderBy(x => Mathf.Abs(x.transform.position.z - transform.position.z)).First();
            Debug.Log(finishItem.getFinishAmount);
        }

        if (other.gameObject.CompareTag("NormalGround") && playerAnimatorController.currentPlayerAnimStatus == PlayerAnimLoopingStatus.Falling)
        {
            playerAnimatorController.SetLoopingAnimMode(PlayerAnimLoopingStatus.Sprint);
            speedZTemp = speedZ;
        }

        if (other.gameObject.CompareTag("CylinderGround") && !cylinderGrounds.Exists(x => x == other.transform))
        {
            cylinderGrounds.Add(other.transform);
            if (cylinderGrounds.Count == 1)
            {
                CancelCheckCylinderInvokes();
                Invoke("CheckAllCylinderGround", 0.25f);
            }
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("NormalGround"))
        {
            playerAnimatorController.SetLoopingAnimMode(PlayerAnimLoopingStatus.Falling);
            speedZTemp = speedZ * 1.5f;
        }

        if (other.gameObject.CompareTag("CylinderGround") && cylinderGrounds.Exists(x => x == other.transform))
        {
            cylinderGrounds.Remove(other.transform);
        }

        if (other.gameObject.CompareTag("NormalGround") || other.gameObject.CompareTag("CylinderGround"))
        {
            CancelCheckCylinderInvokes();
            Invoke("CheckFinish", 1f);
        }
    }

    private void CancelCheckCylinderInvokes()
    {
        CancelInvoke("CheckAllCylinderGround");
        CancelInvoke("CheckFinish");
    }
}
