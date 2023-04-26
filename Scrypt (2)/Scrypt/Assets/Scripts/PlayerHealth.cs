using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int startingHealth = 100;
    public int currentHealth;

    private void Start()
    {
        currentHealth = startingHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Player health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Debug.Log("Game over!");
            SceneManager.LoadScene("MainMenu");
        }
    }
}
