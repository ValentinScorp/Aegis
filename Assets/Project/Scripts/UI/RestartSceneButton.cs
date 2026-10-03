using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class RestartSceneButton : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(Restart);
    }

    private void Restart()
    {
        Debug.Log("restarting");
        Unpause();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    private void Unpause()
    {
        Time.timeScale = 1f;
    }
}