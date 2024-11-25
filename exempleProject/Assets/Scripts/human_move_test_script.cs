using System.Collections.Generic;
using UnityEngine;

public class HumanMoveTestScript : MonoBehaviour
{
    // ���帶ũ �ε����� �� �̸� ����
    private Dictionary<int, string> landmarkToBoneMap = new Dictionary<int, string>()
    {
        {23, "Hip_L"},          // ���� ���
        {24, "Hip_R"},          // ������ ���
        {25, "Knee_L"},         // ���� ����
        {26, "Knee_R"},         // ������ ����
        {27, "Ankle_L"},        // ���� �߸�
        {28, "Ankle_R"},        // ������ �߸�
        {11, "Shoulder_L"},     // ���� ���
        {12, "Shoulder_R"},     // ������ ���
        {13, "Elbow_L"},        // ���� �Ȳ�ġ
        {14, "Elbow_R"},        // ������ �Ȳ�ġ
        {15, "Wrist_L"},        // ���� �ո�
        {16, "Wrist_R"},        // ������ �ո�
        // �ʿ��� ��� �߰�
    };

    // ���帶ũ�� �θ� ���帶ũ ����
    private Dictionary<int, int> parentLandmarkMap = new Dictionary<int, int>()
    {
        {25, 23},    // Knee_L�� �θ�� Hip_L
        {26, 24},    // Knee_R�� �θ�� Hip_R
        {27, 25},    // Ankle_L�� �θ�� Knee_L
        {28, 26},    // Ankle_R�� �θ�� Knee_R
        {13, 11},    // Elbow_L�� �θ�� Shoulder_L
        {14, 12},    // Elbow_R�� �θ�� Shoulder_R
        {15, 13},    // Wrist_L�� �θ�� Elbow_L
        {16, 14},    // Wrist_R�� �θ�� Elbow_R
        // �ʿ��� ��� �߰�
    };

    // ���� Transform ����
    private Dictionary<int, Transform> landmarkToBoneTransform = new Dictionary<int, Transform>();

    // �ʱ� ���� ȸ�� ����
    private Dictionary<int, Quaternion> initialLocalRotations = new Dictionary<int, Quaternion>();

    // �ʱ� ���� ���� ����
    private Dictionary<int, Vector3> initialLocalDirections = new Dictionary<int, Vector3>();

    // Root_M�� Transform
    private Transform rootTransform;

    void Start()
    {
        // Root_M�� Transform ã��
        rootTransform = GameObject.Find("Root_M")?.transform;
        if (rootTransform == null)
        {
            Debug.LogError("Root_M�� ã�� �� �����ϴ�!");
            return;
        }

        // Animator ��Ȱ��ȭ
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.enabled = false;
        }

