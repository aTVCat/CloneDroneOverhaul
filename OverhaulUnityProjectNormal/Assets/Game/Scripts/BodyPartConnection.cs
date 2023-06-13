using System;
using System.Collections.Generic;

[Serializable]
public class BodyPartConnection
{
	public MechBodyPart ChildBodyPart;
	public MechBodyPart ParentBodyPart;
	public List<int> PositionsInParent;
	public List<int> PositionsInChild;
}
