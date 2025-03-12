using UnityEngine;

public class SceneAController : MonoBehaviour
{
    public static SceneAController Instance { get; private set; } // 싱글톤 패턴
    public GameObject sceneAUI; // SceneA UI 그룹

    private void Awake()
    {
        // 싱글톤 설정
        if (Instance == null)
        {
            Instance = this; // Instance 초기화
        }
        else
        {
            Destroy(gameObject); // 중복 방지
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
            sceneAUI.SetActive(true); // 강제 활성화
        }
    }


    // SceneC로 이동
    public void GoToSceneC()
    {
        if (sceneAUI != null)
        {
            sceneAUI.SetActive(false); // SceneA UI 비활성화
        }

        if (GameController.Instance != null)
        {
            GameController.Instance.LoadScene("SceneC"); // SceneC 로드
        }
    }

    // SceneB로 이동
    public void GoToSceneB()
    {
        if (sceneAUI != null)
        {
            sceneAUI.SetActive(false); // SceneA UI 비활성화
        }

        if (GameController.Instance != null)
        {
            GameController.Instance.LoadScene("SceneB"); // SceneB 로드
        }
    }

    // SceneA UI 복원 메서드
    public void RestoreUI()
    {
        if (sceneAUI != null)
        {
            sceneAUI.SetActive(true); // SceneA UI 활성화
        }
    }

    // SceneA로 돌아올 때 호출되는 메서드
    public void OnReturnToSceneA()
    {
        RestoreUI(); // UI 복원
    }
}
