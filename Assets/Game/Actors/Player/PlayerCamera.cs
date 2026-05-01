using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private InputHandler inputHandler;
    
    private float totalRotationAngle = 0f;

    private void Start()
    {
        //Obtem a instância do InputHandler
        inputHandler = InputHandler.Instance;

        //Trava o cursor no centro da tela
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; //Esconde o cursor
    }

    private void Update()
    {
        RotateCamera();
    }

    //Função para rotacionar a camera com base no input do mouse/gamepad
    void RotateCamera()
    {
        //Operador ternário. Se o input for o gamepad, usa a sensibilidade do gamepad, senão usa a sensibilidade do mouse
        float sensitivity = inputHandler.IsGamepad ? SettingsManager.GamepadSensibility : SettingsManager.MouseSensibility;

        //Rotaciona a camera no eixo X (olhar para cima/baixo) com base no input do mouse/gamepad
        float mouseY = inputHandler.LookInput.y * sensitivity * Time.deltaTime;
        totalRotationAngle += mouseY; //Acumula a rotação total para limitar o ângulo de rotação
        totalRotationAngle = Mathf.Clamp(totalRotationAngle, -75f, 75f); //Limitar a rotação para evitar virar de cabeça para baixo
        transform.localRotation = Quaternion.Euler(-totalRotationAngle, 0f, 0f); //Aplica a rotação acumulada no eixo X. Negativo para inverter o movimento do mouse
    }
}