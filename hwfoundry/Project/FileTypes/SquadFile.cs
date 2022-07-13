using System;
using System.Collections.Generic;

namespace SMHEditor.Project.FileTypes
{
    public class Squad
    {
        public string PortraitIcon { get; set; }
        public SquadMinimapIcon MinimapIcon { get; set; }
        public string MinimapScale { get; set; }
        public string DisplayNameID { get; set; }
        public string RolloverTextID { get; set; }
        public string RoleTextID { get; set; }
        public string PrereqTextID { get; set; }
        public int BuildPoints { get; set; }
        public List<SquadCost> Cost { get; set; }
        public string SubSelectSort { get; set; }
        public SquadHPBar HPBar { get; set; }
        public AbilityRecoveryBar AbilityRecoveryBar { get; set; }
        public VeterancyBar VeterancyBar { get; set; }
        public Units Units { get; set; }
        public List<string> Flag { get; set; }
        public Selection Selection { get; set; }
        public string Name { get; set; }
        public string Dbid { get; set; }
        public string StatsNameID { get; set; }
        public string LeashDistance { get; set; }
        public string LeashDeadzone { get; set; }
        public string LeashRecallDelay { get; set; }
        public string AggroDistance { get; set; }
        public Birth Birth { get; set; }
        public string DazeResist { get; set; }
        public string CryoPoints { get; set; }
        public TurnRadius TurnRadius { get; set; }
        public List<SquadSound> Sound { get; set; }
        public string FormationType { get; set; }
        public string BobbleHead { get; set; }
        public string CanAttackWhileMoving { get; set; }
        public string Update { get; set; }
    }

    public class SquadMinimapIcon
    {
        public string Size { get; set; }
        public string Text { get; set; }
    }

    public class SquadCost
    {
        public string Resourcetype { get; set; }
        public string Text { get; set; }
    }

    public class SquadHPBar
    {
        public string Offset { get; set; }
        public string Text { get; set; }
        public string SizeX { get; set; }
        public string SizeY { get; set; }
    }

    public class AbilityRecoveryBar
    {
        public string Centered { get; set; }
        public string Value { get; set; }
    }

    public class VeterancyBar
    {
        public string Centered { get; set; }
        public string Value { get; set; }
    }

    public class Unit
    {
        public string Count { get; set; }
        public string Role { get; set; }
        public string Value { get; set; }
    }

    public class Units
    {
        public List<Unit> Unit { get; set; }
    }

    public class Selection
    {
        public string ConformToTerrain { get; set; }
        public string AllowOrientation { get; set; }
    }

    public enum BirthType
    {
        Trained,
        FlyIn
    }
    public class Birth
    {
        public string TrainerAnim { get; set; }
        public string Spawnpoint { get; set; }
        public BirthType Value { get; set; }
        public string EndPoint { get; set; }
        public string Anim0 { get; set; }
        public string Anim1 { get; set; }
        public string Anim2 { get; set; }
        public string Anim3 { get; set; }
    }

    public class TurnRadius
    {
        public float Min { get; set; }
        public float Max { get; set; }
    }

    public class SquadSound
    {
        public string Type { get; set; }
        public string Text { get; set; }
        public string World { get; set; }
        public string Squad { get; set; }
        public string CastingUnitOnly { get; set; }
    }

}