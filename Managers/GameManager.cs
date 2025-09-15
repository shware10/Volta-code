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

    //���� ������Ʈ ������ ���� ��ũ��Ʈ
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

    //���� ����� ����� ��������Ʈ
    public delegate void GameStateChanged(GameState newState);
    public event GameStateChanged OnStateChanged;

    //���� ���� �Լ�
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
        // ���ӽ�����Ʈ������ �������̽��� ����ϴ� ��� ��� ��ü Ž��
        var Listeners = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<IGameStateListener>();
        foreach (var listener in Listeners)
        {
            // ���� ���� ��ȭ ��������Ʈ ����
            OnStateChanged += listener.OnStateChanged;
        }
    }
    void PostInit()
    {
        StartCoroutine(PostInitRoutine());
    }
    IEnumerator PostInitRoutine() // �� ������ ���� ����� init��
    {
        yield return null;
        //��ư Ŭ�� �̺�Ʈ ����
        ButtonManager.Instance.startButton.OnClick += SetInGame;
        ButtonManager.Instance.exitButton.OnClick += QuitGame;
        ButtonManager.Instance.noButton.OnClick += SetMainMenu;

        #region ���� Ŭ�� �̺�Ʈ ����
        /*
        ButtonManager.Instance.yesButton.OnClick += PlayAD;
        */
        #endregion

        //���� ���� ��Ʈ��
        yield return StartCoroutine(rectangleHandler.Fade(true));//�ٵ� ���� ����� ����

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
        else jetSpawner.SpawnJet(); // life�� 0�� �ƴϰ� �ٿ� �浹 �� ��Ʈ ����
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

    //-----���� �Լ�-----//
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