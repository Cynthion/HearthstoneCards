using HearthstoneCards.Helper;

namespace HearthstoneCards.Model
{
    public enum Rarity
    {
        Free = 0,
        Common = 1,
        Rare = 2,
        Epic = 3,
        Legendary = 4
    }

    public enum Set
    {
        Basic = 0,
        Classic = 1,
        Naxxramas = 2,
        GoblinVsGnomes = 3,
        BlackrockMountain = 4,
        TheGrandTournament = 5,
        LeagueOfExplorers = 6
    }

    public enum Mechanic
    {
        [Description("No Mechanic")]
        None,
        [Description("Adjacent Buff")]
        AdjacentBuff,
        [Description("Affected By Spell Power")]
        AffectedBySpellPower,
        Aura,
        Battlecry,
        Charge,
        Combo,
        Deathrattle,
        [Description("Divine Shield")]
        DivineShield,
        Enrage,
        Freeze,
        [Description("Heal Target")]
        HealTarget,
        [Description("Immune To Spell Power")]
        ImmuneToSpellpower,
        Inspire,    
        Morph,
        [Description("One Turn Effect")]
        OneTurnEffect,
        Overload,
        Poisonous,
        Secret,
        Silence,
        Spellpower,
        Stealth,
        Summoned,
        Taunt,
        Windfury
    }

    public static class EnumHelper
    {
        public static string GetMechanicName(Mechanic mechanic)
        {
            switch (mechanic)
            {
                case Mechanic.None: 
                    return "No Mechanics";
                case Mechanic.AdjacentBuff:
                    return "Adjacent Buff";
                case Mechanic.AffectedBySpellPower:
                    return "Affected By Spell Power";
                case Mechanic.DivineShield:
                    return "Divine Shield";
                case Mechanic.HealTarget:
                    return "Heal Target";
                case Mechanic.ImmuneToSpellpower:
                    return "Immune To Spell Power";
                case Mechanic.OneTurnEffect:
                    return "One Turn Effect";
                default:
                    return mechanic.ToString();
            }
        }
    }
}
