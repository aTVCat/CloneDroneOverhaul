namespace OverhaulMod.Content
{
    public struct WorkshopItemVote
    {
        public bool VoteValue;

        public bool HasVoted;

        public WorkshopItemVote(Steamworks.GetUserItemVoteResult_t t)
        {
            VoteValue = t.m_bVotedUp;
            HasVoted = !t.m_bVoteSkipped && (t.m_bVotedUp || t.m_bVotedDown);
        }

        public WorkshopItemVote(Steamworks.GetUserItemVoteResult_t t, bool ioError)
        {
            VoteValue = t.m_bVotedUp;
            HasVoted = !ioError && !t.m_bVoteSkipped && (t.m_bVotedUp || t.m_bVotedDown);
        }
    }
}
