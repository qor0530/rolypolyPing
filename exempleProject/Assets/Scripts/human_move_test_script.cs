using UnityEngine;
using System.Collections.Generic;

public class BoneTransformation : MonoBehaviour
{
<<<<<<< Updated upstream
    private Transform rootTransform; // Root_M의 Transform
=======
    // 랜드마크 인덱스와 본 이름 매핑
    private Dictionary<int, string> landmarkToBoneMap = new Dictionary<int, string>()
    {
        {11, "Shoulder_L"},     // 왼쪽 어깨
        {12, "Shoulder_R"},     // 오른쪽 어깨
        {13, "Elbow_L"},        // 왼쪽 팔꿈치
        {14, "Elbow_R"},        // 오른쪽 팔꿈치
        {15, "Wrist_L"},        // 왼쪽 손목
        {16, "Wrist_R"},        // 오른쪽 손목
        {23, "Hip_L"},          // 왼쪽 골반
        {24, "Hip_R"},          // 오른쪽 골반
        {25, "Knee_L"},         // 왼쪽 무릎
        {26, "Knee_R"},         // 오른쪽 무릎
        {27, "Ankle_L"},        // 왼쪽 발목
        {28, "Ankle_R"},        // 오른쪽 발목
        {29, "Toes_L"},         // 왼쪽 발가락
        {30, "Toes_R"},         // 오른쪽 발가락
        {31, "ToesEnd_L"},      // 왼쪽 발가락 끝
        {32, "ToesEnd_R"},      // 오른쪽 발가락 끝
        // 필요한 경우 손가락 본들도 추가 가능합니다.
    };

    // 랜드마크의 부모 랜드마크 매핑
    private Dictionary<int, int> parentLandmarkMap = new Dictionary<int, int>()
    {
        {13, 11},    // Elbow_L의 부모는 Shoulder_L
        {14, 12},    // Elbow_R의 부모는 Shoulder_R
        {15, 13},    // Wrist_L의 부모는 Elbow_L
        {16, 14},    // Wrist_R의 부모는 Elbow_R
        {25, 23},    // Knee_L의 부모는 Hip_L
        {26, 24},    // Knee_R의 부모는 Hip_R
        {27, 25},    // Ankle_L의 부모는 Knee_L
        {28, 26},    // Ankle_R의 부모는 Knee_R
        {29, 27},    // Toes_L의 부모는 Ankle_L
        {30, 28},    // Toes_R의 부모는 Ankle_R
        {31, 29},    // ToesEnd_L의 부모는 Toes_L
        {32, 30},    // ToesEnd_R의 부모는 Toes_R
        // 어깨와 골반의 부모는 Spine1_M
        {11, -1},    // Shoulder_L의 부모는 Spine1_M
        {12, -1},    // Shoulder_R의 부모는 Spine1_M
        {23, -1},    // Hip_L의 부모는 Root_M
        {24, -1},    // Hip_R의 부모는 Root_M
    };

    // 본의 Transform 저장
    private Dictionary<int, Transform> landmarkToBoneTransform = new Dictionary<int, Transform>();

    // 초기 로컬 회전 저장 (캘리브레이션 데이터)
    private Dictionary<int, Quaternion> initialLocalRotations = new Dictionary<int, Quaternion>();

    // 초기 로컬 방향 저장 (캘리브레이션 데이터)
    private Dictionary<int, Vector3> initialLocalDirections = new Dictionary<int, Vector3>();

    // Root_M의 Transform
    private Transform rootTransform;
>>>>>>> Stashed changes

    void Start()
    {
        rootTransform = GameObject.Find("Root_M")?.transform;
        if (rootTransform == null)
        {
            Debug.LogError("Root_M을 찾을 수 없습니다!");
            return;
        }
<<<<<<< Updated upstream
=======

        // Animator 비활성화
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.enabled = false;
        }

        // Root_M의 초기 로컬 회전과 방향 저장
        initialLocalRotations[-1] = rootTransform.localRotation;
        initialLocalDirections[-1] = Vector3.forward; // 필요에 따라 수정

