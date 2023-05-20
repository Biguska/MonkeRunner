
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
    [SerializeField] private float _jumpForce = 7f;
    [SerializeField] private float _diveForce = -5f;

    private Rigidbody _rigidbody;
    
    private int _currentPosition = 0;
    private int _oldPosition = 0;
    private bool _isMovingSide = false;
    public Sequence _moveSequence;
    
    public bool _isGround = true;
    private bool _isJumping = false;
    
    public bool _isCroll = false;
    private IEnumerator _crollCoroutine = null;
    private float _crollTime = 1f;

    private void Start()
    {
        _rigidbody = _playerObject.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            if(IsCroll())
                StopScroll();
            MoveToSide(-1);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            if(IsCroll())
                StopScroll();
            MoveToSide(1);
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

    private void MoveToSide(int direction)
    {
        int newSide = _currentPosition + direction;
        var side = _roadsList.Find(road => road.RoadId == newSide);
        if (side != null)
        {
            if (newSide == _oldPosition && _moveSequence != null)
            {
                _moveSequence.Kill();
                _moveSequence = null;
            }
            
            if (_moveSequence == null)
                _moveSequence = DOTween.Sequence();
            
            _oldPosition = _currentPosition;
            _currentPosition = side.RoadId;
            _moveSequence.Append(transform.DOMoveX(side.RoadCoordinate, .25f)
                    .SetEase(Ease.InQuad))
                .Join(_mainCamera.DOMoveX(side.CameraCoordinate, .35f)
                    .SetEase(Ease.InOutSine))
                .OnComplete(() =>
                {
                    _moveSequence = null;
                });
            
            //Нельзя добавлять к секуенсе, которая стартовала уже. Шоб было как в сабвей серфе нужно добавить костыль с очередью секуенцев
        }
            
    }

    private void Jump()
    {
        _rigidbody.AddForce(new Vector3(0f, _jumpForce, 0f), ForceMode.Impulse);
        _isGround = false;
    }

    private void Croll()
    {
        if (!_isGround)
        {
            _rigidbody.AddForce(new Vector3(0f, _diveForce, 0f), ForceMode.Impulse);
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
        _playerModel.localPosition = new Vector3(0f, 0.5f, 0f);
        _playerModel.localScale = new Vector3(1.5f, 1f, 1f);
    }

    private void EndCroll()
    {
        _isCroll = false;
        _playerCollider.center = new Vector3(0f, 1f, 0f);
        _playerCollider.height = 2f;
        _playerModel.localPosition = new Vector3(0f, 1f, 0f);
        _playerModel.localScale = new Vector3(1.5f, 2f, 1f);
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
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isGround = true;
            _rigidbody.velocity = new Vector3(0f, 0f, 0f);
        }

        if (collision.gameObject.CompareTag("Hill"))
        {
            _isGround = true;
        }

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            _chunksGeneratorController.GameOver();
        }
    }
}
