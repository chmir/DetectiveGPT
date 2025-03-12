using UnityEngine;

public class SceneAController : MonoBehaviour
{
    public static SceneAController Instance { get; private set; } // �̱��� ����
    public GameObject sceneAUI; // SceneA UI �׷�

    private void Awake()
    {
        // �̱��� ����
        if (Instance == null)
        {
            Instance = this; // Instance �ʱ�ȭ
        }
        else
        {
            Destroy(gameObject); // �ߺ� ����
        }
    }

    private void Start()
    {
        if (sceneAUI == null)
        {
            Debug.LogError("Scene A UI is not assigned! Assign the Canvas object in the Inspector.");
        }
        else
        {
            sceneAUI.SetActive(true); // ���� Ȱ��ȭ
        }
    }


    // SceneC�� �̵�
    public void GoToSceneC()
    {
        if (sceneAUI != null)
        {
            sceneAUI.SetActive(false); // SceneA UI ��Ȱ��ȭ
        }

        if (GameController.Instance != null)
        {
            GameController.Instance.LoadScene("SceneC"); // SceneC �ε�
        }
    }

    // SceneB�� �̵�
    public void GoToSceneB()
    {
        if (sceneAUI != null)
        {
            sceneAUI.SetActive(false); // SceneA UI ��Ȱ��ȭ
        }

        if (GameController.Instance != null)
        {
            GameController.Instance.LoadScene("SceneB"); // SceneB �ε�
        }
    }

    // SceneA UI ���� �޼���
    public void RestoreUI()
    {
        if (sceneAUI != null)
        {
            sceneAUI.SetActive(true); // SceneA UI Ȱ��ȭ
        }
    }

    // SceneA�� ���ƿ� �� ȣ��Ǵ� �޼���
    public void OnReturnToSceneA()
    {
        RestoreUI(); // UI ����
    }
}
