using UnityEngine;

public class CharacterControl : MonoBehaviour
{
    [SerializeField]
    private CharacterController _charCtrl = null;

    [SerializeField]
    private float _acceleration = 5f;
    [SerializeField]
    private float _maxRunningSpeed = (30.0f * 1000) / (60 * 60); // [m/s], 30 km/h

    [SerializeField]
    [Tooltip("Drag can be used to slow down an object. The higher the drag the more the object slows down.")]
    private float _dragOnGround = 0f;

    [SerializeField]
    [Tooltip("Total height the jump should reach.")]
    private float _jumpHeight = 2;

    private Vector3 _jumpForce;
    private bool _jump;

    [SerializeField]
    private bool _detectCollisions = true;
    [SerializeField]
    private bool _enableOverlapRecovery = true;
    

    [Space]
    [Header("Debug properties")]

    [SerializeField]
    private bool _callMoveFunction = true;

    [SerializeField]
    private bool _showSkinWidth = false;

    [SerializeField]
    private bool _debugOnControllerColliderHit = false;

    [SerializeField]
    private bool _debugIsGrounded = false;


    private Vector3 _velocity;
    private Vector3 _inputVector;

    private void Start()
    {
        _charCtrl.detectCollisions = _detectCollisions;
        _charCtrl.enableOverlapRecovery = _enableOverlapRecovery;

        // Calculate the force it would take to reach the _jumpheight
        #region Calculate jump force info
        // https://en.wikipedia.org/wiki/Equations_of_motion
        // we find the formula: v ^ 2 = v0 ^ 2 + 2 * a(r - r0) where:
        // v = the final speed = 0
        // v0 = the speed we start with, the value we need to find
        // a = out acceleration = -9.81
        // r = end position = 1
        // r0 = start position = 0
        /* This means we can actually find that v0 = sqrt(2 * 9.81 * 1).Notice we didn't take - 9.81 as value, but +
        9.81.A square root of a negative number would not work out. This leads to the following code. Notice
        how we reset the _jump variable since we want to avoid applying the jump multiple times.*/
        // YOU DO NOT NEED TO MEMORISE THIS!
        #endregion
        _jumpForce = -Physics.gravity.normalized * Mathf.Sqrt(2 * Physics.gravity.magnitude * _jumpHeight);
    }

    void Update()
    {
        _inputVector.x = Input.GetAxis("Horizontal");
        _inputVector.z = Input.GetAxis("Vertical");

        if ( Input.GetButtonDown("Jump"))
        {
            _jump = true;
        }
    }

    private void FixedUpdate()
    {
        if (_callMoveFunction)
        {
            ApplyGravity();
            ApplyMovement();
            ApplyGroundDrag();
            ApplySpeedLimitation();
            ApplyJump();
            _charCtrl.Move(_velocity * Time.deltaTime);
        }

        if (_debugIsGrounded)
        {
            Debug.Log(_charCtrl.isGrounded);
        }
    }

    private void ApplyMovement()
    {
        // This is one way of applying input
        // Different ways: forward of the camera, world axis, ...
        Vector3 v = _charCtrl.transform.rotation * _inputVector; // We rotate/transform the input vector according to forward vector (or rotation in this case)

        // F(= m.a) [m/s^2] * t [s]
        // In this case we discard our Mass
        _velocity += v * _acceleration;
        // Another way: discard acceleration, so ApplyGroundDrag can be disabled
        // All depends on the game feel / design
        //_velocity.x = v.x;
        //_velocity.z = v.z;
    }

    private void ApplyGroundDrag()
    {
        if (_charCtrl.isGrounded)
        {
            _velocity *= (1 - Time.deltaTime * _dragOnGround);
        }
    }

    private void ApplySpeedLimitation()
    {
        float tempY = _velocity.y;

        _velocity.y = 0;
        _velocity = Vector3.ClampMagnitude(_velocity, _maxRunningSpeed);

        _velocity.y = tempY;
    }

    private void ApplyGravity()
    {
        if( _charCtrl.isGrounded)
        {
            //_velocity -= Vector3.Project(_velocity, Physics.gravity.normalized);
            _velocity.y = Physics.gravity.y * _charCtrl.skinWidth;
        }
        else
        {
            // g[m/s^2] * t[s]
            _velocity.y += Physics.gravity.y * Time.deltaTime;
        }
    }

    private void ApplyJump()
    {
        if( _jump && _charCtrl.isGrounded)
        {
            // We add the jumpforce, calculated in the Start function
            _velocity += _jumpForce;
        }
        _jump = false;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!_debugOnControllerColliderHit) return;

        Debug.Log("CollisionFlag: " + _charCtrl.collisionFlags);
        Debug.Log("Collider: " + hit.collider.name);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision Enter: " + collision.collider.name);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Enter: " + other.name);
    }

    #region For debugging purpose
    private void OnDrawGizmos()
    {
        if (_charCtrl == null || !_showSkinWidth) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(this.transform.position, _charCtrl.skinWidth + _charCtrl.radius);
    }

    private void OnValidate()
    {
        _charCtrl.detectCollisions = _detectCollisions;
        _charCtrl.enableOverlapRecovery = _enableOverlapRecovery;
    }
    #endregion
}
