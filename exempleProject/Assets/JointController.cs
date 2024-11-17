using System.Collections.Generic;
using UnityEngine;

public class JointController : MonoBehaviour
{
    // 33���� ���� Transform ����Ʈ
    public List<Transform> joints;

    void Update()
    {
        if (UDPReceiver.Instance != null && UDPReceiver.Instance.LatestCoord3D.Count == 33)
        {
            var coord3D = UDPReceiver.Instance.LatestCoord3D;

            for (int i = 0; i < joints.Count; i++)
            {
                var positionData = coord3D[i];
                joints[i].localPosition = new Vector3(positionData[0], positionData[1], positionData[2]);
            }

            Debug.Log("���� ��ġ�� ������Ʈ�Ǿ����ϴ�.");
        }
    }
}
