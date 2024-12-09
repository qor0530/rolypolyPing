using System.Collections.Generic;
using UnityEngine;

public class JointController : MonoBehaviour
{
    public int avata_id;
    public GameObject me;
    private List<GameObject> joints;
    void Start()
    {
        // 현재 게임오브젝트의 모든 자식 게임오브젝트 가져오기
        joints = GetAllChildGameObjects(me);
    }
    // 모든 자식 게임오브젝트를 재귀적으로 가져오는 함수
    List<GameObject> GetAllChildGameObjects(GameObject parent)
    {
        List<GameObject> gameObjects = new List<GameObject>();

        foreach (Transform child in parent.transform)
        {
            gameObjects.Add(child.gameObject); // 게임오브젝트 추가
        }

        return gameObjects;
    }
    private int[] mappedLandmarks =
    {
        0,   // 머리
        11,  // 오른쪽 어깨
        12,  // 왼쪽 어깨
        13,  // 오른쪽 팔꿈치
        14,  // 왼쪽 팔꿈치
        15,  // 오른쪽 손목
        16,  // 왼쪽 손목
        23,  // 오른쪽 골반
        24,  // 왼쪽 골반
        25,  // 오른쪽 무릎
        26,  // 왼쪽 무릎
        27,  // 오른쪽 발목
        28   // 왼쪽 발목
    };

    private Dictionary<int, List<int>> betweenLandmarks = new Dictionary<int, List<int>>
    {
        {33, new List<int> {11, 12}},
        {34, new List<int> {11, 13}},
        {35, new List<int> {13, 15}},
        {36, new List<int> {12, 14}},
        {37, new List<int> {14, 16}},
        {38, new List<int> {11, 23}},
        {39, new List<int> {12, 24}},
        {40, new List<int> {23, 25}},
        {41, new List<int> {25, 27}},
        {42, new List<int> {24, 26}},
        {43, new List<int> {26, 28}},
        {44, new List<int> {23, 24}}
    };

    void Update()
    {
        if (UDPReceiver.Instance != null)
        {
            try
            {
                var coord3D = UDPReceiver.Instance.LatestCoord3Ds[avata_id];
                for (int i = 0; i < 33; i++)
                {
                    var positionData = coord3D[i];

                    joints[i].transform.localPosition = ConvertToUnityCoords(new Vector3(
                           positionData[0], positionData[1], positionData[2]
                        ));


                    // if (i == 0)
                    // {
                    //     joints[i].transform.localPosition = ConvertToUnityCoords(new Vector3(
                    //    positionData[0], positionData[1], positionData[2] + 1.0f
                    // ));
                    // }
                    // else
                    // {
                    //     //좌표 변환 및 스케일 조정
                    //     joints[i].transform.localPosition = ConvertToUnityCoords(new Vector3(
                    //        positionData[0], positionData[1], positionData[2]
                    //     ));
                    // }

                }

                foreach (KeyValuePair<int, List<int>> pair in betweenLandmarks)
                {
                    int target_index = pair.Key;

                    int from_index = pair.Value[0];
                    int to_index = pair.Value[1];

                    Vector3 start_pos = joints[from_index].transform.localPosition;
                    Vector3 end_pos = joints[to_index].transform.localPosition;

                    joints[target_index].transform.localPosition = new((start_pos.x + end_pos.x) / 2, (start_pos.y + end_pos.y) / 2, (start_pos.z + end_pos.z) / 2);
                }
            }
            catch
            {
                Debug.Log("coord3D에 자료 X");
            }

        }
        else
        {
            Debug.Log("UDPReciver.Instance가 null");
        }
    }

    private Vector3 ConvertToUnityCoords(Vector3 mediapipeCoord)
    {
        float scaleFactor = 5.0f; // 필요에 따라 스케일 조정
        return new Vector3(
            mediapipeCoord.x * scaleFactor,
            mediapipeCoord.y * -scaleFactor, // Mediapipe의 z를 Unity의 y로 매핑
            mediapipeCoord.z * scaleFactor  // Mediapipe의 y를 Unity의 z로 매핑
        );
    }
}
