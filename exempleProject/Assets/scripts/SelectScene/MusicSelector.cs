using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Networking;

public class MusicSelector : MonoBehaviour
{
    public List<Sprite> thumbnailList; // 썸네일 이미지 리스트
    public List<string> thumbnailTitles; // 썸네일에 맞는 텍스트 리스트
    public Image displayImage; // UI Image
    public TextMeshProUGUI displayTitle; // UI 텍스트
    public Button leftArrowButton; // 좌 화살표 버튼
    public Button rightArrowButton; // 우 화살표 버튼
    public Button thumbnailButton; // 썸네일 버튼
    public Button longButton; // Long 버튼
    public Button shortButton; // Short 버튼
    public AudioSource audioSource; // AudioSource 컴포넌트

    private int currentIndex = 0; // 현재 인덱스
    private List<string> musicFilePaths = new List<string>(); // 모든 음악 파일 경로 리스트
    private List<Sprite> longThumbnails = new List<Sprite>(); // 1분 이상 썸네일
    private List<string> longTitles = new List<string>(); // 1분 이상 제목
    private List<string> longFilePaths = new List<string>(); // 1분 이상 음악 경로
    private List<Sprite> shortThumbnails = new List<Sprite>(); // 1분 이하 썸네일
    private List<string> shortTitles = new List<string>(); // 1분 이하 제목
    private List<string> shortFilePaths = new List<string>(); // 1분 이하 음악 경로
    private bool isLong = true; // Long 버튼 클릭 여부

    private void Start()
    {
        string basePath = "/Users/jeongsieun/Fiction-Royals-Merge/Fiction-Royals/db";

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // 음악 파일 경로 가져오기
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
            Debug.LogError("경로를 찾을 수 없습니다: " + basePath);
        }

        if (musicFilePaths.Count == 0)
        {
            Debug.LogError("MP3 파일이 발견되지 않았습니다.");
            return;
        }


        // 음악 길이에 따라 분류
        StartCoroutine(ClassifyMusicFiles(() =>
        {
            isLong = true;
            currentIndex = 0;
            UpdateThumbnail();
            PlayMusic();
        }));

        // 버튼 이벤트 연결
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
                    float clipLength = clip.length; // 음악 길이 (초 단위)

                    // 인덱스 확인
                    int index = musicFilePaths.IndexOf(path);
                    if (index != -1 && index < thumbnailList.Count && index < thumbnailTitles.Count)
                    {
                        if (clipLength > 60) // 1분 이상
                        {
                            longFilePaths.Add(path);
                            longThumbnails.Add(thumbnailList[index]);
                            longTitles.Add(thumbnailTitles[index]);
                        }
                        else // 1분 이하
                        {
                            shortFilePaths.Add(path);
                            shortThumbnails.Add(thumbnailList[index]);
                            shortTitles.Add(thumbnailTitles[index]);
                        }
                    }
                    else
                    {
                        Debug.LogError($"유효하지 않은 인덱스: {index} (path: {path})");
                    }
                }
                else
                {
                    Debug.LogError("오디오 파일 로드 실패: " + path);
                }
            }
        }

        onComplete?.Invoke(); // 분류 완료 후 초기화 호출
    }

    private void SwitchCategory(bool toLong)
    {
        isLong = toLong;
        currentIndex = 0; // 첫 번째 노래로 초기화
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
            Debug.LogWarning("유효한 썸네일이 없습니다.");
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
            Debug.LogError("음악 파일이 존재하지 않거나 잘못된 인덱스입니다.");
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
                Debug.LogError("오디오 파일 로드 실패: " + uwr.error);
            }
        }
    }
}
