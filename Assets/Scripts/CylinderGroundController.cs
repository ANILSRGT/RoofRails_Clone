using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CylinderGroundController : MonoBehaviour
{
    public bool isFail = false;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            StartCoroutine(CheckCylinder(other.gameObject.transform.root.GetComponent<PlayerController>()));
        }
    }

    private IEnumerator CheckCylinder(PlayerController player)
    {
        yield return new WaitForSeconds(0.25f);
        if (player.cylinderGrounds.Count == 1)
        {
            bool isLeft = player.transform.position.x < transform.position.x;
            player.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(1, 2, 0) * (isLeft ? -1 : 1), ForceMode.VelocityChange);
            player.OnFinish(isFail);
        }
    }
}
