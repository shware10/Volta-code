using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// PlayFab 기반 로그인 / 회원가입 UI 처리 클래스
public class LoginPagePlayfab : MonoBehaviour
{
    [Header("Login")]
    [SerializeField] GameObject login;                 // 로그인 UI 패널
    [SerializeField] TMP_InputField emailField;        // 이메일 입력 필드
    [SerializeField] TMP_InputField passwordField;     // 비밀번호 입력 필드
    [SerializeField] Button logInButton;               // 로그인 버튼
    [SerializeField] Button newAccountButton;          // 회원가입 화면 이동 버튼
    [SerializeField] TextMeshProUGUI errorMessage;     // 로그인 에러 메시지

    [Header("Register")]
    [SerializeField] GameObject register;              // 회원가입 UI 패널
    [SerializeField] TMP_InputField registerEmailField;
    [SerializeField] TMP_InputField registerPasswordField;
    [SerializeField] TMP_InputField passwordConfirmField;
    [SerializeField] TMP_InputField nickNameField;
    [SerializeField] Button registerButton;
    [SerializeField] Button backButton;
    [SerializeField] TextMeshProUGUI registerErrorMessage; // 회원가입 에러 메시지

    // 로그인 버튼 클릭 시 호출
    public void OnclickedLogInButton()
    {
        // PlayFab 로그인 요청 생성
        var loginRequest = new LoginWithEmailAddressRequest
        {
            Email = emailField.text,
            Password = passwordField.text
        };

        // 비밀번호 길이 체크
        if (passwordField.text.Length < 8)
        {
            errorMessage.text = "Password must be at least 8 characters.";
            return;
        }

        // PlayFab 서버에 로그인 요청
        PlayFabClientAPI.LoginWithEmailAddress(loginRequest, OnLoginSuccess, OnLoginFailure);
    }

    // 로그인 성공 시 로비 씬으로 이동
    private void OnLoginSuccess(LoginResult result)
        => SceneManager.LoadScene("Lobby");

    // 로그인 실패 시 에러 메시지 출력
    private void OnLoginFailure(PlayFabError error)
        => errorMessage.SetText("Invalid email or password.");

    // 새 계정 만들기 버튼 클릭
    public void OnclickedNewAccountButton()
    {
        // 로그인 UI 숨기고 회원가입 UI 표시
        login.SetActive(false);
        register.SetActive(true);

        // 입력 필드 초기화
        ClearField();
    }

    // 회원가입 버튼 클릭
    public void OnclickedRegisterButton()
    {
        // PlayFab 회원가입 요청 생성
        var registerRequest = new RegisterPlayFabUserRequest
        {
            Email = registerEmailField.text,
            Password = registerPasswordField.text,
            Username = nickNameField.text,
            DisplayName = nickNameField.text
        };

        // 비밀번호 확인 체크
        if (registerPasswordField.text != passwordConfirmField.text)
        {
            registerErrorMessage.text = "Password confirmation.";
            return;
        }

        // 비밀번호 길이 체크
        if (registerPasswordField.text.Length < 8)
        {
            registerErrorMessage.text = "Password must be at least 8 characters.";
            return;
        }

        // PlayFab 회원가입 요청
        PlayFabClientAPI.RegisterPlayFabUser(registerRequest, OnRegisterSuccess, OnRegisterFailure);
    }

    // 회원가입 성공 시
    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        // 기본 스코어 데이터 생성
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate { StatisticName = "Score", Value = 0 }
            }
        };

        // 통계 업데이트 요청
        PlayFabClientAPI.UpdatePlayerStatistics(
            request,
            (result) => { },
            (error) => print("값 저장 실패")
        );

        // 로그인 화면으로 돌아가기
        login.SetActive(true);
        register.SetActive(false);

        // 입력 필드 초기화
        ClearField();
    }

    // 회원가입 실패 시 에러 메세지 출력
    private void OnRegisterFailure(PlayFabError error)
    {
        registerErrorMessage.SetText("Register failed.");
    }

    // 뒤로가기 버튼 클릭 (회원가입 → 로그인)
    public void OnclickedBackButton()
    {
        login.SetActive(true);
        register.SetActive(false);
        ClearField();
    }

    // 모든 입력 필드 및 에러 메시지 초기화
    private void ClearField()
    {
        // 로그인 UI 초기화
        errorMessage.SetText("");
        emailField.text = null;
        passwordField.text = null;

        // 회원가입 UI 초기화
        registerErrorMessage.SetText("");
        registerEmailField.text = null;
        passwordConfirmField.text = null;
        registerPasswordField.text = null;
        nickNameField.text = null;
    }
}