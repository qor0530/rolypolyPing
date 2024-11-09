using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RigSelector : MonoBehaviour
{
    public GameObject mainRig; // _MainRig ������Ʈ�� �巡���ؼ� �Ҵ�
    public GameObject checkboxPrefab; // üũ�ڽ� ������ (Toggle�� üũ�ڽ�ó�� ���)
    public Transform checkboxContainer; // Scroll View�� Content�� ���� (Checkbox���� ��ġ�� �θ� �����̳�)

    private List<Transform> rigObjects = new List<Transform>(); // Geometry ������ ���� ������Ʈ ����Ʈ
    private Transform geometry;
    private Toggle activeCheckbox; // ���� Ȱ��ȭ�� üũ�ڽ��� ����

    void Start()
    {
        // Geometry ������Ʈ ��������
        geometry = mainRig.transform.Find("Geometry");

        // Geometry ������ ���� ������Ʈ���� ����Ʈ�� �߰��ϰ� �ʱ�ȭ
        foreach (Transform child in geometry)
        {
            rigObjects.Add(child);
            child.gameObject.SetActive(false); // �ʱ⿡�� ��� ������Ʈ ��Ȱ��ȭ

            // Checkbox(Toggle)�� �����ϰ� UI�� �߰�
            GameObject checkboxObject = Instantiate(checkboxPrefab, checkboxContainer);
            Toggle checkbox = checkboxObject.GetComponent<Toggle>();

            // Checkbox�� Label �ؽ�Ʈ�� ������Ʈ �̸����� ����
            checkboxObject.GetComponentInChildren<Text>().text = child.name;

            // Checkbox�� �ʱ� ���¸� üũ ���� (false)�� ����
            checkbox.isOn = false;

            // Checkbox ���� �� �ش� ������Ʈ�� Ȱ��ȭ �Ǵ� ��Ȱ��ȭ�ϴ� ������ �߰�
            checkbox.onValueChanged.AddListener(isOn => {
                ToggleRigObject(child, isOn);
                if (isOn)
                {
                    SetActiveCheckbox(checkbox);
                }
            });
        }
    }

    // Ư�� ���� ������Ʈ�� Ȱ��ȭ �Ǵ� ��Ȱ��ȭ�ϴ� �޼���
    private void ToggleRigObject(Transform rigObject, bool isOn)
    {
        rigObject.gameObject.SetActive(isOn); // üũ ���¿� ���� Ȱ��ȭ/��Ȱ��ȭ
        if (isOn)
        {
            // ���õ� ������Ʈ�� ȭ�� �߾ӿ� ��ġ
            rigObject.position = new Vector3(0, 0, 0); // ȭ�� �߾� ��ǥ ����
            rigObject.rotation = Quaternion.identity; // �⺻ ȸ�� ����
        }
    }

    // ���� Ȱ��ȭ�� üũ�ڽ��� �����ϰ� ���� üũ ����
    private void SetActiveCheckbox(Toggle newCheckbox)
    {
        if (activeCheckbox != null && activeCheckbox != newCheckbox)
        {
            activeCheckbox.isOn = false; // ���� üũ�ڽ��� ����
        }
        activeCheckbox = newCheckbox;
    }
}
