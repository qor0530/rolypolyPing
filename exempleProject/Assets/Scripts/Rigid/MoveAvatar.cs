using System.Collections.Generic;
using UnityEngine;

class MoveAvatar
{
    private Transform[] joints;       // 관절 트랜스폼 배열
    private Vector3[] realJoint;      // 최신 관절 데이터 (좌표)

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
                // 관절의 위치를 최신 데이터로 업데이트
                joints[i].localPosition = realJoint[i];
                Debug.Log($"Moving Joint {joints[i].name} to Position: {realJoint[i]}");
            }
        }
    }

    public void MoveTorso()
    {
        // 몸통에 해당하는 관절 로직 작성 (예: Spine, Chest)
        if (joints.Length > 0 && realJoint.Length > 0)
        {
            Debug.Log("몸통 움직임 로직 추가 필요");
            // 필요하면 추가 로직 작성
        }
    }
}
