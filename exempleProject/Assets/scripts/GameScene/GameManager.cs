using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI ���� ���ӽ����̽�
using UnityEngine.SceneManagement; // �� ��ȯ�� ���� ���ӽ����̽�
using TMPro; // TextMeshPro ���� ���ӽ����̽� �߰�


public class GameManager : MonoBehaviour
{

    public Button completeGameButton; // ���� �Ϸ� ��ư
    public TextMeshProUGUI scoreText; //�ǽð� ���� text
    private List<float> scores = new List<float>(); // 3�� �������� ������ ���� ����Ʈ

    // �� ���� ������ ������ ī��Ʈ�ϱ� ���� ����
    private int excellentCount = 0;
    private int greatCount = 0;
    private int goodCount = 0;
    private int badCount = 0;
    private int missCount = 0;
    
    private float averageScore = 0f; // 3�� ������ ��� ����

    privatae float starScore = 0; //Result�� ��Ÿ�� �� ����

    void Start()
    {
        // PlayerPrefs�� ����� ī�޶� �� ����� ������ �α׷� ���
        PrintAllPlayerPrefs();
        // ���� �Ϸ� ��ư Ŭ�� �̺�Ʈ ����
        completeGameButton.onClick.AddListener(CompleteGame);

        // ������ �ǽð� ������Ʈ
        StartCoroutine(UpdateScore());
    }

    IEnumerator UpdateScore()
    {
        while (true)
        {
            scores.Clear(); // ���ο� 3�� ������ ���� ����Ʈ �ʱ�ȭ
            float totalScore = 0f;

            // UDPReceiver���� �ֽ� ���� ��������
            if (UDPReceiver.Instance != null && UDPReceiver.Instance.LatestCoord3D != null)
            {
                float latestScore = UDPReceiver.Instance.LatestScore;
                scores.Add(latestScore);
                totalScore += latestScore;

                // ������ ���� ������ ������ ���
                string scoreGrade = GetScoreGrade(latestScore);
                scoreText.text = $"{scoreGrade}";
            }
            
            // 3�� ������ ��� ���� ���
            if (scores.Count > 0)
            {
                averageScore = totalScore / scores.Count;
            }

            // ���� ������ ���� ī��Ʈ
            CountScoreGrades();

            // 3�� �������� ��� ������ �� ������ ������ ���
            Debug.Log($"Average Score (3s): {averageScore:F2}, Excellent: {excellentCount}, Great: {greatCount}, Good: {goodCount}, Bad: {badCount}, Miss: {missCount}");

            yield return new WaitForSeconds(3f); // 3�� �������� ������Ʈ
        }
    }

    // ������ �������� �����ϴ� �Լ�
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

    // PlayerPrefs�� ����� ��� ������ ����ϴ� �Լ� (�ߺ� ����)
    void PrintAllPlayerPrefs()
    {
        // ����� GameMode �ҷ�����
        if (PlayerPrefs.HasKey("GameMode"))
        {
            int gameMode = PlayerPrefs.GetInt("GameMode");

            if (gameMode == 1)
            {
                Debug.Log("�̱��÷��� ���� ����");
            }
            else if (gameMode == 2)
            {
                Debug.Log("��Ƽ�÷��� ���� ����");
            }
            else
            {
                Debug.Log("�߸��� gameMode�Դϴ�.");
            }
        }
        else
        {
            Debug.Log("GameMode�� ã�� �� �����ϴ�.");
        }


        // ���õ� ī�޶� ���� ���
        if (PlayerPrefs.HasKey("SelectedCameraIndex"))
        {
                int selectedCameraIndex = PlayerPrefs.GetInt("SelectedCameraIndex");
                Debug.Log("SelectedCameraIndex: " + selectedCameraIndex);
        }
        else
        {
            Debug.Log("ī�޶� ������ ã�� �� �����ϴ�.");
        }

        // ���õ� ����� ���� ���
        if (PlayerPrefs.HasKey("SelectedMusicIndex"))
        {
            int selectedThumbnailIndex = PlayerPrefs.GetInt("SelectedMusicIndex");
            Debug.Log("SelectedMusicIndex: " + selectedThumbnailIndex);
        }
        else
        {
            Debug.Log("���� ������ ã�� �� �����ϴ�.");
        }

        Debug.Log("PlayerPrefs ����� ��� ������ ����߽��ϴ�.");
    }  
    // ���� �Ϸ� �� ResultScene���� �̵��ϴ� �Լ�
    public void CompleteGame()
    {
        // ������ ���� ����
        PlayerPrefs.SetFloat("AverageScore", averageScore);
        PlayerPrefs.SetInt("ExcellentCount", excellentCount);
        PlayerPrefs.SetInt("GreatCount", greatCount);
        PlayerPrefs.SetInt("GoodCount", goodCount);
        PlayerPrefs.SetInt("BadCount", badCount);
        PlayerPrefs.SetInt("MissCount", missCount);

        // ResultScene���� ��ȯ
        SceneManager.LoadScene("ResultScene");
    }
}