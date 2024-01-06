using System;
using Scripts.Input;
using UnityEngine;

namespace Scripts
{
    public class CameraLogic : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private float _speedMoving = 50;
        [SerializeField] private float _smoothMoving = 5;
        private Controls _input;
        private bool _isMoving;

        private Vector3 _center = Vector3.zero;
        private float _right = 10;
        private float _left = 10;
        private float _up = 10;
        private float _down = 10;
        private float _angle = 45;
        
        private Transform _root;
        private Transform _pivot;
        private Transform _target;
        
        private void Awake()
        {
            _input = new Controls();
            _root = new GameObject("CameraHelper").transform;
            _pivot = new GameObject("CameraPivot").transform;
            _target = new GameObject("CameraTarget").transform;

            _camera.orthographic = true;
            _camera.nearClipPlane = 0;
        }

        private void Start()
        {
            Initialize(Vector3.zero, 10, 10, 10, 10, 45);
        }

        private void Initialize(Vector3 center, float right, float left, float up, float down, float angle)
        {
            _right = right;
            _left = left;
            _up = up;
            _down = down;
            _angle = angle;
            
            _isMoving = false;
            
            _pivot.SetParent(_root);
            _target.SetParent(_pivot);
            
            _root.position = _center;
            _root.localEulerAngles = Vector3.zero;

            _pivot.localPosition = Vector3.zero;
            _pivot.localEulerAngles = new Vector3(_angle, 0, 0);
            
            _target.localPosition = new Vector3(0, 0, -10);
            _target.localEulerAngles = Vector3.zero;
        }

        private void OnEnable()
        {
            _input.Enable();
            _input.Main.Move.started += _ => MoveStarted();
            _input.Main.Move.canceled += _ => MoveCanceled();
        }

        private void OnDisable()
        {
            _input.Disable();
        }

        private void Update()
        {
            if (_isMoving)
            {
                Vector2 move = _input.Main.MoveDelta.ReadValue<Vector2>();
                if (move != Vector2.zero)
                {
                    move.x /= Screen.width;
                    move.y /= Screen.height;

                    _root.position -= _root.right.normalized * move.x * _speedMoving;
                    _root.position -= _root.forward.normalized * move.y * _speedMoving;
                }
            }

            if (_camera.transform.position != _target.position)
            {
                _camera.transform.position = Vector3.Lerp(_camera.transform.position, _target.position, _smoothMoving * Time.deltaTime);
            }
            
            if (_camera.transform.rotation != _target.rotation)
            {
                _camera.transform.rotation = _target.rotation;
            }
        }

        private void MoveStarted()
        {
            _isMoving = true;
        }
        
        private void MoveCanceled()
        {
            _isMoving = false;
        }
    }
}