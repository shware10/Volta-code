
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class StatusView : MonoBehaviour, IHpListener, IStaminaListener, IGameStateListener
{
    [SerializeField] private Image HpBar;
    [SerializeField] private Image StaminarBar;

    [SerializeField] private TextMeshProUGUI HpText;
    [SerializeField] private TextMeshProUGUI playerName;

    public void OnHpChanged(float hp, float maxHp)
    {
        HpBar.fillAmount = hp / maxHp;
        HpText.SetText($"{(int)hp}");
    }
    public void OnStaminaChanged(float stamina, float maxStamina)
    {
        StaminarBar.fillAmount = stamina / maxStamina;
    }

    public void OnStateChanged(GameState state)
    {
        if(state == GameState.Ready) playerName.SetText(GameManager.Instance.UserId);
    }
}
