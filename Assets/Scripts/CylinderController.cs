using System.Collections;
using System.Collections.Generic;
using GameManagerNamespace;
using UnityEngine;
using UnityEngine.Events;

public class CylinderController : MonoBehaviour
{
    [SerializeField] private float resetPositionSpeed = 0.1f;
    private Coroutine lastResetPositionCoroutine;

    private void Start()
    {
        float scaleY = GameManager.Instance.levels[GameManager.Instance.currentLevelIndex].cylinderScale;
        transform.localScale = new Vector3(transform.localScale.x, scaleY, transform.localScale.z);
    }

    public void CutCylinder(Transform cutter, UnityAction callback)
    {
        float y = transform.localScale.y;
        float distance = 0.0f;

        if (cutter.position.x < transform.position.x)
        {
            y -= transform.position.x;
            distance = y + cutter.position.x;

            if (distance / 2 > 0)
            {
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y - distance / 2, transform.localScale.z);
                transform.position = new Vector3(transform.position.x + distance / 2, transform.position.y, transform.position.z);
            }
        }
        else
        {
            y += transform.position.x;
            distance = y - cutter.position.x;

            if (distance / 2 > 0)
            {
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y - distance / 2, transform.localScale.z);
                transform.position = new Vector3(transform.position.x - distance / 2, transform.position.y, transform.position.z);
            }
        }

        CreateCuttingCylinder((cutter.position.x < 0 ? -1 : 1) * (y - distance / 2), distance / 2);
        if (lastResetPositionCoroutine != null) StopCoroutine(lastResetPositionCoroutine);

        if (transform.localScale.y <= 0.25f)
        {
            GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            lastResetPositionCoroutine = StartCoroutine(ResetPosition(callback));
        }
    }

    public void IncOrDecScaleCylinder(float amount, UnityAction callback)
    {
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y + amount, transform.localScale.z);
        if (lastResetPositionCoroutine != null) StopCoroutine(lastResetPositionCoroutine);

        if (transform.localScale.y <= 0.25f)
        {
            GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            lastResetPositionCoroutine = StartCoroutine(ResetPosition(callback));
        }

        if (amount < 0)
        {
            CreateCuttingCylinder((transform.position.x - transform.localScale.y / 2) - (transform.localScale.y / 2 - amount / 4), amount / 2);
            CreateCuttingCylinder((transform.position.x + transform.localScale.y / 2) + (transform.localScale.y / 2 - amount / 4), amount / 2);
        }
    }

    private void CreateCuttingCylinder(float xPosition, float yScale)
    {
        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.transform.localScale = new Vector3(transform.localScale.x, yScale, transform.localScale.z);
        cylinder.transform.rotation = transform.rotation;
        cylinder.transform.position = new Vector3(xPosition, transform.position.y, transform.position.z);
        cylinder.GetComponent<Renderer>().material = GetComponent<Renderer>().material;
        cylinder.AddComponent<Rigidbody>();
        cylinder.GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(-360, 0), 0, 0), ForceMode.VelocityChange);
        Destroy(cylinder, 2f);
    }

    private IEnumerator ResetPosition(UnityAction callback)
    {
        yield return new WaitForSeconds(0.5f);
        Vector3 newPosition = new Vector3(0, transform.localPosition.y, transform.localPosition.z);
        while (transform.localPosition.x != 0)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, newPosition, resetPositionSpeed * Time.deltaTime);
            yield return null;
        }

        callback?.Invoke();
    }
}
