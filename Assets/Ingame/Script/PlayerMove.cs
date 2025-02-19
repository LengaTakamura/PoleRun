using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField]
    float _speed = 5f;

    Rigidbody _rb;

    PlayerState _state;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _rb = GetComponent<Rigidbody>();
        _state = PlayerState.Normal;
    }
    
    private void FixedUpdate()
    {
        StateForUpdate();
        InOutWallRun();
    }

    void StateForUpdate()
    {
        if(_state == PlayerState.Normal)
        {
            NormalMoving();
            AddGravity();
        }
        else if(_state == PlayerState.Pole) 
        {
            PoleMoving();
        }
    }


    private void NormalMoving()
    {
        var input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;

        transform.position += input * _speed;
    }

    private void PoleMoving()
    {

    }

    private void AddGravity()
    {
        _rb.AddForce(Vector3.down, ForceMode.Acceleration);
    }

    private void InOutWallRun()
    {
        if (WallCheck())
        {
            if (Input.GetKey(KeyCode.W))
            {
                _rb.isKinematic = true;
                _state = PlayerState.Pole;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                _rb.isKinematic = false;
                _state = PlayerState.Normal;
            }
           
        }
        
      
    }

    private bool WallCheck()
    {
        bool wallForward = Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 0.5f);
        Debug.DrawRay(transform.position, transform.forward, Color.white);
        return wallForward;
    }

    public enum PlayerState
    {
        Normal,Pole,Air    
    }

}

