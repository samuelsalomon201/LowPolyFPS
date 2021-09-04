using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    [SerializeField] private Transform target;

    private float startFOV, targetFOV;

    [SerializeField] private float zoomSpeed = 7.0f;

    [SerializeField] private Camera camera;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        startFOV = camera.fieldOfView;
        targetFOV = startFOV;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = target.position;
        transform.rotation = target.rotation;

        camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, targetFOV, zoomSpeed * Time.deltaTime);
    }

    public void ZoomIn(float newZoom)
    {
        targetFOV = newZoom;
    }

    public void ZoomOut()
    {
        targetFOV = startFOV;
    }
}
