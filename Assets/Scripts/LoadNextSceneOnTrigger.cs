using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNextSceneOnTrigger : MonoBehaviour
{
    [Tooltip("Тег объекта игрока")]
    public string playerTag = "Player";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Проверяем, если объект соприкосновения имеет нужный тег
        if (collision.CompareTag(playerTag))
        {
            LoadNextScene();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Альтернативная проверка для коллайдеров без триггера
        if (collision.gameObject.CompareTag(playerTag))
        {
            LoadNextScene();
        }
    }

    private void LoadNextScene()
    {
        // Получаем индекс текущей сцены
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        
        // Загружаем следующую сцену по порядку в Build Settings
        SceneManager.LoadScene(currentSceneIndex + 1);
    }
}