using PlayFab.ClientModels;
using PlayFab;
using UnityEngine;
using TMPro;
using System.Linq;

// 로비 화면에서 PlayFab 계정 정보, 국가 정보, 리더보드 정보를 가져와
// UI에 표시하는 클래스
public class LobbyPagePlayfab : MonoBehaviour
{
    private string playFabId;       // 내 PlayFab 고유 ID
    private string userName;        // 내 유저 이름(Username)
    private string playerLocation;  // 내 국가 정보
    private string playerRank;      // 내 랭킹
    private string playerScore;     // 내 점수

    // 로비 상단에 표시할 플레이어 이름 텍스트
    [SerializeField] private TextMeshProUGUI playerName;

    // 리더보드 한 줄 UI 프리팹
    [SerializeField] private GameObject rowPrefab;

    // 리더보드 rowPrefab들이 생성될 부모 오브젝트
    [SerializeField] private Transform rowsParent;

    [SerializeField] private TextMeshProUGUI nameText;     // 닉네임 표시용 텍스트
    [SerializeField] private TextMeshProUGUI tagText;      // PlayFab ID 표시용 텍스트
    [SerializeField] private TextMeshProUGUI locationText; // 국가 표시용 텍스트
    [SerializeField] private TextMeshProUGUI rankText;     // 내 랭킹 표시용 텍스트
    [SerializeField] private TextMeshProUGUI scoreText;    // 내 점수 표시용 텍스트

    void Awake()
    {
        // 현재 클라이언트가 PlayFab에 로그인되어 있는지 확인
        if (PlayFabClientAPI.IsClientLoggedIn())
        {
            // 로그인되어 있으면 계정 정보를 가져오기 위한 요청 객체 생성
            var request = new GetAccountInfoRequest();

            // PlayFab에 계정 정보 요청
            PlayFabClientAPI.GetAccountInfo(request, OnGetAccountInfoSuccess, OnGetAccountInfoFailure);
        }
        else
        {
            // 로그인되지 않은 상태라면 에러 로그 출력
            Debug.LogError("User is not logged in.");
        }

        // 리더보드 정보 요청
        GetLeaderboard();

        // 국가 정보 요청
        GetPlayerCountry();
    }


    // 플레이어의 국가 정보를 가져오는 함수
    public void GetPlayerCountry()
    {
        var request = new GetPlayerProfileRequest
        {
            ProfileConstraints = new PlayerProfileViewConstraints
            {
                // 위치 정보를 포함해서 프로필을 가져오도록 설정
                ShowLocations = true
            }
        };

        // PlayFab에 프로필 정보 요청
        PlayFabClientAPI.GetPlayerProfile(request, OnPlayerCountryGet, (error) => { });
    }

    // 국가 정보 요청 성공 시 호출되는 콜백
    public void OnPlayerCountryGet(GetPlayerProfileResult result)
    {
        // 프로필의 첫 번째 Location 정보에서 국가 코드 추출 ex) KR
        playerLocation = result.PlayerProfile.Locations[0].CountryCode.Value.ToString();

        // 가져온 국가 코드를 UI에 표시
        locationText.text = playerLocation;
    }

