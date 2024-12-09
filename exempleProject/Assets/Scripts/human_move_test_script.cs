using UnityEngine;
using System.Collections.Generic;

public class BoneTransformation : MonoBehaviour
{
<<<<<<< Updated upstream
    private Transform rootTransform; // Root_M�� Transform
=======
    // ���帶ũ �ε����� �� �̸� ����
    private Dictionary<int, string> landmarkToBoneMap = new Dictionary<int, string>()
    {
        {11, "Shoulder_L"},     // ���� ���
        {12, "Shoulder_R"},     // ������ ���
        {13, "Elbow_L"},        // ���� �Ȳ�ġ
        {14, "Elbow_R"},        // ������ �Ȳ�ġ
        {15, "Wrist_L"},        // ���� �ո�
        {16, "Wrist_R"},        // ������ �ո�
        {23, "Hip_L"},          // ���� ���
        {24, "Hip_R"},          // ������ ���
        {25, "Knee_L"},         // ���� ����
        {26, "Knee_R"},         // ������ ����
        {27, "Ankle_L"},        // ���� �߸�
        {28, "Ankle_R"},        // ������ �߸�
        {29, "Toes_L"},         // ���� �߰���
        {30, "Toes_R"},         // ������ �߰���
        {31, "ToesEnd_L"},      // ���� �߰��� ��
        {32, "ToesEnd_R"},      // ������ �߰��� ��
        // �ʿ��� ��� �հ��� ���鵵 �߰� �����մϴ�.
    };

    // ���帶ũ�� �θ� ���帶ũ ����
    private Dictionary<int, int> parentLandmarkMap = new Dictionary<int, int>()
    {
        {13, 11},    // Elbow_L�� �θ�� Shoulder_L
        {14, 12},    // Elbow_R�� �θ�� Shoulder_R
        {15, 13},    // Wrist_L�� �θ�� Elbow_L
        {16, 14},    // Wrist_R�� �θ�� Elbow_R
        {25, 23},    // Knee_L�� �θ�� Hip_L
        {26, 24},    // Knee_R�� �θ�� Hip_R
        {27, 25},    // Ankle_L�� �θ�� Knee_L
        {28, 26},    // Ankle_R�� �θ�� Knee_R
        {29, 27},    // Toes_L�� �θ�� Ankle_L
        {30, 28},    // Toes_R�� �θ�� Ankle_R
        {31, 29},    // ToesEnd_L�� �θ�� Toes_L
        {32, 30},    // ToesEnd_R�� �θ�� Toes_R
        // ����� ����� �θ�� Spine1_M
        {11, -1},    // Shoulder_L�� �θ�� Spine1_M
        {12, -1},    // Shoulder_R�� �θ�� Spine1_M
        {23, -1},    // Hip_L�� �θ�� Root_M
        {24, -1},    // Hip_R�� �θ�� Root_M
    };

    // ���� Transform ����
    private Dictionary<int, Transform> landmarkToBoneTransform = new Dictionary<int, Transform>();

    // �ʱ� ���� ȸ�� ���� (Ķ���극�̼� ������)
    private Dictionary<int, Quaternion> initialLocalRotations = new Dictionary<int, Quaternion>();

    // �ʱ� ���� ���� ���� (Ķ���극�̼� ������)
    private Dictionary<int, Vector3> initialLocalDirections = new Dictionary<int, Vector3>();

    // Root_M�� Transform
    private Transform rootTransform;
>>>>>>> Stashed changes

    void Start()
    {
        rootTransform = GameObject.Find("Root_M")?.transform;
        if (rootTransform == null)
        {
            Debug.LogError("Root_M�� ã�� �� �����ϴ�!");
            return;
        }
<<<<<<< Updated upstream
=======

        // Animator ��Ȱ��ȭ
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.enabled = false;
        }

        // Root_M�� �ʱ� ���� ȸ���� ���� ����
        initialLocalRotations[-1] = rootTransform.localRotation;
        initialLocalDirections[-1] = Vector3.forward; // �ʿ信 ���� ����

