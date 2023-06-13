public class BaseBodyPart : ManagedBehaviour
{
	public bool CanBeDamaged;
	public MechBodyPartType PartType;
	public bool DestroyArrowsOnDamage;
	public bool CalculateDamageOnServerIfDetached;
	public bool RequireSameParentForArmorShatterCascade;
}
