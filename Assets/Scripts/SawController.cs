using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawController : MonoBehaviour
{
    [Serializable] private enum SawType { Normal };
    [SerializeField] private Transform mesh;
    [SerializeField] private SawType sawType = SawType.Normal;
    [Header("Normal")]
    [SerializeField] private float rotateSpeed = 100f;
    private Coroutine activeSawCoroutine;

    private void OnEnable()
    {
        MyEvents.OnGameStart.AddListener(OnGameStart);
        MyEvents.OnGameEnd.AddListener(OnGameEnd);
    }

    private void OnDisable()
    {
        MyEvents.OnGameStart.RemoveListener(OnGameStart);
        MyEvents.OnGameEnd.RemoveListener(OnGameEnd);
    }

    private void OnGameStart()
    {
        if (activeSawCoroutine == null) activeSawCoroutine = StartCoroutine(ActiveSaw());
    }

    private void OnGameEnd()
    {
        if (activeSawCoroutine != null) StopCoroutine(activeSawCoroutine);
    }

    IEnumerator ActiveSaw()
    {
        while (true)
        {
            switch (sawType)
            {
                case SawType.Normal:
                    mesh.Rotate(Vector3.back, rotateSpeed * Time.deltaTime);
                    break;
            }

            yield return null;
        }
    }
}