using UnityEngine;

public class SwordPickUp : MonoBehaviour, Interactable
{
    [System.Serializable]
    public class Sword
    {
        public float damage;
        public float range;
        public int durability;

        public Sword(int damage, int durability)
        {
            this.damage = damage;
            this.durability = durability;
        }
    }

    public Sword sword;

    public void Interact(Interactor interactor)
    {
        if (interactor == null)
            return;

        GameManager gameManager = GameManager.GetInstance();

        if (interactor.isPlayer)
        {
            if (gameManager.playerHasSword)
                return;

            gameManager.GainSword(sword, interactor.isPlayer);

            Destroy(gameObject);
        }
        else
        {
            if (gameManager.enemyHasSword)
                return;

            gameManager.GainSword(sword, interactor.isPlayer);

            Destroy(gameObject);
        }
    }
}