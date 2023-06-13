using PicaVoxel;

public class MechBodyPart : BaseBodyPart
{
	public bool CanBeDamagedByEnvironment;
	public bool DisableHammerPropagation;
	public bool AutoSucceedCuttingForSpear;
	public bool ReduceHammerDamage;
	public bool IgnoreColorBurnForGlowingVoxels;
	public bool IsArtificialRoot;
	public bool OnlyConnectTopYToParent;
	public bool OnlyConnectBottomYToParent;
	public Volume ConnectedParentOverride;
	public BodyPartConnection ParentConnection;
	public float ParticleSizeOverride;
}
