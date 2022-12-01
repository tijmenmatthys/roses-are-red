using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform _playerModel;

    [SerializeField] private float _lookRotation = 90;
    [SerializeField] private float _walkingSpeed = 7;
    [SerializeField] private float _runningSpeed = 10;
    [SerializeField] private float _turningSpeed = 300;
    [SerializeField] private float _globalGravity = 9.81f;

    [Space]
    [Header("Debug properties")]

    [SerializeField] private bool _detectCollisions = true;
    [SerializeField] private bool _enableOverlapRecovery = true;
    [SerializeField] private bool _callMoveFunction = true;
    [SerializeField] private bool _showSkinWidth = false;
    [SerializeField] private bool _debugCollisions = false;
    [SerializeField] private bool _debugIsGrounded = false;

    private CharacterController _charCtrl;
    private Animator _animator;
    private int _flowerLayer;
    private Vector3 _inputVector;
    private Vector3 _previousInputVector;
    [SerializeField] private Vector3 _velocity;
    [SerializeField] private bool _freeze = false;
    [SerializeField] private bool _isMoving = false;
    [SerializeField] private bool _isRunning = false;
    private List<FlowerType> _collidingFlowers = new List<FlowerType>();

    public bool IsMoving
    {
        get { return _isMoving; }
        set
        {
            _isMoving = value;
            _animator.SetBool("IsMoving", value);
        }
    }
    public bool IsRunning
    {
        get { return _isRunning; }
        set
        {
            _isRunning = value;
            _animator.SetBool("IsRunning", value);
        }
    }

    private void Start()
    {
        _flowerLayer = LayerMask.NameToLayer("Flower");
        _animator = _playerModel.GetComponent<Animator>();
        _charCtrl = GetComponent<CharacterController>();
        _charCtrl.detectCollisions = _detectCollisions;
        _charCtrl.enableOverlapRecovery = _enableOverlapRecovery;

        Physics.gravity = new Vector3(0, -_globalGravity, 0);
        _playerModel.localRotation = Quaternion.AngleAxis(180, Vector3.up);
    }

    private void Update()
    {
        RegisterInput();
        RotateModel();
    }

    private void FixedUpdate()
    {
        RotateRoot();

        if (_callMoveFunction && !_freeze)
        {
            ApplyGravity();
            ApplyMovement();
            _charCtrl.Move(_velocity * Time.deltaTime);

            UpdateIsMoving();
            UpdateIsRunning();
        }

        if (_debugIsGrounded)
        {
            Debug.Log(_charCtrl.isGrounded);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == _flowerLayer)
            _collidingFlowers.Add(other.GetComponent<Flower>().Type);
    }

    public void SetFreeze(bool freeze)
    {
        _freeze = freeze;
        if (freeze)
        {
            IsMoving = false;
            _playerModel.rotation = Quaternion.AngleAxis(180, Vector3.up);
        }
    }

    private void RegisterInput()
    {
        _previousInputVector = _inputVector;
        _inputVector.x = Input.GetAxis("Horizontal");
        _inputVector.z = Input.GetAxis("Vertical");
    }

    private void RotateRoot()
    {
        // player forward should be perpendicular to the map center, clockwise
        float rotation = -_lookRotation + Quaternion.LookRotation(transform.position, Vector3.up).eulerAngles.y;
        transform.rotation = Quaternion.AngleAxis(rotation, Vector3.up);
    }
    private void RotateModel()
    {
        float currentRotation = _playerModel.localRotation.eulerAngles.y;

        float targetRotation;
        if (!_freeze && _inputVector != Vector3.zero)
            targetRotation = Quaternion.LookRotation(_inputVector, Vector3.up).eulerAngles.y;
        else if (_freeze)
            targetRotation = 180;
        else
            targetRotation = currentRotation;

        float nextRotation = Mathf.LerpAngle(currentRotation, targetRotation, _turningSpeed * Time.deltaTime);
        _playerModel.localRotation = Quaternion.AngleAxis(nextRotation, Vector3.up);
    }

    private void ApplyMovement()
    {
        // Move relative to player
        float speed = IsRunning ? _runningSpeed : _walkingSpeed;
        Vector3 horizontalMovement = transform.rotation * _inputVector * speed;
        _velocity.x = horizontalMovement.x;
        _velocity.z = horizontalMovement.z;
    }

    private void UpdateIsMoving()
    {
        // to make animations smooth, IsMoving already turns off when input is decreasing
        IsMoving = _previousInputVector.magnitude <= _inputVector.magnitude;
        if (_inputVector.magnitude == 0) IsMoving = false;
        if (_inputVector.magnitude >= .9) IsMoving = true;
    }

    private void UpdateIsRunning()
    {
        IsRunning = false;
        foreach (FlowerType type in _collidingFlowers)
        {
            if (GameManager.Instance.IsAreaComplete(type))
                IsRunning = true;
        }
        _collidingFlowers.Clear();
    }

    private void ApplyGravity()
    {
        if (_charCtrl.isGrounded)
        {
            _velocity.y = Physics.gravity.y * Time.deltaTime;//_charCtrl.skinWidth;
        }
        else
        {
            // v = a * t1-t0
            _velocity.y += Physics.gravity.y * Time.deltaTime;
        }
    }

    #region For debugging purpose
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!_debugCollisions) return;
        Debug.Log("ControllerColliderHit - CollisionFlag: " + _charCtrl.collisionFlags);
        Debug.Log("ControllerColliderHit - Collider: " + hit.collider.name);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!_debugCollisions) return;
        Debug.Log("Collision Enter: " + collision.collider.name);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_debugCollisions) return;
        Debug.Log("Trigger Enter: " + other.name);
    }

    private void OnDrawGizmos()
    {
        if (_charCtrl == null || !_showSkinWidth) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(this.transform.position, _charCtrl.skinWidth + _charCtrl.radius);
    }

    private void OnValidate()
    {
        _charCtrl ??= GetComponent<CharacterController>();
        _charCtrl.detectCollisions = _detectCollisions;
        _charCtrl.enableOverlapRecovery = _enableOverlapRecovery;
    }
    #endregion
}
