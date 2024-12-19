using UnityEngine;
using UnityEngine.InputSystem;

namespace DoKiSan.Controls
{
    public class FirstPlayerControl : MonoBehaviour
    {
        [SerializeField] float speedCharacter, runSpeed, jumpForce, rotateSpeed, gravityInScene, minViewRange, maxViewRange;
        [SerializeField] GameObject headPivot;
        [SerializeField] GameObject headModel;
        [SerializeField] InteractionSystem interactionSystem;

        private Controller inputs;
        private Animator animator;
        private float velocityVertical = 0f, cinemachineTargetPitch = 0, returnSpeed;

        private void OnEnable()
        {
            inputs.Enable();
            //inputs.Player.Jump.performed += Jump_performed; //Create an event subscription "Jump" to handle pressing
            inputs.Player.TakeThis.performed += TakeThis_performed;
            inputs.Player.Run.performed += Run_performed; //Create an event subscription "Run" to handle pressing
            inputs.Player.Run.canceled += Run_canceled; //Create an event subscription "Jump" release the button
        }
        private void OnDisable()
        {
            //inputs.Player.Jump.performed -= Jump_performed; //Delete an event subscription "Jump" to handle pressing
            inputs.Player.TakeThis.performed-= TakeThis_performed;
            inputs.Player.Run.performed -= Run_performed;//Delete an event subscription "Run" to handle pressing
            inputs.Player.Run.canceled -= Run_canceled;//Delete an event subscription "Jump" release the button
            inputs.Disable();
        }
        private void Awake()
        {
            inputs = new Controller();
            animator = GetComponent<Animator>();
        }

        private void TakeThis_performed(InputAction.CallbackContext obj)
        {
            interactionSystem.HandleInteraction();
        }

        private void Run_canceled(InputAction.CallbackContext obj)
        {
            speedCharacter = returnSpeed;
        }

        private void Run_performed(InputAction.CallbackContext obj)
        {
            returnSpeed = speedCharacter;
            speedCharacter = runSpeed;
        }

        private void Jump_performed(InputAction.CallbackContext obj)
        {
            if (GetComponent<CharacterController>().isGrounded) //Check what Character not in air
            {
                velocityVertical = jumpForce;
            }
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            PlayerMove();
        }
        private void LateUpdate()
        {
            PlayerRotation();
        }
        private void PlayerRotation()
        {
            Cursor.lockState = CursorLockMode.Locked; //Lock a cursor for normal rotation
            Cursor.visible = false;
            Vector2 readedRotationHead = inputs.Player.RotateHead.ReadValue<Vector2>(); //Takes Vector2, which appear delta position mouse cursor from new input-system

            cinemachineTargetPitch -= readedRotationHead.y * Time.fixedDeltaTime * rotateSpeed;
            cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, minViewRange, maxViewRange);
            headPivot.transform.localRotation = Quaternion.Euler(cinemachineTargetPitch, 0.0f, 0.0f);
            headModel.transform.localRotation = Quaternion.Euler(cinemachineTargetPitch, 0.0f, 0.0f);
            transform.rotation *= Quaternion.Euler(0, readedRotationHead.x * Time.fixedDeltaTime * rotateSpeed, 0);

        }
        private void PlayerMove()
        {
            //Move player
            velocityVertical -= gravityInScene * Time.fixedDeltaTime;
            Vector2 side = inputs.Player.Moving.ReadValue<Vector2>(); //Takes Vector2, which ranges from [-1; -1] to [1; 1].The first coordinates -move back to the left, the second - forward to the right
            GetComponent<CharacterController>().Move(transform.TransformDirection(side.x, velocityVertical, side.y) * speedCharacter * Time.fixedDeltaTime);
            if (GetComponent<CharacterController>().isGrounded)
                velocityVertical = 0;

            //Animator
            animator.SetFloat("Foward", side.x);
            animator.SetFloat("Right", side.y);

        }
        private static float ClampAngle(float needClampAngle, float minAngleAxis, float maxAngleAxis)
        {
            if (needClampAngle < -360f) needClampAngle += 360f;
            if (needClampAngle > 360f) needClampAngle -= 360f;
            return Mathf.Clamp(needClampAngle, minAngleAxis, maxAngleAxis);
        }
    }
}