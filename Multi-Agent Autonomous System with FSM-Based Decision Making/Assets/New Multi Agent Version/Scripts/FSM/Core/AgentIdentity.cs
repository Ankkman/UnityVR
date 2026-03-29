using UnityEngine;

public class AgentIdentity : MonoBehaviour
{
    [Header("Faction Assignment")]
    public FactionType Faction;

    [Header("Display")]
    public string agentName = "Agent";

    [HideInInspector] public AwarenessModel Awareness;
    [HideInInspector] public IAgentCombat Combat;
    [HideInInspector] public NavigationController Navigation;

    private void Awake()
    {
        Awareness = GetComponent<AwarenessModel>();
        Navigation = GetComponent<NavigationController>();

        CombatController realCombat = GetComponent<CombatController>();
        if (realCombat != null)
        {
            Combat = realCombat;
        }
        else
        {
            CombatStub stub = GetComponent<CombatStub>();
            if (stub != null) Combat = stub;
        }

        // Always use GameObject name if agentName wasn't manually set
        if (string.IsNullOrEmpty(agentName) || agentName == "Agent")
            agentName = gameObject.name;
    }

    private void Start()
    {
        if (FactionManager.Instance != null)
            FactionManager.Instance.Register(this);
    }

    private void OnDestroy()
    {
        if (FactionManager.Instance != null)
            FactionManager.Instance.Unregister(this);
    }

    public bool IsAlive => Combat == null || !Combat.IsDead();
}