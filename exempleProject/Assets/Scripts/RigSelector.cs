using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RigSelector : MonoBehaviour
{
    public GameObject mainRig; // _MainRig 오브젝트를 드래그해서 할당
    public GameObject checkboxPrefab; // 체크박스 프리팹 (Toggle을 체크박스처럼 사용)
    public Transform checkboxContainer; // Scroll View의 Content를 설정 (Checkbox들이 배치될 부모 컨테이너)

    private List<Transform> rigObjects = new List<Transform>(); // Geometry 하위의 리깅 오브젝트 리스트
    private Transform geometry;
    private Toggle activeCheckbox; // 현재 활성화된 체크박스를 저장

    void Start()
    {
        // Geometry 오브젝트 가져오기
        geometry = mainRig.transform.Find("Geometry");

        // Geometry 하위의 리깅 오브젝트들을 리스트에 추가하고 초기화
        foreach (Transform child in geometry)
        {
            rigObjects.Add(child);
            child.gameObject.SetActive(false); // 초기에는 모든 오브젝트 비활성화

            // Checkbox(Toggle)를 생성하고 UI에 추가
            GameObject checkboxObject = Instantiate(checkboxPrefab, checkboxContainer);
            Toggle checkbox = checkboxObject.GetComponent<Toggle>();

            // Checkbox의 Label 텍스트를 오브젝트 이름으로 설정
            checkboxObject.GetComponentInChildren<Text>().text = child.name;

            // Checkbox의 초기 상태를 체크 해제 (false)로 설정
            checkbox.isOn = false;

            // Checkbox 선택 시 해당 오브젝트를 활성화 또는 비활성화하는 리스너 추가
            checkbox.onValueChanged.AddListener(isOn => {
                ToggleRigObject(child, isOn);
                if (isOn)
                {
                    SetActiveCheckbox(checkbox);
                }
            });
        }
    }

    // 특정 리깅 오브젝트를 활성화 또는 비활성화하는 메서드
    private void ToggleRigObject(Transform rigObject, bool isOn)
    {
        rigObject.gameObject.SetActive(isOn); // 체크 상태에 따라 활성화/비활성화
        if (isOn)
        {
            // 선택된 오브젝트를 화면 중앙에 배치
            rigObject.position = new Vector3(0, 0, 0); // 화면 중앙 좌표 설정
            rigObject.rotation = Quaternion.identity; // 기본 회전 설정
        }
    }

    // 현재 활성화된 체크박스를 설정하고 이전 체크 해제
    private void SetActiveCheckbox(Toggle newCheckbox)
    {
        if (activeCheckbox != null && activeCheckbox != newCheckbox)
        {
            activeCheckbox.isOn = false; // 이전 체크박스를 해제
        }
        activeCheckbox = newCheckbox;
    }
}
