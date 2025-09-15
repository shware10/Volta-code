using System.Collections;
using UnityEngine;

public class SpawnedSticky : Sticky
{
    [SerializeField] private float stickDuration = 0.2f;
    [SerializeField] private float rotateDuration = 0.3f;
    [SerializeField] private float targetScale = 1.2f;
    private LineRenderer LineRdr;

    public override void OnStateChanged(GameState state)
    {
        if (state == GameState.GameOver)
        {
            if (gameObject.activeSelf)
            {
                Unstick();
                base.Unsubscribe();
            }
        }
    }
    public override void Init(Material mat, float amount)
    {
        base.Init(mat, amount);
        LineRdr = transform.GetChild(0).GetComponent<LineRenderer>();

        LineRdr.material = mat;

        Color color = LineRdr.startColor;
        color.a = 0f;
        LineRdr.startColor = color;
        LineRdr.endColor = color;

        GameManager.Instance.AddScoreAmount(bonusScore);
    }
    public void Stick(Vector3 stickPos)
    {
        gameObject.SetActive(true);
        transform.position = stickPos;

        StartCoroutine(StickRoutine());
        base.Subscribe();
    }

    public void Unstick()
    {
        FadeOut();
    }
    public void FadeOut()
    {
        StartCoroutine(FadeRoutine());
    }
    IEnumerator FadeRoutine()
    {
        yield return StartCoroutine(FadeMotion(0.2f, 1f, 0f));
        yield return StartCoroutine(ScaleMotion(0.1f, 1f, targetScale));
        yield return StartCoroutine(TiltMotion(0.2f, 5));
        yield return StartCoroutine(ScaleMotion(0.1f, targetScale, 0f));

        StickySpawner.Instance.spawnedStickyPool.Enqueue(gameObject);
        gameObject.SetActive(false);
    }
    IEnumerator StickRoutine()
    {
        yield return StartCoroutine(ScaleMotion(0.1f, 0, targetScale));
        yield return StartCoroutine(TiltMotion(0.2f, 5));
        yield return StartCoroutine(ScaleMotion(0.1f, targetScale, 1f));
        StartCoroutine(FadeMotion(0.5f, 0f, 1f));
    }
    IEnumerator FadeMotion(float fadeDuration, float start, float end)
    {
        float time = 0;
        while (time < fadeDuration)
        {
            float alpha = Mathf.Lerp(start, end, time / fadeDuration);
            SetLineAlpha(alpha);

            time += Time.deltaTime;
            yield return null;
        }
        SetLineAlpha(end);
    }
    void SetLineAlpha(float alpha)
    {
        Gradient grad = LineRdr.colorGradient;
        GradientAlphaKey[] alphaKeys = grad.alphaKeys;

        for (int i = 0; i < alphaKeys.Length; i++)
        {
            alphaKeys[i].alpha = alpha;
        }

        grad.alphaKeys = alphaKeys;
        LineRdr.colorGradient = grad;
    }
    IEnumerator ScaleMotion(float scaleDuration, float startScale, float endScale)
    {
        float time = 0;

        while (time < stickDuration)
        {
            transform.localScale = Vector3.one * Mathf.Lerp(startScale, endScale, time / stickDuration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.localScale = Vector3.one * endScale;
    }
    IEnumerator TiltMotion(float tiltDuration, float tiltAmount)
    {
        float time = 0;
        Vector3 originAngles = transform.localEulerAngles;
        while (time < tiltDuration)
        {
            float radian = Mathf.Lerp(0, 2 * Mathf.PI, time / tiltDuration);
            transform.localEulerAngles = new Vector3(originAngles.x, originAngles.y, originAngles.z + tiltAmount * Mathf.Sin(radian));

            time += Time.deltaTime;
            yield return null;
        }
        transform.localEulerAngles = originAngles;
    }
    IEnumerator RotateMotion()
    {
        float time = 0;
        while(time < rotateDuration)
        {
            float angle = Mathf.Lerp(0, 180, time / rotateDuration);
            transform.rotation = Quaternion.Euler(0, angle, 0);

            time += Time.deltaTime;
            yield return null;
        }
        transform.rotation = Quaternion.identity;
    }
    IEnumerator ScaleRoutine()
    {
        yield return StartCoroutine(ScaleMotion(0.1f, 1f, targetScale));
        yield return StartCoroutine(TiltMotion(0.1f, 5));
        yield return StartCoroutine(ScaleMotion(0.1f, targetScale, 1f));
    }
    public override void Fit()
    {
        int randInt = Random.Range(0, 2);
        switch (randInt)
        {
            case 0: 
                StartCoroutine(RotateMotion());
                break;
            case 1:
                StartCoroutine(ScaleRoutine());
                break;
        }
    }
    
    public override void Unfit()
    {
        return;
    }
}
