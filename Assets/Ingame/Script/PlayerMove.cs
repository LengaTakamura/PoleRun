using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField]
    float _speed = 5f;

    Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        Moving();
    }
    private void Moving()
    {
        var input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;

        transform.position += input * _speed;
    }

    private void AddGravity()
    {
        _rb.AddForce(Vector3.down, ForceMode.Acceleration);
    }
}
