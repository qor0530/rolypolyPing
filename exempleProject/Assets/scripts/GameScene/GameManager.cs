using UnityEngine;
using UnityEngine.UI; // UI ���� ���ӽ����̽�
using UnityEngine.SceneManagement; // �� ��ȯ�� ���� ���ӽ����̽�
using TMPro; // TextMeshPro ���� ���ӽ����̽� �߰�
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using System.IO; // ���� ��� ���� ��� �߰�
using UnityEngine.Networking; // UnityWebRequest�� ����Ϸ��� �ʿ�


public class GameManager : MonoBehaviour
{
    public Button completeGameButton; // ���� �Ϸ� ��ư
    public Image scoreImage; // ������ ���� �̹��� ǥ��
    private List<float> scores = new List<float>(); // 3�� �������� ������ ���� ����Ʈ

    // �� ���� ������ ������ ī��Ʈ�ϱ� ���� ����
    private int excellentCount = 0;
    private int greatCount = 0;
    private int goodCount = 0;
    private int badCount = 0;
    private int missCount = 0;

    private float totalScore = 0f; // ��ü ������ ��
    private int totalScoreCount = 0; // ��ü ���� ����
    private float averageScore = 0f; // 1.5�� ������ ��� ����

    private string starScore = ""; //Result�� ��Ÿ�� ���
    public AudioSource audioSource;

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        string basePath = Path.Combine(Application.dataPath, "../../../../../Fiction-Royals-Merge/Fiction-Royals/db");
        LoadSelectedMusic();

        // PlayerPrefs�� ����� ī�޶� �� ����� ������ �α׷� ���
        PrintAllPlayerPrefs();
        // ���� �Ϸ� ��ư Ŭ�� �̺�Ʈ ����
        completeGameButton.onClick.AddListener(CompleteGame);

        // ������ �ǽð� ������Ʈ
        HideScoreImage();
        StartCoroutine(UpdateScore());
        starScore = GetStarGrade(excellentCount, greatCount, goodCount, badCount, missCount);
    }

    void LoadSelectedMusic()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (PlayerPrefs.HasKey("SelectedMusicPath"))
        {
            string selectedMusicPath = PlayerPrefs.GetString("SelectedMusicPath");
            StartCoroutine(LoadAudio(selectedMusicPath));
        }
        else
        {
            Debug.LogWarning("���õ� ���� ��θ� ã�� �� �����ϴ�.");
        }
    }

    private IEnumerator LoadAudio(string filePath)
    {
        using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip("file://" + filePath, AudioType.MPEG))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.Success)
            {
                audioSource.clip = DownloadHandlerAudioClip.GetContent(uwr);
                audioSource.Play();
                Debug.Log("���õ� ���� ��� ��: " + filePath);
            }
            else
            {
                Debug.LogError("����� ���� �ε� ����: " + filePath);
            }
        }
    }


    void ShowScoreImage()
    {
        Color color = scoreImage.color;
        color.a = 1; // ���İ��� 1�� ���� (���� ������)
        scoreImage.color = color;
    }

    void HideScoreImage()
    {
        Color color = scoreImage.color;
        color.a = 0; // ���İ��� 0���� ���� (���� ����)
        scoreImage.color = color;
    }

    IEnumerator UpdateScore()
    {
        while (true)
        {
            // scores.Clear(); // ���ο� 3�� ������ ���� ����Ʈ �ʱ�ȭ

            // UDPReceiver���� �ֽ� ���� ��������
            if (UDPReceiver.Instance != null && UDPReceiver.Instance.LatestCoord3Ds.Count != 0)
            {
                float latestScore = UDPReceiver.Instance.LatestScores[1];
                scores.Add(latestScore);
                totalScore += latestScore;
                totalScoreCount++;

                // ������ ���� ������ ������ ���
                string scoreGrade = GetScoreGrade(latestScore);
                UpdateScoreImage(scoreGrade);

                Debug.Log($"Latest Score: {latestScore:F2}, Grade: {scoreGrade}");
            }

            // 3�� ������ ��� ���� ���
            if (totalScoreCount > 0)
            {
                averageScore = totalScore / totalScoreCount;
            }

            // ���� ������ ���� ī��Ʈ
            CountScoreGrades();

            // 3�� �������� ��� ������ �� ������ ������ ���
            Debug.Log($"Average Score (3s): {averageScore:F2}, Excellent: {excellentCount}, Great: {greatCount}, Good: {goodCount}, Bad: {badCount}, Miss: {missCount}");

            yield return new WaitForSeconds(1.5f); // 1.5�� �������� ������Ʈ
        }
    }

    // ������ ���� �̹����� �����ϴ� �Լ�
    void UpdateScoreImage(string scoreGrade)
    {

        string imagePath = $"Images/{scoreGrade.ToLower()}"; // �̹��� ��� ���� (Resources ���� �� ���)
        Sprite newSprite = Resources.Load<Sprite>(imagePath);

        if (newSprite != null)
        {
            scoreImage.sprite = newSprite; // �̹��� ����
            ShowScoreImage(); // �̹��� ���̱�
        }
        else
        {
            Debug.LogWarning($"�̹����� �ε��� �� �����ϴ�: {imagePath}");
            HideScoreImage(); // �̹��� �����
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
        PlayerPrefs.SetString("StarScore", starScore);
        PlayerPrefs.SetInt("ExcellentCount", excellentCount);
        PlayerPrefs.SetInt("GreatCount", greatCount);
        PlayerPrefs.SetInt("GoodCount", goodCount);
        PlayerPrefs.SetInt("BadCount", badCount);
        PlayerPrefs.SetInt("MissCount", missCount);

        // ResultScene���� ��ȯ
        SceneManager.LoadScene("ResultScene");
    }
}