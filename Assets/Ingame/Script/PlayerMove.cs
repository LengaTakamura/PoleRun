using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMove : MonoBehaviour
{
    [SerializeField]
    float _speed = 5f;

    Rigidbody _rb;

    PlayerState _state;
    
    CapsuleCollider _capsuleCollider;

    [FormerlySerializedAs("_radiusOffset")] [SerializeField]
    private float radiusOffset = 0.1f;

    [FormerlySerializedAs("_layerMask")] [SerializeField] 
    private LayerMask layerMask;
    
    [SerializeField]
    float poleRunSpeed = 5f;

    private float _angle = 0;

    public bool IsGround;

    [FormerlySerializedAs("_length")] [SerializeField] 
    private float length;
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
        var collider = GetWall();
        var pole = collider as CapsuleCollider;
        Vector3 localUp = pole.transform.up;
        Vector3 yMovement = localUp * Input.GetAxis("Vertical") * _speed;
         _angle = Input.GetAxisRaw("Horizontal") * poleRunSpeed ;
         float radian = -_angle * Mathf.Deg2Rad;
         var newPosition = transform.position +yMovement;
         Debug.Log(newPosition);
         transform.position = newPosition;
        transform.RotateAround(pole.transform.position, Vector3.up, radian);
        
    }

    private Collider GetWall()
    {
        var bottum = transform.position - Vector3.up * (_capsuleCollider.height / 2 + _capsuleCollider.radius) + _capsuleCollider.center;
        var top = transform.position + Vector3.up * (_capsuleCollider.height / 2 - _capsuleCollider.radius) + _capsuleCollider.center;
        var hits = Physics.OverlapCapsule(bottum, top, _capsuleCollider.radius + radiusOffset, layerMask);
        try
        {
            Debug.Log(hits[0].gameObject.name);
            return hits[0];
        }
        catch
        {
            return null;    
        }

    }
    private void AddGravity()
    {
        _rb.AddForce(Vector3.down, ForceMode.Acceleration);
    }

    private void InOutWallRun()
    {
        if (WallCheck())
        {
            if (Input.GetKey(KeyCode.W) && IsGround)
            {
                _rb.isKinematic = true;
                _state = PlayerState.Pole;
            }
            else if (Input.GetKey(KeyCode.S)&& IsGround)
            {
                _rb.isKinematic = false;
                _state = PlayerState.Normal;
            }
           
        }
        
      
    }
    
    void GroundCheck()
    {
        IsGround = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down).normalized * length, out RaycastHit hit, 1.2f);
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down).normalized * length, Color.magenta);

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

