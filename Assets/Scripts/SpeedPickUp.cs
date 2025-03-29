using UnityEngine;

public class SpeedPickUp : MonoBehaviour, Interactable
{
    public float time;
    public void Interact(Interactor interactor)
    {
        if (interactor == null)
            return;

        GameManager gameManager = GameManager.GetInstance();
        gameManager.SpeedUp(time, interactor.isPlayer);

        Destroy(gameObject);
    }
}