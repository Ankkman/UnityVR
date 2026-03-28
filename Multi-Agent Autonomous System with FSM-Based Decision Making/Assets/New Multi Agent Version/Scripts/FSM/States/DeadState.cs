using UnityEngine;

public class DeadState : BaseState<AgentState>
{
    private AgentFSM fsm;

    public DeadState(AgentFSM fsm) : base(AgentState.Dead)
    {
        this.fsm = fsm;
    }

    public override void EnterState()
    {
        Debug.Log($"[STATE] {fsm.gameObject.name} → DEAD");

        // Stop combat
        if (fsm.Identity.Combat != null)
            fsm.Identity.Combat.StopAttack();

        // Stop navigation
        fsm.Navigation.DisableCompletely();

        // Disable perception
        SensorSystem sensor = fsm.GetComponent<SensorSystem>();
        if (sensor != null) sensor.enabled = false;

        // Stop locomotion animation
        AgentAnimator agentAnim = fsm.GetComponentInChildren<AgentAnimator>();
        if (agentAnim != null) agentAnim.StopAll();

        // Death animation is handled by CombatController.HandleDeath() 
        // which calls CharacterAnimatorBridge.PlayDeath()
        // So we don't need to trigger it here

        // Force black indicator
        Transform indicator = fsm.transform.Find("StateIndicator");
        if (indicator != null)
        {
            Renderer r = indicator.GetComponent<Renderer>();
            if (r != null) r.material.color = Color.black;
        }

        // Disable FSM
        fsm.StartCoroutine(DisableAfterDelay());
    }

    private System.Collections.IEnumerator DisableAfterDelay()
    {
        yield return new WaitForSeconds(0.1f);
        fsm.enabled = false;
    }

    public override void ExitState() { }
    public override void UpdateState() { }
    public override AgentState GetNextState() => AgentState.Dead;
}