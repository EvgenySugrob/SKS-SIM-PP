using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

namespace DoKiSan.Controls
{
    public class OrtographicControl : MonoBehaviour
    {
        private Controller inputs;

        [SerializeField] private CinemachineVirtualCamera virtualCamera;

        [Header("Camera movement")]
        [SerializeField] private Transform bodyPivot;
        [SerializeField] private float characterSpeed = 2f;
        [SerializeField] private bool planeXZ;

        private Vector3 targetPosition;
        private Vector3 targetDirection;

        [Header("CinemachineVirtualCamera zoom")]
        [SerializeField] private float zoomSpeed = 32f;
        [SerializeField] private float minZoomDistance = 0.1f;
        [SerializeField] private float maxZoomDistance = 5f;
        [SerializeField] private bool invertZoom;

        private void OnEnable()
        {
            inputs.Enable();
            inputs.Cinemachine.Zoom.performed += Zoom_performed;//Create an event subscription "Zoom" to handle pressing
        }

        private void OnDisable()
        {
            inputs.Cinemachine.Zoom.performed -= Zoom_performed;//Delete an event subscription "Zoom" to handle pressing
            inputs.Disable();
        }

        private void Awake()
        {
            inputs = new Controller();
        }

        void FixedUpdate()
        {
            PlayerMovement();
        }

        private void PlayerMovement()
        {
            Vector2 inputValue = inputs.Cinemachine.Move.ReadValue<Vector2>();

            if (planeXZ)
                targetDirection = new Vector3(inputValue.x, 0, inputValue.y).normalized;
            else
                targetDirection = new Vector3(inputValue.x, inputValue.y, 0).normalized;

            targetPosition += targetDirection;
            bodyPivot.localPosition += targetPosition * characterSpeed * Time.fixedDeltaTime;

            targetPosition = Vector3.zero;
        }


        //Ortographic
        private void Zoom_performed(InputAction.CallbackContext obj)
        {
            float zoomCamera = obj.ReadValue<float>();
            virtualCamera.m_Lens.OrthographicSize = Mathf.Clamp(virtualCamera.m_Lens.OrthographicSize + (invertZoom ? zoomCamera : -zoomCamera) / zoomSpeed, minZoomDistance, maxZoomDistance);
        }
    }
}
