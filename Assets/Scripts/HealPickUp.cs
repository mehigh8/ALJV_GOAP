using UnityEngine;
public class HealPickUp : MonoBehaviour, Interactable
{
    public float health;

    public void Interact(Interactor interactor)
    {
        if (interactor == null)
            return;

        GameManager gameManager = GameManager.GetInstance();

        if (interactor.isPlayer)
            gameManager.playerController.Heal(health);
        else
        {
            gameManager.goapAgent.heals.Remove(transform.position);
            gameManager.enemyController.Heal(health);
        }

        Destroy(gameObject);
    }
}