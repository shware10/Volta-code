using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class IntroManager : MonoBehaviour
{
    [SerializeField] private Material CRTShader;
    private float originValue;
    [SerializeField] private float targetValue = 0.5f;
    [SerializeField] private float introDuration = 1f;
    [SerializeField] private float noiseDuration = 0.5f;

    public BlinkingText btext;
    public Transform title;

    private bool isReady = false;

    void Awake()
    {
        originValue = CRTShader.GetFloat("_Blur_Offset");
        StartCoroutine(Intro());
    }
    IEnumerator Intro()
    {
        float time = 0;
        RenderFeatureToggler.Instance.ToggleFeature(true);
        float expandDuration = introDuration * 0.6f;
        float contractDuration = introDuration * 0.4f;

        while (time < expandDuration)
        {
            float percent = time / expandDuration;
            title.localScale = Vector3.one * Mathf.Lerp(1f, 1.2f, percent);
            float value = Mathf.Lerp(originValue, targetValue, percent);
            CRTShader.SetFloat("_Blur_Offset", value);

            time += Time.deltaTime;
            yield return null;
        }
        title.localScale = Vector3.one * 1.2f;
        CRTShader.SetFloat("_Blur_Offset", targetValue);

        while (time < contractDuration)
        {
            float percent = time / contractDuration;
            title.localScale = Vector3.one * Mathf.Lerp(1.2f, 1f, percent);
            float value = Mathf.Lerp(targetValue, originValue, percent);
            CRTShader.SetFloat("_Blur_Offset", value);

            time += Time.deltaTime;
            yield return null;
        }

        title.localScale = Vector3.one;
        CRTShader.SetFloat("_Blur_Offset", originValue);

        yield return new WaitForSeconds(noiseDuration);
        RenderFeatureToggler.Instance.ToggleFeature(false);
        btext.TurnOn();
        isReady = true;
    }

    void Update()
    {
        if((isReady && Input.touchCount > 0) || Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(1);
        }
    }

}
