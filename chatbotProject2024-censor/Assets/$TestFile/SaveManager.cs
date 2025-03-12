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
        // save ���� ��� ����
        string folderPath = Path.Combine(Application.dataPath, "save");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            Debug.Log("save ���� ����");
        }

        savePath = Path.Combine(folderPath, "savefile.json");

        // ���۽� �ε� �õ�
        Load();
    }

    // Save: ���� ��� ���� -> JSON���� ���̺�
    public void Save()
    {
        if (currentData == null) currentData = new SaveData();

        // ��� ���¸� currentData�� �ݿ�
        currentData.bot_a = toggleA.isOn;
        currentData.bot_b = toggleB.isOn;
        currentData.bot_c = toggleC.isOn;

        // JSON ����ȭ
        string json = JsonUtility.ToJson(currentData, true);
        File.WriteAllText(savePath, json);

        Debug.Log("���� �Ϸ�");
        if (debugText) debugText.text = "���� �Ϸ�";
    }

    // Load: ���̺� ���� -> JSON �б� -> SaveData ���� -> ��� �ݿ�
    public void Load()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            currentData = JsonUtility.FromJson<SaveData>(json);

            // �ε� �� ��� ���� ����
            toggleA.isOn = currentData.bot_a;
            toggleB.isOn = currentData.bot_b;
            toggleC.isOn = currentData.bot_c;

            Debug.Log("�ε� �Ϸ�");
            if (debugText) debugText.text = "�ε� �Ϸ�";
        }
        else
        {
            // ������ ������ �⺻�� ���
            currentData = new SaveData();
            Debug.Log("�ε��� ���� ����, �⺻������ ����");
            if (debugText) debugText.text = "�⺻ ����";
        }
    }

    // ResetData: ������ �ʱ�ȭ �� ����
    public void ResetData()
    {
        if (currentData == null) currentData = new SaveData();
        currentData.Reset();

        // UI �ݿ�
        toggleA.isOn = currentData.bot_a;
        toggleB.isOn = currentData.bot_b;
        toggleC.isOn = currentData.bot_c;

        Save();

        Debug.Log("������ ���� �Ϸ�");
        if (debugText) debugText.text = "������ ���� �Ϸ�";
    }
}
