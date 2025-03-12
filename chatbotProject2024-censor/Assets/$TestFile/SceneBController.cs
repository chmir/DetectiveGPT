using TMPro;
using UnityEngine;

public class SceneBController : MonoBehaviour
{
    public TextMeshProUGUI stageIdText; // ���õ� �������� ID ǥ�� �ؽ�Ʈ
    public GameObject sceneBUI; // SceneB UI �׷�

    private void Start()
    {
        if (sceneBUI != null)
        {
            sceneBUI.SetActive(true); // SceneB UI Ȱ��ȭ
        }

        UpdateStageIdDisplay(); // ���õ� �������� ID�� ȭ�鿡 ǥ��
    }

    // SceneC�� �̵�
    public void GoToSceneC()
    {
        if (sceneBUI != null)
        {
            sceneBUI.SetActive(false); // SceneB UI ����
        }

        if (GameController.Instance != null)
        {
            GameController.Instance.SwitchScene("SceneB", "SceneC"); // SceneB ��ε� �� SceneC �ε�
        }
    }

    // SceneA�� ���ư��� �޼���
    public void GoToSceneA()
    {
        if (sceneBUI != null)
        {
            sceneBUI.SetActive(false); // SceneB UI ����
        }

        if (GameController.Instance != null)
        {
            GameController.Instance.SwitchScene("SceneB", "SceneA"); // SceneB ��ε� �� SceneA ����
        }
    }

    // ���õ� �������� ID�� ȭ�鿡 ǥ���ϴ� �޼���
    private void UpdateStageIdDisplay()
    {
        if (GameController.Instance != null && stageIdText != null)
        {
            stageIdText.text = "Stage ID: " + GameController.Instance.SelectedStageId; // ���õ� �������� ID ǥ��
        }
    }
}
