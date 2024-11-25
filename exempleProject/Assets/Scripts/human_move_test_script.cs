using System.Collections.Generic;
using UnityEngine;

public class HumanMoveTestScript : MonoBehaviour
{
    // 랜드마크 인덱스와 본 이름 매핑
    private Dictionary<int, string> landmarkToBoneMap = new Dictionary<int, string>()
    {
        {23, "Hip_L"},          // 왼쪽 골반
        {24, "Hip_R"},          // 오른쪽 골반
        {25, "Knee_L"},         // 왼쪽 무릎
        {26, "Knee_R"},         // 오른쪽 무릎
        {27, "Ankle_L"},        // 왼쪽 발목
        {28, "Ankle_R"},        // 오른쪽 발목
        {11, "Shoulder_L"},     // 왼쪽 어깨
        {12, "Shoulder_R"},     // 오른쪽 어깨
        {13, "Elbow_L"},        // 왼쪽 팔꿈치
        {14, "Elbow_R"},        // 오른쪽 팔꿈치
        {15, "Wrist_L"},        // 왼쪽 손목
        {16, "Wrist_R"},        // 오른쪽 손목
        // 필요한 경우 추가
    };

    // 랜드마크의 부모 랜드마크 매핑
    private Dictionary<int, int> parentLandmarkMap = new Dictionary<int, int>()
    {
        {25, 23},    // Knee_L의 부모는 Hip_L
        {26, 24},    // Knee_R의 부모는 Hip_R
        {27, 25},    // Ankle_L의 부모는 Knee_L
        {28, 26},    // Ankle_R의 부모는 Knee_R
        {13, 11},    // Elbow_L의 부모는 Shoulder_L
        {14, 12},    // Elbow_R의 부모는 Shoulder_R
        {15, 13},    // Wrist_L의 부모는 Elbow_L
        {16, 14},    // Wrist_R의 부모는 Elbow_R
        // 필요한 경우 추가
    };

    // 본의 Transform 저장
    private Dictionary<int, Transform> landmarkToBoneTransform = new Dictionary<int, Transform>();

    // 초기 로컬 회전 저장
    private Dictionary<int, Quaternion> initialLocalRotations = new Dictionary<int, Quaternion>();

    // 초기 로컬 방향 저장
    private Dictionary<int, Vector3> initialLocalDirections = new Dictionary<int, Vector3>();

    // Root_M의 Transform
    private Transform rootTransform;

    void Start()
    {
        // Root_M의 Transform 찾기
        rootTransform = GameObject.Find("Root_M")?.transform;
        if (rootTransform == null)
        {
            Debug.LogError("Root_M을 찾을 수 없습니다!");
            return;
        }

        // Animator 비활성화
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.enabled = false;
        }

        // 랜드마크와 본의 Transform 매핑 및 초기 상태 저장
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
        // Root_M 아래에서 검색
        return rootTransform.Find(boneName) ?? GameObject.Find(boneName)?.transform;
    }

    // 본의 초기 로컬 방향을 계산하는 함수
    Vector3 GetInitialLocalDirection(Transform bone)
    {
        switch (bone.name)
        {
            case "Hip_L":
            case "Hip_R":
                return Vector3.up; // 골반 본의 초기 방향
            case "Knee_L":
            case "Knee_R":
                return Vector3.down; // 무릎 본의 초기 방향
            case "Ankle_L":
            case "Ankle_R":
                return Vector3.down; // 발목 본의 초기 방향
            case "Shoulder_L":
            case "Shoulder_R":
                return Vector3.down; // 어깨 본의 초기 방향
            case "Elbow_L":
            case "Elbow_R":
                return Vector3.down; // 팔꿈치 본의 초기 방향
            case "Wrist_L":
            case "Wrist_R":
                return Vector3.down; // 손목 본의 초기 방향
            // 필요한 경우 다른 본 추가
            default:
                return Vector3.forward; // 기본 방향
        }
    }

    private Vector3 ConvertToUnityCoords(Vector3 mediapipeCoord)
    {
        float scaleFactor = 1.5f; // 필요에 따라 조정
        return new Vector3(
            -mediapipeCoord.x * scaleFactor, // X축 반전
            mediapipeCoord.y * scaleFactor,  // Y축 유지
            mediapipeCoord.z * scaleFactor   // Z축 유지
        );
    }

    void Update()
    {
        int landmarkCount = 33;
        if (UDPReceiver.Instance != null && UDPReceiver.Instance.LatestCoord3D.Count >= landmarkCount)
        {
            var coord3D = UDPReceiver.Instance.LatestCoord3D;
          
            // 각 본의 회전 업데이트
            foreach (var kvp in landmarkToBoneTransform)
            {
                int landmarkIndex = kvp.Key;
                Transform boneTransform = kvp.Value;

                // 부모 랜드마크가 있는 경우 회전 업데이트
                if (parentLandmarkMap.ContainsKey(landmarkIndex))
                {
                    int parentLandmarkIndex = parentLandmarkMap[landmarkIndex];

                    // 부모 랜드마크의 위치 가져오기
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

    // 본의 초기 방향을 고려하여 회전 업데이트
    void UpdateBoneRotation(int landmarkIndex, Transform bone, Vector3 parentWorldPosition, Vector3 childWorldPosition)
    {
        Vector3 desiredDirection = childWorldPosition - parentWorldPosition;

        if (desiredDirection != Vector3.zero)
        {
            desiredDirection.Normalize();

            // 부모 본의 로컬 공간으로 변환된 목표 방향
            Vector3 localDesiredDirection = bone.parent.InverseTransformDirection(desiredDirection);

            // 본의 초기 로컬 방향
            Vector3 initialLocalDirection = initialLocalDirections[landmarkIndex];

            // 초기 로컬 방향에서 목표 방향으로의 회전 계산
            Quaternion rotationToDesired = Quaternion.FromToRotation(initialLocalDirection, localDesiredDirection);

            // 본의 초기 로컬 회전에 회전 적용
            bone.localRotation = rotationToDesired * initialLocalRotations[landmarkIndex];
        }
    }
}
