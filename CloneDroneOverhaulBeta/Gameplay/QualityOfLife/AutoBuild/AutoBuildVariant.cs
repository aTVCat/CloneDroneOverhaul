namespace CDOverhaul.Gameplay.QualityOfLife
{
    public enum AutoBuildVariant
    {
        /// <summary>
        /// Auto-Build disabled
        /// </summary>
        None,

        /// <summary>
        /// Random build
        /// </summary>
        Random,

        /// <summary>
        /// Sword unlock + Fire sword 1 +  Fire sword 2
        /// </summary>
        FullSword,
        /// <summary>
        /// Bow unlock + Fire arrows 1 + Arrow width 1
        /// </summary>
        FullBow,
        /// <summary>
        /// Hammer unlock + Hammer size 1 + Hammer size 2
        /// </summary>
        FullHammer,
        /// <summary>
        /// Spear unlock + Shield + Fire spear
        /// </summary>
        FullSpear,

        /// <summary>
        /// Kick + Power kick + Energy capacity 1 + Energy capacity 2
        /// </summary>
        Kicker,
        /// <summary>
        /// Kick + Get up  + Energy capacity 1 + Energy capacity 2
        /// </summary>
        LightKicker, // Kick + Get up + energy capacities (1 and 2)
        /// <summary>
        /// Kick + Armor
        /// </summary>
        ArmoredKicker,
        /// <summary>
        /// Kick + Energy capacity 1 + Energy capacity 2 +  Energy capacity 3
        /// </summary>
        KickNDash,
        /// <summary>
        /// Kick + Jetpack + Energy capacity 1
        /// </summary>
        KickNRun,

        /// <summary>
        /// Sword unlock + Energy capacity 1 + Kick 1
        /// </summary>
        TacticalSword,
        /// <summary>
        /// Sword unlock + Fire sword 1 + Energy capacity 1
        /// </summary>
        TacticalFireSword,
        /// <summary>
        /// Sword unlock + Jetpack
        /// </summary>
        JetPackSword,

        /// <summary>
        /// Bow unlock + Arrow width 1 + Energy capacity 1 + Kick
        /// </summary>
        TacticalBow,
        /// <summary>
        /// Bow unlock + Fire arrows 1 + Energy capacity 1 + Kick
        /// </summary>
        TacticalFireBow,

        /// <summary>
        /// Spear unlock + Fire spear + Energy capacity 1
        /// </summary>
        TacticalFireSpear,
        /// <summary>
        /// Spear unlock + Fire spear + Kick
        /// </summary>
        TacticalFireSpearKicker,

        /// <summary>
        /// Hammer unlock + Hammer size 1 + Energy capacity 1
        /// </summary>
        TacticalHammer,
        /// <summary>
        /// Hammer unlock + Hammer size 1 + Kick
        /// </summary>
        TacticalHammerKicker,

        /// <summary>
        /// Energy capacity 1 + Energy capacity 2 +  Energy capacity 3 + Energy recharge 1
        /// </summary>
        Spectator,

        /// <summary>
        /// Sword unlock + Energy capacity 1 + Energy capacity 2
        /// </summary>
        EnergySword,
    }
}