        // ���帶ũ�� ���� Transform ���� �� �ʱ� ���� ���� (Ķ���극�̼�)
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
        // ���� �������� ���� �˻�
        return rootTransform.Find(boneName) ?? GameObject.Find(boneName)?.transform;
    }

    // ���� �ʱ� ���� ������ ����ϴ� �Լ�
    Vector3 GetInitialLocalDirection(Transform bone)
    {
        switch (bone.name)
        {
            case "Shoulder_L":
                return Vector3.left; // ���� ����� �ʱ� ����
            case "Shoulder_R":
                return Vector3.right; // ������ ����� �ʱ� ����
            case "Elbow_L":
            case "Elbow_R":
            case "Wrist_L":
            case "Wrist_R":
                return Vector3.down; // �Ȱ� ���� �ʱ� ����
            case "Hip_L":
                return Vector3.down; // ���� ����� �ʱ� ����
            case "Hip_R":
                return Vector3.down; // ������ ����� �ʱ� ����
            case "Knee_L":
            case "Knee_R":
            case "Ankle_L":
            case "Ankle_R":
                return Vector3.down; // �ٸ��� �ʱ� ����
            default:
                return Vector3.forward; // �⺻ ����
        }
    }

    private Vector3 ConvertToUnityCoords(Vector3 mediapipeCoord)
    {
        float scaleFactor = 1.0f; // �ʿ信 ���� ����
        return new Vector3(
            mediapipeCoord.x * scaleFactor, // X�� ����
            mediapipeCoord.y * scaleFactor,  // Y�� ����
            -mediapipeCoord.z * scaleFactor   // Z�� ����
        );
>>>>>>> Stashed changes
    }

    void Update()
    {
        int landmarkCount = 33;
<<<<<<< Updated upstream
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
=======
        if (UDPReceiver.Instance != null && UDPReceiver.Instance.LatestCoord3Ds[1].Count >= landmarkCount)
        {
            var coord3D = UDPReceiver.Instance.LatestCoord3Ds[1];

            // Root_M�� ȸ�� ������Ʈ (����� ������ �������)
            Vector3 shoulderLeft = ConvertToUnityCoords(new Vector3(
                coord3D[11][0],
                coord3D[11][1],
                coord3D[11][2]
            ));
            Vector3 shoulderRight = ConvertToUnityCoords(new Vector3(
                coord3D[12][0],
                coord3D[12][1],
                coord3D[12][2]
            ));

            Vector3 shoulderDirection = shoulderRight - shoulderLeft;
            if (shoulderDirection != Vector3.zero)
            {
                Quaternion shoulderRotation = Quaternion.LookRotation(Vector3.forward, shoulderDirection);
                rootTransform.rotation = shoulderRotation;
            }

            // �� ���� ȸ�� ������Ʈ
            foreach (var kvp in landmarkToBoneTransform)
            {
                int landmarkIndex = kvp.Key;
                Transform boneTransform = kvp.Value;

                if (parentLandmarkMap.ContainsKey(landmarkIndex))
                {
                    int parentLandmarkIndex = parentLandmarkMap[landmarkIndex];

                    // �θ� ���帶ũ�� ���� ���帶ũ�� ��ġ ��������
                    if (coord3D.Count > landmarkIndex && (parentLandmarkIndex == -1 || coord3D.Count > parentLandmarkIndex))
                    {
                        Vector3 currentWorldPosition = ConvertToUnityCoords(new Vector3(
                            coord3D[landmarkIndex][0],
                            coord3D[landmarkIndex][1],
                            coord3D[landmarkIndex][2]
                        ));

                        Vector3 parentWorldPosition;

                        if (parentLandmarkIndex == -1)
                        {
                            // �θ� ���� ��� ���� �θ� Transform�� ��ġ ���
                            parentWorldPosition = boneTransform.parent.position;
                        }
                        else
                        {
                            parentWorldPosition = ConvertToUnityCoords(new Vector3(
                                coord3D[parentLandmarkIndex][0],
                                coord3D[parentLandmarkIndex][1],
                                coord3D[parentLandmarkIndex][2]
                            ));
                        }

                        UpdateBoneRotation(landmarkIndex, boneTransform, parentWorldPosition, currentWorldPosition);
                    }
                }
            }
        }
    }

    // ���� �ʱ� Transform�� ����Ͽ� ȸ�� ������Ʈ
    void UpdateBoneRotation(int landmarkIndex, Transform bone, Vector3 parentWorldPosition, Vector3 childWorldPosition)
    {
        // ���� ���� ȸ���� �ʱ� ���� ȸ������ ����
        bone.localRotation = initialLocalRotations[landmarkIndex];

        Vector3 desiredDirection = childWorldPosition - parentWorldPosition;
>>>>>>> Stashed changes

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

<<<<<<< Updated upstream
            // ���� ���� ��ǥ ������Ʈ
            bone.position = localPosition;
        }

        // �ڽ� ���� ���� ��������� ȣ��
        foreach (Transform child in bone)
        {
            ApplyBoneTransform(child, coord3D);
=======
            // ���� ȸ���� ���� ���� ȸ���� ����
            bone.localRotation *= rotationToDesired;
>>>>>>> Stashed changes
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
