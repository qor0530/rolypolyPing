using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Networking;
using System;
using System.Text;
using UnityEngine;
using System.Net;
using System.Net.Sockets;


/// /////////////////// Short/Long 은 mp3 재생길이 1분 기준으로 분류

public class MusicSelector : MonoBehaviour
{
    public List<Sprite> thumbnailList; //썸네일 이미지 리스트
    public List<string> thumbnailTitles; //썸네일에 맞는 타이틀 리스트
    public Image displayImage; //UI Image
    public TextMeshProUGUI displayTitle; //UI 텍스트
    public Button leftArrowButton; //좌 화살표 버튼
    public Button rightArrowButton; //우 화살표 버튼
    public Button thumbnailButton; //play 버튼
    public Button longButton; //Long 버튼
    public Button shortButton; //Short 버튼
    public AudioSource audioSource; //AudioSource 컴포넌트

    private int currentIndex = 0; //현재 인덱스
    private List<string> musicFilePaths = new List<string>(); // 모든 음악 파일 경로 리스트
    //Long
    private List<Sprite> longThumbnails = new List<Sprite>(); // 1분 이상 썸네일
    private List<string> longTitles = new List<string>(); // 1분 이상 제목
    private List<string> longFilePaths = new List<string>(); // 1분 이상 음악 경로
    //Short
    private List<Sprite> shortThumbnails = new List<Sprite>(); // 1분 이하 썸네일
    private List<string> shortTitles = new List<string>(); // 1분 이하 제목
    private List<string> shortFilePaths = new List<string>(); // 1분 이하 음악 경로
    private bool isLong = true; // Long 탭일때 화면 시작


    private void Start()
    {
        // string basePath = "/Users/jeongsieun/Fiction-Royals-Merge/Fiction-Royals/db";

        string basePath = Path.Combine(Application.dataPath, "../../../Fiction-Royals/db");

        // 정규화된 경로 출력
        Debug.Log("Base Path (Full): " + Path.GetFullPath(basePath));


        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        //음악 파일 경로 가져오기(mp3확장자로 된 파일만)
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
        thumbnailButton.onClick.AddListener(StartGame);

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

        onComplete?.Invoke(); //분류 완료 후 초기화 호출 (onComplete 콜백 호출)
    }

    private void SwitchCategory(bool toLong)
    {
        isLong = toLong;
        currentIndex = 0; //첫번째 노래로 초기화->첫번째 노래부터 시작하도록
        UpdateThumbnail(); //UI갱신
    }

    private void ChangeThumbnail(int direction)
    {
        currentIndex += direction;

        if (isLong)
        {
            if (currentIndex < 0) //인덱스가 음수가 되면 마지막 썸네일로 이동
                currentIndex = longThumbnails.Count - 1;
            else if (currentIndex >= longThumbnails.Count)
                currentIndex = 0;
        }
        else
        {
            //short 일때
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

    // play 버튼 연결
    private void StartGame()
    {
        // 선택된 음악 정보를 저장
        PlayerPrefs.SetInt("SelectedMusicIndex", currentIndex);
        PlayerPrefs.SetString("SelectedMusicPath", isLong ? longFilePaths[currentIndex] : shortFilePaths[currentIndex]);
        PlayerPrefs.Save();

        // 어떤 곡 골랐는지 파이썬에 전송
        send_to_python(currentIndex);

        // GameScene으로 전환
        SceneManager.LoadScene("GameScene");
    }

    private void send_to_python(int song_id)
    {
        string message = "";
        if (isLong)
        {
            if (song_id == 0)
            {
                message = "start 1"; // Python으로 보낼 메시지 생성
            }
            else if (song_id == 1)
            {
                message = "start 4";
            }
        }

        else
        {
            if (song_id == 0)
            {
                message = "start 0";
            }
            else if (song_id == 1)
            {
                message = "start 2";
            }
            else if (song_id == 2)
            {
                message = "start 3";
            }
        }
        string ip = "127.0.0.1"; // Python 서버의 IP 주소 (로컬)
        int port = 25252; // Python 서버의 포트 번호
        Debug.Log("보내는 곡: " + message + "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");

        using (UdpClient udpClient = new UdpClient())
        {
            try
            {
                // 메시지를 UTF-8로 인코딩하여 바이트 배열로 변환
                byte[] sendBytes = Encoding.UTF8.GetBytes(message);

                // UDP 메시지 전송
                udpClient.Send(sendBytes, sendBytes.Length, ip, port);
                Debug.Log($"메시지 전송 성공: {message} -> {ip}:{port}");
            }
            catch (Exception e)
            {
                Debug.LogError($"메시지 전송 실패: {e.Message}");
            }
        }
    }
}