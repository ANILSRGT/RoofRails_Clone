using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraFollow : MonoBehaviour
{
    [Serializable] enum CameraUpdateMode { Update, FixedUpdate, LateUpdate };

    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float smooth = 0.125f;
    [SerializeField] private bool3 lockPosition = new bool3(false, false, false);
    [SerializeField] private CameraUpdateMode updateMode = CameraUpdateMode.LateUpdate;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        offset = transform.position - target.position;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (updateMode == CameraUpdateMode.Update) SmoothMove();
    }

    private void FixedUpdate()
    {
        if (updateMode == CameraUpdateMode.FixedUpdate) SmoothMove();
    }

    private void LateUpdate()
    {
        if (updateMode == CameraUpdateMode.LateUpdate) SmoothMove();
    }

    private void SmoothMove()
    {
        Vector3 targetPos = target.position + offset;
        if (lockPosition.x) targetPos.x = transform.position.x;
        if (lockPosition.y) targetPos.y = transform.position.y;
        if (lockPosition.z) targetPos.z = transform.position.z;

        Vector3 smoothFollow = Vector3.Lerp(transform.position, targetPos, smooth);

        transform.position = smoothFollow;
    }
}