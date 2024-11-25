using UnityEngine;
using System.Collections.Generic;

public class BoneTransformation : MonoBehaviour
{
    private Transform rootTransform; // Root_M�� Transform

    void Start()
    {
        rootTransform = GameObject.Find("Root_M")?.transform;
        if (rootTransform == null)
        {
            Debug.LogError("Root_M�� ã�� �� �����ϴ�!");
            return;
        }
    }

    void Update()
    {
        int landmarkCount = 33;
        if (UDPReceiver.Instance != null && UDPReceiver.Instance.LatestCoord3Ds.Count >= landmarkCount)
        {
            List<List<List<float>>> coord3Ds = UDPReceiver.Instance.LatestCoord3Ds; // Mediapipe ���� ��ǥ

            // ��� ���� ���� ��������� ��ǥ ��ȯ
            //ApplyBoneTransform(rootTransform, coord3Ds);
        }
    }

    void ApplyBoneTransform(Transform bone, List<List<float>> coord3D)
    {
        // ���� ���� �̸��� Mediapipe ��ǥ ���� Ȯ��
        string boneName = bone.name;
        int? landmarkIndex = FindLandmarkIndex(boneName);

        if (landmarkIndex.HasValue)
        {
            // Mediapipe ���� ��ǥ ��������
            List<float> mediapipeWorldCoord = coord3D[landmarkIndex.Value];
            Vector3 worldPosition = new Vector3(mediapipeWorldCoord[0], mediapipeWorldCoord[1], mediapipeWorldCoord[2]);

            // bone_to_world ���
            Matrix4x4 boneToWorld = bone.localToWorldMatrix;

            // world_to_bone ���
            Matrix4x4 worldToBone = boneToWorld.inverse;

            // Mediapipe ��ǥ�� �� ��ǥ��� ��ȯ
            Vector3 localPosition = worldToBone.MultiplyPoint3x4(worldPosition);

            // ���� ���� ��ǥ ������Ʈ
            bone.position = localPosition;
        }

        // �ڽ� ���� ���� ��������� ȣ��
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
        return null; // �ش��ϴ� ���帶ũ�� ���� ���
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
        // �ʿ��� ���帶ũ �߰�
    };
}
