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


    public TextMeshProUGUI StarScore; // ExcellentCount 텍스트 UI
    public TextMeshProUGUI ExcellentCount; // ExcellentCount 텍스트 UI
    public TextMeshProUGUI GreatCount; // GreatCount 텍스트 UI
    public TextMeshProUGUI GoodCount;      // GoodCount 텍스트 UI
    public TextMeshProUGUI BadCount; // BadCount 텍스트 UI
    public TextMeshProUGUI MissCount;       // MissCount 텍스트 UI

    public Image RankImage; // 랭크에 따른 이미지 표시


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

        // PlayerPrefs로부터 데이터 로드
        float starScore = PlayerPrefs.GetString("StarScore", "");
        int excellentCount = PlayerPrefs.GetInt("ExcellentCount", 0);
        int greatCount = PlayerPrefs.GetInt("GreatCount", 0);
        int goodCount = PlayerPrefs.GetInt("GoodCount", 0);
        int badCount = PlayerPrefs.GetInt("BadCount", 0);
        int missCount = PlayerPrefs.GetInt("MissCount", 0);

        // UI 업데이트
        UpdateUI(starScore, excellentCount, greatCount, goodCount, badCount, missCount);

    }

    void UpdateUI(string starScore, int excellentCount, int greatCount, int goodCount, int badCount, int missCount)
    {
        // TextMeshPro를 사용한 UI 텍스트 갱신
        if (StarScore != null) StarScore.text = $"Score: {starScore}";
        if (ExcellentCount != null) ExcellentCount.text = $"Excellent: {excellentCount}";
        if (GreatCount != null) GreatCount.text = $"Great: {greatCount}";
        if (GoodCount != null) GoodCount.text = $"Good: {goodCount}";
        if (BadCount != null) BadCount.text = $"Bad: {badCount}";
        if (MissCount != null) MissCount.text = $"Miss: {missCount}";
        
        UpdateRankImage(starScore); // 랭크 이미지 업데이트

    }

    void UpdateRankImage(string starScore)
    {
        
        string imagePath = $"Images/{starScore.ToLower()}"; // 이미지 경로 설정 (Resources 폴더 내 경로)
        Sprite newSprite = Resources.Load<Sprite>(imagePath);

        if (newSprite != null)
        {
            RankImage.sprite = newSprite; // 이미지 변경
            ShowScoreImage(); // 이미지 보이기
        }
        else
        {
            Debug.LogWarning($"이미지를 로드할 수 없습니다: {imagePath}");
            HideScoreImage(); // 이미지 숨기기
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

    // 씬 변경을 처리하는 함수
    void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
