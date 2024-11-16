using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PoseUpdaterFromFile : MonoBehaviour
{
    public string filePath = "Assets/pose_data.txt"; // 텍스트 파일 경로
    public Transform[] joints; // 미디어파이프 랜드마크에 매핑될 관절 Transform 배열

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
            if (values.Length < 4) continue; // 데이터가 부족한 줄 무시

            // 랜드마크 번호와 좌표 데이터 파싱
            int index = int.Parse(values[0]);
            float x = float.Parse(values[1]);
            float y = float.Parse(values[2]);
            float z = float.Parse(values[3]);

            // 관절 Transform 업데이트 (joints 배열에 랜드마크 매핑 필요)
            if (index >= 0 && index < joints.Length)
            {
                joints[index].localPosition = new Vector3(x, y, z);
            }
        }
    }
}