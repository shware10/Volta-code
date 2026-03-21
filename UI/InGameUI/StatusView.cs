
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 플레이어 상태를 UI에 표시하는 뷰 클래스
public class StatusView : MonoBehaviour, IHpListener, IStaminaListener, IGameStateListener
{
    [SerializeField] private Image HpBar;                 // 체력 바
    [SerializeField] private Image StaminarBar;           // 스태미나 바
    [SerializeField] private TextMeshProUGUI HpText;      // 체력 수치 텍스트
    [SerializeField] private TextMeshProUGUI playerName;  // 플레이어 이름 표시 텍스트

    // 체력이 변경될 때 호출되는 함수
    public void OnHpChanged(float hp, float maxHp)
    {
        // 현재 체력 비율만큼 UI 바 채우기 (0 ~ 1)
        HpBar.fillAmount = hp / maxHp;

        // 체력 수치를 정수로 표시
        HpText.SetText($"{(int)hp}");
    }

    // 스태미나가 변경될 때 호출되는 함수
    public void OnStaminaChanged(float stamina, float maxStamina)
    {
        // 현재 스태미나 비율만큼 UI 바 채우기
        StaminarBar.fillAmount = stamina / maxStamina;
    }

    // 게임 상태 변경 시 호출
    public void OnStateChanged(GameState state)
    {
        // 게임 진입 시 플레이어 이름을 UI에 표시
        if (state == GameState.Ready)
            playerName.SetText(GameManager.Instance.UserId);
    }
}
