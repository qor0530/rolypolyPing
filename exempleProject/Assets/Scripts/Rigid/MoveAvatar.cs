using System.Collections.Generic;
using UnityEngine;

class MoveAvatar
{
    private Transform[] joints;       // ���� Ʈ������ �迭
    private Vector3[] realJoint;      // �ֽ� ���� ������ (��ǥ)

    public void SetJoints(Transform[] jointTransforms)
    {
        joints = jointTransforms;
    }

    public void SetRequiredData(Vector3[] jointData)
    {
        realJoint = jointData;
    }

    public void MoveLimbs()
    {
        if (realJoint == null || joints == null) return;

        for (int i = 0; i < joints.Length; i++)
        {
            if (i < realJoint.Length && joints[i] != null)
            {
                // ������ ��ġ�� �ֽ� �����ͷ� ������Ʈ
                joints[i].localPosition = realJoint[i];
                Debug.Log($"Moving Joint {joints[i].name} to Position: {realJoint[i]}");
            }
        }
    }

    public void MoveTorso()
    {
        // ���뿡 �ش��ϴ� ���� ���� �ۼ� (��: Spine, Chest)
        if (joints.Length > 0 && realJoint.Length > 0)
        {
            Debug.Log("���� ������ ���� �߰� �ʿ�");
            // �ʿ��ϸ� �߰� ���� �ۼ�
        }
    }
}
