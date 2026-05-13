using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variáveis
    private InputHandler inputHandler;
    [SerializeField] private CharacterController characterController;

    [Header("Movimento")]
    [SerializeField] private ParticleSystem corrida;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] public float speedMultiplier = 1f; //multiplicador de velocidade para powerups
    [SerializeField] private float verticalVelocity = -2f; //'gravidade' do jogador
    [SerializeField] private Vector3 horizontalDirection;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private bool canDash = true;
    [SerializeField] public bool isInvincible = false;
    [SerializeField] public float invencibilityDuration;
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
        GetMovementDot();

        Dash();
        Move();
        Rotation();

        if (speedMultiplier > 1f)
        {
            corrida.Play();
        }
        else
        {
            corrida.Stop();
        }
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

        //Garante que a graviade não vai atuar na diagonal
        Vector3 finalMove = horizontalDirection * moveSpeed * speedMultiplier + Vector3.up * verticalVelocity + dashVelocity;
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

    void Dash()
    {
        //Se o jogador apertar o botão de dash e o dash não estiver no cooldown, aplica a velocidade do dash
        if (inputHandler.IsDashing && canDash)
        {
            //se o jogador não estiver se movendo, dasha para a frente do jogador
            if (horizontalDirection == Vector3.zero)
            {
                horizontalDirection = transform.forward;
            }

            isInvincible = true; //torna o jogador invencível durante o dash
            dashVelocity = horizontalDirection.normalized * dashSpeed; //calcula a velocidade do dash na direção que o jogador está pressionando
            Acoes.OnDash?.Invoke(horizontalDirection.normalized); //AÇÃO DE DASH
            Invoke(nameof(ResetInvincibility), invencibilityDuration); //chama ResetInvincibility após a duração da invencibilidade
            canDash = false; //desativa o dash até o cooldown terminar
            Invoke(nameof(ResetDash), dashCooldown); //chama ResetDash após o cooldown
        }
    }

    void ResetInvincibility()
    {
        isInvincible = false; //desativa a invencibilidade após o dash
    } 

    void ResetDash()
    {
        canDash = true;
        dashVelocity = Vector3.zero; //reseta a velocidade do dash
    }

    public float GetMovementDot()
    {
        float dot = Vector3.Dot(horizontalDirection.normalized, transform.forward);
        return dot;
    }
}