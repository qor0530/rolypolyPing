using UnityEngine;
using UnityEngine.SceneManagement; // �� ��ȯ�� ���� ���ӽ����̽�
using UnityEngine.UI; // UI ��ư�� ����ϱ� ���� ���ӽ����̽�
using System.Collections.Generic; // List ����� ����

public class SceneChange : MonoBehaviour
{
    // Button�� SceneName�� ������ Ŭ������ ����
    [System.Serializable]
    public class ButtonScenePair
    {
        public Button button; // UI ��ư
        public string sceneName; // ��ȯ�� �� �̸�
    }

    // ����Ʈ�� ���� ���� ��ư-�� ������ ����
    public List<ButtonScenePair> buttonScenePairs;

    void Start()
    {
        // �� ��ư�� ���� Ŭ�� �̺�Ʈ�� ����
        foreach (var pair in buttonScenePairs)
        {
            pair.button.onClick.AddListener(() => ChangeScene(pair.sceneName));
        }
    }

    // ���� ��ȯ�ϴ� �޼���
    void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}


// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.SceneManagement;
// using UnityEngine.UI;

// public class ResultManager : MonoBehaviour
// {
//     public Button confirmResultButton; // 확인 버튼 : 누르면 특정 씬으로 이동
//     public Button retryButton;         // 다시 하기 버튼 : 현재 씬을 재시작
//     public Button lobbyButton;         // 로비 버튼 : 로비 화면으로 이동

//     void Start()
//     {
//         PrintResult();

//         // 버튼 클릭 이벤트 연결
//         confirmResultButton.onClick.AddListener(() => ChangeScene("RankingScene")); // 랭킹 화면으로 이동
//         retryButton.onClick.AddListener(() => ChangeScene(SceneManager.GetActiveScene().name)); // 현재 씬 재시작
//         lobbyButton.onClick.AddListener(() => ChangeScene("LobbyScene")); // 로비 화면으로 이동
//     }

//     // 게임 결과 출력 함수
//     void PrintResult()
//     {
//         // 구조 : 싱글 또는 멀티 여부에 따라 결과를 출력
//         Debug.Log("게임 결과 출력");
//     }

//     // 씬 변경을 처리하는 함수
//     void ChangeScene(string sceneName)
//     {
//         SceneManager.LoadScene(sceneName);
//     }
// }
