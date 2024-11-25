using UnityEngine;
using System.Collections.Generic;

public class BoneTransformation : MonoBehaviour
{
    private Transform rootTransform; // Root_M의 Transform

    void Start()
    {
        rootTransform = GameObject.Find("Root_M")?.transform;
        if (rootTransform == null)
        {
            Debug.LogError("Root_M을 찾을 수 없습니다!");
            return;
        }
    }

    void Update()
    {
        int landmarkCount = 33;
        if (UDPReceiver.Instance != null && UDPReceiver.Instance.LatestCoord3Ds.Count >= landmarkCount)
        {
            List<List<List<float>>> coord3Ds = UDPReceiver.Instance.LatestCoord3Ds; // Mediapipe 월드 좌표

            // 모든 본에 대해 재귀적으로 좌표 변환
            //ApplyBoneTransform(rootTransform, coord3Ds);
        }
    }

    void ApplyBoneTransform(Transform bone, List<List<float>> coord3D)
    {
        // 현재 본의 이름과 Mediapipe 좌표 매핑 확인
        string boneName = bone.name;
        int? landmarkIndex = FindLandmarkIndex(boneName);

        if (landmarkIndex.HasValue)
        {
            // Mediapipe 월드 좌표 가져오기
            List<float> mediapipeWorldCoord = coord3D[landmarkIndex.Value];
            Vector3 worldPosition = new Vector3(mediapipeWorldCoord[0], mediapipeWorldCoord[1], mediapipeWorldCoord[2]);

            // bone_to_world 행렬
            Matrix4x4 boneToWorld = bone.localToWorldMatrix;

            // world_to_bone 행렬
            Matrix4x4 worldToBone = boneToWorld.inverse;

            // Mediapipe 좌표를 본 좌표계로 변환
            Vector3 localPosition = worldToBone.MultiplyPoint3x4(worldPosition);

            // 본의 로컬 좌표 업데이트
            bone.position = localPosition;
        }

        // 자식 본에 대해 재귀적으로 호출
        foreach (Transform child in bone)
        {
            ApplyBoneTransform(child, coord3D);
        }
    }

    int? FindLandmarkIndex(string boneName)
    {
        foreach (var pair in landmarkToBoneMap)
        {
            if (pair.Value == boneName)
                return pair.Key;
        }
        return null; // 해당하는 랜드마크가 없는 경우
    }

    private Dictionary<int, string> landmarkToBoneMap = new Dictionary<int, string>()
    {
        {23, "Hip_L"},
        {24, "Hip_R"},
        {25, "Knee_L"},
        {26, "Knee_R"},
        {27, "Ankle_L"},
        {28, "Ankle_R"},
        {29, "Toes_L"},
        {30, "Toes_R"},
        {11, "Shoulder_L"},
        {12, "Shoulder_R"},
        {13, "Elbow_L"},
        {14, "Elbow_R"},
        {15, "Wrist_L"},
        {16, "Wrist_R"}
        // 필요한 랜드마크 추가
    };
}
