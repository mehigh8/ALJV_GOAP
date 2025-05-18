using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class Helpers
{
    [System.Serializable]
    public class HealthBar
    {
        public float maxHealth;
        public float currentHealth;
        public Slider healthBar;

        public HealthBar(float maxHealth, float currentHealth, Slider healthBar)
        {
            this.maxHealth = maxHealth;
            this.currentHealth = currentHealth;
            this.healthBar = healthBar;
        }

        private void UpdateHealthBar()
        {
            healthBar.value = currentHealth / maxHealth;
        }

        public void TakeDamage(float damage)
        {
            currentHealth -= damage;
            if (currentHealth < 0)
                currentHealth = 0;

            UpdateHealthBar();
        }

        public void Heal(float health)
        {
            currentHealth += health;
            if (currentHealth > maxHealth)
                currentHealth = maxHealth;

            UpdateHealthBar();
        }
    }

    public class CountdownTimer
    {
        private float initialTime;
        private float currentTime;

        public Action OnTimerStop = delegate { };
        public Action OnTimerStart = delegate { };

        public CountdownTimer(float initialTime)
        {
            this.initialTime = initialTime;
        }

        public void Start()
        {
            currentTime = initialTime;
            OnTimerStart.Invoke();
        }

        public void Stop()
        {
            currentTime = 0;
            OnTimerStop.Invoke();
        }

        public void Update(float deltaTime)
        {
            if (currentTime <= 0)
                return;

            currentTime -= deltaTime;
            if (currentTime <= 0)
                OnTimerStop.Invoke();
        }
    }


    public static int[] neswX = { 0, 1, 0, -1 };
    public static int[] neswY = { 1, 0, -1, 0 };

    public static Vector3Int ConvertToInt(Vector3 v)
    {
        return new Vector3Int((int)v.x, (int)v.y, (int)v.z);
    }

    public static Vector3Int SnapToGrid(Vector3 position)
    {
        return ConvertToInt(new Vector3(Mathf.Round(position.x), Mathf.Round(position.y), Mathf.Round(position.z)));
    }

    public static (int, int) ScaleToMap(Vector3Int worldPos, Vector2Int mapSize)
    {
        return (worldPos.x + mapSize.x / 2, mapSize.y / 2 - worldPos.y);
    }

    public static Vector3Int ScaleToWorld((int, int) mapPosition, Vector2Int mapSize)
    {
        return new Vector3Int(mapPosition.Item1 - mapSize.x / 2, mapSize.y / 2 - mapPosition.Item2, 0);
    }

    public static int Bool2Int(bool b)
    {
        return b ? 1 : 0;
    }

    public static T GetRandomElement<T>(List<T> list)
    {
        if (list == null || list.Count == 0)
            return default;

        return list[UnityEngine.Random.Range(0, list.Count)];
    }
}
