using PlayFab.ClientModels;
using PlayFab;
using UnityEngine;
using TMPro;
using System.Linq;

public class LobbyPagePlayfab : MonoBehaviour
{
    public string playFabId;
    public string userName;
    public string playerLocation;
    public string playerRank;
    public string playerScore;
    public TextMeshProUGUI playerName;
    public GameObject rowPrefab;
    public Transform rowsParent;

    public LobbyUIManager lobbyUIManager;
    void Awake()
    {
        lobbyUIManager = GetComponent<LobbyUIManager>();
        // 로그인된 상태에서만 PlayFab ID를 가져옵니다.
        if (PlayFabClientAPI.IsClientLoggedIn())
        {
            // GetAccountInfo 요청을 보냅니다.
            var request = new GetAccountInfoRequest();

            PlayFabClientAPI.GetAccountInfo(request, OnGetAccountInfoSuccess, OnGetAccountInfoFailure);
        }
        else
        {
            Debug.LogError("User is not logged in.");
        }

        GetLeaderboard();
        GetPlayerCountry();
    }

    public void GetPlayerCountry()
    {
        var request = new GetPlayerProfileRequest
        {
            ProfileConstraints = new PlayerProfileViewConstraints
            {
                ShowLocations = true // 위치 정보(국가)를 요청합니다.
            }
        };
        PlayFabClientAPI.GetPlayerProfile(request, OnPlayerCountryGet, (error) => { });
    }

    public void OnPlayerCountryGet(GetPlayerProfileResult result)
    {
        playerLocation = result.PlayerProfile.Locations[0].CountryCode.Value.ToString();
        lobbyUIManager.locationText.text = playerLocation;
    }
    public void GetLeaderboard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "Score",
            StartPosition = 0,
            MaxResultsCount = 100
        };
        PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardGet, (error) => { });
    }

    // 기존 GetLeaderboard 요청의 성공 콜백
    public void OnLeaderboardGet(GetLeaderboardResult result)
    {
        // 1) 리스트 비우기
        foreach (Transform item in rowsParent)
            Destroy(item.gameObject);

        // 2) Top 10 렌더 + 내 항목 발견 여부 체크
        bool foundMe = false;

        for (int i = 0; i < result.Leaderboard.Count; i++)
        {
            if (i < 10)
            {
                GameObject row = Instantiate(rowPrefab, rowsParent);
                TextMeshProUGUI[] texts = row.GetComponentsInChildren<TextMeshProUGUI>();
                texts[0].SetText($"{result.Leaderboard[i].Position + 1}");
                texts[1].SetText(result.Leaderboard[i].DisplayName);
                texts[2].SetText($"{result.Leaderboard[i].StatValue}");
            }

            if (result.Leaderboard[i].PlayFabId == playFabId)
            {
                foundMe = true;
                ApplyMyEntry(result.Leaderboard[i]);
            }
        }

        // 3) 목록에 내가 없으면, AroundPlayer로 내 랭크/점수만 별도 갱신
        if (!foundMe)
        {
            InitMyRankAndScore();
        }
    }

    // 내 항목을 UI/필드에 적용
    private void ApplyMyEntry(PlayerLeaderboardEntry me)
    {
        playerRank = (me.Position + 1).ToString();
        playerScore = me.StatValue.ToString();

        if (lobbyUIManager != null)
        {
            lobbyUIManager.rankText.SetText(playerRank);
            lobbyUIManager.scoreText.SetText(playerScore);
        }
    }

    // 내가 리스트에 없을 때 호출: 내 주변 리더보드(사실상 내 랭크) 1개만 받아오기
    private void InitMyRankAndScore()
    {
        var req = new GetLeaderboardAroundPlayerRequest
        {
            PlayFabId = playFabId,
            MaxResultsCount = 1 // 나만 딱 받아오면 됨
        };

        PlayFabClientAPI.GetLeaderboardAroundPlayer(
            req,
            OnInitSuccess,
            OnInitFailed
        );
    }

    private void OnInitSuccess(GetLeaderboardAroundPlayerResult res)
    {
        var me = res.Leaderboard.FirstOrDefault(e => e.PlayFabId == playFabId);
        if (me != null) ApplyMyEntry(me);
    }

    private void OnInitFailed(PlayFabError err)
    {
        // 오류 시에도 UI가 비어 보이지 않도록 최소한의 방어
        if (lobbyUIManager != null && string.IsNullOrEmpty(lobbyUIManager.rankText.text))
        {
            lobbyUIManager.rankText.text = "-";
            lobbyUIManager.scoreText.text = "0";
        }
    }


    // 계정 정보 가져오기 성공 시 호출될 콜백 함수
    public void OnGetAccountInfoSuccess(GetAccountInfoResult result)
    {
        playFabId = result.AccountInfo.PlayFabId;
        userName = result.AccountInfo.Username;
        GameManager.Instance.UserId = userName;
        lobbyUIManager.nameText.text = userName;
        lobbyUIManager.tagText.text = playFabId;
        playerName.text = userName;
    }

    // 계정 정보 가져오기 실패 시 호출될 콜백 함수
    public void OnGetAccountInfoFailure(PlayFabError error)
    {
        Debug.LogError("GetAccountInfo request failed: " + error.GenerateErrorReport());
    }
}
