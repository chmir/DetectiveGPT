using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SaveManager : MonoBehaviour
{
    [SerializeField] private Toggle toggleA;
    [SerializeField] private Toggle toggleB;
    [SerializeField] private Toggle toggleC;
    [SerializeField] private TMPro.TextMeshProUGUI debugText;


    private SaveData currentData;
    private string savePath;

    void Awake()
    {
        // save 폴더 경로 설정
        string folderPath = Path.Combine(Application.dataPath, "save");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            Debug.Log("save 폴더 생성");
        }

        savePath = Path.Combine(folderPath, "savefile.json");

        // 시작시 로드 시도
        Load();
    }

    // Save: 현재 토글 상태 -> JSON으로 세이브
    public void Save()
    {
        if (currentData == null) currentData = new SaveData();

        // 토글 상태를 currentData에 반영
        currentData.bot_a = toggleA.isOn;
        currentData.bot_b = toggleB.isOn;
        currentData.bot_c = toggleC.isOn;

        // JSON 직렬화
        string json = JsonUtility.ToJson(currentData, true);
        File.WriteAllText(savePath, json);

        Debug.Log("저장 완료");
        if (debugText) debugText.text = "저장 완료";
    }

    // Load: 세이브 파일 -> JSON 읽기 -> SaveData 복원 -> 토글 반영
    public void Load()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            currentData = JsonUtility.FromJson<SaveData>(json);

            // 로드 후 토글 상태 갱신
            toggleA.isOn = currentData.bot_a;
            toggleB.isOn = currentData.bot_b;
            toggleC.isOn = currentData.bot_c;

            Debug.Log("로드 완료");
            if (debugText) debugText.text = "로드 완료";
        }
        else
        {
            // 파일이 없으면 기본값 사용
            currentData = new SaveData();
            Debug.Log("로드할 파일 없음, 기본값으로 시작");
            if (debugText) debugText.text = "기본 상태";
        }
    }

    // ResetData: 데이터 초기화 후 저장
    public void ResetData()
    {
        if (currentData == null) currentData = new SaveData();
        currentData.Reset();

        // UI 반영
        toggleA.isOn = currentData.bot_a;
        toggleB.isOn = currentData.bot_b;
        toggleC.isOn = currentData.bot_c;

        Save();

        Debug.Log("데이터 리셋 완료");
        if (debugText) debugText.text = "데이터 리셋 완료";
    }
}
