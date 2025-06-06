using UnityEngine;

public class ToggleObjectsOnS : MonoBehaviour
{
    public GameObject object1; // Первый объект (перетащи в инспекторе)
    public GameObject object2; // Второй объект (перетащи в инспекторе)

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            // Инвертируем активность объектов
            if (object1 != null) object1.SetActive(!object1.activeSelf);
            if (object2 != null) object2.SetActive(!object2.activeSelf);
        }
    }
}