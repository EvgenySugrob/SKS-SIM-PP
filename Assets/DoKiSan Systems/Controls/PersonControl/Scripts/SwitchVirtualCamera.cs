using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

namespace DoKiSan.Controls
{
    public class SwitchVirtualCamera : MonoBehaviour
    {
        private Controller inputs;

        [SerializeField] private CinemachineVirtualCamera[] virtualCameras;
        private int currentCameraIndex;

        private void OnEnable()
        {
            inputs.Enable();

            inputs.Cinemachine.SwitchCamera.performed += Switch_performed;//Create an event subscription "SwitchCamera" to handle pressing
        }

        private void OnDisable()
        {
            inputs.Cinemachine.SwitchCamera.performed -= Switch_performed;//Delete an event subscription "SwitchCamera" to handle pressing

            inputs.Disable();
        }

        private void Awake()
        {
            inputs = new Controller();
        }

        private void Switch_performed(InputAction.CallbackContext obj)
        {
            virtualCameras[currentCameraIndex].gameObject.SetActive(false);
            currentCameraIndex++;

            if (currentCameraIndex >= virtualCameras.Length)
                currentCameraIndex = 0;

            virtualCameras[currentCameraIndex].gameObject.SetActive(true);
        }
    }
}