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

    
    private bool _isGround = false;

    public bool IsGround
    {
        get => _isGround;
        set
        {
            if (_isGround != value)
            {
                _isGround = value;
                if (value)
                {
                    _state = PlayerState.Normal;
                }
            }
            
        }
    }

    private bool _onPole ;

    public bool OnPole
    {
        get
        {
            return _onPole;
        }
        set
        {
            if (_onPole != value)
            {
                _onPole = value;
                if (value)
                {
                    _state = PlayerState.Pole;
                }
            }
        }
    }

    [FormerlySerializedAs("length")] [SerializeField] 
    private float _length;
    
    [SerializeField]
    private float _jumpForce = 5f;
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
        ChangeState();
       
    }

    void StateForUpdate()
    {
        var pole = GetPole();
       
        if(_state != PlayerState.Pole)
        {
            NormalMoving();
            AddGravity();
        }
        else if(_state == PlayerState.Pole && pole != null) 
        {
            var normal = transform.position - pole.ClosestPoint(transform.position);
            normal.Normalize();
            PoleMoving(pole);
            PoleJump();
        }
    }

    private void NormalMoving()
    {
        var input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;

        transform.position += input * _speed;

        if (input == Vector3.zero)
        {
            return;
        }
        var rot = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(input), 10f);
        transform.rotation = rot;
    }

    private void PoleMoving(Collider other)
    {
        Vector3 prevPos = transform.position;
        var pole = other as CapsuleCollider;
        Vector3 localUp = pole.transform.up;
        Vector3 yMovement = localUp * Input.GetAxis("Vertical") * _speed;
         _angle = Input.GetAxisRaw("Horizontal") * _poleRunSpeed ;
         float radian = -_angle * Mathf.Deg2Rad;
         var newPosition = transform.position +yMovement;
         transform.position = newPosition;
        transform.RotateAround(pole.transform.position, Vector3.up, radian);
        Vector3 dir = (transform.position - prevPos).normalized;
        if (dir != Vector3.zero)
        {
            var rot =  Quaternion.LookRotation(dir, Vector3.up);
            rot.x = 0;
            rot.z = 0;
            transform.rotation = rot;
        }
        
    }

    private Collider GetPole()
    {
        var bottom = transform.position - Vector3.up * (_capsuleCollider.height / 2 + _capsuleCollider.radius) + _capsuleCollider.center;
        var top = transform.position + Vector3.up * (_capsuleCollider.height / 2 - _capsuleCollider.radius) + _capsuleCollider.center;
        var hits = Physics.OverlapCapsule(bottom, top, _capsuleCollider.radius + _radiusOffset, _layerMask);
        if (hits.Length > 0)
        {
            OnPole = true;
            return hits[0];
        }
        else
        {
            OnPole = false;
            return null;
        }
           
        

    }
    private void AddGravity()
    {
        _rb.AddForce(Vector3.down, ForceMode.Acceleration);
    }

    private void ChangeState()
    {
        if (OnPole)
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

    void PoleJump()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Debug.Log(transform.forward);
            _rb.AddForce(transform.forward * _jumpForce, ForceMode.Acceleration);
            _state = PlayerState.Air;
        }
    }
    public enum PlayerState
    {
        Normal,Pole,Air    
    }

}

