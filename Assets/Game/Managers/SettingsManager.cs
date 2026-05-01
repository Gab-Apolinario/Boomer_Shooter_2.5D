using UnityEngine;
using UnityEngine.InputSystem;

public class SettingsManager : MonoBehaviour
{
    //Nome das Chaves do PlayerPrefs
    private const string KEY_MOUSE_SENS = "MouseSensibility";
    private const string KEY_GAMEPAD_SENS = "GamepadSensibility";

    //Valores padrão (caso ainda não tenha sensibilidade salva)
    private const float DEFAULT_MOUSE_SENS = 15.0f;
    private const float DEFAULT_GAMEPAD_SENS = 150.0f;

    //Propriedades públicas que o PlayerCamera vai usar para ler
    public static float MouseSensibility { get; private set; }
    public static float GamepadSensibility { get; private set; }

    private void Awake()
    {
        //Carregar valores salvos ou usar os padrões se for a primeira vez
        MouseSensibility = PlayerPrefs.GetFloat(KEY_MOUSE_SENS, DEFAULT_MOUSE_SENS); //internamente equivale a 'if (Playerrefs.HasKey(KEY_MOUSE_SENS) return PlayerPrefs.GetFloat(KEY_MOUSE_SENS)  else return DEFAULT_MOUSE_SENS)
        GamepadSensibility = PlayerPrefs.GetFloat(KEY_GAMEPAD_SENS, DEFAULT_GAMEPAD_SENS);
    }

    //public static pq vai ser chamado de outro script
    public static void SetMouseSensibility(float value)
    {
        MouseSensibility = value;
        PlayerPrefs.SetFloat(KEY_MOUSE_SENS, value);
        PlayerPrefs.Save();
    }

    public static void SetGamepadSensibility(float value)
    {
        GamepadSensibility = value;
        PlayerPrefs.SetFloat(KEY_GAMEPAD_SENS, value);
        PlayerPrefs.Save();
    }
}