    // Score 통계를 기준으로 리더보드를 가져오는 함수
    public void GetLeaderboard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "Score", // Score 통계 기준
            StartPosition = 0,       // 0등부터
            MaxResultsCount = 100    // 최대 100명까지 가져오기
        };

        // PlayFab에 리더보드 요청
        PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardGet, (error) => { });
    }

    // 리더보드 요청 성공 시 호출되는 콜백
    public void OnLeaderboardGet(GetLeaderboardResult result)
    {
        // 기존에 rowsParent 아래 생성되어 있던 리더보드 UI를 모두 삭제
        foreach (Transform item in rowsParent)
            Destroy(item.gameObject);

        // 현재 받아온 리더보드 목록 안에 내 정보가 있는지 체크하는 변수
        bool foundMe = false;

        // 받아온 리더보드 전체를 순회
        for (int i = 0; i < result.Leaderboard.Count; i++)
        {
            // 상위 10명까지만 화면에 표시
            if (i < 10)
            {
                // rowPrefab을 rowsParent 아래에 생성
                GameObject row = Instantiate(rowPrefab, rowsParent);

                // row 안에 있는 텍스트들 가져오기
                TextMeshProUGUI[] texts = row.GetComponentsInChildren<TextMeshProUGUI>();

                // PlayFab Position은 0부터 시작하므로 +1 해서 보여줌
                texts[0].SetText($"{result.Leaderboard[i].Position + 1}");

                // 유저 DisplayName 표시
                texts[1].SetText(result.Leaderboard[i].DisplayName);

                // 점수 표시
                texts[2].SetText($"{result.Leaderboard[i].StatValue}");
            }

            // 현재 순회 중인 엔트리의 PlayFabId가 내 PlayFabId와 같으면
            if (result.Leaderboard[i].PlayFabId == playFabId)
            {
                foundMe = true;

                // 내 랭킹/점수 UI 갱신
                ApplyMyEntry(result.Leaderboard[i]);
            }
        }

        // Top100 안에서 내 정보를 못 찾았다면 따로 내 주변 리더보드를 요청해서 내 랭킹/점수를 가져옴
        if (!foundMe)
        {
            InitMyRankAndScore();
        }
    }

    // 내 리더보드 엔트리를 받아서 랭킹/점수 필드와 UI에 반영하는 함수
    private void ApplyMyEntry(PlayerLeaderboardEntry me)
    {
        // 랭킹 저장 (0부터 시작하므로 +1)
        playerRank = (me.Position + 1).ToString();

        // 점수 저장
        playerScore = me.StatValue.ToString();

        // UI에 반영
        rankText.SetText(playerRank);
        scoreText.SetText(playerScore);
    }

    // 내 정보가 기존 Top100 목록에 없을 때 내 주변 리더보드를 다시 요청해서 내 랭킹/점수를 가져오는 함수
    private void InitMyRankAndScore()
    {
        var req = new GetLeaderboardAroundPlayerRequest
        {
            PlayFabId = playFabId, // 내 PlayFabId 기준
            MaxResultsCount = 1    // 나 자신만 가져오기
        };

        // 내 주변 리더보드 요청
        PlayFabClientAPI.GetLeaderboardAroundPlayer(req, OnInitSuccess, OnInitFailed);
    }

    // 내 주변 리더보드 요청 성공 시 호출
    private void OnInitSuccess(GetLeaderboardAroundPlayerResult res)
    {
        // 받아온 결과 중에서 내 PlayFabId와 일치하는 엔트리 찾기
        var me = res.Leaderboard.FirstOrDefault(e => e.PlayFabId == playFabId);

        // 내 엔트리가 존재하면 UI 갱신
        if (me != null)
            ApplyMyEntry(me);
    }

    // 내 주변 리더보드 요청 실패 시 호출
    private void OnInitFailed(PlayFabError err)
    {
        // 실패한 경우 기본값 표시
        rankText.text = "-";
        scoreText.text = "0";
    }


    // 계정 정보 요청 성공 시 호출되는 콜백
    public void OnGetAccountInfoSuccess(GetAccountInfoResult result)
    {
        // PlayFab ID 저장
        playFabId = result.AccountInfo.PlayFabId;

        // Username 저장
        userName = result.AccountInfo.Username;

        // 다른 곳에서도 사용할 수 있도록 GameManager에 유저 이름 저장
        GameManager.Instance.UserId = userName;

        // 계정 정보 UI 갱신
        nameText.text = userName;   // 닉네임 표시
        tagText.text = playFabId;   // PlayFab ID 표시
        playerName.text = userName; // 상단 플레이어 이름 표시
    }

    // 계정 정보 요청 실패 시 호출되는 콜백
    public void OnGetAccountInfoFailure(PlayFabError error)
    {
        // 실패 원인을 로그에 출력
        Debug.LogError("GetAccountInfo request failed: " + error.GenerateErrorReport());
    }
}