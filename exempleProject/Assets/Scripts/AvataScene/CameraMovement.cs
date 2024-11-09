using UnityEngine;

public class CameraMovementAndSelection : MonoBehaviour
{
    public float moveDistanceWS = 5f; // W/S 이동 거리 조절
    public float moveDistanceAD = 5f; // A/D 이동 거리 조절
    public float moveSpeedWS = 2f;    // W/S 이동 속도 조절 (보간 속도)
    public float moveSpeedAD = 2f;    // A/D 이동 속도 조절 (보간 속도)

    // 이동 카운트 제한 변수
    public int minMoveCountX = -5; // X축 최소 이동 카운트 (왼쪽 이동 최대 횟수)
    public int maxMoveCountX = 5;  // X축 최대 이동 카운트 (오른쪽 이동 최대 횟수)
    public int minMoveCountZ = -5; // Z축 최소 이동 카운트 (뒤로 이동 최대 횟수)
    public int maxMoveCountZ = 5;  // Z축 최대 이동 카운트 (앞으로 이동 최대 횟수)

    // 현재 이동 카운트 추적 변수
    private int currentMoveCountX = 0; // X축 현재 이동 카운트
    private int currentMoveCountZ = 0; // Z축 현재 이동 카운트

    private bool isMoving = false;        // 현재 이동 중인지 여부
    private Vector3 startPosition;        // 이동 시작 위치
    private Vector3 targetPosition;       // 이동 목표 위치
    private float lerpTime = 0f;          // 보간 시간
    private float currentMoveSpeed = 0f;  // 현재 이동 속도

    // 선택된 Prefab을 저장할 변수
    private Transform selectedPrefab;

    void Start()
    {
        // 초기 위치에서 선택된 Prefab을 설정
        SelectCenterPrefab();
    }

    void Update()
    {
        if (!isMoving)
        {
            // 이동 중이 아닐 때 키 입력 처리
            if (Input.GetKeyDown(KeyCode.W))
            {
                if (currentMoveCountZ < maxMoveCountZ)
                {
                    StartMovement(Vector3.forward, moveSpeedWS, moveDistanceWS);
                    currentMoveCountZ++;
                }
                else
                {
                    Debug.Log("앞으로 더 이상 이동할 수 없습니다.");
                }
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                if (currentMoveCountZ > minMoveCountZ)
                {
                    StartMovement(Vector3.back, moveSpeedWS, moveDistanceWS);
                    currentMoveCountZ--;
                }
                else
                {
                    Debug.Log("뒤로 더 이상 이동할 수 없습니다.");
                }
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                if (currentMoveCountX > minMoveCountX)
                {
                    StartMovement(Vector3.left, moveSpeedAD, moveDistanceAD);
                    currentMoveCountX--;
                }
                else
                {
                    Debug.Log("왼쪽으로 더 이상 이동할 수 없습니다.");
                }
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                if (currentMoveCountX < maxMoveCountX)
                {
                    StartMovement(Vector3.right, moveSpeedAD, moveDistanceAD);
                    currentMoveCountX++;
                }
                else
                {
                    Debug.Log("오른쪽으로 더 이상 이동할 수 없습니다.");
                }
            }
        }
        else
        {
            // 이동 중일 때 선형 보간으로 이동
            lerpTime += Time.deltaTime * currentMoveSpeed;
            transform.position = Vector3.Lerp(startPosition, targetPosition, lerpTime);

            if (lerpTime >= 1f)
            {
                // 이동 완료
                transform.position = targetPosition;
                isMoving = false;
                lerpTime = 0f;

                // 이동이 완료되면 화면 중앙에 보이는 Prefab 선택
                SelectCenterPrefab();
            }
        }
    }

    void StartMovement(Vector3 direction, float speed, float distance)
    {
        isMoving = true;
        startPosition = transform.position;
        targetPosition = startPosition + direction * distance;
        lerpTime = 0f;
        currentMoveSpeed = speed;
    }

    void SelectCenterPrefab()
    {
        // 화면의 중심에서 Ray를 쏴서 가장 먼저 만나는 오브젝트를 선택

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        // 레이어 마스크 설정 (필요한 경우 특정 레이어만 검출)
        int layerMask = LayerMask.GetMask("Default"); // 또는 원하는 레이어 이름으로 변경

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            // 이전에 선택된 Prefab의 강조 표시를 해제
            if (selectedPrefab != null)
            {
                UnhighlightPrefab(selectedPrefab);
            }

            // 새로 선택된 Prefab 저장
            selectedPrefab = hit.transform;
            Debug.Log(selectedPrefab);
            // 선택된 Prefab에 원하는 처리를 합니다.
            HighlightPrefab(selectedPrefab);
        }
    }

    void HighlightPrefab(Transform prefab)
    {
        // Prefab을 강조 표시하거나 선택 상태로 만드는 로직을 구현합니다.
        // 예시: Prefab의 색상을 변경하거나 효과를 추가
        Renderer renderer = prefab.GetComponent<Renderer>();
        if (renderer != null)
        {
            // 기존 색상을 저장하고 강조 표시 색상으로 변경
            renderer.material.color = Color.yellow; // 강조 표시 색상
        }
    }

    void UnhighlightPrefab(Transform prefab)
    {
        // 강조 표시를 해제하는 로직
        Renderer renderer = prefab.GetComponent<Renderer>();
        if (renderer != null)
        {
            // 머티리얼 색상을 원래대로 복원
            renderer.material.color = Color.white; // 기본 색상
        }
    }
}
