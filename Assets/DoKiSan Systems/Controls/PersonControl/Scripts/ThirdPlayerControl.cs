using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

namespace DoKiSan.Controls
{
    public class ThirdPlayerControl : MonoBehaviour
    {
        private Controller inputs;

        [Header("Camera movement")]
        [SerializeField] private float characterSpeed = 2f;
        private Vector3 targetPosition;

        [Header("Camera rotation. Rotation (left-right)")]
        [SerializeField] private Transform bodyPivot;
        [SerializeField] private float rotationSpeed = 1f;
        [SerializeField] private bool invertRotation;

        private float bodyRotation;

        [Header("Camera rotation. Pitch (up-down)")]
        [SerializeField] private Transform headPivot;
        [SerializeField] private float speedPitch = 1f;
        [SerializeField] float minViewRange, maxViewRange;
        [SerializeField] private bool invertPitch;

        private float headPitch;

        [Header("CinemachineVirtualCamera zoom")]
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private float zoomSpeed = 32f;
        [SerializeField] private float minZoomDistance = 0.1f;
        [SerializeField] private float maxZoomDistance = 5f;
        [SerializeField] private bool invertZoom;

        private CinemachineFramingTransposer framingTransposer;

        private bool rotatePressed = false;

        private void OnEnable()
        {
            inputs.Enable();

            inputs.Cinemachine.Rotate.performed += Rotate_performed;//Create an event subscription "Rotate" to handle pressing
            inputs.Cinemachine.Rotate.canceled += Rotate_canceled;//Create an event subscription "Rotate" to handle canceled
            inputs.Cinemachine.Zoom.performed += Zoom_performed;//Create an event subscription "Zoom" to handle pressing
        }



        private void OnDisable()
        {
            inputs.Cinemachine.Rotate.performed -= Rotate_performed;//Delete an event subscription "Rotate" to handle pressing
            inputs.Cinemachine.Rotate.canceled -= Rotate_canceled;//Delete an event subscription "Rotate" to handle canceled
            inputs.Cinemachine.Zoom.performed -= Zoom_performed;//Delete an event subscription "Zoom" to handle pressing

            inputs.Disable();
        }

        private void Awake()
        {
            inputs = new Controller();

            framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }

        void FixedUpdate()
        {
            PlayerMovement();
            PlayerRotate();
        }


        private void PlayerMovement()
        {
            Vector2 inputValue = inputs.Cinemachine.Move.ReadValue<Vector2>();

            Vector3 direction = inputValue.x * GetPivotRight() + inputValue.y * GetPivotForward();
            direction = direction.normalized;

            targetPosition += direction;
            bodyPivot.localPosition += targetPosition * characterSpeed * Time.fixedDeltaTime;

            targetPosition = Vector3.zero;
        }

        private Vector3 GetPivotForward()
        {
            Vector3 forward = bodyPivot.forward;
            forward.y = 0;
            return forward;
        }     
        
        private Vector3 GetPivotRight()
        {
            Vector3 right = bodyPivot.right;
            right.y = 0;
            return right;
        }
        private void PlayerRotate()
        {
            if (rotatePressed)
            {
                Vector2 targetRotation = inputs.Cinemachine.DeltaPointer.ReadValue<Vector2>();

                headPitch += targetRotation.y * speedPitch * Time.fixedDeltaTime;
                headPitch = ClampAngle(headPitch, minViewRange, maxViewRange);

                bodyRotation = targetRotation.x * rotationSpeed * Time.fixedDeltaTime;

                bodyPivot.localRotation = Quaternion.Euler((invertPitch ? headPitch : -headPitch), bodyPivot.localRotation.eulerAngles.y
                    + ((invertRotation ? -bodyRotation : bodyRotation)), 0.0f);
            }
        }

        private void Rotate_performed(InputAction.CallbackContext obj)
        {
            Cursor.lockState = CursorLockMode.Locked; //Lock a cursor for normal rotation
            Cursor.visible = false;
            rotatePressed = true;

        }
        private void Rotate_canceled(InputAction.CallbackContext obj)
        {
            Cursor.lockState = CursorLockMode.None; //Lock a cursor for normal rotation
            Cursor.visible = true;
            rotatePressed = false;
        }

        private static float ClampAngle(float needClampAngle, float minAngleAxis, float maxAngleAxis)
        {
            if (needClampAngle < -360f) needClampAngle += 360f;
            if (needClampAngle > 360f) needClampAngle -= 360f;
            return Mathf.Clamp(needClampAngle, minAngleAxis, maxAngleAxis);
        }

        //Perspective
        private void Zoom_performed(InputAction.CallbackContext obj)
        {
            float zoomCamera = obj.ReadValue<float>();
            framingTransposer.m_CameraDistance = Mathf.Clamp(framingTransposer.m_CameraDistance + (invertZoom ? zoomCamera : -zoomCamera) / zoomSpeed, minZoomDistance, maxZoomDistance);
        }
    }
}