using System.Collections;
using TMPro;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform playerTransform; // Transform del jugador
    [SerializeField]
    private float mouseSensitivity = 1f; // Sensibilidad del raton, recomendado tocar entre 0.05 y 1
    [SerializeField]
    private float smoothing = 2f; // Suavizado de la camara al seguir la rotacion del jugador
    [SerializeField]
    private bool invertY = false; // Invertir rotacion del eje y
    [SerializeField]
    private Vector3 cameraOffset = new Vector3(0, 0.9f, 0); // Desplazamiento de la camara en relacion con la posicion del personaje

    private Vector2 mouseDelta;
    private Vector2 smoothedMouse;
    private Camera realCamera;

    private float currentYaw = 0f; // Rotacion x
    private float currentPitch = 0f; // Rotacion y
    private float originalFov = 0f; // Fov default

    [SerializeField]
    private float fovReturnSpeed = 15f; // Velocidad a la que se vuelve al fov original si este cambia

    public float interactRange = 100f;
    public LayerMask interactLayer;
    public LayerMask socketLayer;

    private bool isLocked = false;
    private Coroutine lookAtCoroutine;
    [SerializeField]
    private Transform grabSpot;
    private IGrabbable grabbedObject;

    // Efecto andar provisional
    [SerializeField]
    private float andarSpeed = 2f;
    [SerializeField]
    private float andarOffset = 0.1f;
    [SerializeField]
    private float andarSmoothTime = 0.1f; // Tiempo de smooth al parar. Funciona raro

    private float andarCycle = 0f; // Acumulacion de ciclo del seno
    private float andarCurrentOffset = 0f; // El offset aplicado actualmente
    private float andarCurrentVel = 0f; // Used by SmoothDamp for a smooth stop

    private IInteractable selectedInteractable;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        realCamera = GetComponent<Camera>();
        originalFov = realCamera.fieldOfView;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            UnlockCamera();
        }
        if(!isLocked)
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            mouseDelta.x = mouseX * mouseSensitivity;
            mouseDelta.y = mouseY * mouseSensitivity * (invertY ? 1f : -1f);

            if (smoothing > 0f)
            {
                smoothedMouse.x = Mathf.Lerp(smoothedMouse.x, mouseDelta.x, 1f / smoothing);
                smoothedMouse.y = Mathf.Lerp(smoothedMouse.y, mouseDelta.y, 1f / smoothing);
            }
            else
            {
                smoothedMouse = mouseDelta;
            }

            currentYaw += smoothedMouse.x;
            currentPitch += smoothedMouse.y;
            currentPitch = Mathf.Clamp(currentPitch, -90f, 90f);

            if (realCamera.fieldOfView != originalFov)
            {
                realCamera.fieldOfView = Mathf.Lerp(realCamera.fieldOfView, originalFov, Time.deltaTime * fovReturnSpeed);
            }

            // DROPEAR OBJECTOS (HACER FUNCION DE CHECK INPUTS)
            if(grabbedObject != null && Input.GetKeyDown(KeyCode.F))
            {
                grabbedObject.GameObject.transform.parent = null;
                grabbedObject.Drop(null);
                grabbedObject = null;
            }

            if(Input.GetKeyDown(KeyCode.E))
            {
                CheckInteraccionarV2();
            }
            CheckInteraccionar();
        }
    }

    void LateUpdate()
    {
        if (playerTransform != null && !isLocked)
        {
            //transform.position = playerTransform.position + cameraOffset + GetHeadBobbingOffset();
            transform.position = playerTransform.position + cameraOffset;
        }

        if (!isLocked)
        {
            transform.rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
        }
    }

    private void CheckInteraccionarV2()
    {

        if (grabbedObject != null)
        {
            Debug.Log(grabbedObject.GameObject.name + " HOLA");
            Ray raySocket = realCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            RaycastHit hitSocket;
            if (Physics.Raycast(raySocket, out hitSocket, interactRange, socketLayer))
            {
                ISocket socket = hitSocket.collider.GetComponent<ISocket>();
                if(socket != null)
                {
                    Debug.Log("EXISTO");
                    if (socket.PlaceInSocket(grabbedObject))
                    {
                        grabbedObject = null;
                        return;
                    }
                }
            }
        }


        Ray ray = realCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, interactRange, interactLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if(interactable != null)
            {
                if (selectedInteractable is IGrabbable grabbable)
                {
                    if (TryGrabObject(grabbable))
                    {
                        grabbable.Grab();
                    }
                }
                if (selectedInteractable is IHablable hablable)
                {
                    hablable.Hablar(grabbedObject);
                }
                else
                {
                    selectedInteractable.Interact();
                }
            }
        }
    }

    private void CheckInteraccionar()
    {
        Ray ray = realCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, interactRange, interactLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                // Si se empieza a mirar a otro interaccionable se cambia el focus
                if (selectedInteractable != null && selectedInteractable != interactable)
                {
                    selectedInteractable.Unhighlight();
                }
                selectedInteractable = interactable;
                selectedInteractable.Highlight();

                // Interactuar con el interaccionable hitteado
                //if (Input.GetKeyDown(KeyCode.E))
                //{
                //    Debug.Log("Interactuar con " + selectedInteractable.GameObject.name);

                //    if (selectedInteractable is IGrabbable grabbable)
                //    {
                //        if (TryGrabObject(grabbable))
                //        {
                //            grabbable.Grab();
                //        }
                //    }
                //    else if (grabbedObject != null && selectedInteractable is ISocket socket)
                //    {
                //        if (socket.PlaceInSocket(grabbedObject))
                //        {
                //            grabbedObject = null;
                //        }
                //    }
                //    else
                //    {
                //        selectedInteractable.Interact();
                //    }
                //}
            }
            else
            {
                // No se hittea nada interaccionable
                if (selectedInteractable != null)
                {
                    selectedInteractable.Unhighlight();
                    selectedInteractable = null;
                }
            }
        }
        else
        {
            // No se hittea nada
            if (selectedInteractable != null)
            {
                selectedInteractable.Unhighlight();
                selectedInteractable = null;
            }
        }

        Ray socketRay = realCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit socketHit;
        if (Physics.Raycast(socketRay, out socketHit, interactRange, socketLayer))
        {

        }
    }

    private Vector3 GetHeadBobbingOffset()
    {
        // Solo mira el input (a cambiar)
        bool isMoving = (Mathf.Abs(Input.GetAxis("Horizontal")) + Mathf.Abs(Input.GetAxis("Vertical"))) > 0.01f;

        if (isMoving)
        {
            // El ciclo solo se updatea aqui, mientras te mueves
            andarCycle += Time.deltaTime * andarSpeed;
            // Se calcula y aplica el offset sinuoidal
            float targetOffset = Mathf.Sin(andarCycle) * andarOffset;
            andarCurrentOffset = targetOffset;
        }
        else
        {
            // Al parar, el offset se pone a 0
            andarCurrentOffset = Mathf.SmoothDamp(andarCurrentOffset, 0f, ref andarCurrentVel, andarSmoothTime);
        }

        return new Vector3(0f, andarCurrentOffset, 0f);
    }

    public void LookAtSimple(Transform pos)
    {
        LookAtPoint(pos.position, 30, 1);
    }

    /// <summary>
    /// Lockea la camara y la hace mirar a un punto concreto.
    /// </summary>
    /// <param name="lookTarget">La posicion a la que mira.</param>
    /// <param name="lookFov">El fov con el que mira al punto.</param>
    /// <param name="lookDuration">Lo que tarda en mirar al punto.</param>
    public void LookAtPoint(Vector3 lookTarget, float lookFov, float lookDuration)
    {
        // Si ya hay una corrutina de mirar activa la para.
        if (lookAtCoroutine != null)
        {
            StopCoroutine(lookAtCoroutine);
        }
        lookAtCoroutine = StartCoroutine(LookCoroutine(lookTarget, lookFov, lookDuration));
    }

    private IEnumerator LookCoroutine(Vector3 targetPoint, float targetFov, float duration)
    {
        isLocked = true;

        float elapsed = 0f;
        Quaternion startRotation = transform.rotation;

        Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
        float startFov = realCamera.fieldOfView;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);

            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            realCamera.fieldOfView = Mathf.Lerp(startFov, targetFov, t);

            yield return null;
        }

        transform.rotation = targetRotation;
        realCamera.fieldOfView = targetFov;

        // Se actualiza el yaw y el pitch para que la rotacion de la camara al volver sea la misma
        Vector3 euler = transform.rotation.eulerAngles;
        currentYaw = euler.y;
        currentPitch = euler.x > 180f ? euler.x - 360f : euler.x;

        lookAtCoroutine = null;
    }

    /// <summary>
    /// Se devuelve el control de la camara. (CAMBIAR FOV AQUI?)
    /// </summary>
    public void UnlockCamera()
    {
        if (lookAtCoroutine == null)
        {
            isLocked = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void LockCamera(bool lockCam)
    {
        isLocked = lockCam;
    }

    public bool TryGrabObject(IGrabbable grabbedObject)
    {
        if(this.grabbedObject == null && grabbedObject.IsGrabbable)
        {
            Debug.Log("HOLIWI");
            this.grabbedObject = grabbedObject;
            grabbedObject.GameObject.layer = 8;
            grabbedObject.GameObject.transform.parent = grabSpot;
            StartCoroutine(GrabObjectCoroutine());
            return true;
        }
        else
        {
            // AUDIO MANOS LLENAS
            return false;
        }
    }

        private IEnumerator GrabObjectCoroutine()
        {
            Transform objTransform = grabbedObject.GameObject.transform;
            while (Vector3.Distance(objTransform.localPosition, Vector3.zero) > 0.05f)
            {
                objTransform.localPosition = Vector3.MoveTowards(objTransform.localPosition, Vector3.zero, 10f * Time.deltaTime);

                yield return null;
            }
            objTransform.localPosition = Vector3.zero;
        }

    void OnApplicationFocus(bool hasFocus)
    {
    if (hasFocus)
        Cursor.lockState = CursorLockMode.Locked;
    else
        Cursor.lockState = CursorLockMode.None;
    }

    public float GetCurrentYaw()
    {
        return currentYaw;
    }

    public Camera GetCamera()
    {
        return realCamera;
    }
}