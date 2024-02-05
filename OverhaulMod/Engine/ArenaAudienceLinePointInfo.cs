namespace OverhaulMod.Engine
{
    public class ArenaAudienceLinePointInfo
    {
        public int ID;

        public bool IsEnd;

        public bool IsLower;

        public bool IsMiddle;

        public bool IsTop;

        public bool IsVIP;

        public float PositionX, PositionY, PositionZ;

        public float EulerAnglesX, EulerAnglesY, EulerAnglesZ;

        public ArenaAudienceLinePointInfo()
        {

        }

        public ArenaAudienceLinePointInfo(ArenaAudienceLinePoint arenaAudienceLinePoint)
        {
            ID = arenaAudienceLinePoint.ID;
            IsEnd = arenaAudienceLinePoint.IsEnd;
            IsLower = arenaAudienceLinePoint.IsLower;
            IsMiddle = arenaAudienceLinePoint.IsMiddle;
            IsTop = arenaAudienceLinePoint.IsTop;
            IsVIP = arenaAudienceLinePoint.IsVIP;
            PositionX = arenaAudienceLinePoint.transform.position.x;
            PositionY = arenaAudienceLinePoint.transform.position.y;
            PositionZ = arenaAudienceLinePoint.transform.position.z;
            EulerAnglesX = arenaAudienceLinePoint.transform.eulerAngles.x;
            EulerAnglesY = arenaAudienceLinePoint.transform.eulerAngles.y;
            EulerAnglesZ = arenaAudienceLinePoint.transform.eulerAngles.z;
        }
    }
}
