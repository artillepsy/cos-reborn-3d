namespace Game.Scripts.Gameplay.Shared.Skills
{
public interface ISkillState
{
	/// <summary> Current skill index in player's skills array </summary>
	public int SkillIndex { get; }
	/// <summary> Max skill reload time (primary) </summary>
	public float CooldownMax { get; }
	/// <summary> Current skill reload time. Starts from <exception cref="CooldownMax"></exception>> and reduces to 0 </summary>
	public float CooldownCurr { get; }
	/// <summary> Skill is ready when its cooldown has passed.  </summary>
	public bool IsReady { get; }
}
}