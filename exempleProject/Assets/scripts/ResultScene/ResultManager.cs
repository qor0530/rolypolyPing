using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class ResultManager : MonoBehaviour
{
    public Button RetryBtn; // Retry 버튼: ChoiceGame 씬으로 이동
    public Button LobbyBtn; // Lobby 버튼: Main 씬으로 이동
    public Button NextBtn; // Next 버튼: 결과를 넘기는 버튼


    public TextMeshProUGUI StarScore; // ExcellentCount 텍스트 UI
    public TextMeshProUGUI ExcellentCount; // ExcellentCount 텍스트 UI
    public TextMeshProUGUI GreatCount; // GreatCount 텍스트 UI
    public TextMeshProUGUI GoodCount;      // GoodCount 텍스트 UI
    public TextMeshProUGUI BadCount; // BadCount 텍스트 UI
    public TextMeshProUGUI MissCount;       // MissCount 텍스트 UI

    public Image RankImage; // 랭크에 따른 이미지 표시

    public Image PlayImage; // 플레이어 수에 따른 이미지 표시

    private int currentPlayerIndex = 0; // 현재 보여줄 플레이어의 인덱스
    private List<PlayerData> playerDataList = new List<PlayerData>(); // 플레이어 데이터 리스트


    void Start()
    {
        // Retry 버튼 이벤트 연결
        if (RetryBtn == null)
        {
            Debug.LogError("Retry Button is not assigned in the Inspector!");
        }
        else
        {
            RetryBtn.onClick.AddListener(() => ChangeScene("ChoiceGameTypeScene")); // ChoiceGame 씬으로 이동
        }

        // Lobby 버튼 이벤트 연결
        if (LobbyBtn == null)
        {
            Debug.LogError("Lobby Button is not assigned in the Inspector!");
        }
        else
        {
            LobbyBtn.onClick.AddListener(() => ChangeScene("MainScene")); // Main 씬으로 이동
        }

        // Next 버튼 이벤트 연결
        NextBtn.onClick.AddListener(ShowNextPlayerResult);

        // PlayerPrefs에서 데이터 로드
        LoadPlayerData();

        // 첫 번째 플레이어의 결과를 표시
        ShowPlayerResult(currentPlayerIndex);
    }

    void LoadPlayerData()
    {
        int playerCount = PlayerPrefs.GetInt("PlayerCount", 1); // 저장된 플레이어 수 가져오기
        Debug.Log($"PlayerCount: {playerCount}");

        for (int i = 0; i < playerCount; i++)
        {
            int excellentCount = PlayerPrefs.GetInt($"Player{i + 1}_ExcellentCount");
            int greatCount = PlayerPrefs.GetInt($"Player{i + 1}_GreatCount");
            int goodCount = PlayerPrefs.GetInt($"Player{i + 1}_GoodCount");
            int badCount = PlayerPrefs.GetInt($"Player{i + 1}_BadCount");
            int missCount = PlayerPrefs.GetInt($"Player{i + 1}_MissCount");
            string starScore = PlayerPrefs.GetString($"Player{i + 1}_StarScore");

            Debug.Log($"Loaded Data for Player {i + 1}: Excellent {excellentCount}, Great {greatCount}, Good {goodCount}, Bad {badCount}, Miss {missCount}, StarScore: {starScore}");

            PlayerData playerData = new PlayerData
            {
                excellentCount = excellentCount,
                greatCount = greatCount,
                goodCount = goodCount,
                badCount = badCount,
                missCount = missCount,
                starScore = starScore
            };
            playerDataList.Add(playerData);
        }
    }

    void ShowPlayerResult(int playerIndex)
    {
        // playerDataList가 null인지 확인
        if (playerDataList == null)
        {
            Debug.LogError("playerDataList is not initialized!");
            return;
        }

        // playerIndex가 유효한지 확인
        if (playerIndex < 0 || playerIndex >= playerDataList.Count)
        {
            Debug.LogError($"Invalid playerIndex: {playerIndex}. It must be between 0 and {playerDataList.Count - 1}.");
            return;
        }

        PlayerData playerData = playerDataList[playerIndex];

        // UI 업데이트
        Debug.Log($"Updating UI for Player {playerIndex + 1}: StarScore: {playerData.starScore}, Excellent: {playerData.excellentCount}, Great: {playerData.greatCount}, Good: {playerData.goodCount}, Bad: {playerData.badCount}, Miss: {playerData.missCount}");

        ExcellentCount.text = playerData.excellentCount.ToString();
        GreatCount.text = playerData.greatCount.ToString();
        GoodCount.text = playerData.goodCount.ToString();
        BadCount.text = playerData.badCount.ToString();
        MissCount.text = playerData.missCount.ToString();

        UpdateRankImage(playerData.starScore); // 랭크 이미지 업데이트
        UpdatePlayerImage(playerIndex+1); // 플레이어 이미지 업데이트
    }

    void ShowNextPlayerResult()
    {
        currentPlayerIndex++;
        if (currentPlayerIndex >= playerDataList.Count)
        {
            currentPlayerIndex = 0; // 마지막 플레이어 후 첫 번째 플레이어로 돌아가기
        }
        ShowPlayerResult(currentPlayerIndex);
    }
    
    void UpdateRankImage(string starScore)
    {
        string imagePath = $"Images/{starScore}"; // 이미지 경로 설정
        Sprite newSprite = Resources.Load<Sprite>(imagePath);

        if (newSprite != null)
        {
            RankImage.sprite = newSprite;
            ShowScoreImage();
        }
        else
        {
            Debug.LogWarning($"이미지를 로드할 수 없습니다: {imagePath}");
            HideScoreImage();
        }
    }

    void UpdatePlayerImage(int player)
    {
        string currentPlayer = player.ToString();
        string imagePath = $"Images/{currentPlayer}p";
        Sprite newSprite = Resources.Load<Sprite>(imagePath);
        if (newSprite != null)
        {
            PlayImage.sprite = newSprite;
            ShowPlayerImage();
        }
        else
        {
            Debug.LogWarning($"이미지를 로드할 수 없습니다: {imagePath}");
            HidePlayerImage();
        }
    }

    void ShowScoreImage()
    {
        Color color = RankImage.color;
        color.a = 1; // 알파값을 1로 설정 (완전 불투명)
        RankImage.color = color;
    }

    void HideScoreImage()
    {
        Color color = RankImage.color;
        color.a = 0; // 알파값을 0으로 설정 (완전 투명)
        RankImage.color = color;
    }

    void ShowPlayerImage()
    {
        Color color = PlayImage.color;
        color.a = 1; // 알파값을 1로 설정 (완전 불투명)
        RankImage.color = color;
    }

    void HidePlayerImage()
    {
        Color color = PlayImage.color;
        color.a = 0; // 알파값을 0으로 설정 (완전 투명)
        RankImage.color = color;
    }

    // 씬 변경을 처리하는 함수
    void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    private class PlayerData
    {
        public int excellentCount = 0;
        public int greatCount = 0;
        public int goodCount = 0;
        public int badCount = 0;
        public int missCount = 0;
        public string starScore = ""; 
    }
}
