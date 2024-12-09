using UnityEngine;
using UnityEngine.UI; // UI ???? ??????????
using UnityEngine.SceneManagement; // ?? ????? ???? ??????????
using TMPro; // TextMeshPro ???? ?????????? ???
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using System.IO; // ???? ??? ???? ??? ???
using UnityEngine.Networking; // UnityWebRequest?? ???????? ???


public class GameManager : MonoBehaviour
{
    public Button completeGameButton; // ???? ??? ???
    public List<Image> scoreImages; // �ǽð� �򰡸� ǥ���� �̹��� ����Ʈ
    private List<PlayerData> playerDataList = new List<PlayerData>(); // �� ������� �� �����͸� ����

    private List<float> scores = new List<float>(); // 3?? ???????? ?????? ???? ?????

    public AudioSource audioSource;

    private float lastScoreUpdateTime = 0f; // ������ ���� ���� �ð�
    private float scoreUpdateInterval = 1.5f; // ���� ���� ����

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        string basePath = Path.Combine(Application.dataPath, "../../../../../Fiction-Royals-Merge/Fiction-Royals/db");
        LoadSelectedMusic();

        PlayerPrefs.Save();
        PrintAllPlayerPrefs();

        InitializePlayerData(3);

        HideScoreImage();

        completeGameButton.onClick.AddListener(CompleteGame);
    }

    void Update()
    {
        float currentTime = Time.time;
        if (currentTime - lastScoreUpdateTime >= scoreUpdateInterval)
        {
            if (UDPReceiver.Instance != null && UDPReceiver.Instance.LatestScores.Count > 0)
            {
                for (int i = 0; i < playerDataList.Count; i++)
                {
                    if (i < UDPReceiver.Instance.LatestScores.Count)
                    {
                        float latestScore = UDPReceiver.Instance.LatestScores[i + 1];
                        PlayerData playerData = playerDataList[i];

                        string scoreGrade = GetScoreGrade(latestScore, playerData);
                        UpdateScoreImage(i, scoreGrade);
                    }
                }
            }
            // ������ ���� ���� �ð��� ���� �ð����� ������Ʈ
            lastScoreUpdateTime = currentTime;
        }
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
            Debug.LogWarning("????? ???? ????? ??? ?? ???????.");
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
                Debug.Log("????? ???? ??? ??: " + filePath);
            }
            else
            {
                Debug.LogError("????? ???? ???? ????: " + filePath);
            }
        }
    }

    // �÷��̾� �����͸� �ʱ�ȭ
    void InitializePlayerData(int playerCount)
    {
        for (int i = 0; i < playerCount; i++)
        {
            playerDataList.Add(new PlayerData());
        }
    }

    void ShowScoreImage()
    {
        foreach (var scoreImage in scoreImages)
        {
            if (scoreImage != null)
            {
                Color color = scoreImage.color;
                color.a = 1;
                scoreImage.color = color;
            }
        }
    }

    void HideScoreImage()
    {
        foreach (var scoreImage in scoreImages)
        {
            if (scoreImage != null)
            {
                Color color = scoreImage.color;
                color.a = 0; // ���İ��� 0���� �����Ͽ� ����
                scoreImage.color = color;
            }
        }
    }

    void UpdateScoreImage(int playerIndex, string scoreGrade)
    {
        if (playerIndex < scoreImages.Count)
        {
            string imagePath = $"Images/{scoreGrade.ToLower()}"; // �̹��� ���
            Sprite newSprite = Resources.Load<Sprite>(imagePath);

            if (newSprite != null)
            {
                scoreImages[playerIndex].sprite = newSprite;
                ShowScoreImage();
            }
            else
            {
                Debug.LogWarning($"�̹����� ã�� �� �����ϴ�: {imagePath}");
                HideScoreImage();
            }
        }
    }

    string GetScoreGrade(float score, PlayerData playerData)
    {
        if (score >= 91)
        {
            playerData.excellentCount++;
            return "Excellent";
        }
        else if (score >= 76)
        {
            playerData.greatCount++;
            return "Great";
        }
        else if (score >= 61)
        {
            playerData.goodCount++;
            return "Good";
        }
        else if (score >= 51)
        {
            playerData.badCount++;
            return "Bad";
        }
        else
        {
            playerData.missCount++;
            return "Miss";
        }
    }

    string GetStarGrade(int excellentCount, int greatCount, int goodCount, int badCount, int missCount)
    {
        // SS ���: great, good, bad, miss�� ��� 0�̰�, excellent�� �־�� �Ѵ�.
        if (goodCount == 0 && badCount == 0 && missCount == 0 && excellentCount > 0 && greatCount == 0)
        {
            return "SS";
        }

        // S ���: good, bad, miss�� ��� 0�̰�, excellent�� great�� �־�� �Ѵ�.
        if (goodCount == 0 && badCount == 0 && missCount == 0 && excellentCount > 0 && greatCount > 0)
        {
            return "S";
        }

        // A ���: good, bad, miss�� ��� 0�̰�, excellent, great, good�� �־�� �Ѵ�.
        if (goodCount > 0 && badCount == 0 && missCount == 0 && excellentCount > 0)
        {
            return "A";
        }

        // B ���: miss�� 0�̰�, excellent, great, good, bad�� �־�� �Ѵ�.
        if (missCount == 0 && excellentCount > 0 && greatCount > 0 && goodCount > 0 && badCount > 0)
        {
            return "B";
        }

        // C ���: miss�� 1~2���� ���.
        if (missCount >= 1 && missCount <= 2)
        {
            return "C";
        }

        // F ���: miss�� 3�� �̻��� ���.
        if (missCount >= 3)
        {
            return "F";
        }

        // �� �� ��� ��쿡�� �⺻������ F�� ó��
        return "F";
    }

    void CalculateStarScores()
    {
        foreach (var playerData in playerDataList)
        {
            playerData.starScore = GetStarGrade(
                playerData.excellentCount,
                playerData.greatCount,
                playerData.goodCount,
                playerData.badCount,
                playerData.missCount
            );
        }
    }

    // PlayerPrefs?? ????? ??? ?????? ?????? ??? (??? ????)
    void PrintAllPlayerPrefs()
    {
        // ????? GameMode ???????
        if (PlayerPrefs.HasKey("GameMode"))
        {
            int gameMode = PlayerPrefs.GetInt("GameMode");

            if (gameMode == 1)
            {
                Debug.Log("????��???? ???? ????");
            }
            else if (gameMode == 2)
            {
                Debug.Log("????��???? ???? ????");
            }
            else
            {
                Debug.Log("????? gameMode????.");
            }
        }
        else
        {
            Debug.Log("GameMode?? ??? ?? ???????.");
        }


        // ????? ???? ???? ???
        if (PlayerPrefs.HasKey("SelectedCameraIndex"))
        {
            int selectedCameraIndex = PlayerPrefs.GetInt("SelectedCameraIndex");
            Debug.Log("SelectedCameraIndex: " + selectedCameraIndex);
        }
        else
        {
            Debug.Log("???? ?????? ??? ?? ???????.");
        }

        // ????? ????? ???? ???
        if (PlayerPrefs.HasKey("SelectedMusicIndex"))
        {
            int selectedThumbnailIndex = PlayerPrefs.GetInt("SelectedMusicIndex");
            Debug.Log("SelectedMusicIndex: " + selectedThumbnailIndex);
        }
        else
        {
            Debug.Log("???? ?????? ??? ?? ???????.");
        }

        Debug.Log("PlayerPrefs ????? ??? ?????? ?????????.");
    }
    // ???? ??? ?? ResultScene???? ?????? ???
    public void CompleteGame()
    {
        // PlayerCount ����
        PlayerPrefs.SetInt("PlayerCount", playerDataList.Count);

        CalculateStarScores();

        for (int i = 0; i < playerDataList.Count; i++)
        {
            PlayerData playerData = playerDataList[i];
            PlayerPrefs.SetInt($"Player{i + 1}_ExcellentCount", playerData.excellentCount);
            PlayerPrefs.SetInt($"Player{i + 1}_GreatCount", playerData.greatCount);
            PlayerPrefs.SetInt($"Player{i + 1}_GoodCount", playerData.goodCount);
            PlayerPrefs.SetInt($"Player{i + 1}_BadCount", playerData.badCount);
            PlayerPrefs.SetInt($"Player{i + 1}_MissCount", playerData.missCount);
            PlayerPrefs.SetString($"Player{i + 1}_StarScore", playerData.starScore);

            Debug.Log($"Player: {playerDataList.Count}, Loaded Scores - StarScore: {playerData.starScore}, Excellent: {playerData.excellentCount}, Great: {playerData.greatCount}, Good: {playerData.goodCount}, Bad: {playerData.badCount}, Miss: {playerData.missCount}");
        }

        SceneManager.LoadScene("ResultScene");
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