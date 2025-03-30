using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public PlayerController playerController;
    public EnemyController enemyController;
    public PickUpSpawner pickupSpawner;

    private static GameManager instance = null;
    private Coroutine playerSpeedCoroutine = null;
    private Coroutine enemySpeedCoroutine = null;

    public bool playerHasSword = false;
    public bool enemyHasSword = false;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    void Update()
    {
        
    }

    public static GameManager GetInstance() => instance;

    public void SpeedUp(float time, bool isPlayer)
    {
        if (isPlayer)
        {
            if (playerSpeedCoroutine != null)
                StopCoroutine(playerSpeedCoroutine);

            playerSpeedCoroutine = StartCoroutine(ApplySpeedUp(time, isPlayer));
        }
        else
        {
            if (enemySpeedCoroutine != null)
                StopCoroutine(enemySpeedCoroutine);

            enemySpeedCoroutine = StartCoroutine(ApplySpeedUp(time, isPlayer));
        }
    }

    private IEnumerator ApplySpeedUp(float time, bool isPlayer)
    {
        if (isPlayer)
            playerController.speedUp = 2f;
        else
            enemyController.characterController.speedUp = 2f;

        yield return new WaitForSeconds(time);

        if (isPlayer)
        {
            playerController.speedUp = 1f;
            playerSpeedCoroutine = null;
        }
        else
        {
            enemyController.characterController.speedUp = 1f;
            enemySpeedCoroutine = null;
        }
    }

    public SwordPickUp.Sword GainSword(SwordPickUp.Sword sword, bool isPlayer)
    {
        if (isPlayer)
            return playerController.GainSword(sword);

        return enemyController.GainSword(sword);
    }
}
