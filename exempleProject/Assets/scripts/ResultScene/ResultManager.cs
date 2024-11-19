using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    public Button RetryBtn; // Retry 버튼: ChoiceGame 씬으로 이동
    public Button LobbyBtn; // Lobby 버튼: Main 씬으로 이동


    public Text ExcellentCount; // ExcellentCount 텍스트 UI
    public Text GoodCount;      // GoodCount 텍스트 UI
    public Text BadCount;       // BadCount 텍스트 UI

    // 게임 결과 데이터 (외부에서 할당받을 값)
    private int excellent = 30;
    private int good = 10;
    private int bad = 3;


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

        UpdateResultUI();

    }

    // 씬 변경을 처리하는 함수
    void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    void UpdateResultUI()
    {
        if (ExcellentCount == null || GoodCount == null || BadCount == null)
        {
            Debug.LogError("One or more Text UI elements are not assigned in the Inspector!");
            return;
        }

        // UI 텍스트 업데이트
        ExcellentCount.text = excellent.ToString();
        GoodCount.text = good.ToString();
        BadCount.text = bad.ToString();
    }

    // 외부에서 결과 데이터를 설정할 수 있도록 메서드 추가
    public void SetResults(int excellent, int good, int bad)
    {
        this.excellent = excellent;
        this.good = good;
        this.bad = bad;

        // UI 업데이트
        UpdateResultUI();
    }
}
