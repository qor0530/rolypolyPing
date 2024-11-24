using System.Collections.Generic;
using UnityEngine;

public class JointController : MonoBehaviour
{
    public List<Transform> joints; // Unity에서 사용하려는 본(관절) Transform
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
        if (UDPReceiver.Instance != null && UDPReceiver.Instance.LatestCoord3D.Count >= mappedLandmarks.Length)
        {
            var coord3D = UDPReceiver.Instance.LatestCoord3D;

            for (int i = 0; i < mappedLandmarks.Length; i++)
            {
                int landmarkIndex = mappedLandmarks[i];
                var positionData = coord3D[landmarkIndex];

                // 좌표 변환 및 스케일 조정
                joints[i].localPosition = ConvertToUnityCoords(new Vector3(
                    positionData[0], positionData[1], positionData[2]
                ));
            }

            Debug.Log("특정 본 업데이트 완료");
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
