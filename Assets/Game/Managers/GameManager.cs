using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    #region VARIÁVEIS
    private enum GameState
    {
        Playing,
        GameOver,
        Victory,
        TimeOver
    }

    [SerializeField] private ControlPoint[] controlPoint;
    private Queue<ControlPoint> captureQueue = new Queue<ControlPoint>();
    

    [SerializeField] private GameState currentState;
    [SerializeField] private UIManager UIManager;
    [SerializeField] private SceneLoader SceneLoader;
    [SerializeField] private int score;
    private int scorePerSeconds;
    [SerializeField] private float timeLimit; //segundos
    [SerializeField] private float timePlayed; //segundos
    [SerializeField] private bool timerStarted;
    [SerializeField] private bool isPaused;

    #endregion

    #region AÇÕES
    private void OnEnable()
    {
        Acoes.OnEnemyDie += IncreaseScore;
        Acoes.OnPlayerDeath += HandleGameOver;
        Acoes.OnWaveSpawn += StartTimer;
        Acoes.OnPointCotrolled += CheckVictory;
        Acoes.OnPointReset += HandlePointReset;
    }

    private void OnDisable()
    {
        Acoes.OnEnemyDie -= IncreaseScore;
        Acoes.OnPlayerDeath -= HandleGameOver;
        Acoes.OnWaveSpawn -= StartTimer;
        Acoes.OnPointCotrolled -= CheckVictory;
        Acoes.OnPointReset -= HandlePointReset;
    }
    #endregion


    private void Awake()
    {
        if (UIManager == null)
        {
            UIManager = FindAnyObjectByType<UIManager>();
        }

        if (SceneLoader == null)
        {
            SceneLoader = FindAnyObjectByType<SceneLoader>();
        }
    }

    private void Start()
    {
        currentState = GameState.Playing;
        score = 0;
        scorePerSeconds = 10;
        timePlayed = 0;
        timeLimit = 240f; //4 minutos

        ShuffleControlPoints();
        Acoes.OnControlPointsOrdered?.Invoke(controlPoint); //avisa a UI da ordem dos pontos
        foreach (var point in controlPoint)
        {
            captureQueue.Enqueue(point);
        }

        ActivateNextPoint(captureQueue.Peek());
    }

    private void Update()
    {
        if (currentState == GameState.Playing && Keyboard.current.f1Key.wasPressedThisFrame)
        {
            SceneLoader.LoadMenu();
        }

        if (currentState == GameState.Playing && (Keyboard.current.pKey.wasPressedThisFrame || (Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame)))
        {
            isPaused = !isPaused;
            Time.timeScale = isPaused ? 0 : 1;

            if (isPaused)
            {
                UIManager.ShowPause();
            }
            else
            {
                UIManager.HidePause();
            }
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Application.Quit();
        }

        if (currentState == GameState.Playing && timerStarted)
        {
            timeLimit -= Time.deltaTime;
            timePlayed += Time.deltaTime;
            UIManager.UpdateTimer(timeLimit);
        }
        
        if (currentState == GameState.Playing && timeLimit <= 0)
        {
            TimeOver();
        }
    }

    public void IncreaseScore(int amount)
    {
        if (currentState != GameState.Playing)
        {
            return;
        }

        score += amount;

        Acoes.ResolveScore?.Invoke(score); //Atualizar UI
    }

    void HandleGameOver()
    {
        currentState = GameState.GameOver;
        Time.timeScale = 0;

        Acoes.ResolveTime?.Invoke(timePlayed);
        Acoes.ResolveScore?.Invoke(score); //Atualizar UI
        Acoes.GameOver?.Invoke(); //Entrar modal UI
    }

    void HandleVictory()
    {
        currentState = GameState.Victory;
        Time.timeScale = 0;
        int timeBonus = (int)timeLimit * scorePerSeconds;

        score += timeBonus;
        Acoes.ResolveTime?.Invoke(timePlayed);
        Acoes.ResolveScore?.Invoke(score); //Atualizar UI
        Acoes.Victory?.Invoke(); //Atualizar UI
    }

    void TimeOver()
    {
        currentState = GameState.TimeOver;
        Time.timeScale = 0;

        Acoes.ResolveTime?.Invoke(timePlayed);
        Acoes.ResolveScore?.Invoke(score); //Atualizar UI
        Acoes.TimeOver?.Invoke(); //Entra modal UI
    }

    void StartTimer(int _)
    {
        timerStarted = true;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1;
        UIManager.HidePause();
    }

    void ShuffleControlPoints()
    {
        //"Começo no primeiro elemento, enquanto i for menor que a quantidade de pontos, acrescenta i a cada iteração"
        for (int i = 0; i < controlPoint.Length; i++)
        {
            int randomIndex = Random.Range(i, controlPoint.Length);

            var temp = controlPoint[i];
            controlPoint[i] = controlPoint[randomIndex];
            controlPoint[randomIndex] = temp;
        }
    }

    void CheckVictory()
    {
        /* "Para cada ponto de controle no array, verifique se está controlado. Se encontrar qualquer um que NÃO esteja, saia.
        Se passar por todos sem encontrar nenhum não-controlado, é vitória." */
        bool allControlled = true;

        foreach (ControlPoint point in controlPoint)
        {
            if (!point.IsControlled)
            {
                allControlled = false;
                break;
            }
        }
        

        if (allControlled)
        {
            HandleVictory();
            return;
        }
        
        captureQueue.Dequeue();
        if (captureQueue.Count > 0)
        {
            ActivateNextPoint(captureQueue.Peek());
        }

        Acoes.OnPointCotrolledWithReward?.Invoke();
    }

    void HandlePointReset(ControlPoint point)
    {
        captureQueue.Enqueue(point);
        if(captureQueue.Count == 1)
        {
            ActivateNextPoint(captureQueue.Peek());
        }
    }

    void ActivateNextPoint(ControlPoint point)
    {
        point.EnableBeam();
        Acoes.OnPointActivated?.Invoke(point);
    }
}
