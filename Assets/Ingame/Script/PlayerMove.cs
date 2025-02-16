using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField]
    float _speed = 5f;


    private void FixedUpdate()
    {
        Moving();
    }
    private void Moving()
    {
        var input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;

        transform.position += input * _speed;
    }
}
