using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Menu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI sensibilityAmount;
    [SerializeField] private float sensibility;
    private void Start() {
        sensibilityAmount.text = 10.ToString();
        sensibility = 10f;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
    public void LoadGame()
    {
        SceneManager.LoadScene("Demo_Scene_Recursive");
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void AddSense()
    {
        sensibility += 1;
        sensibilityAmount.text = sensibility.ToString();
        GameManager.GetGameManager().SetSensibility(sensibility);
    }
    public void MinusSense()
    {
        sensibility -= 1;
        sensibilityAmount.text = sensibility.ToString();
        GameManager.GetGameManager().SetSensibility(sensibility);
    }

}
