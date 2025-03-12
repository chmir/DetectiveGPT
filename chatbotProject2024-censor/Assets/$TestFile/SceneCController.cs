using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneCController : MonoBehaviour
{
    public GameObject sceneCUI; // SceneC UI �׷�
    public Button[] stageButtons; // �������� ��ư �迭

    private void Start()
    {
        if (sceneCUI != null)
        {
            sceneCUI.SetActive(true); // SceneC UI Ȱ��ȭ
        }

        AssignStageButtonListeners(); // �������� ��ư ������ �߰�
    }

    // �� �������� ��ư�� Ŭ�� �����ʸ� �߰��ϴ� �޼���
    private void AssignStageButtonListeners()
    {
        for (int i = 0; i < stageButtons.Length; i++)
        {
            int stageId = i + 1; // �������� ID�� 1���� ����
            stageButtons[i].onClick.AddListener(() => SelectStage(stageId));
        }
    }

    // �������� ���� �� ȣ��Ǵ� �޼���
    private void SelectStage(int stageId)
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.SetStageId(stageId); // ���õ� �������� ID ����

            if (sceneCUI != null)
            {
                sceneCUI.SetActive(false); // SceneC UI ����
            }

            GameController.Instance.SwitchScene("SceneC", "SceneB"); // SceneC ��ε� �� SceneB �ε�
        }
    }

    // SceneA�� ���ư��� �޼���
    public void GoToSceneA()
    {
        if (GameController.Instance != null)
        {
            Debug.Log("Switching from SceneC to SceneA");
            GameController.Instance.SwitchScene("SceneC", "SceneA");
        }

        if (SceneAController.Instance != null)
        {
            Debug.Log("Calling RestoreUI on SceneAController");
            SceneAController.Instance.OnReturnToSceneA();
        }
        else
        {
            Debug.LogError("SceneAController.Instance is null! SceneA may not be loaded yet.");
        }
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "SceneA" && SceneAController.Instance != null)
        {
            SceneAController.Instance.OnReturnToSceneA(); // SceneA UI Ȱ��ȭ
            SceneManager.sceneLoaded -= OnSceneLoaded; // �̺�Ʈ ���� ����
        }
    }

}
