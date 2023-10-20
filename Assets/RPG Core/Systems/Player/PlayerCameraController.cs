using BansheeGz.BGDatabase;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RPG.Core.Player
{
    public class PlayerCameraController : M_Camera, BGAddonSaveLoad.BeforeSaveReciever
    {
        //actions
        private PlayerInput playerInput;
        private InputAction panCamreraAction;
        private InputAction rotateCamreraAction;
        private InputAction zoomCamreraAction;

        //panning
        [SerializeField]
        private float panSpeed = 5f;
        private Vector2 userInput = Vector2.zero;

        //looking
        [SerializeField]
        private float rotationSpeed = 5f;
        private float rotationDirection;

        //zooming
        [SerializeField]
        private Transform zoomStart;
        [SerializeField]
        private Transform zoomEnd;

        [SerializeField]
        private int zoomSteps = 7;
        private int currentStep = 0;

        [SerializeField]
        private float zoomRotateSpeed = 1.5f;
        [SerializeField]
        private float zoomTranslateSpeed = 2f;

        private float zoomTime = 1f;
        private float rotateTime = 1f;

        [SerializeField]
        private Transform stepTo;
        private Vector3 moveFrom;
        private Quaternion rotateFrom;

        private new void Start()
        {
            currentStep = f_zoomStep;
            RotateCamera(f_yRotation);
            ZoomCamera(true);
        }

        private void OnEnable()
        {
            playerInput = GetComponent<PlayerInput>();
            panCamreraAction = playerInput.actions["PanCamera"];
            rotateCamreraAction = playerInput.actions["RotateCamera"];
            zoomCamreraAction = playerInput.actions["ZoomCamera"];

            panCamreraAction.performed += PanCameraAction;
            panCamreraAction.canceled += CancelPanAction;
            rotateCamreraAction.performed += RotateCameraAction;
            zoomCamreraAction.performed += ZoomCameraAction;
        }
        private void OnDisable()
        {
            panCamreraAction.performed -= PanCameraAction;
            panCamreraAction.canceled -= CancelPanAction;
            rotateCamreraAction.performed -= RotateCameraAction;
            zoomCamreraAction.performed -= ZoomCameraAction;
        }
        public void Update()
        {
            Debug.DrawLine(zoomEnd.position,zoomEnd.position + zoomEnd.forward * 10,Color.red);
            Debug.DrawLine(zoomStart.position,zoomStart.position + zoomStart.forward * 10,Color.blue);
            Debug.DrawLine(transform.position,transform.position + transform.forward * 5,Color.magenta);

            if (userInput != Vector2.zero)
            {
                //move the cameras parent in the xz plane based on user input
                Vector3 userInputXZPlane = new Vector3(userInput.x,0,userInput.y);
                userInputXZPlane = Quaternion.Euler(0f,transform.eulerAngles.y,0f) * userInputXZPlane;
                Vector3 translation = userInputXZPlane.normalized * panSpeed * Time.deltaTime;
                transform.parent.transform.Translate(translation,Space.World);
            }

            if (zoomTime < 1f)
            {
                //perfom zoom
                zoomTime += zoomTranslateSpeed * Time.deltaTime;
                zoomTime = Mathf.Clamp01(zoomTime);
                transform.position = Vector3.Lerp(moveFrom,stepTo.position,zoomTime);
            }

            if (rotateTime < 1f)
            {
                rotateTime += zoomRotateSpeed * Time.deltaTime;
                rotateTime = Mathf.Clamp01(rotateTime);
                transform.rotation = Quaternion.Slerp(rotateFrom,stepTo.rotation,rotateTime);
            }
        }
        private void PanCameraAction(InputAction.CallbackContext context)
        {
            userInput = context.ReadValue<Vector2>();
        }
        private void CancelPanAction(InputAction.CallbackContext context)
        {
            userInput = Vector2.zero;
        }
        private void RotateCameraAction(InputAction.CallbackContext context)
        {
            rotationDirection = context.ReadValue<float>();
            RotateCamera(rotationDirection * rotationSpeed * Time.deltaTime);
        }
        private void ZoomCameraAction(InputAction.CallbackContext context)
        {
            int direction = Mathf.Clamp((int)context.ReadValue<float>(),-1,1);
            if (currentStep + direction < 0 || currentStep + direction > zoomSteps) return;
            currentStep += direction;
            ZoomCamera();
        }

        public void ZoomCamera(bool snap = false)
        {
            moveFrom = transform.position;
            stepTo.position = Vector3.Lerp(zoomStart.position,zoomEnd.position,(float)currentStep / zoomSteps);

            rotateFrom = transform.rotation;
            stepTo.rotation = Quaternion.Slerp(zoomStart.rotation,zoomEnd.rotation,(float)currentStep / zoomSteps);

            if (!snap)
            {
                zoomTime = 0f;
                rotateTime = 0f;
            } else
            {
                transform.position = stepTo.position;
                transform.rotation = stepTo.rotation;
            }
        }

        public void RotateCamera(float roationAmount)
        {
            transform.Rotate(Vector3.up,roationAmount,Space.World);
            zoomStart.RotateAround(transform.position,Vector3.up,roationAmount);
            zoomEnd.RotateAround(transform.position,Vector3.up,roationAmount);
            stepTo.RotateAround(transform.position,Vector3.up,roationAmount);
        }

        public void OnBeforeSave()
        {
            if (!gameObject.activeInHierarchy) return;

            if (!gameObject.scene.IsValid()) return;
            f_yRotation = transform.eulerAngles.y;
            f_zoomStep = currentStep;
        }
    }
}