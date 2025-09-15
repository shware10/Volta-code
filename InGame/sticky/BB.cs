using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BB : Sticky
{
    [SerializeField] private float wobbleStrength = 0.05f;
    [SerializeField] private float wobbleSpeed = 6f;
    [SerializeField] private Vector3 originPos;
    [SerializeField] private Vector3 originScale;
    Animator anim;

    Coroutine wobbleRoutine;

    void Start()
    {
        Intro();
        base.Subscribe();
        anim = GetComponent<Animator>();
        originPos = transform.position;
        originScale = Vector3.one;    
    }
    public override void OnStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.MainMenu :
                Intro();
                break;
            case GameState.GameOver:
                Outro();
                break; 
        }
    }
    void Intro()
    {
        gameObject.SetActive(true);
        StartCoroutine(IntroRoutine());
        wobbleRoutine = StartCoroutine(WobbleMotion());
    }
    void Outro()
    {
        StartCoroutine(OutroRoutine());
        StopCoroutine(wobbleRoutine);
    }
    IEnumerator IntroRoutine()
    {
        yield return StartCoroutine(ScaleMotion(0f, 1.2f, 0.2f));
        yield return StartCoroutine(TiltMotion(0.2f, 10f));
        yield return StartCoroutine(ScaleMotion(1.2f, 1f, 0.1f));
    }
    IEnumerator OutroRoutine()
    {
        yield return StartCoroutine(TiltMotion(0.2f, 10f));
        yield return StartCoroutine(ScaleMotion(1.2f, 0f, 0.1f));
        gameObject.SetActive(false);
    }
    IEnumerator ScaleMotion(float start, float end, float scaleDuration)
    {
        float time = 0;
        while(time < scaleDuration)
        {
            transform.localScale = Vector3.one * Mathf.Lerp(start, end, time / scaleDuration);

            time += Time.deltaTime;
            yield return null;
        }
        transform.localScale = Vector3.one * end;
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

    IEnumerator WobbleMotion()
    {
        yield return new WaitForSeconds(0.5f);
        while (true)
        {
            yield return null;
            float offsetY = Mathf.Abs(Mathf.Sin(Time.time * wobbleSpeed) * wobbleStrength);
            float scaleOffset = 0.01f * Mathf.Abs(Mathf.Sin(Time.time * wobbleSpeed));
            transform.position = new Vector3(originPos.x, originPos.y + offsetY, originPos.z);
            transform.localScale = new Vector3(originScale.x + (scaleOffset*2), originScale.y - scaleOffset, originScale.z);
        }
    }
    public override void Fit()
    {
        anim.SetTrigger("Fit");

    }

    public override void Unfit()
    {
        anim.SetTrigger("Unfit");
    }
}
