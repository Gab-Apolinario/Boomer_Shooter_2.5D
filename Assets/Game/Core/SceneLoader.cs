using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    //Função para carregar a cena do jogo
    public void LoadGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Prototype_01");
    }

    //Função para carregar a cena do menu
    public void LoadMenu()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.None; //Libera o cursor para o menu
        Cursor.visible = true; //Mostra o cursor para o menu
        SceneManager.LoadScene("MainMenu"); 
    }

    private void Update()
    {
        //Se o jogador apertar ESC, fecha o jogo
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Application.Quit();
        }
    }
}
