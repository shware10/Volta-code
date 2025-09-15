using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public static ButtonManager Instance;

    [Header("Clikable Text")]
    [Header("MainMenu")]
    public ClickableText startButton;
    public ClickableText exitButton;

    [Header("InGame Text")]
    public ClickableText FeverBonusText;

    [Header("GameOver")]
    public ClickableText freePresentButton;
    public ClickableText yesButton;
    public ClickableText noButton;

    [Header("흔들림 팩터")]
    //회전 떨림
    public float rotWobbleStrength = 5f;
    public float rotWobbleSpeed = 5f;

    // 탑다운 떨림
    public float tdWobbleStrength = 2f;
    public float tdWobbleSpeed = 10f;


    [Header("깜빡거림 팩터")]
    [SerializeField] private float blinkDuration = 0.25f;
    void Awake()
    {
        Instance = this; 
        //mainmenu
        startButton.OnEnter += DarkenText;
        startButton.OnExit += LightenText;
        startButton.OnClick += BlinkText;
        startButton.OnIdle += WobbleTextR;

        exitButton.OnEnter += DarkenText;
        exitButton.OnExit += LightenText;
        exitButton.OnIdle += WobbleTextTD;

        //InGame
        FeverBonusText.OnIdle += WobbleTextTD;

        //gameover
        yesButton.OnEnter += DarkenText;
        yesButton.OnExit += LightenText;
        yesButton.OnIdle += WobbleTextR;

        noButton.OnEnter += DarkenText;
        noButton.OnExit += LightenText;
        noButton.OnIdle += WobbleTextR;

        freePresentButton.OnIdle += WobbleTextTD;
    }

    //Method Group - Clickable Text//
    #region

    void LightenText(TextMeshProUGUI text)
    {
        Color color = text.color;
        color.a = 1f;
        text.color = color;
    }

    void DarkenText(TextMeshProUGUI text)
    {
        Color color = text.color;
        color.a = 0.7f;
        text.color = color;
    }

    void BlinkText(TextMeshProUGUI text)
    {
        StartCoroutine(Blink(text));
    }
    IEnumerator Blink(TextMeshProUGUI text)
    {
        CanvasGroup canvasGroup = text.transform.GetComponent<CanvasGroup>();
        float time = 0f;
        while (time < blinkDuration)
        {
            float radian = Mathf.Lerp(0, Mathf.PI * 2, time / blinkDuration);
            float pingpong = 1 - Mathf.Abs(Mathf.Sin(radian));
            canvasGroup.alpha = pingpong;

            time += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    void WobbleTextR(TextMeshProUGUI text)
    {
        StartCoroutine(RotateWobbling(text));
    }
    void WobbleTextTD(TextMeshProUGUI text)
    {
        StartCoroutine(TopDownWobbling(text));
    }

    //텍스트 회전 흔들림 코루틴
    IEnumerator RotateWobbling(TextMeshProUGUI text)
    {
        // 텍스트 메시 정보를 최신 상태로 갱신 -> 텍스트 정보는 최적화를 위해
        while(true)
        {
            text.ForceMeshUpdate();

            TMP_TextInfo textInfo = text.textInfo; //문자열 텍스트의 정보 

            for (int i = 0; i < textInfo.characterCount; ++i)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i]; //텍스트 중 단어 하나 가져오기

                if (!charInfo.isVisible) continue; // 실제로 보이는 경우에만 처리 -> 공백 무시

                //단어의 꼭지점 첫 인덱스 (quad의 왼쪽 아래)
                int vertexIndex = charInfo.vertexIndex;
                // 머터리얼 메쉬의 인덱스를 가져옴 (머터리얼이 꼭지점 배열정보를 가지고 있음)
                int materialIndex = charInfo.materialReferenceIndex;

                // 꼭지점 배열 정보 가져오기 참조임!
                Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

                // 흔들림 오프셋
                Vector3 offset = new Vector3(
                    Mathf.Sin(Time.time * rotWobbleSpeed + i) * rotWobbleStrength, // x
                    Mathf.Cos(Time.time * rotWobbleSpeed + i) * rotWobbleStrength, // y
                    0); // z

                // 꼭지점 마다 오프셋 더해주기
                for (int j = 0; j < 4; ++j)
                {
                    if (j % 2 == 0) vertices[vertexIndex + j] += offset;
                    else vertices[vertexIndex + j] += offset / 2;
                }
            }

            for (int i = 0; i < textInfo.meshInfo.Length; ++i)
            {

                TMP_MeshInfo meshInfo = textInfo.meshInfo[i];
                //실제 메쉬에 임시 꼭지점 정보를 대입
                meshInfo.mesh.vertices = meshInfo.vertices;
                //메쉬 변경을 알려 화면에 즉시 반영
                text.UpdateGeometry(meshInfo.mesh, i);
            }

            yield return null;
        }
    }

    IEnumerator TopDownWobbling(TextMeshProUGUI text)
    {
        while(true)
        {
            // 텍스트 메시 정보를 최신 상태로 갱신 -> 텍스트 정보는 최적화를 위해 
            text.ForceMeshUpdate();

            TMP_TextInfo textInfo = text.textInfo; //문자열 텍스트의 정보 

            for (int i = 0; i < textInfo.characterCount; ++i)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i]; //텍스트 중 단어 하나 가져오기

                if (!charInfo.isVisible) continue; // 실제로 보이는 경우에만 처리 공백 무시

                //단어의 꼭지점 첫 인덱스 (quad의 왼쪽 아래)
                int vertexIndex = charInfo.vertexIndex;
                // 머터리얼 메쉬의 인덱스를 가져옴 (머터리얼이 꼭지점 배열정보를 가지고 있음)
                int materialIndex = charInfo.materialReferenceIndex;

                // 꼭지점 배열 정보 가져오기 참조임!
                Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

                // 흔들림 오프셋
                Vector3 offset = new Vector3(
                    0, // x
                    Mathf.Sin(Time.time * tdWobbleSpeed + i) * tdWobbleStrength, // y
                    0); // z

                // 꼭지점 마다 오프셋 더해주기
                for (int j = 0; j < 4; ++j)
                {
                    vertices[vertexIndex + j] += offset;
                }
            }

            for (int i = 0; i < textInfo.meshInfo.Length; ++i)
            {

                TMP_MeshInfo meshInfo = textInfo.meshInfo[i];
                //실제 메쉬에 임시 꼭지점 정보를 대입
                meshInfo.mesh.vertices = meshInfo.vertices;
                //메쉬 변경을 알려 화면에 즉시 반영
                text.UpdateGeometry(meshInfo.mesh, i);
            }

            yield return null;
        }

    }
    #endregion

    //Method Group - Clickable Image//
    void DarkenImage(CanvasGroup cg)
    {
        cg.alpha = 0.7f;
    }

    void LightenImage(CanvasGroup cg)
    {
        cg.alpha = 1f;
    }

    void BlinkImage(CanvasGroup cg)
    {
        StartCoroutine(Blink(cg));
    }
    IEnumerator Blink(CanvasGroup cg)
    {
        float time = 0f;
        while (time < blinkDuration)
        {
            float radian = Mathf.Lerp(0, Mathf.PI * 2, time / blinkDuration);
            float pingpong = 1 - Mathf.Abs(Mathf.Sin(radian));
            cg.alpha = pingpong;

            time += Time.deltaTime;
            yield return null;
        }
        cg.alpha = 1f;
    }
}
