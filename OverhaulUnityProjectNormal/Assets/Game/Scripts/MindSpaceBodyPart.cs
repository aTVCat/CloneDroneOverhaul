using UnityEngine;

public class MindSpaceBodyPart : BaseBodyPart
{
	public MindSpaceBodyPart ParentBodyPart;
	public Material TakingDamageMaterial;
	public MinMaxRange WaitBeforeDestroy;
	public float DeathDetachWaitTime;
}
