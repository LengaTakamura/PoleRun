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
        AddGravity();
        InOutWallRun();
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

    private void InOutWallRun()
    {
        if (WallCheck() && Input.GetKey(KeyCode.S))
        {
            _rb.isKinematic = false;
           
        }
        else if (WallCheck() && Input.GetKey(KeyCode.W))
        {
            _rb.isKinematic = true;
        }
    }

    private bool WallCheck()
    {
        bool wallForward = Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 0.5f);
        return wallForward;
    }
}
