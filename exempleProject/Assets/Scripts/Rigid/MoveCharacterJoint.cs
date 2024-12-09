using System.Collections.Generic;
using UnityEngine;

public class MoveCharacterJoint : MonoBehaviour
{
    public Animator anim;
    private MoveAvatar moveAvatar;
    public int userIndex = 1; // ����� ���� ��ȣ (1���� ����)
    public Transform[] jointTransforms; // ĳ������ ���� Ʈ������ ����Ʈ

    void Start()
    {
        anim = GetComponent<Animator>();
        moveAvatar = new MoveAvatar();

        // ���� Ʈ������ ����
        moveAvatar.SetJoints(jointTransforms);
    }

    void Update()
    {
        if (UDPReceiver.Instance.LatestCoord3Ds.Count > userIndex)
        {
            List<List<float>> userCoord3D = UDPReceiver.Instance.LatestCoord3Ds[userIndex];
            Vector3[] realJoint = new Vector3[userCoord3D.Count];
            for (int i = 0; i < userCoord3D.Count; i++)
            {
                List<float> coord = userCoord3D[i];
                realJoint[i] = new Vector3(coord[0], coord[1], coord[2]);
            }

            moveAvatar.SetRequiredData(realJoint);
            moveAvatar.MoveLimbs();
            moveAvatar.MoveTorso();
        }
    }
}
