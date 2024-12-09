using System.Collections.Generic;
using UnityEngine;

public class JointController : MonoBehaviour
{
    public int avata_id;
    public GameObject me;
    private List<GameObject> joints;
    void Start()
    {
        // ���� ���ӿ�����Ʈ�� ��� �ڽ� ���ӿ�����Ʈ ��������
        joints = GetAllChildGameObjects(me);
    }
    // ��� �ڽ� ���ӿ�����Ʈ�� ��������� �������� �Լ�
    List<GameObject> GetAllChildGameObjects(GameObject parent)
    {
        List<GameObject> gameObjects = new List<GameObject>();

        foreach (Transform child in parent.transform)
        {
            gameObjects.Add(child.gameObject); // ���ӿ�����Ʈ �߰�
        }

        return gameObjects;
    }
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
                //��ǥ ��ȯ �� ������ ����
                joints[i].transform.localPosition = ConvertToUnityCoords(new Vector3(
                   positionData[0], positionData[1], positionData[2]
                ));
                    }

            }
            }
            catch
            {
                Debug.Log("coord3D�� �ڷ� X");
            }
            
        }
        else
        {
            Debug.Log("UDPReciver.Instance�� null");
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
