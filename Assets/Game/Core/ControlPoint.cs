using System.Collections;
using UnityEngine;

public class ControlPoint : MonoBehaviour
{
    #region VARIÁVEIS
    public enum ControlPointState
    {
        Neutral,
        Controlling,
        Controlled
    }

    [SerializeField] private ParticleSystem controlPointParticles;
    [SerializeField] private GameManager GameManager;
    [SerializeField] private ControlPointState currentState;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Sprite runeSprite;
    public Sprite RuneSprite => runeSprite;
    [SerializeField] private float timeToControl = 3f;
    [SerializeField] private float controlTimer;
    [SerializeField] private float resetTimerMax = 90f;
    [SerializeField] private float resetTimer;
    [SerializeField] private bool isControlling;
    [SerializeField] private bool isControlled;
    public bool IsControlled => isControlled;
    [SerializeField] private bool isActive;
    public bool IsActive => isActive;
    private Coroutine coroutine;
    private Coroutine resetCoroutine;

    [Header("Beam Settings")]
    private LineRenderer lineRenderer;
    [SerializeField] private float beamWidth = 3f;
    [SerializeField] private Transform beamOrigin;
    [SerializeField] private Material beamMaterial;

    #endregion

    void Awake()
    {
        SetUpBeam();
        lineRenderer.enabled = false;
        GetComponent<ControlPointPointer>().enabled = false;
        controlPointParticles.Stop();
    }

    void Start()
    {
        if (GameManager == null)
        {
            GameManager = FindAnyObjectByType<GameManager>();
        }
        
        meshRenderer.material.color = Color.red;
        controlTimer = 0;
        resetTimer = resetTimerMax;
        lineRenderer.material.SetColor("_BaseColor", Color.red * 5f);
    }

    private void Update()
    {
        if (isControlling)
        {
            controlTimer += Time.deltaTime;
            Acoes.OnCaptureProgress?.Invoke(this, controlTimer / timeToControl);
            if (controlTimer >= timeToControl)
            {
                currentState = ControlPointState.Controlled;
                isControlled = true;
                isControlling = false;
                Acoes.OnPointCotrolled?.Invoke();
            }
        }

        if (currentState == ControlPointState.Controlled && resetCoroutine == null)
        {
            Debug.Log("PONTO CONTROLADO!");
            GameManager.IncreaseScore(100);
            meshRenderer.material.color = Color.blue;
            controlTimer = 0;
            resetTimer = resetTimerMax;
            resetCoroutine = StartCoroutine(ResetControlPoint());
            GetComponent<ControlPointPointer>().enabled = false;
            controlPointParticles.Stop();
        }

        if (isControlled && lineRenderer.enabled)
        {
            float progress = resetTimer / resetTimerMax;
            Color beamColor = Color.Lerp(Color.red, Color.blue, progress);
            Acoes.OnResetProgress?.Invoke(this, resetTimer / resetTimerMax);

            //piscar
            float threshold = resetTimerMax * 0.15f;
            if(resetTimer < threshold)
            {
                float blink = Mathf.PingPong(Time.time * 6f, 1f); //2f = vlocidade que pisca
                lineRenderer.material.SetColor("_BaseColor", beamColor * blink * 5f);   
            }
            else
            {
                lineRenderer.material.SetColor("_BaseColor", beamColor * 5f);   
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && currentState != ControlPointState.Controlled && isActive)
        {
            //Garante que não tenha duas coroutines rodando ao mesmo tempo
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
            Acoes.OnCaptureProgress?.Invoke(this, controlTimer / timeToControl);
            yield return null;
        }

        controlTimer = 0;
        currentState = ControlPointState.Neutral;
        meshRenderer.material.color = Color.red;
        Debug.Log("PONTO NEUTRO!");
    }

    IEnumerator ResetControlPoint()
    {
        while (resetTimer > 0)
        {
            resetTimer -= Time.deltaTime;
            yield return null;
        }

        currentState = ControlPointState.Neutral;
        meshRenderer.material.color = Color.red;
        isControlling = false;
        isControlled = false;
        controlTimer = 0;
        resetCoroutine = null;

        lineRenderer.enabled = false;
        isActive = false;
        Acoes.OnPointReset?.Invoke(this);
        GetComponent<ControlPointPointer>().enabled = false;
        Debug.Log("PONTO RESETADO!");
    }

    void SetUpBeam()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = beamWidth;
        lineRenderer.endWidth = beamWidth;
        lineRenderer.SetPosition(0, beamOrigin.position);
        lineRenderer.SetPosition(1, beamOrigin.position + Vector3.up * 1000f);
        lineRenderer.material = beamMaterial;
    }

    public void EnableBeam()
    {
        isActive = true;
        lineRenderer.enabled = true;
        controlPointParticles.Play();
        GetComponent<ControlPointPointer>().enabled = true;
    }
}
