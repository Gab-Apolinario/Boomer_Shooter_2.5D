using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variáveis
    private InputHandler inputHandler;
    [SerializeField] private CharacterController characterController;

    [Header("Movimento")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 15f;
    [SerializeField] private float stamina;
    private float maxStamina = 100f;
    [SerializeField] private bool isRegeneratingStamina = false;
    [SerializeField] private bool canSprint = true;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float verticalVelocity = -2f; //'gravidade' do jogador
    [SerializeField] private Vector3 horizontalDirection;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private bool canDash = true;
    [SerializeField] private float dashCooldown = 2f;
    [SerializeField] private float dashDeceleration = 5f;
    [SerializeField] private Vector3 dashVelocity;
    #endregion

    //Awake() roda antes do Start(), bom para inniciar variáveis que outros scripts possam precisar no Start()
    private void Awake()
    {
        //Inicializar o InputHandler
        inputHandler = new InputHandler();
    }

    private void Start()
    {
        stamina = maxStamina;

        //SEGURANÇA - Pegar o CharacterController do jogador se no começo estiver vazio
        if (characterController == null)
        {
            Debug.LogWarning("CharacterController não encontrado");
            characterController = GetComponent<CharacterController>();
        }
    }

    private void Update()
    {
        inputHandler.UpdateActiveDevice();
        Jump();
        Dash();
        Move();
        Rotation();
    }

    void Move()
    {
        //Se não está no chão, aplica a gravidade
        if (!characterController.isGrounded)
        {
            verticalVelocity += Physics.gravity.y * Time.deltaTime; //cresce a cada frame
        }
        else if (verticalVelocity < 0)  //se está no chão e ainda tem velocidade negativa, reseta para um valor pequeno
        {
            verticalVelocity = -2f;     //pequena força para manter o jogador no chão
        }
        
        verticalVelocity = Mathf.Max(verticalVelocity, -20f);   //limita a velocidade de queda

        horizontalDirection = new Vector3(inputHandler.MoveInput.x, 0, inputHandler.MoveInput.y);
        horizontalDirection = transform.TransformDirection(horizontalDirection);    //fazer o player sempre para o lado que está olhando

        //se está correndo, usa velocidade de corrida, senão usa a normal
        float currentSpeed = moveSpeed;
        //Jogado só pode correr se estiver stamina e se estiver se movendo para frente
        if (inputHandler.IsSprinting && canSprint && inputHandler.MoveInput.y > 0)
        {
            currentSpeed = sprintSpeed;
            stamina -= 30f * Time.deltaTime; //consome stamina enquanto corre
            Acoes.OnStaminaChanged?.Invoke(stamina, maxStamina); //atualiza a barra de stamina na UI

            if (stamina <= 0)
            {
                stamina = 0;
                canSprint = false; //desativa a corrida quando a stamina acabar
                StartCoroutine(StaminaCooldown()); //começa a regenerar a stamina após um tempo
            }
        }

        //Se o jogador parar de correr e a stamina não estiver cheia, começa a regenerar a stamina
        if (!inputHandler.IsSprinting && stamina < 100f && !isRegeneratingStamina)
        {
            StartCoroutine(StaminaCooldown());
        }

        //Garante que a graviade não vai atuar na diagonal
        Vector3 finalMove = horizontalDirection * currentSpeed + Vector3.up * verticalVelocity + dashVelocity;
        characterController.Move(finalMove * Time.deltaTime);
        
        //Desacelera o dash ao longo do tempo
        dashVelocity = Vector3.Lerp(dashVelocity, Vector3.zero, dashDeceleration * Time.deltaTime);
    }

    IEnumerator StaminaCooldown()
    {
        isRegeneratingStamina = true;
        yield return new WaitForSeconds(1.5f); //espera X segundos antes de começar a regenerar a stamina
        
        //regenera a stamina enquanto ela não estiver cheia e o jogador não estiver correndo
        while (stamina < 100f && !inputHandler.IsSprinting)
        {
            stamina += 35f * Time.deltaTime; //regenera a stamina ao longo do tempo
            Acoes.OnStaminaChanged?.Invoke(stamina, maxStamina);
            yield return null; //espera o próximo frame
        }

        if (stamina >= 100f)
        {
            stamina = 100f;
        }

        stamina = Mathf.Min(stamina, 100f);
        Acoes.OnStaminaChanged?.Invoke(stamina, maxStamina);
        canSprint = true;
        isRegeneratingStamina = false;
    }

    void Rotation()
    {
        //Operador ternário. Se o input for o gamepad, usa a sensibilidade do gamepad, senão usa a sensibilidade do mouse
        float sensitivity = inputHandler.IsGamepad ? SettingsManager.GamepadSensibility : SettingsManager.MouseSensibility;
        transform.Rotate(0, inputHandler.LookInput.x * Time.deltaTime * sensitivity, 0);
    }

    void Jump()
    {
        //Se o jogador apertar o botão de pulo e estiver no chão, aplica a força de pulo
        if (inputHandler.JumpInput && characterController.isGrounded)
        {
            Acoes.OnJump?.Invoke();
            verticalVelocity = jumpForce;
        }
    }

    void Dash()
    {
        //Se o jogador apertar o botão de dash e o dash não estiver no cooldown, aplica a velocidade do dash
        if (inputHandler.IsDashing && canDash)
        {
            Acoes.OnDash?.Invoke(); //AÇÃO DE DASH
            //se o jogador não estiver se movendo, dasha para a frente do jogador
            if (horizontalDirection == Vector3.zero)
            {
                horizontalDirection = transform.forward;
            }

            dashVelocity = horizontalDirection.normalized * dashSpeed; //calcula a velocidade do dash na direção que o jogador está pressionando
            canDash = false; //desativa o dash até o cooldown terminar
            Invoke(nameof(ResetDash), dashCooldown); //chama ResetDash após o cooldown
        }
    }

    void ResetDash()
    {
        canDash = true;
        dashVelocity = Vector3.zero; //reseta a velocidade do dash
    }
}