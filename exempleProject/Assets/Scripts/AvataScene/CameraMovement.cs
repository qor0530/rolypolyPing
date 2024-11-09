using UnityEngine;

public class CameraMovementAndSelection : MonoBehaviour
{
    public float moveDistanceWS = 5f; // W/S �̵� �Ÿ� ����
    public float moveDistanceAD = 5f; // A/D �̵� �Ÿ� ����
    public float moveSpeedWS = 2f;    // W/S �̵� �ӵ� ���� (���� �ӵ�)
    public float moveSpeedAD = 2f;    // A/D �̵� �ӵ� ���� (���� �ӵ�)

    // �̵� ī��Ʈ ���� ����
    public int minMoveCountX = -5; // X�� �ּ� �̵� ī��Ʈ (���� �̵� �ִ� Ƚ��)
    public int maxMoveCountX = 5;  // X�� �ִ� �̵� ī��Ʈ (������ �̵� �ִ� Ƚ��)
    public int minMoveCountZ = -5; // Z�� �ּ� �̵� ī��Ʈ (�ڷ� �̵� �ִ� Ƚ��)
    public int maxMoveCountZ = 5;  // Z�� �ִ� �̵� ī��Ʈ (������ �̵� �ִ� Ƚ��)

    // ���� �̵� ī��Ʈ ���� ����
    private int currentMoveCountX = 0; // X�� ���� �̵� ī��Ʈ
    private int currentMoveCountZ = 0; // Z�� ���� �̵� ī��Ʈ

    private bool isMoving = false;        // ���� �̵� ������ ����
    private Vector3 startPosition;        // �̵� ���� ��ġ
    private Vector3 targetPosition;       // �̵� ��ǥ ��ġ
    private float lerpTime = 0f;          // ���� �ð�
    private float currentMoveSpeed = 0f;  // ���� �̵� �ӵ�

    // ���õ� Prefab�� ������ ����
    private Transform selectedPrefab;

    void Start()
    {
        // �ʱ� ��ġ���� ���õ� Prefab�� ����
        SelectCenterPrefab();
    }

    void Update()
    {
        if (!isMoving)
        {
            // �̵� ���� �ƴ� �� Ű �Է� ó��
            if (Input.GetKeyDown(KeyCode.W))
            {
                if (currentMoveCountZ < maxMoveCountZ)
                {
                    StartMovement(Vector3.forward, moveSpeedWS, moveDistanceWS);
                    currentMoveCountZ++;
                }
                else
                {
                    Debug.Log("������ �� �̻� �̵��� �� �����ϴ�.");
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
                    Debug.Log("�ڷ� �� �̻� �̵��� �� �����ϴ�.");
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
                    Debug.Log("�������� �� �̻� �̵��� �� �����ϴ�.");
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
                    Debug.Log("���������� �� �̻� �̵��� �� �����ϴ�.");
                }
            }
        }
        else
        {
            // �̵� ���� �� ���� �������� �̵�
            lerpTime += Time.deltaTime * currentMoveSpeed;
            transform.position = Vector3.Lerp(startPosition, targetPosition, lerpTime);

            if (lerpTime >= 1f)
            {
                // �̵� �Ϸ�
                transform.position = targetPosition;
                isMoving = false;
                lerpTime = 0f;

                // �̵��� �Ϸ�Ǹ� ȭ�� �߾ӿ� ���̴� Prefab ����
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
        // ȭ���� �߽ɿ��� Ray�� ���� ���� ���� ������ ������Ʈ�� ����

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        // ���̾� ����ũ ���� (�ʿ��� ��� Ư�� ���̾ ����)
        int layerMask = LayerMask.GetMask("Default"); // �Ǵ� ���ϴ� ���̾� �̸����� ����

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            // ������ ���õ� Prefab�� ���� ǥ�ø� ����
            if (selectedPrefab != null)
            {
                UnhighlightPrefab(selectedPrefab);
            }

            // ���� ���õ� Prefab ����
            selectedPrefab = hit.transform;
            Debug.Log(selectedPrefab);
            // ���õ� Prefab�� ���ϴ� ó���� �մϴ�.
            HighlightPrefab(selectedPrefab);
        }
    }

    void HighlightPrefab(Transform prefab)
    {
        // Prefab�� ���� ǥ���ϰų� ���� ���·� ����� ������ �����մϴ�.
        // ����: Prefab�� ������ �����ϰų� ȿ���� �߰�
        Renderer renderer = prefab.GetComponent<Renderer>();
        if (renderer != null)
        {
            // ���� ������ �����ϰ� ���� ǥ�� �������� ����
            renderer.material.color = Color.yellow; // ���� ǥ�� ����
        }
    }

    void UnhighlightPrefab(Transform prefab)
    {
        // ���� ǥ�ø� �����ϴ� ����
        Renderer renderer = prefab.GetComponent<Renderer>();
        if (renderer != null)
        {
            // ��Ƽ���� ������ ������� ����
            renderer.material.color = Color.white; // �⺻ ����
        }
    }
}
