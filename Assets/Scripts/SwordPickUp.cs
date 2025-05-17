using UnityEngine;
using TMPro;

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

    public TMP_Text durabilityText;
    public Sword sword;

    private void Start()
    {
        durabilityText.text = sword.durability.ToString();
    }

    public void Interact(Interactor interactor)
    {
        if (interactor == null)
            return;

        GameManager gameManager = GameManager.GetInstance();

        Sword changedSword = gameManager.GainSword(sword, interactor.isPlayer);

        if (changedSword == null || changedSword.durability <= 0)
        {
            if (!interactor.isPlayer)
                gameManager.goapAgent.swords.Remove(transform.position);

            Destroy(gameObject);
        }
        else
        {
            sword = changedSword;
            durabilityText.text = sword.durability.ToString();
        }
    }
}