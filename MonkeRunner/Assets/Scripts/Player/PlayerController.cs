using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public class Road
    {
        private int _roadId;
        private int _roadCoordinate;

        public Road(int roadId, int roadCoordinate)
        {
            _roadId = roadId;
            _roadCoordinate = roadCoordinate;
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
    }
    
    [SerializeField] private GameObject _playerObject;
    [SerializeField] private CapsuleCollider _playerCollider;
    [SerializeField] private SphereCollider _jumpTriger;

    /*
     left -1
     midle 0
     right 1
     */
    private List<Road> _roadsList = new List<Road>
    {
        new Road(-1, -2),
        new Road(0, 0),
        new Road(1, 2)
    };

    private Rigidbody _rigidbody;
    private Transform _transform;
    
    private int _currentPosition = 0;
    private bool _isGround = true;
    private bool _isJumping = false;
    private bool _isMovingSide = false;

    private void Start()
    {
        _rigidbody = _playerObject.GetComponent<Rigidbody>();
        _transform = _playerObject.GetComponent<Transform>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            MoveToSide(-1);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            MoveToSide(1);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(TryToJump()) Jump();
        }
    }

    private void MoveToSide(int direction)
    {
        int newSide = _currentPosition + direction;
        var side = _roadsList.Find(road => road.RoadId == newSide);
        if (side != null)
        {
            _currentPosition = side.RoadId;
            _transform.position = new Vector3(side.RoadCoordinate, _transform.position.y, _transform.position.z);
        }
    }

    private bool TryToJump()
    {
        if (_isGround && !_isJumping)
            return true;
        else
            return false;
    }

    private void Jump()
    {
        _rigidbody.AddForce(new Vector3(0, 5, 0), ForceMode.Impulse);
    }
}