        // 랜드마크와 본의 Transform 매핑 및 초기 상태 저장 (캘리브레이션)
        foreach (var kvp in landmarkToBoneMap)
        {
            int landmarkIndex = kvp.Key;
            string boneName = kvp.Value;

            Transform boneTransform = FindBoneTransform(boneName);
            if (boneTransform != null)
            {
                landmarkToBoneTransform[landmarkIndex] = boneTransform;

                // 초기 로컬 회전 저장
                initialLocalRotations[landmarkIndex] = boneTransform.localRotation;

                // 초기 로컬 방향 저장
                Vector3 initialDirection = GetInitialLocalDirection(boneTransform);
                initialLocalDirections[landmarkIndex] = initialDirection;
            }
            else
            {
                Debug.LogError($"본 {boneName}을 찾을 수 없습니다!");
            }
        }
    }

    // 본의 Transform을 이름으로 찾는 함수
    Transform FindBoneTransform(string boneName)
    {
        // 계층 구조에서 본을 검색
        return rootTransform.Find(boneName) ?? GameObject.Find(boneName)?.transform;
    }

    // 본의 초기 로컬 방향을 계산하는 함수
    Vector3 GetInitialLocalDirection(Transform bone)
    {
        switch (bone.name)
        {
            case "Shoulder_L":
                return Vector3.left; // 왼쪽 어깨의 초기 방향
            case "Shoulder_R":
                return Vector3.right; // 오른쪽 어깨의 초기 방향
            case "Elbow_L":
            case "Elbow_R":
            case "Wrist_L":
            case "Wrist_R":
                return Vector3.down; // 팔과 손의 초기 방향
            case "Hip_L":
                return Vector3.down; // 왼쪽 골반의 초기 방향
            case "Hip_R":
                return Vector3.down; // 오른쪽 골반의 초기 방향
            case "Knee_L":
            case "Knee_R":
            case "Ankle_L":
            case "Ankle_R":
                return Vector3.down; // 다리의 초기 방향
            default:
                return Vector3.forward; // 기본 방향
        }
    }

    private Vector3 ConvertToUnityCoords(Vector3 mediapipeCoord)
    {
        float scaleFactor = 1.0f; // 필요에 따라 조정
        return new Vector3(
            mediapipeCoord.x * scaleFactor, // X축 반전
            mediapipeCoord.y * scaleFactor,  // Y축 유지
            -mediapipeCoord.z * scaleFactor   // Z축 반전
        );
>>>>>>> Stashed changes
    }

    void Update()
    {
        int landmarkCount = 33;
<<<<<<< Updated upstream
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
=======
        if (UDPReceiver.Instance != null && UDPReceiver.Instance.LatestCoord3Ds[1].Count >= landmarkCount)
        {
            var coord3D = UDPReceiver.Instance.LatestCoord3Ds[1];

            // Root_M의 회전 업데이트 (어깨의 방향을 기반으로)
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

            // 각 본의 회전 업데이트
            foreach (var kvp in landmarkToBoneTransform)
            {
                int landmarkIndex = kvp.Key;
                Transform boneTransform = kvp.Value;

                if (parentLandmarkMap.ContainsKey(landmarkIndex))
                {
                    int parentLandmarkIndex = parentLandmarkMap[landmarkIndex];

                    // 부모 랜드마크와 현재 랜드마크의 위치 가져오기
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
                            // 부모가 없는 경우 본의 부모 Transform의 위치 사용
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

    // 본의 초기 Transform을 고려하여 회전 업데이트
    void UpdateBoneRotation(int landmarkIndex, Transform bone, Vector3 parentWorldPosition, Vector3 childWorldPosition)
    {
        // 본의 로컬 회전을 초기 로컬 회전으로 리셋
        bone.localRotation = initialLocalRotations[landmarkIndex];

        Vector3 desiredDirection = childWorldPosition - parentWorldPosition;
>>>>>>> Stashed changes

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

<<<<<<< Updated upstream
            // 본의 로컬 좌표 업데이트
            bone.position = localPosition;
        }

        // 자식 본에 대해 재귀적으로 호출
        foreach (Transform child in bone)
        {
            ApplyBoneTransform(child, coord3D);
=======
            // 계산된 회전을 본의 로컬 회전에 적용
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
