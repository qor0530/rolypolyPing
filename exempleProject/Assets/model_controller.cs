using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PoseUpdaterFromFile : MonoBehaviour
{
    public string filePath = "Assets/pose_data.txt"; // �ؽ�Ʈ ���� ���
    public Transform[] joints; // �̵�������� ���帶ũ�� ���ε� ���� Transform �迭

    void Start()
    {
        UpdateJointsFromFile();
    }

    void UpdateJointsFromFile()
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError($"File not found: {filePath}");
            return;
        }

        string[] lines = File.ReadAllLines(filePath);

        foreach (string line in lines)
        {
            string[] values = line.Split(',');
            if (values.Length < 4) continue; // �����Ͱ� ������ �� ����

            // ���帶ũ ��ȣ�� ��ǥ ������ �Ľ�
            int index = int.Parse(values[0]);
            float x = float.Parse(values[1]);
            float y = float.Parse(values[2]);
            float z = float.Parse(values[3]);

            // ���� Transform ������Ʈ (joints �迭�� ���帶ũ ���� �ʿ�)
            if (index >= 0 && index < joints.Length)
            {
                joints[index].localPosition = new Vector3(x, y, z);
            }
        }
    }
}