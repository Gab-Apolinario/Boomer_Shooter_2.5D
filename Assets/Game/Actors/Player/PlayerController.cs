using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variáveis
    private InputHandler inputHandler;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float verticalVelocity = -2f; //'gravidade' do jogador
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

        Vector3 horizontalMove = new Vector3(inputHandler.MoveInput.x, 0, inputHandler.MoveInput.y);
        horizontalMove = transform.TransformDirection(horizontalMove);    //fazer o player sempre para o lado que está olhando

        //se está correndo, usa velocidade de corrida, senão usa a normal
        float currentSpeed = moveSpeed;
        if (inputHandler.IsSprinting)
        {
            currentSpeed = sprintSpeed;
        }

        //Garante que a graviade não vai atuar na diagonal
        Vector3 finalMove = horizontalMove * currentSpeed + Vector3.up * verticalVelocity + dashVelocity;
        characterController.Move(finalMove * Time.deltaTime);
        
        //Desacelera o dash ao longo do tempo
        dashVelocity = Vector3.Lerp(dashVelocity, Vector3.zero, dashDeceleration * Time.deltaTime);
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
            verticalVelocity = jumpForce;
        }
    }

    void Dash()
    {
        //Se o jogador apertar o botão de dash e o dash não estiver no cooldown, aplica a velocidade do dash
        if (inputHandler.IsDashing && canDash)
        {
            dashVelocity = transform.forward * dashSpeed; //calcula a velocidade do dash na direção que o jogador está olhando
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