using System.Collections;
using System.Collections.Generic;
using PlayerAnimatorControllerNamespace;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using System.Linq;
using GameManagerNamespace;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speedZ = 10f;
    [SerializeField] private float cylinderSlidingSpeedZ = 17.5f;
    [SerializeField] private float swerveLimitX = 2.1f;
    [SerializeField] private float swerveSpeed = 0.5f;
    [SerializeField] private CylinderController cylinderController;
    [SerializeField] private Rig handRig;
    [SerializeField] private float fallTime = 2f;

    private Rigidbody rb;

    private RigBuilder rigBuilder;
    private PlayerAnimatorController playerAnimatorController;
    private float touchPositionX;
    private float speedZTemp;

    public List<Transform> cylinderGrounds { get; private set; } = new List<Transform>();
    private List<Coroutine> checkCylinderCoroutines = new List<Coroutine>();
    private bool isGround = true;
    private bool isFinish = false;
    private bool isFail = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rigBuilder = GetComponent<RigBuilder>();
        playerAnimatorController = GetComponent<PlayerAnimatorController>();
        speedZTemp = speedZ;
    }

    private void OnEnable()
    {
        MyEvents.OnGameStart.AddListener(OnGameStart);
    }

    private void OnDisable()
    {
        MyEvents.OnGameStart.RemoveListener(OnGameStart);
    }

    private void Update()
    {
        if (!isFinish) Move();

        if (!isFail)
        {
            CheckInputs();
            CheckCylinderScale();
        }
    }

    private void OnGameStart()
    {
        playerAnimatorController.SetLoopingAnimMode(PlayerAnimLoopingStatus.Sprint);
    }

    private void CheckInputs()
    {
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
        else if (playerAnimatorController.currentPlayerAnimStatus == PlayerAnimLoopingStatus.Falling)
        {
            transform.position += Vector3.forward * speedZTemp * Time.deltaTime;
        }
    }

    private void CheckCylinderScale()
    {
        if (cylinderController.transform.localScale.y <= 0.25f)
        {
            ActiveRigLayer(handRig, false);
            playerAnimatorController.SetLoopingAnimMode(PlayerAnimLoopingStatus.Idle);
            GameManager.Instance.Fail();
        }
    }

    private void ActiveRigLayer(Rig rig, bool isActive)
    {
        playerAnimatorController.SetRigged(isActive);
        rigBuilder.layers.Find(x => x.rig == rig).active = isActive;
    }

    private IEnumerator CheckAllCylinderGround(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (cylinderGrounds.Count == 0)
        {
            yield break;
        }

        if (!cylinderGrounds.Exists(x => x.position.x <= transform.position.x - 0.5f) || !cylinderGrounds.Exists(x => x.position.x >= transform.position.x + 0.5) || cylinderGrounds.Count == 0)
        {
            OnFinish();
        }
    }

    public void OnFinish(bool isFail = false)
    {
        this.isFail = true;
        speedZTemp = speedZ * 0.1f;
        ActiveRigLayer(handRig, false);
        cylinderController.transform.SetParent(null);
        cylinderController.gameObject.AddComponent<Rigidbody>();

        if (isFail)
        {
            isFinish = true;
            playerAnimatorController.SetLoopingAnimMode(PlayerAnimLoopingStatus.Falling);
            GameManager.Instance.Fail();
        }
        else isFinish = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Cutter"))
        {
            other.enabled = false;
            ActiveRigLayer(handRig, false);
            cylinderController.CutCylinder(other.gameObject.transform, () => ActiveRigLayer(handRig, true));
        }

        if (other.gameObject.CompareTag("BlockAndDrop"))
        {
            float amount = other.gameObject.GetComponent<BlockAndDrops>().getAmont;
            ActiveRigLayer(handRig, false);
            cylinderController.IncOrDecScaleCylinder(amount, () => ActiveRigLayer(handRig, true));
            Destroy(other.gameObject);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Finish") && !isFinish)
        {
            playerAnimatorController.SetLoopingAnimMode(PlayerAnimLoopingStatus.None);
            playerAnimatorController.ActiveLanding();
            FinishGroundItem finishItem = other.gameObject.GetComponentsInChildren<FinishGroundItem>().OrderBy(x => Mathf.Abs(x.transform.position.z - transform.position.z)).First();
            GameManager.Instance?.Success();
        }

        if (other.gameObject.CompareTag("NormalGround") && playerAnimatorController.currentPlayerAnimStatus == PlayerAnimLoopingStatus.Falling)
        {
            isGround = true;
            playerAnimatorController.SetLoopingAnimMode(PlayerAnimLoopingStatus.Sprint);
            speedZTemp = speedZ;
        }

        if (other.gameObject.CompareTag("CylinderGround") && !cylinderGrounds.Exists(x => x == other.transform))
        {
            speedZTemp = cylinderSlidingSpeedZ;
            isGround = true;
            cylinderGrounds.Add(other.transform);
            if (cylinderGrounds.Count == 1)
            {
                StopCheckCylinderCoroutine();
                checkCylinderCoroutines.Add(StartCoroutine(CheckAllCylinderGround(0.25f)));
            }
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("NormalGround"))
        {
            playerAnimatorController.SetLoopingAnimMode(PlayerAnimLoopingStatus.Falling);
        }

        if (other.gameObject.CompareTag("CylinderGround") && cylinderGrounds.Exists(x => x == other.transform))
        {
            cylinderGrounds.Remove(other.transform);
        }

        if (other.gameObject.CompareTag("NormalGround") || other.gameObject.CompareTag("CylinderGround"))
        {
            speedZTemp = speedZ;
            isGround = false;
            StopCheckCylinderCoroutine();
            if (other.gameObject.CompareTag("CylinderGround"))
            {
                if (other.gameObject.GetComponent<CylinderGroundController>().isFail)
                {
                    checkCylinderCoroutines.Add(StartCoroutine(CheckTriggerGround(fallTime, true)));
                }
                else
                {
                    checkCylinderCoroutines.Add(StartCoroutine(CheckTriggerGround(fallTime)));
                }
            }
            else
            {
                checkCylinderCoroutines.Add(StartCoroutine(CheckTriggerGround(fallTime)));
            }
        }
    }

    private IEnumerator CheckTriggerGround(float delay, bool isFail = false)
    {
        yield return new WaitForSeconds(delay);
        if (!isGround)
        {
            if (isFail) isFinish = true;

            OnFinish(isFail);
        }
    }

    private void StopCheckCylinderCoroutine()
    {
        if (checkCylinderCoroutines.Count > 0)
        {
            foreach (Coroutine coroutine in checkCylinderCoroutines.ToList())
            {
                if (coroutine == null) continue;

                StopCoroutine(coroutine);
                checkCylinderCoroutines.Remove(coroutine);
            }
        }
    }
}
