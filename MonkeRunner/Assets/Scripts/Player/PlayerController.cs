
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public class Road
    {
        private int _roadId;
        private int _roadCoordinate;
        private int _cameraCoordinate;

        public Road(int roadId, int roadCoordinate, int cameraCoordinate)
        {
            _roadId = roadId;
            _roadCoordinate = roadCoordinate;
            _cameraCoordinate = cameraCoordinate;
        }

        public int RoadId
        {
            get { return _roadId; }
            set { _roadId = value; }
        }

        public int RoadCoordinate
        {
            get { return _roadCoordinate; }
            set { _roadCoordinate = value; }
        }
        
        public int CameraCoordinate
        {
            get { return _cameraCoordinate; }
            set { _cameraCoordinate = value; }
        }
    }
    /*
     left -1
     midle 0
     right 1
     */
    private List<Road> _roadsList = new List<Road>
    {
        new Road(-1, -4,-3),
        new Road(0, 0,0),
        new Road(1, 4,3)
    };

    [SerializeField] private Transform _mainCamera;
    [SerializeField] private ChunksGeneratorController _chunksGeneratorController;
    [SerializeField] private GameObject _playerObject;
    [SerializeField] private CapsuleCollider _playerCollider;
    [SerializeField] private Transform _playerModel;
    [SerializeField] private float _jumpForce = 57f;
    [SerializeField] private float _diveForce = 90f;

    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _groundDistance = 0.05f;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private LayerMask _hillMask;
    
    [SerializeField] private Animator _animator;
    public int _bananaScore = 0;
    
    //Хуйня для движения
    private Rigidbody _rigidbody;
    private int _currentPosition = 0;
    private int _oldPosition = 0;
    private bool _isMovingSide = false;
    private Sequence _moveSequence;
    public bool _isGround = true;
    private bool _isJumping = false;
    private bool _isCameraDown = true;
    private bool _isGettingHit = false;
    public bool _isCroll = false;
    private IEnumerator _crollCoroutine = null;
    private float _crollTime = 1f;
    private float _dashTime = .25f;

    private void Start()
    {
        _rigidbody = _playerObject.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _isGround = Physics.CheckSphere(_groundCheck.position, _groundDistance, _groundMask);

        if (_isGround && _rigidbody.velocity.y == 0f)
            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, -1f, _rigidbody.velocity.z);

        if (transform.localPosition.y > 2.6f && _isCameraDown)
        {
            _isCameraDown = false;
            _mainCamera.DOKill();
            _mainCamera.DOMoveY(9.75f, .35f)
                .SetEase(Ease.InOutSine);
        }

        if (transform.localPosition.y < 2.6f && !_isCameraDown)
        {
            _isCameraDown = true;
            _mainCamera.DOKill();
            _mainCamera.DOMoveY(7.5f, .35f)
                .SetEase(Ease.InOutSine);
        }
        
        if (Input.GetKeyDown(KeyCode.A))
        {
            if(IsCroll())
                StopScroll();
            MoveToSide(-1, false);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            if(IsCroll())
                StopScroll();
            MoveToSide(1, false);
        }

        if (Input.GetKeyDown(KeyCode.Space) && _isGround)
        { 
            if(IsCroll())
                StopScroll();
            Jump();
        }
        
        if (Input.GetKeyDown(KeyCode.S) && !_isCroll)
        { 
            if(!IsCroll())
                Croll();
        }
    }

    private void MoveToSide(int direction, bool blockMovement)
    {
        int newSide = _currentPosition + direction;
        var side = _roadsList.Find(road => road.RoadId == newSide);
        if (side != null && !_isGettingHit)
        {
            if (blockMovement)
                _isGettingHit = true;
            
            if (newSide == _oldPosition && _moveSequence != null)
            {
                _moveSequence.Kill();
                _moveSequence = null;
            }
            
            if (_moveSequence == null)
                _moveSequence = DOTween.Sequence();
            
            _oldPosition = _currentPosition;
            _currentPosition = side.RoadId;
            _moveSequence.Append(transform.DOMoveX(side.RoadCoordinate, _dashTime)
                    .SetEase(Ease.InSine))
                .Join(_mainCamera.DOMoveX(side.CameraCoordinate, _dashTime + 0.1f)
                    .SetEase(Ease.InOutSine))
                .OnComplete(() =>
                {
                    if (_isGettingHit)
                        _isGettingHit = false;
                    _moveSequence = null;
                });
            //Нельзя добавлять к секуенсе, которая стартовала уже. Шоб было как в сабвей серфе нужно добавить костыль с очередью секуенцев
        }
            
    }

    private void Jump()
    {
        _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
    }

    private void Croll()
    {
        if (!_isGround)
        {
            _rigidbody.AddForce(Vector3.down * _diveForce, ForceMode.Impulse);
        }

        _crollCoroutine = CrollCoroutine();
        StartCoroutine(_crollCoroutine);
    }

    private IEnumerator CrollCoroutine()
    {
        StartCroll();
        yield return new WaitForSeconds(_crollTime);
        EndCroll();
    }

    private void StartCroll()
    {
        _isCroll = true;
        _playerCollider.center = new Vector3(0f, 0.5f, 0f);
        _playerCollider.height = 1f;
        _playerModel.localPosition = new Vector3(0f, 0.675f, 0f);
        _playerModel.localScale = new Vector3(1.5f, 1.25f, 1f);
    }

    private void EndCroll()
    {
        _isCroll = false;
        _playerCollider.center = new Vector3(0f, 1f, 0f);
        _playerCollider.height = 2f;
        _playerModel.localPosition = new Vector3(0f, 1.25f, 0f);
        _playerModel.localScale = new Vector3(1.5f, 2.5f, 1f);
        _crollCoroutine = null;
    }

    private bool IsCroll()
    {
        if (_crollCoroutine == null)
            return false;
        else
            return true;
    }

    private void StopScroll()
    {
        StopCoroutine(_crollCoroutine);
        EndCroll();
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("Game over");
            _chunksGeneratorController.GameOver();
        }
        
        if (collision.gameObject.CompareTag("Hit") && !_isGettingHit)
        {
            Debug.Log("Get hit");
            if(_currentPosition > _oldPosition)
                MoveToSide(-1, true);
            else
                MoveToSide(1, true);
        }

        if (collision.gameObject.CompareTag("Banana"))
        {
            var banana = collision.gameObject.GetComponent<BananaController>();
            banana.Disable();
            _bananaScore++;
        }
    }
}
