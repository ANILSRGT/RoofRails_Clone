using System;
using System.Collections;
using System.Collections.Generic;
using GameManagerNamespace;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class CameraFollow : MonoBehaviour
{
    [Serializable] enum CameraUpdateMode { Update, FixedUpdate, LateUpdate };

    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float smooth = 0.125f;
    [SerializeField] private bool3 lockPosition = new bool3(false, false, false);
    [SerializeField] private CameraUpdateMode updateMode = CameraUpdateMode.LateUpdate;

    private Coroutine moveToCoroutine;

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
        if (GameManager.Instance.gameStatus == GameStatus.PLAY)
        {

            Vector3 targetPos = target.position + offset;
            if (lockPosition.x) targetPos.x = transform.position.x;
            if (lockPosition.y) targetPos.y = transform.position.y;
            if (lockPosition.z) targetPos.z = transform.position.z;

            Vector3 smoothFollow = Vector3.Lerp(transform.position, targetPos, smooth);

            transform.position = smoothFollow;
        }
    }

    public void MoveTo(Transform to, float delay, float seconds, UnityAction callback)
    {
        if (moveToCoroutine != null) StopCoroutine(moveToCoroutine);
        moveToCoroutine = StartCoroutine(MoveToCoroutine(transform, to, delay, seconds, callback));
    }

    private IEnumerator MoveToCoroutine(Transform from, Transform to, float delay, float seconds, UnityAction callback)
    {
        yield return new WaitForSeconds(delay);
        float elapsedTime = 0;
        while (elapsedTime < seconds)
        {
            transform.position = Vector3.Lerp(from.position, to.position, elapsedTime / seconds);
            transform.rotation = Quaternion.Lerp(from.rotation, to.rotation, elapsedTime / seconds);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        callback?.Invoke();
    }
}