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
    void Update()
    {
<<<<<<< Updated upstream
        if (UDPReceiver.Instance != null)
        {
            try
=======
        if (UDPReceiver.Instance != null && UDPReceiver.Instance.LatestCoord3Ds[1].Count >= mappedLandmarks.Length)
        {
            var coord3D = UDPReceiver.Instance.LatestCoord3Ds[1];

            for (int i = 0; i < mappedLandmarks.Length; i++)
>>>>>>> Stashed changes
            {
                var coord3D = UDPReceiver.Instance.LatestCoord3Ds[avata_id];    
                for (int i = 0; i < 33; i++)
            {
                var positionData = coord3D[i];
                if (i == 0) {
                        joints[i].transform.localPosition = ConvertToUnityCoords(new Vector3(
                       positionData[0], positionData[1], positionData[2] + 1.0f
                    ));
                    }
                else
                    {
                //좌표 변환 및 스케일 조정
                joints[i].transform.localPosition = ConvertToUnityCoords(new Vector3(
                   positionData[0], positionData[1], positionData[2]
                ));
                    }

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