        // ���帶ũ�� ���� Transform ���� �� �ʱ� ���� ����
        foreach (var kvp in landmarkToBoneMap)
        {
            int landmarkIndex = kvp.Key;
            string boneName = kvp.Value;

            Transform boneTransform = FindBoneTransform(boneName);
            if (boneTransform != null)
            {
                landmarkToBoneTransform[landmarkIndex] = boneTransform;

                // �ʱ� ���� ȸ�� ����
                initialLocalRotations[landmarkIndex] = boneTransform.localRotation;

                // �ʱ� ���� ���� ����
                Vector3 initialDirection = GetInitialLocalDirection(boneTransform);
                initialLocalDirections[landmarkIndex] = initialDirection;
            }
            else
            {
                Debug.LogError($"�� {boneName}�� ã�� �� �����ϴ�!");
            }
        }
    }

    // ���� Transform�� �̸����� ã�� �Լ�
    Transform FindBoneTransform(string boneName)
    {
        // Root_M �Ʒ����� �˻�
        return rootTransform.Find(boneName) ?? GameObject.Find(boneName)?.transform;
    }

    // ���� �ʱ� ���� ������ ����ϴ� �Լ�
    Vector3 GetInitialLocalDirection(Transform bone)
    {
        switch (bone.name)
        {
            case "Hip_L":
            case "Hip_R":
                return Vector3.up; // ��� ���� �ʱ� ����
            case "Knee_L":
            case "Knee_R":
                return Vector3.down; // ���� ���� �ʱ� ����
            case "Ankle_L":
            case "Ankle_R":
                return Vector3.down; // �߸� ���� �ʱ� ����
            case "Shoulder_L":
            case "Shoulder_R":
                return Vector3.down; // ��� ���� �ʱ� ����
            case "Elbow_L":
            case "Elbow_R":
                return Vector3.down; // �Ȳ�ġ ���� �ʱ� ����
            case "Wrist_L":
            case "Wrist_R":
                return Vector3.down; // �ո� ���� �ʱ� ����
            // �ʿ��� ��� �ٸ� �� �߰�
            default:
                return Vector3.forward; // �⺻ ����
        }
    }

    private Vector3 ConvertToUnityCoords(Vector3 mediapipeCoord)
    {
        float scaleFactor = 1.5f; // �ʿ信 ���� ����
        return new Vector3(
            -mediapipeCoord.x * scaleFactor, // X�� ����
            mediapipeCoord.y * scaleFactor,  // Y�� ����
            mediapipeCoord.z * scaleFactor   // Z�� ����
        );
    }

    void Update()
    {
        int landmarkCount = 33;
        if (UDPReceiver.Instance != null && UDPReceiver.Instance.LatestCoord3D.Count >= landmarkCount)
        {
            var coord3D = UDPReceiver.Instance.LatestCoord3D;
          
            // �� ���� ȸ�� ������Ʈ
            foreach (var kvp in landmarkToBoneTransform)
            {
                int landmarkIndex = kvp.Key;
                Transform boneTransform = kvp.Value;

                // �θ� ���帶ũ�� �ִ� ��� ȸ�� ������Ʈ
                if (parentLandmarkMap.ContainsKey(landmarkIndex))
                {
                    int parentLandmarkIndex = parentLandmarkMap[landmarkIndex];

                    // �θ� ���帶ũ�� ��ġ ��������
                    if (coord3D.Count > parentLandmarkIndex && coord3D.Count > landmarkIndex)
                    {
                        Vector3 parentWorldPosition = ConvertToUnityCoords(new Vector3(
                            coord3D[parentLandmarkIndex][0],
                            coord3D[parentLandmarkIndex][1],
                            coord3D[parentLandmarkIndex][2]
                        ));

                        Vector3 childWorldPosition = ConvertToUnityCoords(new Vector3(
                            coord3D[landmarkIndex][0],
                            coord3D[landmarkIndex][1],
                            coord3D[landmarkIndex][2]
                        ));

                        UpdateBoneRotation(landmarkIndex, boneTransform, parentWorldPosition, childWorldPosition);
                    }
                }
            }
        }
    }

    // ���� �ʱ� ������ ����Ͽ� ȸ�� ������Ʈ
    void UpdateBoneRotation(int landmarkIndex, Transform bone, Vector3 parentWorldPosition, Vector3 childWorldPosition)
    {
        Vector3 desiredDirection = childWorldPosition - parentWorldPosition;

        if (desiredDirection != Vector3.zero)
        {
            desiredDirection.Normalize();

            // �θ� ���� ���� �������� ��ȯ�� ��ǥ ����
            Vector3 localDesiredDirection = bone.parent.InverseTransformDirection(desiredDirection);

            // ���� �ʱ� ���� ����
            Vector3 initialLocalDirection = initialLocalDirections[landmarkIndex];

            // �ʱ� ���� ���⿡�� ��ǥ ���������� ȸ�� ���
            Quaternion rotationToDesired = Quaternion.FromToRotation(initialLocalDirection, localDesiredDirection);

            // ���� �ʱ� ���� ȸ���� ȸ�� ����
            bone.localRotation = rotationToDesired * initialLocalRotations[landmarkIndex];
        }
    }
}
