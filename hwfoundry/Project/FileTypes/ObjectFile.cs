using System.Collections.Generic;
using System.ComponentModel;

namespace SMHEditor.Project.FileTypes
{
    public class ObjectFile : CustomTypeDescriptor
    {
        public string Visual { get; set; }
        public string Name { get; set; }
        public string Derived { get; set; }
        public string ObjectClass { get; set; }
        public List<string> Flag { get; set; }
        public List<Sound> Sound { get; set; }
        public string Hitpoints { get; set; }
        public string MovementType { get; set; }
        public float FlightLevel { get; set; }
        public List<string> ObjectType { get; set; }
        public List<string> ObjectChild { get; set; }
        public string DisplayNameID { get; set; }
        public string RolloverTextID { get; set; }
        public string SelectedRadiusX { get; set; }
        public string SelectedRadiusZ { get; set; }
        public float PickRadius { get; set; }
        public string PickOffset { get; set; }
        public string PickPriority { get; set; }
        public int LOS { get; set; }
        public string ObstructionRadiusX { get; set; }
        public string ObstructionRadiusY { get; set; }
        public string ObstructionRadiusZ { get; set; }
        public string Velocity { get; set; }
        public string Lifespan { get; set; }
        public string MaxProjectileHeight { get; set; }
        public ImpactDecal ImpactDecal { get; set; }
        public string Tactics { get; set; }
        public string PhysicsReplacementInfo { get; set; }
        public string TrackingDelay { get; set; }
        public string TrackInterceptDistance { get; set; }
        public string StartingVelocity { get; set; }
        public string Acceleration { get; set; }
        public string Fuel { get; set; }
        public string TurnRate { get; set; }
        public string PerturbanceChance { get; set; }
        public string PerturbanceVelocity { get; set; }
        public string PerturbanceMinTime { get; set; }
        public string PerturbanceMaxTime { get; set; }
        public PerturbInitialVelocity PerturbInitialVelocity { get; set; }
        public string BeamHead { get; set; }
        public string BeamTail { get; set; }
        public string LifeSpan { get; set; }
        public string VisualDisplayPriority { get; set; }
        public string PhysicsInfo { get; set; }
        public string Bounty { get; set; }
        public string RoleTextID { get; set; }
        public MinimapColor MinimapColor { get; set; }
        public string ResourceAmount { get; set; }
        public string SelectType { get; set; }
        public string SurfaceType { get; set; }
        public string DeathFadeDelayTime { get; set; }
        public string PortraitIcon { get; set; }
        public MinimapIcon MinimapIcon { get; set; }
        public string StatsNameID { get; set; }
        public List<string> CombatValue { get; set; }
        public string BuildPoints { get; set; }
        public PopCapAddition PopCapAddition { get; set; }
        public List<Cost> Cost { get; set; }
        public List<Veterancy> Veterancy { get; set; }
        public string GotoType { get; set; }
        public string ExitFromDirection { get; set; }
        public List<Command> Command { get; set; }
        public ChildObjects ChildObjects { get; set; }
        public List<DamageType> DamageType { get; set; }
        public Socket Socket { get; set; }
        public string ChildObjectDamageTakenScalar { get; set; }
        public string BuildingStrengthDisplay { get; set; }
        public FlashUI FlashUI { get; set; }
        public HPBar HPBar { get; set; }
        public List<SquadModeAnim> SquadModeAnim { get; set; }
        public string DeathFadeTime { get; set; }
        public string GathererLimit { get; set; }
        public string AIAssetValueAdjust { get; set; }
        public string CostEscalation { get; set; }
        public List<string> CostEscalationObject { get; set; }
        public AddResource AddResource { get; set; }
        public string TerrainHeightTolerance { get; set; }
        public Rate Rate { get; set; }
        public AutoParkingLot AutoParkingLot { get; set; }
        public string BuildRotation { get; set; }
        public string BuildOffset { get; set; }
        public string UIVisual { get; set; }
        public string RallyPoint { get; set; }
        public string PrereqTextID { get; set; }
        public List<Hardpoint> Hardpoint { get; set; } = new List<Hardpoint>();
        public string AttackGradeDPS { get; set; }
        public string DazeResist { get; set; }
        public string TrueLOSHeight { get; set; }
        public Pop Pop { get; set; }
        public string RamDodgeFactor { get; set; }
        public string LevelUpEffect { get; set; }
        public string RepairPoints { get; set; }
        public string SubSelectSort { get; set; }
        public string ExtendedSoundBank { get; set; }
        public string AbilityCommand { get; set; }
        public string SingleBoneIK { get; set; }
        public string AmmoMax { get; set; }
        public string AmmoRegenRate { get; set; }
        public string Shieldpoints { get; set; }
        public string ReverseSpeed { get; set; }
        public string NumStasisFieldsToStop { get; set; }
        public string Contain { get; set; }
        public List<string> MaxContained { get; set; }
        public TrainerType TrainerType { get; set; }
        public string ShieldType { get; set; }
        public string Physicsinfo { get; set; }
        public string Ability { get; set; }
        public string DeathReplacement { get; set; }
        public string Power { get; set; }
        public string RevealRadius { get; set; }
        public List<GroundIK> GroundIK { get; set; }
        public SweetSpotIK SweetSpotIK { get; set; }
        public string MaxVelocity { get; set; }
        public string GarrisonTime { get; set; }
        public string TargetBeam { get; set; }
        public string KillBeam { get; set; }
        public string MinimapIconName { get; set; }
        public string Update { get; set; }
        public string ShieldPoints { get; set; }
        public DeathSpawnSquad DeathSpawnSquad { get; set; }
    }

