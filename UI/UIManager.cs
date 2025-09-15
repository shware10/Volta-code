using System.Collections;
using UnityEngine;
using TMPro;


public class UIManager : MonoBehaviour, IGameStateListener, IBarStateListener
{
    public static UIManager Instance;

    [Header("점수/루미나")]
    public TextMeshProUGUI scoreAmountText;
    public TextMeshProUGUI bestScoreText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI feverText;

    [Header("CanvasGroup")]
    [SerializeField] private CanvasGroup MainMenuUIGroup;
    [SerializeField] private CanvasGroup InGameUIGroup;
    [SerializeField] private CanvasGroup GameOverUIGroup;

    [Header("사라지기/나타나기 팩터")]
    [SerializeField] private float vanishDuration = 1f;
    [SerializeField] private float appearDuration = 1f;

    [Header("점수 바운스 팩터")]
    [SerializeField] private float bounceDuration = 0.2f;
    [SerializeField] private float targetScale = 1.2f;

    void Awake()
    {
        Instance = this;
    }
    //Method Group - CanvasGroup//

    void Fade(CanvasGroup canvasGroup, bool isCurGroup)
    {
        if (canvasGroup.gameObject.activeSelf && isCurGroup) return;
        StartCoroutine(FadeRoutine(canvasGroup, isCurGroup));
    }
    #region
    IEnumerator FadeRoutine(CanvasGroup canvasGroup, bool isCurGroup)
    {
        if (isCurGroup) yield return new WaitForSeconds(vanishDuration);//이전 그룹이 사라지고 등장
        float time = 0f;
        if (isCurGroup == false)
        {
            //vanish
            canvasGroup.blocksRaycasts = false;
            while (time <= vanishDuration)
            {
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, time / vanishDuration);

                time += Time.deltaTime;
                yield return null;
            }
            canvasGroup.alpha = 0f;
            canvasGroup.gameObject.SetActive(false);
        }
        if (isCurGroup == true)
        {
            //appear
            canvasGroup.gameObject.SetActive(true);
            while (time <= appearDuration)
            {
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, time / appearDuration);

                time += Time.deltaTime;
                yield return null;
            }
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }
    }
    #endregion

    //Method Group - ShowUI//
    public void OnStateChanged(GameState state)
    {
        Fade(MainMenuUIGroup, state == GameState.MainMenu);
        Fade(InGameUIGroup, state == GameState.InGame);
        Fade(GameOverUIGroup, state == GameState.GameOver);
    }

    public void Init()
    {
        Fade(MainMenuUIGroup, true);
    }

    public void ScoreScale(TextMeshProUGUI text, Color targetColor, Color endColor)
    { 
        StartCoroutine(ScaleRoutine(text, targetColor, endColor));
    }
    IEnumerator ScaleRoutine(TextMeshProUGUI text, Color targetColor, Color endColor)
    {
        yield return ScaleMotion(text, 1f, targetScale, targetColor);
        yield return new WaitForSeconds(1f);
        yield return ScaleMotion(text, targetScale, 1f, endColor);
    }
    IEnumerator ScaleMotion(TextMeshProUGUI text, float originScale,  float targetScale, Color targetColor)
    {
        float time = 0f;
        float startScale = originScale;
        float endScale = targetScale;
        Color startColor = text.color;
        Color endColor = targetColor;
        float duration = bounceDuration / 2;
        while (time < duration)
        {
            float scale = Mathf.Lerp(startScale, endScale, time / duration);
            text.transform.localScale = Vector3.one * scale;
            text.color = Vector4.Lerp(startColor, endColor, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        text.transform.localScale = Vector3.one * targetScale;
        text.color = endColor;
    }

    IEnumerator BounceMotion(TextMeshProUGUI text, float duration, Color targetColor = default)
    {
        if (targetColor == default) targetColor = Color.white;
        float time = 0;
        float startScale = 1;
        float endScale = targetScale;

        Color originColor = text.color;

        while (time < duration)
        {
            float t = time / duration;
            float pingPong = Mathf.Sin(t * Mathf.PI);
            float scale = Mathf.Lerp(startScale, endScale, pingPong);
            Color color = Vector4.Lerp(originColor, targetColor, pingPong);
            text.transform.localScale = Vector3.one * scale;
            text.color = color;

            time += Time.deltaTime;
            yield return null;
        }
        text.transform.localScale = Vector3.one;
        text.color = originColor;
    }
    public void ScoreBounce(TextMeshProUGUI text, float duration, Color targetColor = default)
    {
        StartCoroutine(BounceMotion(text, duration, targetColor));
    }

    public void ToText(TextMeshProUGUI text,int i)
    {
        text.SetText("{0}", i);
    }
    public void ToText(TextMeshProUGUI text, float f)
    {
        text.SetText("{0:+0.00}", f); // ex) +1.53
    }

    public void OffGameOverUIGroup()
    {
        GameOverUIGroup.gameObject.SetActive(false);
    }

    public void UpdateBounce()
    {
        ScoreScale(scoreText, Color.white, Color.white);
        ScoreScale(bestScoreText, Color.yellow, Color.grey);
    }

    public void Fit()
    {
        ScoreBounce(scoreText, bounceDuration);
        ScoreBounce(feverText, bounceDuration);
        ScoreBounce(scoreAmountText, bounceDuration ,Color.green);
    }

    public void Unfit() { return; }
}

