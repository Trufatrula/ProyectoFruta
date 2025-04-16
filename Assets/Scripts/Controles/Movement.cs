using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Movement : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField]
    private float moveSpeed = 10f; // Velocidad normal al moverse.
    [SerializeField]
    private float sprintMultiplier = 1.5f; // Multiplicador a la velocidad de movimiento al correr.
    [SerializeField]
    private float airAcceleration = 10f; // Aceleracion que se aplica en el aire.
    private float actualAirAcceleration = 0f;
    [SerializeField]
    private float friction = 15f; // Friccion contra el suelo.

    [Header("Salto")]
    [SerializeField]
    private float jumpForce = 7f; // Fuerza del salto.
    [SerializeField]
    private float coyoteTime = 0.1f; // Ventana de tiempo en la que puedes saltar al salir de una plataforma.
    [SerializeField]
    private float jumpBufferTime = 0.15f; // Tiempo en el que permanece guardado el input de salto cuando aun no puedes saltar.
    [SerializeField]
    private LayerMask groundMask; // Layers que cuentan como suelo.

    [Header("Groundcheck")]
    [SerializeField]
    private float groundCheckDistance = 0.1f;
    [SerializeField]
    private float extraGravity = 20f; // Gravedad adicional aplicada al personaje.
    [SerializeField]
    private float finalGravity = 5f; // Gravedad final cuando estas a punto de llegar al suelo.
    [SerializeField]
    private float groundGravity = 5f; // Gravedad final cuando estas a punto de llegar al suelo.
    [SerializeField]
    private float finalGravityDistance = 2f; // Distancia desde la que se aplica el final gravity y caes mas rapido.

    [Header("Steps")]
    [SerializeField]
    private float maxStepHeight = 0.5f; // Altura maxima de los escalones que puedes pasar.
    [SerializeField]
    private float maxStepHeightOnAir = 0.3f; // Altura maxima de los escalones que puedes pasar.
    [SerializeField]
    private float stepRayLength = 0.5f; // Distancia a la que se detecta el escalon.
    private float actualStepHeight;

    [Header("Slopes")]
    [SerializeField]
    private float maxSlopeAngle = 40f;
    [SerializeField]
    private float maxSlopeDistance = 0.25f;
    [SerializeField]
    private float steepSlopeSlideSpeed = 2f;
    //[SerializeField]
    //private float slopeExitBumpReduction = 200f;
    //[SerializeField]
    //private float slopeEnterBumpReduction = 200f;
    private RaycastHit slopeHit;
    public bool isOnSlope = false;

    [Header("Referencias")]
    [SerializeField]
    private CameraController boomerCamera; // Referencia a la camara.
    [SerializeField]
    private InputActionManager playerInput;

    private CapsuleCollider col;
    private Rigidbody rb;
    private Vector3 moveDirection;
    public bool isGrounded;
    private float lastGroundedTime;
    private float lastJumpPressedTime = -5f;
    private float justJumpedTimer = 0f;
    private float playerHeight;

    private bool moveLock;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.freezeRotation = true;
        actualStepHeight = maxStepHeight;
        actualAirAcceleration = airAcceleration;
        playerHeight = transform.localScale.y;
        col = GetComponent<CapsuleCollider>();
    }
    private void OnEnable()
    {
        if (playerInput != null)
        {
            playerInput.OnMove += HandleMove;
            playerInput.OnJump += HandleJump;
        }
    }

    private void OnDisable()
    {
        if (playerInput != null)
        {
            playerInput.OnMove -= HandleMove;
            playerInput.OnJump -= HandleJump;
        }
    }

    private void HandleMove(Vector2 input)
    {
        if (moveLock) return;
        float moveX = input.x;
        float moveZ = input.y;

        moveDirection = new Vector3(moveX, 0, moveZ).normalized;
    }

    private void HandleJump()
    {
        if (moveLock) return;
        lastJumpPressedTime = Time.time;
        Debug.Log("Saltar");
    }

    private void Update()
    {
        HandleAdditionalInput();
        CheckGroundStatus();

        if (justJumpedTimer > 0f)
            justJumpedTimer -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        // Rotar al personaje en relacion a la rotacion x del raton.
        Quaternion targetRotation = Quaternion.Euler(0f, boomerCamera.GetCurrentYaw(), 0f);
        rb.MoveRotation(targetRotation);

        isOnSlope = OnSlope();

        ApplyMovement();
        StepClimb();
        ApplyFriction();
        ApplyJump();
        ApplyExtraGravity();
    }

    // Para inputs forzados y debugging
    private void HandleAdditionalInput()
    {
        //float moveX = Input.GetAxisRaw("Horizontal");
        //float moveZ = Input.GetAxisRaw("Vertical");

        //moveDirection = new Vector3(moveX, 0, moveZ).normalized;

        if(Input.GetKeyDown(KeyCode.K))
        {
            if(playerInput.GetCurrentActionMap().Equals("PlayerMovement"))
            {
                playerInput.SwitchActionMap("UI");
            }
            else
            {
                playerInput.SwitchActionMap("PlayerMovement");
            }
        }
    }

    private void CheckGroundStatus()
    {
        Vector3 rayOrigin = new Vector3(transform.position.x, col.bounds.min.y + 0.1f, transform.position.z);
        isGrounded = Physics.Raycast(rayOrigin, Vector3.down, groundCheckDistance, groundMask);
        if (isGrounded)
        {
            lastGroundedTime = Time.time;
            if(actualStepHeight != maxStepHeight)
            {
                actualStepHeight = maxStepHeight;
                //Debug.Log(actualStepHeight);
            }
        }
        else
        {
            if(actualStepHeight != maxStepHeightOnAir)
            {
                actualStepHeight = maxStepHeightOnAir;
                //Debug.Log(actualStepHeight);
            }
        }
    }

    private void ApplyMovement()
    {
        float movmentSpeed = moveSpeed * (Input.GetKey(KeyCode.LeftShift) ? sprintMultiplier : 1f);

        Vector3 targetHorizontalVelocity = transform.TransformDirection(moveDirection) * movmentSpeed;

        if (isGrounded)
        {
            if (isOnSlope)
            {
                //Debug.Log("Vector3.Angle(Vector3.up, slopeHit.normal).ToString()");
                
                rb.linearVelocity = Vector3.ProjectOnPlane(targetHorizontalVelocity, slopeHit.normal).normalized * movmentSpeed;
            }
            else
            {
                rb.linearVelocity = new Vector3(targetHorizontalVelocity.x, rb.linearVelocity.y, targetHorizontalVelocity.z);
                rb.AddForce(Vector3.down * groundGravity, ForceMode.Force);
            }
        }
        else
        {
            // El movimiento aereo se hace con fuerzas para que sea mas suave y menos controlable, pero pudiendo influir en la direccion.
            float currentAcceleration = actualAirAcceleration;
            Vector3 currentHorizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            Vector3 velocityChange = targetHorizontalVelocity - currentHorizontalVelocity;
            velocityChange = Vector3.ClampMagnitude(velocityChange, currentAcceleration * Time.fixedDeltaTime);
            rb.AddForce(velocityChange, ForceMode.VelocityChange);
        }

        rb.useGravity = !isOnSlope;
    }

    private void ApplyFriction()
    {
        if (isGrounded && moveDirection.magnitude < 0.1f)
        {
            Vector3 frictionForce = -rb.linearVelocity * friction * Time.fixedDeltaTime;
            rb.AddForce(new Vector3(frictionForce.x, 0, frictionForce.z), ForceMode.VelocityChange);
        }
    }

    private void ApplyJump()
    {
        if ((Time.time - lastGroundedTime <= coyoteTime) && (Time.time - lastJumpPressedTime <= jumpBufferTime))
        {
            justJumpedTimer = 0.25f;
            float actualJumpForce = jumpForce;

            rb.linearVelocity = new Vector3(rb.linearVelocity.x, actualJumpForce, rb.linearVelocity.z);

            lastJumpPressedTime = 0;
        }
    }

    /// <summary>
    /// Se aplica un extra de gravedad cuando el jugador no esta pisando el suelo. Cuando le queda poco para impactar contra
    /// el suelo la gravedad aumenta mucho para caer antes.
    /// </summary>
    private void ApplyExtraGravity()
    {
        if (!isGrounded)
        {
            float finalExtraGravity = extraGravity;

            // Dependiendo de la distancia al suelo la velocidad es mucho mayor
            if (Physics.Raycast(transform.position + (Vector3.down * playerHeight), Vector3.down, out RaycastHit hit, finalGravityDistance, groundMask) && rb.linearVelocity.y < 0)
            {
                //Debug.Log(rb.velocity.y);

                finalExtraGravity *= finalGravity;
            }
            rb.AddForce(Vector3.down * finalExtraGravity, ForceMode.Acceleration);
        }
    }

    /// <summary>
    /// Para que el personaje pueda pasar por escalones mas bajos que el actualStepHeight.
    /// </summary>
    private void StepClimb()
    {
        // Para hacer el dash solo si se esta moviendo, quitado por ahora
        //if (moveDirection.magnitude < 0.1f)
        //return;

        // Primer raycast desde los pies en direccion a la que se esta moviendo el personaje para ver si hay pared.

        // Usa la direccion del dash si esta en mitad de un dasheo, si no la direcion en la que se mueve.
        Vector3 forward = transform.TransformDirection(moveDirection);
        Vector3 lowerRayOrigin = new Vector3(transform.position.x, col.bounds.min.y + 0.05f, transform.position.z);
        if (Physics.Raycast(lowerRayOrigin, forward, out RaycastHit lowerHit, stepRayLength, groundMask))
        {
            Debug.Log(Vector3.Angle(lowerHit.normal, Vector3.up));
            // Si el angulo de la pared no es recto no se hace el step.
            if (Vector3.Angle(lowerHit.normal, Vector3.up) != 90) { return; }
            //Debug.Log(Vector3.Angle(lowerHit.normal, Vector3.up));

            // Segundo raycast desde la altura maxima del step para ver si hay pared o si esta libre para subir al personaje.
            Vector3 upperRayOrigin = lowerRayOrigin + Vector3.up * actualStepHeight;
            bool upperBlocked = Physics.Raycast(upperRayOrigin, forward, stepRayLength, groundMask);
            if (!upperBlocked)
            {
                // Si el segundo raycast no impacta se lanza un tercer raycast hacia abajo para calcular la altura del step.
                Vector3 stepRayOrigin = upperRayOrigin + forward * stepRayLength;
                if (Physics.Raycast(stepRayOrigin, Vector3.down, out RaycastHit stepHit, actualStepHeight, groundMask))
                {
                    float stepHeight = stepHit.point.y - stepRayOrigin.y + actualStepHeight;
                    Debug.Log("Altura del step: " + stepHeight);

                    if (stepHeight > 0f && stepHeight <= actualStepHeight)
                    {
                        Vector3 normalizedForward = forward.normalized;
                        Vector3 newPosition = rb.position + normalizedForward * 0.2f;
                        //newPosition.y += stepHeight + 0.05f;
                        newPosition.y += stepHeight;
                        rb.MovePosition(newPosition);
                        if(isOnSlope)
                        {
                            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
                        }
                        //rb.position = new Vector3(rb.position.x, rb.position.y + stepHeight, rb.position.z);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Raycast hacia abajo, detecta si el suelo tiene curvatura. El 0.25f es la distancia extra del raycast hacia debajo
    /// del personaje por si la curva es demasiado empinada.
    /// </summary>
    /// <returns>
    /// False si el suelo es recto o si el angulo es mayor al maxSlopeAngle. True si el suelo tiene un angulo menor que maxSlopeAngle.
    /// </returns>
    private bool OnSlope()
    {
        Vector3 rayOrigin = new Vector3(transform.position.x, col.bounds.min.y + 0.05f, transform.position.z);
        if (Physics.Raycast(rayOrigin, Vector3.down, out slopeHit, maxSlopeDistance))
        {
            //Debug.Log(slopeHit.collider.gameObject.name);
            //return slopeAngle < maxSlopeAngle && slopeAngle != 0;
            float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
            if(slopeAngle < maxSlopeAngle && slopeAngle != 0)
            {
                if(!isOnSlope && justJumpedTimer <= 0 && isGrounded)
                {
                    Debug.Log("ENTRADA A RAMPA");
                    //rb.AddForce(Vector3.down * slopeEnterBumpReduction, ForceMode.Force);
                    rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
                }
                return true;
            }
            else if(slopeAngle > maxSlopeAngle)
            {
                rb.AddForce(Vector3.down * steepSlopeSlideSpeed, ForceMode.Force);
            }

            if (isOnSlope && justJumpedTimer <= 0)
            {
                Debug.Log("SALIDA DE RAMPA A SUELO");
                //rb.AddForce(Vector3.down * slopeExitBumpReduction, ForceMode.Force);
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            }
        }
        return false;
    }

    public void SetMovementLock(bool moveLock)
    {
        this.moveLock = moveLock;
    }

    private void OnDrawGizmos()
    {
        //Vector3 directionless = transform.TransformDirection(moveDirection);
        //Vector3 lowerRayOrigin = new Vector3(transform.position.x, col.bounds.min.y + 0.05f, transform.position.z);
        //Vector3 endRay = new Vector3(transform.position.x * directionless.x, col.bounds.min.y + 0.05f, transform.position.z * directionless.z);
        //Gizmos.DrawLine(lowerRayOrigin, endRay);
    }
}