using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public float health = 0.0f; // 100 health
    public float stamina = 0.0f; // 100 stamina

    [Header("UI")]
    public RectTransform healthBar;

    private float healthBarWidth;

    public static PlayerStats playerStats;

    private void Start(){
        health = 100.0f;
        stamina = 100.0f;

        healthBarWidth = healthBar.sizeDelta.x;
        UpdateHealth(-70);

        playerStats = this;
    }

    /// Adds addedHealth parameter to health
    public void UpdateHealth(float addedHealth){
        health = Mathf.Clamp(health + addedHealth, 0.0f, 100.0f);
        if(health == 0.0f)
            Die();

        // Update the health bar
        float p = health / 100.0f;
        healthBar.anchoredPosition = new Vector2(-(1-p) * healthBarWidth, healthBar.anchoredPosition.y);
    }
    
    /// Restarts the active scene
    public void Die(){
        SceneTransition.mainSceneTransition.ChangeScene(-1);
    }
}
