using HearthstoneCards.Helper;

namespace HearthstoneCards.Model
{
    public enum Rarity
    {
        Free,
        Common,
        Rare,
        Epic,
        Legendary
    }

    public enum Set
    {
        Basic,
        Classic,
        Naxxramas,
        GoblinVsGnomes,
        BlackrockMountain,
        TheGrandTournament,
        LeagueOfExplorers
    }

    public enum Mechanic
    {
        None,
        AdjacentBuff,
        AffectedBySpellPower,
        Aura,
        Battlecry,
        Charge,
        Combo,
        Deathrattle,
        DivineShield,
        Enrage,
        Freeze,
        HealTarget,
        ImmuneToSpellpower,
        Inspire,    
        Morph,
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

    public enum CardType
    {
        Hero,
        HeroPower,
        Minion,
        Spell,
        Enchantment,
        Weapon,
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
