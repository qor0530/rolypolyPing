using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Networking;

public class MusicSelector : MonoBehaviour
{
    public List<Sprite> thumbnailList; // ����� �̹��� ����Ʈ
    public List<string> thumbnailTitles; // ����Ͽ� �´� �ؽ�Ʈ ����Ʈ
    public Image displayImage; // UI Image
    public TextMeshProUGUI displayTitle; // UI �ؽ�Ʈ
    public Button leftArrowButton; // �� ȭ��ǥ ��ư
    public Button rightArrowButton; // �� ȭ��ǥ ��ư
    public Button thumbnailButton; // ����� ��ư
    public Button longButton; // Long ��ư
    public Button shortButton; // Short ��ư
    public AudioSource audioSource; // AudioSource ������Ʈ

    private int currentIndex = 0; // ���� �ε���
    private List<string> musicFilePaths = new List<string>(); // ��� ���� ���� ��� ����Ʈ
    private List<Sprite> longThumbnails = new List<Sprite>(); // 1�� �̻� �����
    private List<string> longTitles = new List<string>(); // 1�� �̻� ����
    private List<string> longFilePaths = new List<string>(); // 1�� �̻� ���� ���
    private List<Sprite> shortThumbnails = new List<Sprite>(); // 1�� ���� �����
    private List<string> shortTitles = new List<string>(); // 1�� ���� ����
    private List<string> shortFilePaths = new List<string>(); // 1�� ���� ���� ���
    private bool isLong = true; // Long ��ư Ŭ�� ����

    private void Start()
    {
        string basePath = "/Users/jeongsieun/Fiction-Royals-Merge/Fiction-Royals/db";

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // ���� ���� ��� ��������
        if (Directory.Exists(basePath))
        {
            string[] mp3Files = Directory.GetFiles(basePath, "*.mp3", SearchOption.AllDirectories);
            foreach (string file in mp3Files)
            {
                musicFilePaths.Add(file);
            }
        }
        else
        {
            Debug.LogError("��θ� ã�� �� �����ϴ�: " + basePath);
        }

        if (musicFilePaths.Count == 0)
        {
            Debug.LogError("MP3 ������ �߰ߵ��� �ʾҽ��ϴ�.");
            return;
        }


        // ���� ���̿� ���� �з�
        StartCoroutine(ClassifyMusicFiles(() =>
        {
            isLong = true;
            currentIndex = 0;
            UpdateThumbnail();
            PlayMusic();
        }));

        // ��ư �̺�Ʈ ����
        longButton.onClick.AddListener(() => { SwitchCategory(true); PlayMusic(); });
        shortButton.onClick.AddListener(() => { SwitchCategory(false); PlayMusic(); });
        leftArrowButton.onClick.AddListener(() => ChangeThumbnail(-1));
        rightArrowButton.onClick.AddListener(() => ChangeThumbnail(1));
    }
    

    private System.Collections.IEnumerator ClassifyMusicFiles(System.Action onComplete)
    {
        foreach (string path in musicFilePaths)
        {
            using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip("file://" + path, AudioType.MPEG))
            {
                yield return uwr.SendWebRequest();

                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(uwr);
                    float clipLength = clip.length; // ���� ���� (�� ����)

                    // �ε��� Ȯ��
                    int index = musicFilePaths.IndexOf(path);
                    if (index != -1 && index < thumbnailList.Count && index < thumbnailTitles.Count)
                    {
                        if (clipLength > 60) // 1�� �̻�
                        {
                            longFilePaths.Add(path);
                            longThumbnails.Add(thumbnailList[index]);
                            longTitles.Add(thumbnailTitles[index]);
                        }
                        else // 1�� ����
                        {
                            shortFilePaths.Add(path);
                            shortThumbnails.Add(thumbnailList[index]);
                            shortTitles.Add(thumbnailTitles[index]);
                        }
                    }
                    else
                    {
                        Debug.LogError($"��ȿ���� ���� �ε���: {index} (path: {path})");
                    }
                }
                else
                {
                    Debug.LogError("����� ���� �ε� ����: " + path);
                }
            }
        }

        onComplete?.Invoke(); // �з� �Ϸ� �� �ʱ�ȭ ȣ��
    }

    private void SwitchCategory(bool toLong)
    {
        isLong = toLong;
        currentIndex = 0; // ù ��° �뷡�� �ʱ�ȭ
        UpdateThumbnail();
    }

    private void ChangeThumbnail(int direction)
    {
        currentIndex += direction;

        if (isLong)
        {
            if (currentIndex < 0)
                currentIndex = longThumbnails.Count - 1;
            else if (currentIndex >= longThumbnails.Count)
                currentIndex = 0;
        }
        else
        {
            if (currentIndex < 0)
                currentIndex = shortThumbnails.Count - 1;
            else if (currentIndex >= shortThumbnails.Count)
                currentIndex = 0;
        }

        UpdateThumbnail();
        PlayMusic();
    }

    private void UpdateThumbnail()
    {
        if (isLong && longThumbnails.Count > 0)
        {
            displayImage.sprite = longThumbnails[currentIndex];
            displayTitle.text = longTitles[currentIndex];
        }
        else if (!isLong && shortThumbnails.Count > 0)
        {
            displayImage.sprite = shortThumbnails[currentIndex];
            displayTitle.text = shortTitles[currentIndex];
        }
        else
        {
            Debug.LogWarning("��ȿ�� ������� �����ϴ�.");
        }
    }

    private void PlayMusic()
    {
        if (isLong && longFilePaths.Count > currentIndex)
        {
            string musicPath = longFilePaths[currentIndex];
            StartCoroutine(LoadAudio(musicPath));
        }
        else if (!isLong && shortFilePaths.Count > currentIndex)
        {
            string musicPath = shortFilePaths[currentIndex];
            StartCoroutine(LoadAudio(musicPath));
        }
        else
        {
            Debug.LogError("���� ������ �������� �ʰų� �߸��� �ε����Դϴ�.");
        }
    }

    private System.Collections.IEnumerator LoadAudio(string filePath)
    {
        using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip("file://" + filePath, AudioType.MPEG))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.Success)
            {
                audioSource.clip = DownloadHandlerAudioClip.GetContent(uwr);
                audioSource.Play();
            }
            else
            {
                Debug.LogError("����� ���� �ε� ����: " + uwr.error);
            }
        }
    }
}
