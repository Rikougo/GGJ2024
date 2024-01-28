using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button m_playButton;

    private void OnEnable()
    {
        m_playButton.onClick.AddListener(this.StartGame);
    }

    private void OnDisable()
    {
        m_playButton.onClick.RemoveListener(this.StartGame);
    }

    private void StartGame()
    {
        SceneManager.LoadScene("Level");
    }
}
