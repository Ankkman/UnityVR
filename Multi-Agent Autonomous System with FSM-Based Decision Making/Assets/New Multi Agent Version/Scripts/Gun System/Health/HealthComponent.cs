using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;

    public HealthModel Model { get; private set; }

    public System.Action OnDeathEvent;

    public bool IsHealing => Model.IsHealing;//to check healing state in my state machine

    private void Awake()
    {
        Model = new HealthModel(maxHealth);
        Model.OnDeath += HandleDeath;
    }

    public void TakeDamage(int amount)
    {
        Model.TakeDamage(amount);
        Debug.Log(gameObject.name + " HP: " + Model.CurrentHealth);

    }

    public void StartHeal(int amount, float duration)
    {
        if (Model.IsDead) return;

        StartCoroutine(HealRoutine(amount, duration));
    }

    private System.Collections.IEnumerator HealRoutine(int amount, float duration)
    {
        Model.StartHeal();

        yield return new WaitForSeconds(duration);

        Model.FinishHeal(amount);
    }


    private void HandleDeath()
    {
        OnDeathEvent?.Invoke();
    }
}
