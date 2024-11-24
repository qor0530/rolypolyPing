using System.Collections.Generic;
using UnityEngine;

public class JointController : MonoBehaviour
{
    public List<Transform> joints; // Unity���� ����Ϸ��� ��(����) Transform
    private int[] mappedLandmarks =
    {
        0,   // �Ӹ�
        11,  // ������ ���
        12,  // ���� ���
        13,  // ������ �Ȳ�ġ
        14,  // ���� �Ȳ�ġ
        15,  // ������ �ո�
        16,  // ���� �ո�
        23,  // ������ ���
        24,  // ���� ���
        25,  // ������ ����
        26,  // ���� ����
        27,  // ������ �߸�
        28   // ���� �߸�
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

                // ��ǥ ��ȯ �� ������ ����
                joints[i].localPosition = ConvertToUnityCoords(new Vector3(
                    positionData[0], positionData[1], positionData[2]
                ));
            }

            Debug.Log("Ư�� �� ������Ʈ �Ϸ�");
        }
    }

    private Vector3 ConvertToUnityCoords(Vector3 mediapipeCoord)
    {
        float scaleFactor = 5.0f; // �ʿ信 ���� ������ ����
        return new Vector3(
            mediapipeCoord.x * scaleFactor,
            mediapipeCoord.y * -scaleFactor, // Mediapipe�� z�� Unity�� y�� ����
            mediapipeCoord.z * scaleFactor  // Mediapipe�� y�� Unity�� z�� ����
        );
    }
}
