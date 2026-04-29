using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private InputHandler inputHandler;
    [SerializeField] private float mouseSensitivity = 10f;
    [SerializeField] private float gamepadSensitivity = 150f;
    private float totalRotationAngle = 0f;


    private void Start()
    {
        inputHandler = InputHandler.Instance;

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
        Debug.Log(inputHandler.IsGamepad);
        float sensitivity = inputHandler.IsGamepad ? gamepadSensitivity : mouseSensitivity;
        float mouseY = inputHandler.LookInput.y * sensitivity * Time.deltaTime;
        totalRotationAngle += mouseY;
        totalRotationAngle = Mathf.Clamp(totalRotationAngle, -75f, 75f); //Limitar a rotação para evitar virar de cabeça para baixo
        transform.localRotation = Quaternion.Euler(-totalRotationAngle, 0f, 0f);
    }
}