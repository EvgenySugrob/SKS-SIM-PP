using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DoKiSan.Controls
{
    public class FirstPlayerControl : MonoBehaviour
    {
        [Header("Character")]
        [SerializeField] float speedCharacter;
        [SerializeField] float runSpeed;
        [SerializeField] float jumpForce;
        [SerializeField] float rotateSpeed;
        [SerializeField] float gravityInScene; 
        [SerializeField] float minViewRange; 
        [SerializeField] float maxViewRange;
        [SerializeField] GameObject headPivot;
        [SerializeField] GameObject headModel;
        [SerializeField] InteractionSystem interactionSystem;

        [Header("UI")]
        [SerializeField] UIManipulation uiManipulation;

        [Header("Manipulation")]
        [SerializeField] WorkModeManipulation workMode;

        [Header("Eyes move over PP")]
        [SerializeField] bool limitlStateControl;
        [SerializeField] Transform eyes;
        [SerializeField] float speed;
        [SerializeField] float leftLimit = -1;
        [SerializeField] float rightLimit = 1;
        private Transform _startParentEyes;
        private Vector3 _startPosition;
        private Quaternion _startRotation;


        private Controller inputs;
        private Animator animator;
        private float velocityVertical = 0f, cinemachineTargetPitch = 0, returnSpeed;


        private void OnEnable()
        {
            inputs.Enable();
            //inputs.Player.Jump.performed += Jump_performed; //Create an event subscription "Jump" to handle pressing
            inputs.Player.TakeThis.performed += TakeThis_performed;
            inputs.Player.OpenUI.performed += OpenUI_Performed;
            inputs.Player.Run.performed += Run_performed; //Create an event subscription "Run" to handle pressing
            inputs.Player.Run.canceled += Run_canceled; //Create an event subscription "Jump" release the button
        }

        private void OnDisable()
        {
            //inputs.Player.Jump.performed -= Jump_performed; //Delete an event subscription "Jump" to handle pressing
            inputs.Player.TakeThis.performed-= TakeThis_performed;
            inputs.Player.OpenUI.performed-= OpenUI_Performed;
            inputs.Player.Run.performed -= Run_performed;//Delete an event subscription "Run" to handle pressing
            inputs.Player.Run.canceled -= Run_canceled;//Delete an event subscription "Jump" release the button
            inputs.Disable();
        }
        private void Awake()
        {
            inputs = new Controller();
            animator = GetComponent<Animator>();
        }

        private void OpenUI_Performed(InputAction.CallbackContext context)
        {
            uiManipulation.OpenUIMainGroup(); 
        }

        private void TakeThis_performed(InputAction.CallbackContext obj)
        {
            if(!uiManipulation.MainUIGroupIsActive())
            {
                interactionSystem.HandleInteraction();
            }
            else
            {
                workMode.CheckObject();
            }
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
        private void Start()
        {
            _startParentEyes = eyes.transform.parent;
        }
        // Update is called once per frame
        void FixedUpdate()
        {
            if (!uiManipulation.MainUIGroupIsActive() && !limitlStateControl)
            {
                PlayerMove();
            }
            
        }
        private void LateUpdate()
        {
            if (!uiManipulation.MainUIGroupIsActive() && !limitlStateControl)
            {
                PlayerRotation();
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }

            if (limitlStateControl)
            {
                LimitMoveEyes();
            }
        }
        private void PlayerRotation()
        {
            Cursor.lockState = CursorLockMode.Locked;
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
            if(!limitlStateControl)
            {
                velocityVertical -= gravityInScene * Time.fixedDeltaTime;
            }

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

        public void SwitchTypeMovePlayer(bool state)
        {
            limitlStateControl = state;
        }

        public void PointForMove(Transform point)
        {
            StartCoroutine(MoveEyesToPP(point));
        }
        private IEnumerator MoveEyesToPP(Transform newParent)
        {
            _startPosition = eyes.localPosition;
            _startRotation = eyes.localRotation;
            eyes.transform.parent = newParent;
            yield return StartMoveToPP();
            
        }
        private YieldInstruction StartMoveToPP()
        {
            return DOTween.Sequence()
                .Append(eyes.DOLocalMove(Vector3.zero, 1.5f))
                .Join(eyes.DOLocalRotate(Vector3.zero, 1.5f))
                .Play()
                .WaitForCompletion();
        }
        
        private void LimitMoveEyes()
        {
            float input = Input.GetAxis("Horizontal");
            Vector3 localMove = new Vector3(input * speed * Time.deltaTime, 0f, 0f);

            Vector3 newPosition = eyes.transform.localPosition + localMove;
            newPosition.x = Mathf.Clamp(newPosition.x, leftLimit, rightLimit);

            eyes.transform.localPosition= newPosition;
        }
    }
}