using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OxygenSystem : MonoBehaviour
{
    public static OxygenSystem Instance;
    
    [Header("Настройки")]
    public float maxOxygen = 100f;
    public float oxygenDepletionRate = 5f;
    public float oxygenRefillRate = 20f;
    
    [Header("UI")]
    public Slider oxygenSlider;
    
    private float currentOxygen;
    private int refillingCharacters = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            currentOxygen = maxOxygen;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // Обновление уровня кислорода
        float oxygenChange = refillingCharacters > 0 
            ? oxygenRefillRate * Time.deltaTime 
            : -oxygenDepletionRate * Time.deltaTime;
        
        currentOxygen = Mathf.Clamp(currentOxygen + oxygenChange, 0f, maxOxygen);
        
        // Обновление UI
        if (oxygenSlider != null)
        {
            oxygenSlider.value = currentOxygen / maxOxygen;
        }
        
        // Проверка на окончание кислорода
        if (currentOxygen <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    // Метод для начала пополнения (должен быть public)
    public void AddRefillingCharacter()
    {
        refillingCharacters++;
    }

    // Метод для остановки пополнения (должен быть public)
    public void RemoveRefillingCharacter()
    {
        refillingCharacters = Mathf.Max(0, refillingCharacters - 1);
    }
}