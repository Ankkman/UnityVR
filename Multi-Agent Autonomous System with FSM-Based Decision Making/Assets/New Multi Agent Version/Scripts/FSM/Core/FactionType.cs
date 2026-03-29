// ============================================================================
// FactionType.cs
// Defines the factions (teams) in the simulation.
// Used by FactionManager to determine ally/enemy relationships.
// Currently two factions for the prototype.
// Easily extensible — just add more enum values for more teams.
// ============================================================================

/// <summary>
/// Identifies which team/faction an agent belongs to.
/// Agents of the same faction are allies.
/// Agents of different factions are enemies.
/// </summary>
public enum FactionType
{
    /// <summary>
    /// First faction — will patrol one area of the map.
    /// </summary>
    TeamAlpha,

    /// <summary>
    /// Second faction — will patrol another area of the map.
    /// Enemies of TeamAlpha.
    /// </summary>
    TeamBravo
}