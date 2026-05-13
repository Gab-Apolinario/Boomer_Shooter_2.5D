using System.Collections;
using UnityEngine;

public class ControlPoint : MonoBehaviour
{
    public enum ControlPointState
    {
        Neutral,
        Controlling,
        Controlled
    }

    [SerializeField] private ControlPointState currentState;
    [SerializeField] private float timeToControl = 3f;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private float controlTimer;
    [SerializeField] private bool isControlling;
    [SerializeField] private bool isControlled;
    [SerializeField] private Coroutine coroutine;

    void Start()
    {
        meshRenderer.material.color = Color.red;
        controlTimer = 0;
    }

    private void Update()
    {
        if (isControlling)
        {
            controlTimer += Time.deltaTime;
            if (controlTimer >= timeToControl)
            {
                currentState = ControlPointState.Controlled;
                Acoes.OnPointCotrolled?.Invoke();
            }
        }

        if (currentState == ControlPointState.Controlled && !isControlled)
        {
            meshRenderer.material.color = Color.blue;
            controlTimer = 0;
            isControlled = true;
            isControlling = false;
            Debug.Log("PONTO CONTROLADO!");
            //ignora player
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && currentState != ControlPointState.Controlled)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            
            currentState = ControlPointState.Controlling;
            isControlling = true;
            meshRenderer.material.color = Color.yellow;
            Debug.Log("PONTO CARREGANDO!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && currentState == ControlPointState.Controlling)
        {
            isControlling = false;
            coroutine = StartCoroutine(ControlDown());
        }
    }

    IEnumerator ControlDown()
    {
        while (controlTimer > 0)
        {
            controlTimer -= Time.deltaTime;
            yield return null;
        }

        controlTimer = 0;
        currentState = ControlPointState.Neutral;
        meshRenderer.material.color = Color.red;
        Debug.Log("PONTO NEUTRO!");
    }
}
