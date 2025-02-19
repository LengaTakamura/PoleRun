using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMove : MonoBehaviour
{
    [SerializeField]
    float _speed = 5f;

    private Rigidbody _rb;

    private PlayerState _state;
    
    private CapsuleCollider _capsuleCollider;

    [SerializeField]
    private float _radiusOffset = 0.1f;

    [SerializeField] 
    private LayerMask _layerMask;
    
    [SerializeField]
    private float _poleRunSpeed = 5f;

    private float _angle = 0;

    public bool IsGround;
    
    private bool _onPole = false;

    [FormerlySerializedAs("length")] [SerializeField] 
    private float _length;
    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _capsuleCollider = GetComponent<CapsuleCollider>();
        _rb = GetComponent<Rigidbody>();
        _state = PlayerState.Normal;
    }
    
    private void FixedUpdate()
    {
        GroundCheck();
        StateForUpdate();
        InOutWallRun();
       
    }

    void StateForUpdate()
    {
        var pole = GetPole();
        if(_state == PlayerState.Normal)
        {
            NormalMoving();
            AddGravity();
        }
        else if(_state == PlayerState.Pole) 
        {
            PoleMoving(pole);
        }
    }


    private void NormalMoving()
    {
        var input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;

        transform.position += input * _speed;
    }

    private void PoleMoving(Collider other)
    {
        var pole = other as CapsuleCollider;
        Vector3 localUp = pole.transform.up;
        Vector3 yMovement = localUp * Input.GetAxis("Vertical") * _speed;
         _angle = Input.GetAxisRaw("Horizontal") * _poleRunSpeed ;
         float radian = -_angle * Mathf.Deg2Rad;
         var newPosition = transform.position +yMovement;
         transform.position = newPosition;
        transform.RotateAround(pole.transform.position, Vector3.up, radian);
        
    }

    private Collider GetPole()
    {
        var bottom = transform.position - Vector3.up * (_capsuleCollider.height / 2 + _capsuleCollider.radius) + _capsuleCollider.center;
        var top = transform.position + Vector3.up * (_capsuleCollider.height / 2 - _capsuleCollider.radius) + _capsuleCollider.center;
        var hits = Physics.OverlapCapsule(bottom, top, _capsuleCollider.radius + _radiusOffset, _layerMask);
        if (hits.Length > 0)
        {
            _onPole = true;
            return hits[0];
        }
        else
        {
            _onPole = false;
            return null;
        }
           
        

    }
    private void AddGravity()
    {
        _rb.AddForce(Vector3.down, ForceMode.Acceleration);
    }

    private void InOutWallRun()
    {
        if (_onPole)
        {
            if (Input.GetKey(KeyCode.W) && IsGround)
            {
                _state = PlayerState.Pole;
            }
            else if (Input.GetKey(KeyCode.S)&& IsGround)
            {
               
                _state = PlayerState.Normal;
            }
           
        }
        
      
    }
    
    void GroundCheck()
    {
        IsGround = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down).normalized * _length, out RaycastHit hit, 1.2f);
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down).normalized * _length, Color.magenta);
    
    }

    // private bool WallCheck()
    // {
    //     bool wallForward = Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 0.5f);
    //     Debug.DrawRay(transform.position, transform.forward, Color.white);
    //     return wallForward;
    // }

    public enum PlayerState
    {
        Normal,Pole,Air    
    }

}

