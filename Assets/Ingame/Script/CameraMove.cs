using System;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField]
    float _mouseSensitivity;

    private GameObject _player;

    private float _rotationX; 
    private float _rotationY;

    [SerializeField] private Vector3 _cameraOffset;
    
    [SerializeField]
    private float _speed;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        RotateCamera();
        FollowCamera();
    }

    private void RotateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X")  * _mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y")  * _mouseSensitivity;
        _rotationX -= mouseY;
        _rotationX = Mathf.Clamp(_rotationX, -90f, 90f);
        _rotationY += mouseX;
        transform.localRotation = Quaternion.Euler(_rotationX, _rotationY, 0f);
    }

    private void FollowCamera()
    {
        var position = _player.transform.position + _cameraOffset;
        transform.position = Vector3.Lerp(transform.position, position, _speed * Time.deltaTime);
    }
}
