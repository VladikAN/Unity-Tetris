using System;
using UnityEngine;

public class FitCameraComponent : MonoBehaviour
{
    public float Width;
    private Camera _camera;

    public void LateUpdate()
    {
        if (Width == 0)
        {
            return;
        }

        _camera = _camera ?? GetComponent<Camera>();
        _camera.orthographicSize = Width * Screen.height / Screen.width * 0.5f;
    }
}