using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class RectangleHandler : MonoBehaviour, IGameStateListener, IBarStateListener
{
    public List<Bar> barList = new List<Bar>();
    private List<Collider> colList = new List<Collider>();

    Color[] BarColors = { Color.red, Color.green, Color.blue, Color.white };

    [Header("Touch")]
    private Vector2 swipeStart;
    private float inchesThreshold = 0.2f; // 1inch = 2.54cm;
    private float dpi;
    private float minSwipeDist;
    private float halfWidth;

    [Header("확장/수축 모션 팩터")]
    [SerializeField] private float introDuration = 0.5f;
    [SerializeField] private float targetScale = 4f;
    [SerializeField] private float targetAngle = 180f;
    [SerializeField] private float barTargetScale = 0.04f;
    [SerializeField] private float barTargetPos = 0.3f;
    private float[] barDir = { 1, 1, -1, -1 };

    [Header("회전 모션 팩터")]
    [SerializeField] private float rotateZDuration = 0.2f;
    [SerializeField] private float rotateXDuration = 0.2f;

    [Header("바운스 모션 팩터")]
    [SerializeField] private float bounceDuration = 0.2f;
    [SerializeField] private float bounceScale = 4.5f;

    [Header("페이드 팩터")]
    [SerializeField] private float fadeDuration = 1f;
    private bool isRotating = false;

    private bool isInGame = false;
    private bool isBack = false;

    public void OnStateChanged(GameState state)
    {
        isInGame = state == GameState.InGame;
        switch (state)
        {
            case GameState.InGame:
                //인트로 모션이 끝나면 collider 활성화
                StartCoroutine(Intro());
                break;
            case GameState.GameOver:
                StartCoroutine(Outro());
                break;
            case GameState.MainMenu:
                FadeIn();
                break;
        }
    }

    void Start()
    {
        int idx = 0; //칼라 인덱싱
        List<IBarStateListener> Listeners = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                                            .OfType<IBarStateListener>()
                                            .ToList();

        foreach (Transform barTransform in transform)
        {
            Bar bar = barTransform.GetComponent<Bar>();
            bar.color = BarColors[idx++];

            foreach (IBarStateListener listener in Listeners)
            {
                bar.FitColor += listener.Fit;
                bar.UnfitColor += listener.Unfit;
            }
            barList.Add(bar);
            colList.Add(barTransform.GetComponent<Collider>());
        }

        dpi = Screen.dpi;
        minSwipeDist = inchesThreshold * dpi;
        halfWidth = Screen.width * 0.5f;
    }


    //인트로/아웃트로 움직임 코루틴
    IEnumerator Expand()
    {
        float time = 0;
        float barOriginScale = barList[0].transform.localScale.x;
        while (time < introDuration)
        {
            float progress = time / introDuration;
            //parent scale
            float parentScale = Mathf.Lerp(1f, targetScale, progress);
            transform.localScale = new Vector3(parentScale, parentScale, 1); 
            //parent rotation
            float parentAngle = Mathf.Lerp(0, targetAngle, progress);
            transform.rotation = Quaternion.Euler(0, 0, parentAngle);
            //childs scale
            float barScale = Mathf.Lerp(barOriginScale, barTargetScale, progress);
            float barPos = Mathf.Lerp(0, barTargetPos, progress);
            for (int i = 0; i < 4; ++i)
            {
                
                if (i % 2 == 0)
                {
                    barList[i].transform.localScale = new Vector3(barOriginScale, barScale, barOriginScale);
                    barList[i].transform.localPosition = new Vector3(0, barDir[i] * barPos, 0);
                }
                else
                {
                    barList[i].transform.localScale = new Vector3(barScale, barOriginScale, barOriginScale);
                    barList[i].transform.localPosition = new Vector3(barDir[i] * barPos, 0, 0);
                }
            }

            time += Time.deltaTime;

            yield return null;
        }

        // 위치 보정
        for(int i = 0; i < 4; ++i)
        {
            if (i % 2 == 0)
            {
                barList[i].transform.localScale = new Vector3(barOriginScale, barTargetScale, barOriginScale);
                barList[i].transform.localPosition = new Vector3(0, barDir[i] * barTargetPos, 0);
            }
            else
            {
                barList[i].transform.localScale = new Vector3(barTargetScale, barOriginScale, barOriginScale);
                barList[i].transform.localPosition = new Vector3(barDir[i] * barTargetPos, 0, 0);
            }
        }
        transform.localScale = new Vector3(targetScale, targetScale, 1);
        transform.rotation = Quaternion.Euler(0, 0, targetAngle);
    }
    IEnumerator Contract()
    {
        float time = 0;

        float barOriginScale = barList[0].transform.localScale.x;

        while (time < introDuration)
        {
            float progress = time / introDuration;
            //사각형의 스케일을 늘립니다

            float parentScale = Mathf.Lerp(targetScale, 1f, progress);
            transform.localScale = Vector3.one * parentScale;
            //사각형 전체를 회전시킵니다
            float parentAngle = Mathf.Lerp(targetAngle, 0, progress);
            transform.rotation = Quaternion.Euler(0, 0, parentAngle);
            //사각형을 이루도록 바들 각각의 스케일과 위치를 조정합니다
            float barScale = Mathf.Lerp(barTargetScale, barOriginScale, progress);
            float barPos = Mathf.Lerp(barTargetPos, 0, progress);
            for (int i = 0; i < 4; ++i)
            {
                if (i % 2 == 0)
                {
                    barList[i].transform.localScale = new Vector3(barOriginScale, barScale, barOriginScale);
                    barList[i].transform.localPosition = new Vector3(0, barDir[i] * barPos, 0);
                }
                else
                {
                    barList[i].transform.localScale = new Vector3(barScale, barOriginScale, barOriginScale);
                    barList[i].transform.localPosition = new Vector3(barDir[i] * barPos, 0, 0);
                }
            }

            time += Time.deltaTime;

            yield return null;
        }
        // 위치 보정
        for (int i = 0; i < 4; ++i)
        {
            barList[i].transform.localScale = Vector3.one / 2;
            barList[i].transform.localPosition = new Vector3(0, transform.position.y, 0);
        }
        transform.localScale = Vector3.one;
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }
    IEnumerator Intro()
    {
        yield return StartCoroutine(Expand());
        EnableCollider(isInGame);
        yield return StartCoroutine(ActivateControll());
    }
    IEnumerator Outro()
    {
        EnableCollider(isInGame);
        yield return StartCoroutine(Contract());
        FadeOut();
        //yield return StartCoroutine(Spin(state));
    }
    void FadeOut()
    {
        StartCoroutine(Fade(false));
    }
    void FadeIn()
    {
        StartCoroutine(Fade(true));
    }
    public IEnumerator Fade(bool activate)
    {
        if (activate) yield return new WaitForSeconds(fadeDuration);

        int startIdx = activate ? 0 : 3;
        int value = activate ? 1 : -1;
        int endIdx = 3 - startIdx;

        for (int i = startIdx; i != endIdx + value; i += value)
        {
            yield return StartCoroutine(FadeStep(activate, fadeDuration / 4, i));
        }
    }
    IEnumerator FadeStep(bool activate, float scaleDuration, int barIdx) //메인 메뉴면 켜지고 
    {
        float time = 0;
        float start = activate ? 0f : 0.5f;
        float end = 0.5f - start;

        Bar bar = barList[barIdx];

        while (time <= scaleDuration)
        {
            float percent = time / scaleDuration;
            bar.transform.localScale = Vector3.one * Mathf.Lerp(start, end, percent);

            time += Time.deltaTime;
            yield return null;
        }

        bar.transform.localScale = Vector3.one * end;
    }

    //인게임 Input controll
    IEnumerator ActivateControll()
    {
        isBack = false;
        while (isInGame)
        {
            if(isRotating == false)
            {
                
                #region For keyboard
                
                if (Input.GetKeyDown(KeyCode.LeftControl))
                {
                    StartCoroutine(RotateZ(transform.localEulerAngles.z, transform.localEulerAngles.z + (!isBack ? 90 : -90)));
                }
                else if (Input.GetKeyDown(KeyCode.LeftAlt))
                {
                    StartCoroutine(RotateZ(transform.localEulerAngles.z, transform.localEulerAngles.z + (!isBack ? -90 : 90)));
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    StartCoroutine(RotateX(transform.localEulerAngles.x, transform.localEulerAngles.x + 180));
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    StartCoroutine(RotateX(transform.localEulerAngles.x, transform.localEulerAngles.x - 180));
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    StartCoroutine(RotateY(transform.localEulerAngles.y, transform.localEulerAngles.y + 180));
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    StartCoroutine(RotateY(transform.localEulerAngles.y, transform.localEulerAngles.y - 180));
                }
                
                #endregion
                
                #region For TouchPad
                
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    if(touch.phase == TouchPhase.Began)
                    {
                        swipeStart = touch.position;
                    }
                    else if(touch.phase == TouchPhase.Ended)
                    {
                        Vector2 swipeEnd = touch.position;
                        Vector2 dist = swipeEnd - swipeStart;

                        if (dist.sqrMagnitude > minSwipeDist * minSwipeDist)
                        {
                            dist.Normalize();
                            if (Mathf.Abs(dist.x) > Mathf.Abs(dist.y)) //가로 성분이 더 크면
                            {
                                if (dist.x > 0)
                                { StartCoroutine(RotateY(transform.localEulerAngles.y, transform.localEulerAngles.y - 180)); } // 오른쪽으로 스와이프
                                else
                                { StartCoroutine(RotateY(transform.localEulerAngles.y, transform.localEulerAngles.y + 180)); } // 왼쪽으로 스와이프
                            }
                            else // 세로 성분이 더 크면
                            {
                                if (dist.y > 0)
                                { StartCoroutine(RotateX(transform.localEulerAngles.x, transform.localEulerAngles.x + 180)); } // 위로 스와이프
                                else
                                { StartCoroutine(RotateX(transform.localEulerAngles.x, transform.localEulerAngles.x - 180)); } // 아래로 스와이프
                            }
                        }
                        else // 그냥 터치
                        {
                            if (swipeEnd.x > halfWidth)
                            { StartCoroutine(RotateZ(transform.localEulerAngles.z, transform.localEulerAngles.z + (!isBack ? -90 : 90))); } // 오른쪽으로 회전
                            else
                            { StartCoroutine(RotateZ(transform.localEulerAngles.z, transform.localEulerAngles.z + (!isBack ? 90 : -90))); } // 왼쪽으로 회전
                        }
                    }
                }
                
                #endregion
                
            }

            yield return null;
        }
    }

    IEnumerator RotateZ(float start, float end)
    {
        float time = 0;
        isRotating = true;
        float originX = transform.localEulerAngles.x;
        float originY = transform.localEulerAngles.y;
        while (time <= rotateZDuration)
        {
            float curAngle = Mathf.Lerp(start, end, time / rotateZDuration);
            transform.rotation = Quaternion.Euler(new Vector3(originX, originY, curAngle));

            time += Time.deltaTime;
            yield return null;
        }

        transform.rotation = Quaternion.Euler(new Vector3(originX, originY, end));
        isRotating = false;
    }
    IEnumerator RotateY(float start, float end)
    {
        isBack = !isBack;
        float time = 0;
        isRotating = true;
        float originX = transform.localEulerAngles.x;
        float originZ = transform.localEulerAngles.z;
        while (time <= rotateZDuration)
        {
            float curAngle = Mathf.Lerp(start, end, time / rotateZDuration);
            transform.rotation = Quaternion.Euler(new Vector3(originX, curAngle, originZ));

            time += Time.deltaTime;
            yield return null;
        }

        transform.rotation = Quaternion.Euler(new Vector3(originX, end, originZ));
        isRotating = false;
    }
    IEnumerator RotateX(float start, float end)
    {
        isBack = !isBack;
        float time = 0;
        isRotating = true;
        float originZ = transform.localEulerAngles.z;
        float originY = transform.localEulerAngles.y;

        while (time <= rotateXDuration)
        {
            float curAngle = Mathf.Lerp(start, end, time / rotateXDuration);
            transform.rotation = Quaternion.Euler(new Vector3(curAngle, originY, originZ));

            time += Time.deltaTime;
            yield return null;
        }

        transform.rotation = Quaternion.Euler(new Vector3(end, originY, originZ));
        isRotating = false;
    }

    IEnumerator Bounce()
    {
        float time = 0;
        float start = targetScale;
        while(time < bounceDuration)
        {
            float t = time / bounceDuration;
            float pingPong = Mathf.Sin(t * Mathf.PI);
            float scale = Mathf.Lerp(start, bounceScale, pingPong);
            transform.localScale = Vector3.one * scale;

            time += Time.deltaTime;
            yield return null;
        }
    }
    public void RectangleBounce()
    {
        StartCoroutine(Bounce());
    }
    void EnableCollider(bool isInGame)
    {
        foreach(Collider col in colList)
        {
            col.enabled = isInGame;
        }
    } //인트로 아웃트로 중 바 끼리 충돌 막기
    public void InitBarLevel()
    {
        for (int i = 0; i < 4; ++i)
        {
            barList[i].level = 1;
        }
    }
    public void Fit()
    {
        RectangleBounce();
    }
    public void Unfit() { }

    public void FeverCheck()
    {
        if (GameManager.Instance.isFever == true) return;
        bool isFever = true;
        for (int i = 0; i < barList.Count; ++i)
        {
            if (barList[i].level != 3) isFever = false;
        }
        if(isFever)
        {
            GameManager.Instance.isFever = isFever;
            ButtonManager.Instance.FeverBonusText.gameObject.SetActive(true);
            FeverEffecter.Instance.FireWork();
        }
    }

}