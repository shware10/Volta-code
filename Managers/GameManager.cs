using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;


public enum GameState
{
    MainMenu,
    InGame,
    GameOver
}
public class GameManager : Singleton<GameManager>,IBarStateListener
{
    public GameState CurrentState { get; private set; } = GameState.MainMenu;

    //counting factor
    [SerializeField] private float exchangeDuration = 1f;

    //개별 오브젝트 접근을 위한 스크립트
    public RectangleHandler rectangleHandler;
    public LifeHandler lifeHandler;
    public JetSpawner jetSpawner;
    public StickySpawner stickySpawner;

    public bool isFever = false;
    private int bestScore = 0;
    private float scoreAmount = 1.0f;

    private float _score = 0;
    public int score { get { return (int)_score; } set { _score = value; } }

    private int _luminar = 1000;
    public int luminar
    { 
      get { return _luminar; }
      set { _luminar = Math.Max(value,0); }
    }

    private int life = 3;

    //상태 변경시 사용할 델리게이트
    public delegate void GameStateChanged(GameState newState);
    public event GameStateChanged OnStateChanged;

    //상태 변경 함수
    public void SetState(GameState state)
    {
        CurrentState = state;
        OnStateChanged?.Invoke(CurrentState);
    }

    protected override void Awake()
    {
        base.Awake();
        Init();
        PostInit();
    }

    //Method Group - Init,Subscribe//
    #region
    void Init()
    {
        // 게임스테이트리스너 인터페이스를 상속하는 모든 모노 객체 탐색
        var Listeners = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<IGameStateListener>();
        foreach (var listener in Listeners)
        {
            // 게임 상태 변화 델리게이트 구독
            OnStateChanged += listener.OnStateChanged;
        }
    }
    void PostInit()
    {
        StartCoroutine(PostInitRoutine());
    }
    IEnumerator PostInitRoutine() // 한 프레임 쉬고 실행될 init들
    {
        yield return null;
        //버튼 클릭 이벤트 구독
        ButtonManager.Instance.startButton.OnClick += SetInGame;
        ButtonManager.Instance.exitButton.OnClick += QuitGame;
        ButtonManager.Instance.noButton.OnClick += SetMainMenu;

        #region 광고 클릭 이벤트 구독
        /*
        ButtonManager.Instance.yesButton.OnClick += PlayAD;
        */
        #endregion

        //게임 시작 인트로
        yield return StartCoroutine(rectangleHandler.Fade(true));//바들 먼저 생기고 시작

        UIManager.Instance.Init();
    }
    //for clikable text
    void SetInGame(TextMeshProUGUI text)
    {
        SetState(GameState.InGame);
    }
    void SetMainMenu(TextMeshProUGUI text)
    {
        SetState(GameState.MainMenu);
    }

    #endregion

    //Method Group - InGame//
    #region 
    void InitInGame()
    {
        StartCoroutine(InitInGameRoutine());
    }
    IEnumerator InitInGameRoutine()
    {
        UpdateBestScore();
        yield return StartCoroutine(ExchangeMotion());
        yield return new WaitForSeconds(1f);
        SetState(GameState.GameOver);
        yield return new WaitForSeconds(2f);

        life = 3;
        lifeHandler.InitLife();
        scoreAmount = 1.0f;
        UIManager.Instance.ToText(UIManager.Instance.scoreAmountText, scoreAmount);
    }
    public void Fit()
    {
        rectangleHandler.FeverCheck();
        AddScore();
    }

    public void Unfit()
    {
        LoseLife();
        isFever = false;
        ButtonManager.Instance.FeverBonusText.gameObject.SetActive(false);
    }
    public void UpdateBestScore()
    {
        if(score > bestScore)
        {
            bestScore = score;
            UIManager.Instance.ToText(UIManager.Instance.bestScoreText, bestScore);
            UIManager.Instance.UpdateBounce();
        }
    }

    public void AddScoreAmount(float amount)
    {
        scoreAmount += amount;
        UIManager.Instance.ToText(UIManager.Instance.scoreAmountText, scoreAmount);
    }

    public void AddScore()
    {
        _score += isFever ? scoreAmount * 2 : scoreAmount; 
        UIManager.Instance.ToText(UIManager.Instance.scoreText, score);

        stickySpawner.SpawnSticky();
        jetSpawner.SpawnJet();
    }
    public void LoseLife()
    {
        --life;
        lifeHandler.lifeList[life].gameObject.SetActive(false);
        if (life == 0) 
        { 
            for(int i = 0; i < 4; ++i) rectangleHandler.barList[i].InitBarLevel();

            InitInGame();
        }
        else jetSpawner.SpawnJet(); // life가 0이 아니고 바와 충돌 시 제트 스폰
    }
    #endregion

    //Method Group - GameOver//
    #region
    IEnumerator ExchangeMotion() //->0
    {
        yield return new WaitForSeconds(1.5f);

        float time = 0;
        float startS = score;
        float endS = 0f;

        _score = 0f;

        while (time < exchangeDuration)
        {
            float percent = time / exchangeDuration;
            float scoreTmp = Mathf.Lerp(startS, endS, percent);

            UIManager.Instance.ToText(UIManager.Instance.scoreText, (int)scoreTmp);

            time += Time.deltaTime;
            yield return null;
        }
        UIManager.Instance.ToText(UIManager.Instance.scoreText, 0);
    }

    //-----광고 함수-----//
    /*
    void PlayAD(TextMeshProUGUI text)
    {
        Time.timeScale = 0;
        UIManager.Instance.OffGameOverUIGroup();
        adManager.ShowRewardedInterstitialAd();
    }
    */
    #endregion

    //Method Group - Quit//
    public void QuitGame(TextMeshProUGUI text)
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}