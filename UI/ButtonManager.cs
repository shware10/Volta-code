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

    [Header("��鸲 ����")]
    //ȸ�� ����
    public float rotWobbleStrength = 5f;
    public float rotWobbleSpeed = 5f;

    // ž�ٿ� ����
    public float tdWobbleStrength = 2f;
    public float tdWobbleSpeed = 10f;


    [Header("�����Ÿ� ����")]
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

    //�ؽ�Ʈ ȸ�� ��鸲 �ڷ�ƾ
    IEnumerator RotateWobbling(TextMeshProUGUI text)
    {
        // �ؽ�Ʈ �޽� ������ �ֽ� ���·� ���� -> �ؽ�Ʈ ������ ����ȭ�� ����
        while(true)
        {
            text.ForceMeshUpdate();

            TMP_TextInfo textInfo = text.textInfo; //���ڿ� �ؽ�Ʈ�� ���� 

            for (int i = 0; i < textInfo.characterCount; ++i)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i]; //�ؽ�Ʈ �� �ܾ� �ϳ� ��������

                if (!charInfo.isVisible) continue; // ������ ���̴� ��쿡�� ó�� -> ���� ����

                //�ܾ��� ������ ù �ε��� (quad�� ���� �Ʒ�)
                int vertexIndex = charInfo.vertexIndex;
                // ���͸��� �޽��� �ε����� ������ (���͸����� ������ �迭������ ������ ����)
                int materialIndex = charInfo.materialReferenceIndex;

                // ������ �迭 ���� �������� ������!
                Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

                // ��鸲 ������
                Vector3 offset = new Vector3(
                    Mathf.Sin(Time.time * rotWobbleSpeed + i) * rotWobbleStrength, // x
                    Mathf.Cos(Time.time * rotWobbleSpeed + i) * rotWobbleStrength, // y
                    0); // z

                // ������ ���� ������ �����ֱ�
                for (int j = 0; j < 4; ++j)
                {
                    if (j % 2 == 0) vertices[vertexIndex + j] += offset;
                    else vertices[vertexIndex + j] += offset / 2;
                }
            }

            for (int i = 0; i < textInfo.meshInfo.Length; ++i)
            {

                TMP_MeshInfo meshInfo = textInfo.meshInfo[i];
                //���� �޽��� �ӽ� ������ ������ ����
                meshInfo.mesh.vertices = meshInfo.vertices;
                //�޽� ������ �˷� ȭ�鿡 ��� �ݿ�
                text.UpdateGeometry(meshInfo.mesh, i);
            }

            yield return null;
        }
    }

    IEnumerator TopDownWobbling(TextMeshProUGUI text)
    {
        while(true)
        {
            // �ؽ�Ʈ �޽� ������ �ֽ� ���·� ���� -> �ؽ�Ʈ ������ ����ȭ�� ���� 
            text.ForceMeshUpdate();

            TMP_TextInfo textInfo = text.textInfo; //���ڿ� �ؽ�Ʈ�� ���� 

            for (int i = 0; i < textInfo.characterCount; ++i)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i]; //�ؽ�Ʈ �� �ܾ� �ϳ� ��������

                if (!charInfo.isVisible) continue; // ������ ���̴� ��쿡�� ó�� ���� ����

                //�ܾ��� ������ ù �ε��� (quad�� ���� �Ʒ�)
                int vertexIndex = charInfo.vertexIndex;
                // ���͸��� �޽��� �ε����� ������ (���͸����� ������ �迭������ ������ ����)
                int materialIndex = charInfo.materialReferenceIndex;

                // ������ �迭 ���� �������� ������!
                Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

                // ��鸲 ������
                Vector3 offset = new Vector3(
                    0, // x
                    Mathf.Sin(Time.time * tdWobbleSpeed + i) * tdWobbleStrength, // y
                    0); // z

                // ������ ���� ������ �����ֱ�
                for (int j = 0; j < 4; ++j)
                {
                    vertices[vertexIndex + j] += offset;
                }
            }

            for (int i = 0; i < textInfo.meshInfo.Length; ++i)
            {

                TMP_MeshInfo meshInfo = textInfo.meshInfo[i];
                //���� �޽��� �ӽ� ������ ������ ����
                meshInfo.mesh.vertices = meshInfo.vertices;
                //�޽� ������ �˷� ȭ�鿡 ��� �ݿ�
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