    public class Sound
    {
        public string Type { get; set; }
        public string Text { get; set; }
        public string Action { get; set; }
        public string Squad { get; set; }
    }

    public class ImpactDecal
    {
        public string SizeX { get; set; }
        public string SizeZ { get; set; }
        public string TimeFullyOpaque { get; set; }
        public string FadeOutTime { get; set; }
        public string Orientation { get; set; }
        public string Text { get; set; }
    }

    public class PerturbInitialVelocity
    {
        public string MinTime { get; set; }
        public string MaxTime { get; set; }
        public string Text { get; set; }
    }

    public class MinimapColor
    {
        public string Red { get; set; }
        public string Green { get; set; }
        public string Blue { get; set; }
    }

    public class MinimapIcon
    {
        public string Size { get; set; }
        public string Text { get; set; }
    }

    public class PopCapAddition
    {
        public string Type { get; set; }
        public string Text { get; set; }
    }

    public class Cost
    {
        public string ResourceType { get; set; }
        public string Text { get; set; }
    }

    public class Veterancy
    {
        public string Level { get; set; }
        public string XP { get; set; }
        public string Damage { get; set; }
        public string Velocity { get; set; }
        public string Accuracy { get; set; }
        public string WorkRate { get; set; }
        public string WeaponRange { get; set; }
        public string DamageTaken { get; set; }
    }

    public class Command
    {
        public string Type { get; set; }
        public string Position { get; set; }
        public string Text { get; set; }
        public string AutoClose { get; set; }
    }

    public class ChildObjects
    {
        public List<ObjectFile> Object { get; set; }
    }

    public class DamageType
    {
        public string Direction { get; set; }
        public string Text { get; set; }
        public string Mode { get; set; }
    }

    public class Socket
    {
        public string Player { get; set; }
        public string Text { get; set; }
    }

    public class FlashUI
    {
        public string CircleMenuFrameIDNormal { get; set; }
        public string CircleMenuFrameIDHighlight { get; set; }
    }

    public class HPBar
    {
        public string Offset { get; set; }
        public string Text { get; set; }
    }

    public class SquadModeAnim
    {
        public string Mode { get; set; }
        public string Text { get; set; }
    }

    public class AddResource
    {
        public string Amount { get; set; }
        public string Text { get; set; }
    }

    public class Rate
    {
        public string _Rate { get; set; }
        public string Text { get; set; }
    }

    public class AutoParkingLot
    {
        public string Offset { get; set; }
        public string Rotation { get; set; }
        public string Text { get; set; }
    }

    public class Hardpoint
    {
        public string Name { get; set; }
        public string Autocenter { get; set; }
        public string Yawrate { get; set; }
        public string Pitchrate { get; set; }
        public string PitchMinAngle { get; set; }
        public string PitchMaxAngle { get; set; }
        public string Yawattachment { get; set; }
        public string Pitchattachment { get; set; }
        public string Singleboneik { get; set; }
        public string RelativeToUnit { get; set; }
        public string YawMaxAngle { get; set; }
        public string InfiniteRateWhenHasTarget { get; set; }
        public string StartYawSound { get; set; }
        public string StopYawSound { get; set; }
        public string Yawmaxangle { get; set; }
        public string Pitchmaxangle { get; set; }
        public string UseYawAndPitchAsTolerance { get; set; }
        public string Pitchminangle { get; set; }
        public string Combined { get; set; }
    }

    public class Pop
    {
        public string Type { get; set; }
        public string Text { get; set; }
    }

    public class TrainerType
    {
        public string ApplyFormation { get; set; }
        public string Text { get; set; }
    }

    public class GroundIK
    {
        public string IkRange { get; set; }
        public string LinkCount { get; set; }
        public string X { get; set; }
        public string Z { get; set; }
        public string Text { get; set; }
    }

    public class SweetSpotIK
    {
        public string LinkCount { get; set; }
        public string Text { get; set; }
    }

    public class DeathSpawnSquad
    {
        public string CheckPos { get; set; }
        public string MaxPopCount { get; set; }
        public string Text { get; set; }
    }

}
