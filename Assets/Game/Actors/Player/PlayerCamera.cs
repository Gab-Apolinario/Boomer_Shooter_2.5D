using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private InputHandler inputHandler;
    [SerializeField] private float mouseSensitivity = 10f;
    private float totalRotationAngle = 0f;


    private void Start()
    {
        //Inicializar o InputHandler
        inputHandler = new InputHandler();

        //Trava o cursor no centro da tela
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        RotateCamera();
    }

    void RotateCamera()
    {
        float mouseY = inputHandler.LookInput.y * mouseSensitivity * Time.deltaTime;
        totalRotationAngle += mouseY;
        totalRotationAngle = Mathf.Clamp(totalRotationAngle, -75f, 75f); //Limitar a rotação para evitar virar de cabeça para baixo
        transform.localRotation = Quaternion.Euler(-totalRotationAngle, 0f, 0f);
    }
}