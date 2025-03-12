using UnityEngine;

public class StageSceneController : MonoBehaviour
{
    public void SelectBot(string botName)
    {
        // GameManager를 통해 봇 선택 및 인게임씬 로드
        GameManager.Instance.SelectBotAndGoInGame(botName);
    }

    public void ReturnToMain()
    {
        // GameManager를 통해 메인씬으로 돌아가기
        GameManager.Instance.OnReturnToMain();
    }
}
