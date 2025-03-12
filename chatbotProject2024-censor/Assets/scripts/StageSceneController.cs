using UnityEngine;

public class StageSceneController : MonoBehaviour
{
    public void SelectBot(string botName)
    {
        // GameManager�� ���� �� ���� �� �ΰ��Ӿ� �ε�
        GameManager.Instance.SelectBotAndGoInGame(botName);
    }

    public void ReturnToMain()
    {
        // GameManager�� ���� ���ξ����� ���ư���
        GameManager.Instance.OnReturnToMain();
    }
}
