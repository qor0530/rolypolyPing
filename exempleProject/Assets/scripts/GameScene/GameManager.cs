using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI 관련 네임스페이스
using UnityEngine.SceneManagement; // 씬 전환을 위한 네임스페이스
using TMPro; // TextMeshPro 관련 네임스페이스 추가
using UnityEditor;


public class GameManager : MonoBehaviour
{

    public Button completeGameButton; // 게임 완료 버튼
    public Image scoreImage; // 점수에 따른 이미지 표시
    private List<float> scores = new List<float>(); // 3초 간격으로 수집된 점수 리스트

    // 각 점수 구간의 개수를 카운트하기 위한 변수
    private int excellentCount = 0;
    private int greatCount = 0;
    private int goodCount = 0;
    private int badCount = 0;
    private int missCount = 0;
    
    private float totalScore = 0f; // 전체 점수의 합
    private int totalScoreCount = 0; // 전체 점수 개수
    private float averageScore = 0f; // 1.5초 간격의 평균 점수

    private string starScore = ""; //Result에 나타낼 등급

    void Start()
    {
        // PlayerPrefs에 저장된 카메라 및 썸네일 정보를 로그로 출력
        PrintAllPlayerPrefs();
        // 게임 완료 버튼 클릭 이벤트 연결
        completeGameButton.onClick.AddListener(CompleteGame);

        // 점수를 실시간 업데이트
        HideScoreImage();
        StartCoroutine(UpdateScore());
        starScore = GetStarGrade(excellentCount, greatCount, goodCount, badCount, missCount);
    }

    void ShowScoreImage()
    {
        Color color = scoreImage.color;
        color.a = 1; // 알파값을 1로 설정 (완전 불투명)
        scoreImage.color = color;
    }

    void HideScoreImage()
    {
        Color color = scoreImage.color;
        color.a = 0; // 알파값을 0으로 설정 (완전 투명)
        scoreImage.color = color;
    }

    IEnumerator UpdateScore()
    {
        while (true)
        {
            // scores.Clear(); // 새로운 3초 간격을 위해 리스트 초기화

            // UDPReceiver에서 최신 점수 가져오기
            if (UDPReceiver.Instance != null && UDPReceiver.Instance.LatestCoord3D != null)
            {
                float latestScore = UDPReceiver.Instance.LatestScore;
                // 점수를 리스트에 추가하지 않고 바로 누적
                totalScore += latestScore;
                totalScoreCount++;

                // scores.Add(latestScore);
                // totalScore += latestScore;

                // 점수에 따라 구분을 나누고 출력
                string scoreGrade = GetScoreGrade(latestScore);
                UpdateScoreImage(scoreGrade);

                Debug.Log($"Latest Score: {latestScore:F2}, Grade: {scoreGrade}");
            }
            
            // 3초 동안의 평균 점수 계산
            if (totalScoreCount > 0)
            {
                averageScore = totalScore / totalScoreCount;
            }

            // 점수 구간별 개수 카운트
            CountScoreGrades();

            // 3초 간격으로 평균 점수와 각 구간별 개수를 기록
            Debug.Log($"Average Score (3s): {averageScore:F2}, Excellent: {excellentCount}, Great: {greatCount}, Good: {goodCount}, Bad: {badCount}, Miss: {missCount}");

            yield return new WaitForSeconds(1.5f); // 1.5초 간격으로 업데이트
        }
    }

// 점수에 따라 이미지를 변경하는 함수
    void UpdateScoreImage(string scoreGrade)
    {
        
        string imagePath = $"Images/{scoreGrade.ToLower()}"; // 이미지 경로 설정 (Resources 폴더 내 경로)
        Sprite newSprite = Resources.Load<Sprite>(imagePath);

        if (newSprite != null)
        {
            scoreImage.sprite = newSprite; // 이미지 변경
            ShowScoreImage(); // 이미지 보이기
        }
        else
        {
            Debug.LogWarning($"이미지를 로드할 수 없습니다: {imagePath}");
            HideScoreImage(); // 이미지 숨기기
        }
    }
    // 점수를 구간별로 구분하는 함수
    string GetScoreGrade(float score)
    {
        if (score >= 91)
        {
            excellentCount++;
            return "Excellent";
        }
        else if (score >= 76)
        {
            greatCount++;
            return "Great";
        }
        else if (score >= 61)
        {
            goodCount++;
            return "Good";
        }
        else if (score >= 51)
        {
            badCount++;
            return "Bad";
        }
        else
        {
            missCount++;
            return "Miss";
        }
    }

    string GetStarGrade(int excellentCount, int greatCount, int goodCount, int badCount, int missCount)
    {
        if (missCount >= 3)
            return "F";
        if (missCount > 0)
            return "C";
        if (badCount > 0)
            return "B";
        if (goodCount > 0)
            return "A";
        if (greatCount > 0)
            return "S";
        return "SS";
    }

    void CountScoreGrades()
    {
        excellentCount = 0;
        greatCount = 0;
        goodCount = 0;
        badCount = 0;
        missCount = 0;

        foreach (float score in scores)
        {
            GetScoreGrade(score);
        }
    }

    // PlayerPrefs에 저장된 모든 내용을 출력하는 함수 (중복 제거)
    void PrintAllPlayerPrefs()
    {
        // 저장된 GameMode 불러오기
        if (PlayerPrefs.HasKey("GameMode"))
        {
            int gameMode = PlayerPrefs.GetInt("GameMode");

            if (gameMode == 1)
            {
                Debug.Log("싱글플레이 모드로 시작");
            }
            else if (gameMode == 2)
            {
                Debug.Log("멀티플레이 모드로 시작");
            }
            else
            {
                Debug.Log("잘못된 gameMode입니다.");
            }
        }
        else
        {
            Debug.Log("GameMode를 찾을 수 없습니다.");
        }


        // 선택된 카메라 정보 출력
        if (PlayerPrefs.HasKey("SelectedCameraIndex"))
        {
                int selectedCameraIndex = PlayerPrefs.GetInt("SelectedCameraIndex");
                Debug.Log("SelectedCameraIndex: " + selectedCameraIndex);
        }
        else
        {
            Debug.Log("카메라 정보를 찾을 수 없습니다.");
        }

        // 선택된 썸네일 정보 출력
        if (PlayerPrefs.HasKey("SelectedMusicIndex"))
        {
            int selectedThumbnailIndex = PlayerPrefs.GetInt("SelectedMusicIndex");
            Debug.Log("SelectedMusicIndex: " + selectedThumbnailIndex);
        }
        else
        {
            Debug.Log("음악 정보를 찾을 수 없습니다.");
        }

        Debug.Log("PlayerPrefs 저장된 모든 정보를 출력했습니다.");
    }  
    // 게임 완료 시 ResultScene으로 이동하는 함수
    public void CompleteGame()
    {
        // 점수와 개수 저장
        PlayerPrefs.SetString("StarScore", starScore);
        PlayerPrefs.SetInt("ExcellentCount", excellentCount);
        PlayerPrefs.SetInt("GreatCount", greatCount);
        PlayerPrefs.SetInt("GoodCount", goodCount);
        PlayerPrefs.SetInt("BadCount", badCount);
        PlayerPrefs.SetInt("MissCount", missCount);

        // ResultScene으로 전환
        SceneManager.LoadScene("ResultScene");
    }
}