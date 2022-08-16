using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CylinderController : MonoBehaviour
{
    [SerializeField] private float resetPositionSpeed = 0.1f;
    private Coroutine lastResetPositionCoroutine;

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

        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.transform.localScale = new Vector3(transform.localScale.x, distance / 2, transform.localScale.z);
        cylinder.transform.rotation = transform.rotation;
        cylinder.transform.position = new Vector3((cutter.position.x < 0 ? -1 : 1) * (y - cylinder.transform.localScale.y), transform.position.y, transform.position.z);
        cylinder.GetComponent<Renderer>().material = GetComponent<Renderer>().material;
        cylinder.AddComponent<Rigidbody>();
        cylinder.GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(-360, 0), 0, 0), ForceMode.VelocityChange);
        Destroy(cylinder, 2f);

        if (lastResetPositionCoroutine != null) StopCoroutine(lastResetPositionCoroutine);
        lastResetPositionCoroutine = StartCoroutine(ResetPosition(callback));
    }

    private IEnumerator ResetPosition(UnityAction callback)
    {
        yield return new WaitForSeconds(0.25f);
        Vector3 newPosition = new Vector3(0, transform.localPosition.y, transform.localPosition.z);
        do
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, newPosition, resetPositionSpeed);
            yield return null;
        } while (transform.localPosition.x != 0);

        callback?.Invoke();
    }
}
