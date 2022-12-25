using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

//TODO: replace this
using OpenTK.Input;

using Newtonsoft.Json;
using System.Xml.Linq;
using System.IO;
using WeifenLuo.WinFormsUI.Docking;
using static Foundry.FoundryInstance;

namespace Foundry.Project.Modules.Triggerscripter
{
    public class Input
    {
        public string name;
        public string valueType;
        public bool optional;
        public int sigId;
    }
    public class Output
    {
        public string name;
        public string valueType;
        public bool optional;
        public int sigId;
    }
    public class Effect
    {
        public string name;
        public List<Input> inputs = new List<Input>();
        public List<Output> outputs = new List<Output>();
        public List<string> sources = new List<string>();
        public int dbid;
        public int version;
    }
    public class Condition
    {
        public string name;
        public List<Input> inputs = new List<Input>();
        public List<Output> outputs = new List<Output>();
        public int dbid;
        public int version;
    }


    public class SerializedVariable
    {
        public string name;
        public string value;
        public string type;
    }
    public class SerializedTrigger
    {
        public string name;
        public bool cndIsOr;
        public bool active;
    }
    public class SerializedNodeLink
    {
        public string sourceType;
        public string sourceSocketName;
        public int sourceId;

        public int targetId;
        public string targetType;
        public string targetSocketName;
    }
    public class SerializableNode
    {
        public int id;
        public int x, y;
        public string handleAs;
        public bool selected;
        public SerializedTrigger trigger;
        public SerializedVariable variable;
        public Effect effect;
        public Condition condition;
    }
    public class SerializedTriggerscripter
    {
        public int lastTrg, lastVar, lastEff, lastCnd;
        public List<SerializableNode> nodes = new List<SerializableNode>();
        public List<SerializedNodeLink> links = new List<SerializedNodeLink>();
    }


    public class TriggerscriptEditorPage : FoundryPage
    {
        private static Dictionary<string, string> nodeSubCategories = new Dictionary<string, string>()
        {
            {"AISquadAnalysisVar", "Page 1"},
            {"AISquadAnalysisComponentVar", "Page 1"},
            {"AnimTypeVar", "Page 1"},
            {"BidStateVar", "Page 1"},
            {"BidTypeVar", "Page 1"},
            {"BoolVar", "Page 1"},
            {"BuildingCommandStateVar", "Page 1"},
            {"ChatSpeakerVar", "Page 1"},
            {"CinematicVar", "Page 1"},
            {"CivVar", "Page 1"},
            {"ColorVar", "Page 1"},
            {"CommandTypeVar", "Page 1"},
            {"ConceptVar", "Page 1"},
            {"ConceptListVar", "Page 1"},
            {"ControlTypeVar", "Page 1"},
            {"CostVar", "Page 1"},
            {"DataScalarVar", "Page 1"},
            {"DesignLineVar", "Page 1"},
            {"DesignLineListVar", "Page 1"},
            {"DifficultyVar", "Page 1"},
            {"DiplomacyVar", "Page 1"},
            {"EntityVar", "Page 1"},
            {"EntityFilterSetVar", "Page 1"},
            {"EventTypeVar", "Page 1"},
            {"ExposedScriptVar", "Page 1"},
            {"FlareTypeVar", "Page 1"},
            {"FlashableUIItemVar", "Page 1"},
            {"FloatVar", "Page 1"},
            {"FloatListVar", "Page 1"},
            {"GameStatePredicateVar", "Page 1"},
            {"GroupVar", "Page 1"},
            {"HUDItemVar", "Page 1"},
            {"IconTypeVar", "Page 2"},
            {"IntegerVar", "Page 2"},
            {"IntegerListVar", "Page 2"},
            {"IteratorVar", "Page 2"},
            {"KBBaseVar", "Page 2"},
            {"KBBaseListVar", "Page 2"},
            {"KBBaseQueryVar", "Page 2"},
            {"KBSquadFilterSetVar", "Page 2"},
            {"KBSquadListVar", "Page 2"},
            {"KBSquadQueryVar", "Page 2"},
            {"LeaderVar", "Page 2"},
            {"ListPositionVar", "Page 2"},
            {"LocationVar", "Page 2"},
            {"LocStringIDVar", "Page 2"},
            {"LOSTypeVar", "Page 2"},
            {"MathOperatorVar", "Page 2"},
            {"MessageIndexVar", "Page 2"},
            {"MessageJustifyVar", "Page 2"},
            {"MessagePointVar", "Page 2"},
            {"MissionStateVar", "Page 2"},
            {"MissionTargetTypeVar", "Page 2"},
            {"MissionTypeVar", "Page 2"},
            {"ObjectVar", "Page 2"},
            {"ObjectDataRelativeVar", "Page 2"},
            {"ObjectDataTypeVar", "Page 2"},
            {"ObjectiveVar", "Page 2"},
            {"ObjectListVar", "Page 2"},
            {"ObjectTypeVar", "Page 2"},
            {"ObjectTypeListVar", "Page 2"},
            {"OperatorVar", "Page 2"},
            {"PercentVar", "Page 2"},
            {"PlacementRuleVar", "Page 3"},
            {"PlayerVar", "Page 3"},
            {"PlayerListVar", "Page 3"},
            {"PlayerStateVar", "Page 3"},
            {"PowerVar", "Page 3"},
            {"ProtoObjectVar", "Page 3"},
            {"ProtoObjectListVar", "Page 3"},
            {"ProtoSquadVar", "Page 3"},
            {"ProtoSquadListVar", "Page 3"},
            {"RefCountTypeVar", "Page 3"},
            {"RumbleTypeVar", "Page 3"},
            {"SoundVar", "Page 3"},
            {"SquadVar", "Page 3"},
            {"SquadDataTypeVar", "Page 3"},
            {"SquadFlagVar", "Page 3"},
            {"SquadListVar", "Page 3"},
            {"SquadModeVar", "Page 3"},
            {"StringVar", "Page 3"},
            {"TalkingHeadVar", "Page 3"},
            {"TeamVar", "Page 3"},
            {"TeamListVar", "Page 3"},
            {"TechVar", "Page 3"},
            {"TechListVar", "Page 3"},
            {"TechStatusVar", "Page 3"},
            {"TimeVar", "Page 3"},
            {"UIButtonVar", "Page 3"},
            {"UILocationVar", "Page 3"},
            {"UILocationMinigameVar", "Page 3"},
            {"UISquadVar", "Page 3"},
            {"UISquadListVar", "Page 3"},
            {"UnitVar", "Page 4"},
            {"UnitFlagVar", "Page 4"},
            {"UnitListVar", "Page 4"},
            {"UserClassTypeVar", "Page 4"},
            {"VectorVar", "Page 4"},
            {"VectorListVar", "Page 4"},
            {"AIAnalyzeKBSquadListEff", "AI"},
            {"AIAnalyzeOffenseAToBEff", "AI"},
            {"AIAnalyzeProtoSquadListEff", "AI"},
            {"AIAnalyzeSquadListEff", "AI"},
            {"AIBindLogEff", "AI"},
            {"AICalculateOffenseRatioAToBEff", "AI"},
            {"AIClearOpportunityRequestsEff", "AI"},
            {"AICreateAreaTargetEff", "AI"},
            {"AICreateTargetWrapperEff", "AI"},
            {"AIDeleteCTFMissionEff", "AI|Mission"},
            {"AIFactoidSubmitEff", "AI"},
            {"AIGetAlertDataEff", "AI"},
            {"AIGetAttackAlertsEff", "AI"},
            {"AIGetFlareAlertsEff", "AI"},
            {"AIGetLastAttackAlertEff", "AI"},
            {"AIGetLastFlareAlertEff", "AI"},
            {"AIGetMemoryEff", "AI"},
            {"AIGetMissionsEff", "AI|Mission"},
            {"AIGetMissionTargetsEff", "AI|Mission"},
            {"AIGetTerminalMissionsEff", "AI|Mission"},
            {"AIMissionAddSquadsEff", "AI|Mission"},
            {"AIMissionCancelEff", "AI|Mission"},
            {"AIMissionCreateEff", "AI|Mission"},
            {"AIMissionGetLaunchScoresEff", "AI|Mission"},
            {"AIMissionGetTargetEff", "AI|Mission"},
            {"AIMissionGetValidSquadsEff", "AI|Mission"},
            {"AIMissionRemoveSquadsEff", "AI|Mission"},
            {"AIMissionSetFlagsEff", "AI|Mission"},
            {"AIMissionSetMoveAttackEff", "AI|Mission"},
            {"AIMissionSetTargetInfoEff", "AI|Mission"},
            {"AIMissionTargetGetLocationEff", "AI|Mission"},
            {"AIMissionTargetGetScoresEff", "AI|Mission"},
            {"AIQueryMissionTargetsEff", "AI|Mission"},
            {"AIRegisterHookEff", "AI"},
            {"AIRemoveFromMissionsEff", "AI|Mission"},
            {"AISAGetComponentEff", "AI"},
            {"AIScoreMissionTargetsEff", "AI|Mission"},
            {"AISetAssetMultipliersEff", "AI|Set"},
            {"AISetBiasesEff", "AI|Set"},
            {"AISetFocusEff", "AI|Set"},
            {"AISetPlayerAssetModifierEff", "AI|Set"},
            {"AISetPlayerBuildSpeedModifiersEff", "AI|Set"},
            {"AISetPlayerDamageModifiersEff", "AI|Set"},
            {"AISetPlayerMultipliersEff", "AI|Set"},
            {"AISetScoringParmsEff", "AI|Set"},
            {"AISortMissionTargetsEff", "AI|Mission"},
            {"AITopicCreateEff", "AI"},
            {"AITopicLottoEff", "AI"},
            {"AITopicModifyTicketsEff", "AI"},
            {"AITopicPriorityRequestEff", "AI"},
            {"AIWrapperModifyFlagsEff", "AI"},
            {"AIWrapperModifyParmsEff", "AI"},
            {"AIWrapperModifyRadiusEff", "AI"},
            {"AsFloatEff", "Math|Convert"},
            {"AsIntEff", "Math|Convert"},
            {"AsStringEff", "Math|Convert"},
            {"AsTimeEff", "Math|Convert"},
            {"AttachmentAddObjectEff", "Game|Units|Attachment"},
            {"AttachmentAddTypeEff", "Game|Units|Attachment"},
            {"AttachmentRemoveAllEff", "Game|Units|Attachment"},
            {"AttachmentRemoveTypeEff", "Game|Units|Attachment"},
            {"BidAddToMissionsEff", "Misc|Bid"},
            {"BidCreateBuildingEff", "Misc|Bid"},
            {"BidCreatePowerEff", "Misc|Bid"},
            {"BidCreateSquadEff", "Misc|Bid"},
            {"BidCreateTechEff", "Misc|Bid"},
            {"BidDeleteEff", "Misc|Bid"},
            {"BidGetDataEff", "Misc|Bid"},
            {"BidPurchaseEff", "Misc|Bid"},
            {"BidQueryEff", "Misc|Bid"},
            {"BidSetBlockedBuildersEff", "Misc|Bid"},
            {"BidSetQueueLimitsEff", "Misc|Bid"},
            {"BlockLeaderPowersEff", "Game|Powers"},
            {"BlockMinimapEff", "Game|Player"},
            {"BuildingCommandEff", "Game|Create"},
            {"CalculatePercentTimeEff", "Math"},
            {"CameraShakeEff", "Game|Player"},
            {"ChangeOwnerEff", "Game|Units"},
            {"ChangeSquadModeEff", "Game|Units"},
            {"ClearBuildingCommandStateEff", "Game|Create"},
            {"ClearCorpseUnitsEff", "Game|World"},
            {"CloakEff", "Game|Units"},
            {"CloakDetectedEff", "Game|Units"},
            {"CombatDamageEff", "Game|Units"},
            {"ConceptGetParametersEff", "Misc|Concept"},
            {"ConceptPermissionEff", "Misc|Concept"},
            {"ConceptResetCooldownEff", "Misc|Concept"},
            {"ConceptSetParametersEff", "Misc|Concept"},
            {"ConceptSetPreconditionEff", "Misc|Concept"},
            {"ConceptSetStateEff", "Misc|Concept"},
            {"ConceptStartSubEff", "Misc|Concept"},
            {"ConvertKBSquadsToSquadsEff", "Math|Convert"},
            {"CopyAISquadAnalysisEff", "Copy"},
            {"CopyBoolEff", "Copy"},
            {"CopyChatSpeakerEff", "Copy"},
            {"CopyColorEff", "Copy"},
            {"CopyCostEff", "Game|Player|Cost"},
            {"CopyCountEff", "Copy"},
            {"CopyDesignLineEff", "Copy"},
            {"CopyDesignLineListEff", "Copy"},
            {"CopyDirectionEff", "Copy"},
            {"CopyDistanceEff", "Copy"},
            {"CopyFloatEff", "Copy"},
            {"CopyFloatListEff", "Copy"},
            {"CopyIntEff", "Copy"},
            {"CopyIntegerListEff", "Copy"},
            {"CopyKBBaseEff", "Copy"},
            {"CopyLocationEff", "Copy"},
            {"CopyLocationListEff", "Copy"},
            {"CopyLocStringIDEff", "Copy"},
            {"CopyObjectiveEff", "Copy"},
            {"CopyObjectListEff", "Copy"},
            {"CopyObjectTypeEff", "Copy"},
            {"CopyObjectTypeListEff", "Copy"},
            {"CopyPercentEff", "Copy"},
            {"CopyPlayerEff", "Copy"},
            {"CopyProtoObjectEff", "Copy"},
            {"CopyProtoObjectListEff", "Copy"},
            {"CopyProtoSquadEff", "Copy"},
            {"CopyProtoSquadListEff", "Copy"},
            {"CopySoundEff", "Copy"},
            {"CopySquadEff", "Copy"},
            {"CopySquadListEff", "Copy"},
            {"CopyStringEff", "Copy"},
            {"CopyTechEff", "Copy"},
            {"CopyTechListEff", "Copy"},
            {"CopyTimeEff", "Copy"},
            {"CopyUnitEff", "Copy"},
            {"CopyUnitListEff", "Copy"},
            {"CostToFloatEff", "Game|Player|Cost"},
            {"CountIncrementEff", "Math"},
            {"CreateIconObjectEff", "Game|Create"},
            {"CreateObjectEff", "Game|Create"},
            {"CreateObstructionUnitEff", "Game|Create"},
            {"CreateSquadEff", "Game|Create"},
            {"CreateSquadsEff", "Game|Create"},
            {"CreateUnitEff", "Game|Create"},
            {"CustomCommandAddEff", "Game|Units"},
            {"CustomCommandExecuteEff", "Game|Units"},
            {"CustomCommandRemoveEff", "Game|Units"},
            {"DamageEff", "Game|Units"},
            {"DesignLineGetPointsEff", "Misc|Design"},
            {"DesignLineListAddEff", "Misc|Design"},
            {"DesignLineListGetSizeEff", "Misc|Design"},
            {"DesignLineListRemoveEff", "Misc|Design"},
            {"DestroyEff", "Game|Units"},
            {"EnableChatsEff", "Game|UI/Input"},
            {"EnableFogOfWarEff", "Game|World"},
            {"EnableMusicManagerEff", "Game|Audio"},
            {"EventClearFiltersEff", "Misc|Event"},
            {"EventFilterCameraEff", "Misc|Event"},
            {"EventFilterEntityEff", "Misc|Event"},
            {"EventFilterGameStateEff", "Misc|Event"},
            {"EventFilterNumericEff", "Misc|Event"},
            {"EventResetEff", "Misc|Event"},
            {"EventSubscribeEff", "Misc|Event"},
            {"EventSubscribeUseCountEff", "Misc|Event"},
            {"FadeTransitionEff", "Game|UI/Input"},
            {"FilterAddCanChangeOwnerEff", "Misc|Filter"},
            {"FilterAddDiplomacyEff", "Misc|Filter"},
            {"FilterAddIsAliveEff", "Misc|Filter"},
            {"FilterAddIsIdleEff", "Misc|Filter"},
            {"FilterAddIsSelectedEff", "Misc|Filter"},
            {"FilterAddJackingEff", "Misc|Filter"},
            {"FilterAddMaxObjectTypeEff", "Misc|Filter"},
            {"FilterAddObjectTypesEff", "Misc|Filter"},
            {"FilterAddPlayersEff", "Misc|Filter"},
            {"FilterAddProtoObjectsEff", "Misc|Filter"},
            {"FilterAddProtoSquadsEff", "Misc|Filter"},
            {"FilterAddRefCountEff", "Misc|Filter"},
            {"FilterAddTeamsEff", "Misc|Filter"},
            {"FilterClearEff", "Misc|Filter"},
            {"FlareMinimapNormalEff", "Game|UI/Input"},
            {"FlareMinimapSpoofEff", "Game|UI/Input"},
            {"FlashEntityEff", "Game|Units|Visual"},
            {"FlashUIElementEff", "Game|UI/Input"},
            {"FloatListAddEff", "Lists|Float"},
            {"FloatListRemoveEff", "Lists|Float"},
            {"ForbidEff", "Game|Player"},
            {"GetChildUnitsEff", "Game|Units|Get"},
            {"GetClosestPowerSquadEff", "Game|Units|Get"},
            {"GetClosestSquadEff", "Game|Units|Get"},
            {"GetClosestUnitEff", "Game|Units|Get"},
            {"GetCostEff", "Game|Player|Cost"},
            {"GetCTFFlagEff", "Game|Scenario|DLC"},
            {"GetCTFFlagCarrierEff", "Game|Scenario|DLC"},
            {"GetDifficultyEff", "Game|Player"},
            {"GetDirectionEff", "Game|World"},
            {"GetDirectionFromLocationsEff", "Game|World"},
            {"GetDistanceLocationLocationEff", "Game|World"},
            {"GetDistanceUnitLocationEff", "Game|World"},
            {"GetDLCGameModeEff", "Game|Scenario|DLC"},
            {"GetGameModeEff", "Game|Scenario|DLC"},
            {"GetGameTimeEff", "Game|Time"},
            {"GetGarrisonedSquadsEff", "Game|Units|Get"},
            {"GetGarrisonedUnitsEff", "Game|Units|Get"},
            {"GetHealthEff", "Game|Units|Get"},
            {"GetIdleDurationEff", "Game|Units|Get"},
            {"GetKBBaseLocationEff", "Game|World"},
            {"GetLegalBuildingsEff", "Game|Player"},
            {"GetLegalSquadsEff", "Game|Player"},
            {"GetLegalTechsEff", "Game|Player"},
            {"GetLocationEff", "Game|Units|Get"},
            {"GetMeanLocationEff", "Game|World"},
            {"GetNPCPlayersByNameEff", "Game|Player"},
            {"GetNumTransportsEff", "Game|Units|Get"},
            {"GetObstructionRadiusEff", "Game|World"},
            {"GetOwnerEff", "Game|Units|Get"},
            {"GetParentSquadEff", "Game|Units|Get"},
            {"GetPlayerCivEff", "Game|Player"},
            {"GetPlayerEconomyEff", "Game|Player"},
            {"GetPlayerLeaderEff", "Game|Player"},
            {"GetPlayerPopEff", "Game|Player"},
            {"GetPlayersEff", "Game|Player"},
            {"GetPlayers2Eff", "Game|Player"},
            {"GetPlayerScoreEff", "Game|Player"},
            {"GetPlayerTeamEff", "Game|Player"},
            {"GetPopEff", "Game|Player"},
            {"GetPopularSquadTypeEff", "Game|Units|Get"},
            {"GetPowerRadiusEff", "Game|Powers"},
            {"GetProtoObjectEff", "Game|Units|Get"},
            {"GetProtoSquadEff", "Game|Units|Get"},
            {"GetResourcesEff", "Game|Player"},
            {"GetSquadsEff", "Game|Player"},
            {"GetSquadTrainerTypeEff", "Game|Units|Get"},
            {"GetTeamPlayersEff", "Game|Team"},
            {"GetTeamsEff", "Game|Team"},
            {"GetTechResearcherTypeEff", "Game|Player"},
            {"GetUnitsEff", "Game|Player"},
            {"GetUTechBldingsEff", "Game|Player"},
            {"GrantAchievementEff", "Game|Player"},
            {"GrantAchievementToPlayerEff", "Game|Player"},
            {"HintCalloutCreateEff", "Game|Hints"},
            {"HintCalloutDestroyEff", "Game|Hints"},
            {"HintGlowToggleEff", "Game|Hints"},
            {"HintMessageDestroyEff", "Game|Hints"},
            {"HintMessageShowEff", "Game|Hints"},
            {"HUDToggleEff", "Game|UI/Input"},
            {"IgnoreDpadEff", "Game|UI/Input"},
            {"InfectEff", "Game|Units"},
            {"InputUIButtonEff", "Game|UI/Input"},
            {"InputUILocationEff", "Game|UI/Input"},
            {"InputUILocationMinigameEff", "Game|UI/Input"},
            {"InputUIPlaceSquadsEff", "Game|UI/Input"},
            {"InputUISquadEff", "Game|UI/Input"},
            {"InputUISquadListEff", "Game|UI/Input"},
            {"IntDecrementEff", "Math"},
            {"IntegerListAddEff", "Lists|Int"},
            {"IntegerListGetSizeEff", "Lists|Int"},
            {"IntegerListRemoveEff", "Lists|Int"},
            {"IntIncrementEff", "Math"},
            {"IntToCountEff", "Math|Convert"},
            {"InvertBoolEff", "Math|Convert"},
            {"IteratorKBBaseListEff", "Misc|KB"},
            {"IteratorLocationListEff", "Lists|Location"},
            {"IteratorPlayerListEff", "Lists|Player"},
            {"IteratorProtoSquadListEff", "Lists|Proto"},
            {"IteratorSquadListEff", "Lists|Squad"},
            {"IteratorTeamListEff", "Lists|Team"},
            {"IteratorUnitListEff", "Lists|Unit"},
            {"KBAddSquadsToKBEff", "Misc|KB"},
            {"KBBaseGetMassEff", "Misc|KB"},
            {"KBBQExecuteEff", "Misc|KB"},
            {"KBBQExecuteClosestEff", "Misc|KB"},
            {"KBBQPlayerRelationEff", "Misc|KB"},
            {"KBSFAddObjectTypesEff", "Misc|KB"},
            {"KBSQExecuteEff", "Misc|KB"},
            {"KBSQInitEff", "Misc|KB"},
            {"KBSQObjectTypeEff", "Misc|KB"},
            {"KBSQPlayerRelationEff", "Misc|KB"},
            {"KBSquadListFilterEff", "Misc|KB"},
            {"KillEff", "Game|Units"},
            {"KillUnitsEff", "Game|Units"},
            {"LaunchCinematicEff", "Game|UI/Input"},
            {"LaunchProjectileEff", "Game|World"},
            {"LaunchScriptEff", "Special"},
            {"TriggerActivateEff", "Special"},
            {"TriggerDeactivateEff", "Special"},
            {"ShutdownEff", "Special"},
            {"ClearBlackMapEff", "Misc"},
            {"ResetBlackMapEff", "Misc"},
            {"LerpIntEff", "Math"},
            {"LerpLocationEff", "Math"},
            {"LerpPercentEff", "Math"},
            {"LerpTimeEff", "Math"},
            {"LocationAdjustEff", "Game|World"},
            {"LocationAdjustDirEff", "Game|World"},
            {"LocationListAddEff", "Lists|Location"},
            {"LocationListGetByIndexEff", "Lists|Location"},
            {"LocationListGetClosestEff", "Lists|Location"},
            {"LocationListGetSizeEff", "Lists|Location"},
            {"LocationListPartitionEff", "Lists|Location"},
            {"LocationListRemoveEff", "Lists|Location"},
            {"LocationListShuffleEff", "Lists|Location"},
            {"LocationTieToGroundEff", "Game|World"},
            {"MathFloatEff", "Math"},
            {"MathHitpointsEff", "Math"},
            {"MathIntEff", "Math"},
            {"MathLocationEff", "Math"},
            {"MathPercentEff", "Math"},
            {"MathResourcesEff", "Math"},
            {"MathTimeEff", "Math"},
            {"MegaTurretAttackEff", "Game|World"},
            {"MissionResultEff", "Game|Scenario"},
            {"ModifyDataScalarEff", "Math"},
            {"ModifyProtoDataEff", "Game|Player"},
            {"ModifyProtoSquadDataEff", "Game|Player"},
            {"MoveEff", "Game|Units"},
            {"MovePathEff", "Game|Units"},
            {"ObjectiveCompleteEff", "Game|Scenario"},
            {"ObjectiveDecrementCounterEff", "Game|Scenario"},
            {"ObjectiveDisplayEff", "Game|Scenario"},
            {"ObjectiveGetFinalCounterEff", "Game|Scenario"},
            {"ObjectiveIncrementCounterEff", "Game|Scenario"},
            {"ObjectListRemoveEff", "Lists|Object"},
            {"ObjectTypeToProtoObjectsEff", "Math|Convert"},
            {"PatherObstructionRebuildEff", "Game|World"},
            {"PatherObstructionUpdatesEff", "Game|World"},
            {"PayCostEff", "Game|Player|Cost"},
            {"PlayAnimationObjectEff", "Game|Units|Visual"},
            {"PlayAnimationSquadEff", "Game|Units|Visual"},
            {"PlayAnimationUnitEff", "Game|Units|Visual"},
            {"PlayChatEff", "Game|UI/Input"},
            {"PlayerListAddEff", "Lists|Player"},
            {"PlayerListGetSizeEff", "Lists|Player"},
            {"PlayerListRemoveEff", "Lists|Player"},
            {"PlayRelationSoundEff", "Game|UI/Input"},
            {"PlaySoundEff", "Game|UI/Input"},
            {"PlaySoundFileEff", "Game|UI/Input"},
            {"PlayWorldSoundOnEntityEff", "Game|UI/Input"},
            {"PowerChargeUseOfEff", "Game|Powers"},
            {"PowerClearEff", "Game|Powers"},
            {"PowerGrantEff", "Game|Powers"},
            {"PowerInvokeEff", "Game|Powers"},
            {"PowerMenuEnableEff", "Game|Powers"},
            {"PowerRevokeEff", "Game|Powers"},
            {"PowerUserShutdownEff", "Game|Powers"},
            {"ProtoObjectListAddEff", "Lists|Proto"},
            {"ProtoObjectListRemoveEff", "Lists|Proto"},
            {"ProtoObjectListShuffleEff", "Lists|Proto"},
            {"ProtoSquadListAddEff", "Lists|Proto"},
            {"ProtoSquadListGetSizeEff", "Lists|Proto"},
            {"ProtoSquadListRemoveEff", "Lists|Proto"},
            {"ProtoSquadListShuffleEff", "Lists|Proto"},
            {"RallyPointGetEff", "Game|Player"},
            {"RallyPointSetEff", "Game|Player"},
            {"RandomCountEff", "Math"},
            {"RandomIntEff", "Math"},
            {"RandomLocationEff", "Math"},
            {"RandomTimeEff", "Math"},
            {"RecycleBuildingEff", "Game|Units"},
            {"RefCountSquadAddEff", "Misc|Ref"},
            {"RefCountSquadRemoveEff", "Misc|Ref"},
            {"RefCountUnitAddEff", "Misc|Ref"},
            {"RefCountUnitRemoveEff", "Misc|Ref"},
            {"RefundCostEff", "Game|Player|Cost"},
            {"RepairEff", "Game|Units"},
            {"RepairByCombatValueEff", "Game|Units"},
            {"ResetAbilityTimerEff", "Game|Units"},
            {"ResetDoppleEff", "Game|Units"},
            {"RevealerEff", "Game|World"},
            {"RumbleStartEff", "Game|UI/Input"},
            {"SetAutoAttackableEff", "Game|Units|Set"},
            {"SetCameraEff", "Game|Player"},
            {"setCTFCountEff", "Game|Scenario|DLC"},
            {"SetCTFFlagEff", "Game|Scenario|DLC"},
            {"ClearCTFFlagOrCarrierEff", "Game|Scenario|DLC"},
            {"SetDirectionEff", "Game|World"},
            {"SetGarrisonedCountEff", "Game|Units|Set"},
            {"SetIgnoreUserInputEff", "Game|UI/Input"},
            {"SetLevelEff", "Game|Units|Set"},
            {"SetMinimapNorthPointerRotationEff", "Game|UI/Input"},
            {"SetMinimapSkirtMirroringEff", "Game|UI/Input"},
            {"SetMobileEff", "Game|Units|Set"},
            {"SetOverrideTintEff", "Game|Units|Visual"},
            {"SetPlayableBoundsEff", "Game|World"},
            {"SetPlayerPopEff", "Game|Player"},
            {"SetPlayerStateEff", "Game|Player"},
            {"SetPositionEff", "Game|World"},
            {"SetRenderTerrainSkirtEff", "Game|World"},
            {"SetResourceHandicapEff", "Game|Player"},
            {"SetResourcesEff", "Game|Player"},
            {"SetScenarioScoreInfoEff", "Game|Scenario"},
            {"SetSelectableEff", "Game|Units|Set"},
            {"SetTeleporterDestinationEff", "Game|Units|Set"},
            {"SettleEff", "Game|Units"},
            {"SetTowerWallDestinationEff", "Misc"},
            {"SetTransportPickUpLocationsEff", "Game|Units|Set"},
            {"SetTrickleRateEff", "Game|Player"},
            {"SetUIPowerRadiusEff", "Game|UI/Input"},
            {"SetUnitAttackTargetEff", "Game|Units|Set"},
            {"ShowMessageEff", "Game|UI/Input"},
            {"ShowObjectivePointerEff", "Game|UI/Input"},
            {"SquadFlagSetEff", "Game|Units"},
            {"SquadListAddEff", "Lists|Squad"},
            {"SquadListDiffEff", "Lists|Squad"},
            {"SquadListFilterEff", "Lists|Squad"},
            {"SquadListGetSizeEff", "Lists|Squad"},
            {"SquadListPartitionEff", "Lists|Squad"},
            {"SquadListRemoveEff", "Lists|Squad"},
            {"SquadListShuffleEff", "Lists|Squad"},
            {"TableLoadEff", "Game|UI/Input"},
            {"TeamListAddEff", "Lists|Team"},
            {"TeamListRemoveEff", "Lists|Team"},
            {"TeamSetDiplomacyEff", "Game|Team"},
            {"TeamsToPlayersEff", "Game|Team"},
            {"TechActivateEff", "Game|Player"},
            {"TechDeactivateEff", "Game|Player"},
            {"TechListAddEff", "Lists|Tech"},
            {"TechListRemoveEff", "Lists|Tech"},
            {"TeleportEff", "Game|Units"},
            {"TeleportUnitsOffObstructionEff", "Game|Units"},
            {"TimerCreateEff", "Game|Time"},
            {"TimerDestroyEff", "Game|Time"},
            {"TimerGetEff", "Game|Time"},
            {"TimerSetEff", "Game|Time"},
            {"TimerSetPausedEff", "Game|Time"},
            {"TimeToFloatEff", "Math|Convert"},
            {"TowSetScoreEff", "Game|Scenario|DLC"},
            {"TowSetVisibleEff", "Game|Scenario|DLC"},
            {"TransferGarrisonedEff", "Game|Units"},
            {"TransformEff", "Game|World"},
            {"TransportSquadsEff", "Game|Units"},
            {"TugOfWarMarkPlayersAsTrailingEff", "Game|Scenario|DLC"},
            {"UITogglePowerOverlayEff", "Game|UI/Input"},
            {"UIUnlockEff", "Game|UI/Input"},
            {"UnitFlagSetEff", "Game|Units"},
            {"UnitListAddEff", "Lists|Unit"},
            {"UnitListDiffEff", "Lists|Unit"},
            {"UnitListFilterEff", "Lists|Unit"},
            {"UnitListGetSizeEff", "Lists|Unit"},
            {"UnitListPartitionEff", "Lists|Unit"},
            {"UnitListRemoveEff", "Lists|Unit"},
            {"UnitListShuffleEff", "Lists|Unit"},
            {"UnloadEff", "Game|Units"},
            {"UsePowerEff", "Game|Powers"},
            {"UserMessageEff", "Game|UI/Input"},
            {"WorkEff", "Game|Units"},
            {"AICanGetDifficultySettingCnd", "AI"},
            {"AITopicGetTicketsCnd", "AI"},
            {"AITopicIsActiveCnd", "AI"},
            {"ASYNCPlayerSelectingSquadCnd", "Game|Player"},
            {"ASYNCUnitsOnScreenSelectedCnd", "Game|Units"},
            {"BidStateCnd", "Misc|Bid"},
            {"BuildingCommandDoneCnd", "Game|Create"},
            {"CanGetBuilderCnd", "CanGet"},
            {"CanGetCentroidCnd", "CanGet"},
            {"CanGetCoopPlayerCnd", "CanGet"},
            {"CanGetCorpseUnitsCnd", "CanGet"},
            {"CanGetCTFMissionTargetCnd", "CanGet"},
            {"CanGetDesignSpheresCnd", "CanGet"},
            {"CanGetGreatestThreatCnd", "CanGet"},
            {"CanGetHoverPointCnd", "CanGet"},
            {"CanGetOneDesignLineCnd", "CanGet"},
            {"CanGetOneFloatCnd", "CanGet"},
            {"CanGetOneIntegerCnd", "CanGet"},
            {"CanGetOneLocationCnd", "CanGet"},
            {"CanGetOneObjectCnd", "CanGet"},
            {"CanGetOnePlayerCnd", "CanGet"},
            {"CanGetOneProtoObjectCnd", "CanGet"},
            {"CanGetOneProtoSquadCnd", "CanGet"},
            {"CanGetOneSocketUnitCnd", "CanGet"},
            {"CanGetOneSquadCnd", "CanGet"},
            {"CanGetOneTeamCnd", "CanGet"},
            {"CanGetOneTechCnd", "CanGet"},
            {"CanGetOneUnitCnd", "CanGet"},
            {"CanGetRandomLocationCnd", "CanGet"},
            {"CanGetSocketParentBuildingCnd", "CanGet"},
            {"CanGetSocketPlugUnitCnd", "CanGet"},
            {"CanGetSocketUnitsCnd", "CanGet"},
            {"CanGetSquadsCnd", "CanGet"},
            {"CanGetUnitLaunchLocationCnd", "CanGet"},
            {"CanGetUnitsCnd", "CanGet"},
            {"CanGetUnitsAlongRayCnd", "CanGet"},
            {"CanPayCostCnd", "Game|Player|Cost"},
            {"CanRemoveOneFloatCnd", "Lists|Float"},
            {"CanRemoveOneIntegerCnd", "Lists|Int"},
            {"CanRemoveOneLocationCnd", "Lists|Location"},
            {"CanRemoveOneProtoObjectCnd", "Lists|Proto"},
            {"CanRemoveOneProtoSquadCnd", "Lists|Proto"},
            {"CanRemoveOneTechCnd", "Lists|Tech"},
            {"CanRetrieveExternalFloatCnd", "Setup"},
            {"CanRetrieveExternalLocationCnd", "Setup"},
            {"CanRetrieveExternalLocationListCnd", "Setup"},
            {"CanRetrieveExternalsCnd", "Setup"},
            {"CanUsePowerCnd", "Game|Powers"},
            {"ChatCompletedCnd", "Game|UI/Input"},
            {"CheckAndSetFalseCnd", "Misc"},
            {"CheckDifficultyCnd", "Game"},
            {"CheckDiplomacyCnd", "Game"},
            {"CheckModeChangeCnd", "Game|Units"},
            {"CheckPlacementCnd", "Game|World"},
            {"CheckPopCnd", "Game|Player"},
            {"CheckResourcesTotalsCnd", "Game|Player"},
            {"CinematicCompletedCnd", "Game|UI/Input"},
            {"CompareAIMissionStateCnd", "Compare"},
            {"CompareAIMissionTypeCnd", "Compare"},
            {"CompareBoolCnd", "Compare"},
            {"CompareCivCnd", "Compare"},
            {"CompareCountCnd", "Compare"},
            {"CompareDesignLineCnd", "Compare"},
            {"CompareFloatCnd", "Compare"},
            {"CompareHitpointsCnd", "Compare"},
            {"CompareIntegerCnd", "Compare"},
            {"CompareLeaderCnd", "Compare"},
            {"CompareLocStringIDCnd", "Compare"},
            {"ComparePercentCnd", "Compare"},
            {"ComparePlayersCnd", "Compare"},
            {"ComparePlayerSquadCountCnd", "Compare"},
            {"ComparePlayerUnitCountCnd", "Compare"},
            {"CompareProtoObjectCnd", "Compare"},
            {"CompareProtoSquadCnd", "Compare"},
            {"CompareStringCnd", "Compare"},
            {"CompareTeamsCnd", "Compare"},
            {"CompareTechCnd", "Compare"},
            {"CompareTimeCnd", "Compare"},
            {"CompareUnitCnd", "Compare"},
            {"CompareVectorCnd", "Compare"},
            {"ConceptGetCommandCnd", "Misc|Concept"},
            {"ConceptGetStateChangeCnd", "Misc|Concept"},
            {"ContainsGarrisonedCnd", "Contains"},
            {"CustomCommandCheckCnd", "Game|Units"},
            {"EventTriggeredCnd", "Misc|Event"},
            {"FadeCompletedCnd", "Game|UI/Input"},
            {"GameTimeCnd", "Game"},
            {"GameTimeReachedCnd", "Game"},
            {"GetTableRowCnd", "CustomData"},
            {"HasGarrisonedCnd", "Game"},
            {"IsAliveCnd", "Game|Units"},
            {"IsAttachedCnd", "Game|Units"},
            {"IsAttackingCnd", "Game|Units"},
            {"IsBuiltCnd", "Game|Units"},
            {"IsCapturingCnd", "Game|Units"},
            {"IsConfigDefinedCnd", "Misc"},
            {"IsCoopCnd", "Game"},
            {"IsDeadCnd", "Game|Units"},
            {"IsEmptySocketUnitCnd", "Game|Units"},
            {"IsForbiddenCnd", "Game"},
            {"IsGarrisonedCnd", "Game|Units"},
            {"IsGatheringCnd", "Game|Units"},
            {"IsHitchedCnd", "Game|Units"},
            {"IsIdleCnd", "Game|Units"},
            {"IsMapCnd", "Game"},
            {"IsMovingCnd", "Game|Units"},
            {"IsObjectTypeCnd", "Game"},
            {"IsOwnedByCnd", "Game"},
            {"IsProtoObjectCnd", "Game"},
            {"IsSelectableCnd", "Game|Units"},
            {"IsUnderAttackCnd", "Game|Units"},
            {"MarkerSquadsInAreaCnd", "Game"},
            {"NextKBBaseCnd", "Iterator"},
            {"NextLocationCnd", "Iterator"},
            {"NextPlayerCnd", "Iterator"},
            {"NextProtoSquadCnd", "Iterator"},
            {"NextSquadCnd", "Iterator"},
            {"NextTeamCnd", "Iterator"},
            {"NextUnitCnd", "Iterator"},
            {"PlayerInStateCnd", "Game"},
            {"PlayerIsComputerAICnd", "Game|Player"},
            {"PlayerIsHumanCnd", "Game|Player"},
            {"PlayerUsingLeaderCnd", "Game|Player"},
            {"ProtoObjectListContainsCnd", "Lists|Proto"},
            {"ProtoSquadListContainsCnd", "Lists|Proto"},
            {"RefCountSquadCnd", "Misc|Ref"},
            {"RefCountUnitCnd", "Misc|Ref"},
            {"SquadLocationDistanceCnd", "Game|World"},
            {"TechListContainsCnd", "Lists|Tech"},
            {"TechStatusCnd", "Game"},
            {"TimerIsDoneCnd", "Game"},
            {"TriggerActiveTimeCnd", "Setup"},
            {"UIButtonPressedCnd", "Game|UI/Input"},
            {"UILocationCancelCnd", "Game|UI/Input"},
            {"UILocationMinigameCancelCnd", "Game|UI/Input"},
            {"UILocationMinigameOKCnd", "Game|UI/Input"},
            {"UILocationMinigameUILockErrorCnd", "Game|UI/Input"},
            {"UILocationMinigameWaitingCnd", "Game|UI/Input"},
            {"UILocationOKCnd", "Game|UI/Input"},
            {"UILocationUILockErrorCnd", "Game|UI/Input"},
            {"UILocationWaitingCnd", "Game|UI/Input"},
            {"UISquadCancelCnd", "Game|UI/Input"},
            {"UISquadListCancelCnd", "Game|UI/Input"},
            {"UISquadListOKCnd", "Game|UI/Input"},
            {"UISquadListUILockErrorCnd", "Game|UI/Input"},
            {"UISquadListWaitingCnd", "Game|UI/Input"},
            {"UISquadOKCnd", "Game|UI/Input"},
            {"UISquadUILockErrorCnd", "Game|UI/Input"},
            {"UISquadWaitingCnd", "Game|UI/Input"},
            {"UnitLocationDistanceCnd", "Game|World"},
            {"UnitUnitDistanceCnd", "Game|World"}
        };
        private List<TriggerscripterNode> nodes = new List<TriggerscripterNode>();

        private ContextMenu cm;
        private Timer t;
        public TriggerscriptEditorPage(FoundryInstance i, string file) : base(i, file)
        {
            Text = Path.GetFileName(file);
            DoubleBuffered = true;
            Paint += new PaintEventHandler(DrawControl);

            #region Context Menu
            cm = new ContextMenu();
            ContextMenu = cm;
            cm.Popup += CaptureMousePos;
            MenuItem trg = new MenuItem("New Trigger");
            trg.Click += CreateNewTriggerPressed;
            cm.MenuItems.Add(trg);

            MenuItem cndMI = new MenuItem("Conditions");
            MenuItem effMI = new MenuItem("Effects");
            MenuItem varMI = new MenuItem("Variables");
            cm.MenuItems.Add(cndMI);
            cm.MenuItems.Add(effMI);
            cm.MenuItems.Add(varMI);

            //populate with main categories
            Dictionary<string, MenuItem> hierarchy = new Dictionary<string, MenuItem>() {
                { "cnd|", cndMI },
                { "eff|", effMI },
                { "var|", varMI }
            };

            //deserialize and sort effects.
            List<Tuple<Effect, string>> effsSorted = new List<Tuple<Effect, string>>();
            foreach (Effect e in JsonConvert.DeserializeObject<List<Effect>>(Properties.Resources.eff))
            {
                effsSorted.Add(new Tuple<Effect, string>(e, nodeSubCategories[e.name + "Eff"]));
            }
            effsSorted = effsSorted.OrderBy(x => (x.Item2 + "|" + x.Item1.name)).ToList();
            //create MenuItems for effects.
            foreach (var v in effsSorted)
            {
                string concat = "eff|";
                string[] split = v.Item2.Split('|');
                MenuItem last = hierarchy[concat];
                foreach (string s in split)
                {
                    concat += s + "|";
                    MenuItem item;
                    if (hierarchy.ContainsKey(concat))
                    {
                        item = hierarchy[concat];
                    }
                    else
                    {
                        item = new MenuItem(s);
                        hierarchy.Add(concat, item);
                    }
                    last.MenuItems.Add(item);
                    last = item;
                }
                MenuItem effI = new MenuItem(v.Item1.name + " v" + v.Item1.version);
                effI.Tag = v.Item1;
                effI.Click += CreateNewEffectPressed;
                last.MenuItems.Add(effI);
            }


            //deserialize and sort conditions.
            List<Tuple<Condition, string>> cndsSorted = new List<Tuple<Condition, string>>();
            foreach (Condition c in JsonConvert.DeserializeObject<List<Condition>>(Properties.Resources.cnd))
                cndsSorted.Add(new Tuple<Condition, string>(c, nodeSubCategories[c.name + "Cnd"]));
            cndsSorted = cndsSorted.OrderBy(x => (x.Item2 + "|" + x.Item1.name)).ToList();
            //create MenuItems for conditions
            foreach (var v in cndsSorted)
            {
                string concat = "cnd|";
                string[] split = v.Item2.Split('|');
                MenuItem last = hierarchy[concat];
                foreach (string s in split)
                {
                    concat += s + "|";
                    MenuItem item;
                    if (hierarchy.ContainsKey(concat))
                    {
                        item = hierarchy[concat];
                    }
                    else
                    {
                        item = new MenuItem(s);
                        hierarchy.Add(concat, item);
                    }
                    last.MenuItems.Add(item);
                    last = item;
                }
                MenuItem cndI = new MenuItem(v.Item1.name + " v" + v.Item1.version);
                cndI.Tag = v.Item1;
                cndI.Click += CreateNewConditionPressed;
                last.MenuItems.Add(cndI);
            }


            //deserialize and sort variable types.
            List<string> vs = JsonConvert.DeserializeObject<List<string>>(Properties.Resources.var);
            vs.Sort();
            foreach (string v in vs)
            {
                string vStr = nodeSubCategories[v + "Var"];
                string concat = "var|";
                MenuItem last = varMI;
                foreach (string s in vStr.Split('|'))
                {
                    concat += s + "|";
                    if (hierarchy.Keys.Contains(concat))
                    {
                        last = hierarchy[concat];
                    }
                    else
                    {
                        MenuItem item = new MenuItem(s);
                        hierarchy.Add(concat, item);
                        last.MenuItems.Add(item);
                        last = item;
                    }
                }

                MenuItem varItem = new MenuItem(v);
                varItem.Tag = v;
                varItem.Click += CreateNewVarPressed;
                last.MenuItems.Add(varItem);
            }
            #endregion

            //init matrices.
            PollInput();
            UpdateMatrices();

            //start draw timer.
            t = new Timer();
            t.Interval = 1;
            t.Tick += Tick;
            t.Start();
        }
        void Tick(object o, EventArgs e)
        {
            PollInput();
            UpdateMatrices();
            Invalidate();
        }

        #region page
        protected override void OnOpen(string file)
        {
            string text = File.ReadAllText(file);
            SerializedTriggerscripter sts = JsonConvert.DeserializeObject<SerializedTriggerscripter>(text);
            Load(sts);
        }
        protected override void OnSave(string file)
        {
            string text = JsonConvert.SerializeObject(GetSerializedGraph());
            File.WriteAllText(file, text);
        }
        #endregion

        #region editor
        #region draw
        Pen gridPen = new Pen(Color.FromArgb(255, 150, 150, 150));
        public int majorGridSpace = 200;
        float x = 0, y = 0;
        public float zoom = .25f;
        float zoomMax = 1.5f, zoomMin = .025f;
        public Matrix transform = new Matrix();
        public Matrix transformInv = new Matrix();
        void DrawControl(object o, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.High;
            g.Transform = transform;

            g.Clear(Color.DarkGray);
            
            PointF[] points = new PointF[]
            {
                new PointF(e.ClipRectangle.Left, e.ClipRectangle.Top),
                new PointF(e.ClipRectangle.Right, e.ClipRectangle.Bottom)
            };
            transformInv.TransformPoints(points);

            float left = points[0].X;
            float right = points[1].X;
            float top = points[0].Y;
            float bottom = points[1].Y;

            int xOffs = (int)Math.Round(left / (float)majorGridSpace) * majorGridSpace;
            int yOffs = (int)Math.Round(top / (float)majorGridSpace) * majorGridSpace;

            //draw grid lines
            for (int x = xOffs; x < right; x += majorGridSpace)
            {
                g.DrawLine(gridPen, x, top, x, bottom);
            }
            for (int y = yOffs; y < bottom; y += majorGridSpace)
            {
                g.DrawLine(gridPen, left, y, right, y);
            }
            //draw connections
            foreach (TriggerscripterNode n in nodes)
            {
                foreach (TriggerscripterSocket s in n.sockets.Values)
                {
                    if (s is TriggerscripterSocket_Output)
                    {
                        ((TriggerscripterSocket_Output)s).DrawConnections(e);
                    }
                }
            }
           
            //draw temp connection
            Point[] pos = new Point[] { PointToClient(new Point((int)lastX, (int)lastY)) };
            transformInv.TransformPoints(pos);
            if (mouseState == CurrentMouseState.DraggingSocket)
            {
                e.Graphics.DrawLine(new Pen(selectedSocket.color, 5.0f), new Point(
                    selectedSocket.node.x + selectedSocket.rect.X + selectedSocket.rect.Width / 2,
                    selectedSocket.node.y + selectedSocket.rect.Y + selectedSocket.rect.Height / 2),
                    pos[0]);
            }
            
            //draw nodes
            foreach (TriggerscripterNode n in nodes)
            {
                n.Draw(e);
                //draw sockets
                foreach (TriggerscripterSocket s in n.sockets.Values)
                {
                    s.Draw(e);
                }
            }
            
            //draw marquee
            if (mouseState == CurrentMouseState.DraggingMarquee)
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(100, 10, 10, 10)),
                    Math.Min(marqueeX1, marqueeX2),
                    Math.Min(marqueeY1, marqueeY2),
                    Math.Abs(marqueeX2 - marqueeX1),
                    Math.Abs(marqueeY2 - marqueeY1));
            }
        }
        void UpdateMatrices()
        {
            transform.Reset();
            transform.Translate(Width / 2, Height / 2);
            transform.Scale(zoom, zoom);
            transform.Translate(x, y);

            transformInv.Reset();
            transformInv.Translate(-x, -y);
            transformInv.Scale(1 / zoom, 1 / zoom);
            transformInv.Translate(-Width / 2, -Height / 2);
        }
        #endregion

        #region ui/ux
        TriggerscripterSocket selectedSocket = null;
        bool lastClickedL = false, lastClickedR = false;
        bool suspendInput = false;
        float lastX = 0, lastY = 0, lastM = 0;
        bool copyPastePressed;

        EventHandler OnEdit;

        enum CurrentMouseState
        {
            None,
            NodesSelected,
            DraggingMarquee,
            DraggingSocket
        }

        int marqueeX1, marqueeY1;
        int marqueeX2, marqueeY2;

        CurrentMouseState mouseState;
        void PollInput()
        {
            if (IsDisposed) return;
            MouseState m = Mouse.GetCursorState();
            KeyboardState keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Key.Delete))
            {
                OnEdit?.Invoke(this, null);
                foreach (TriggerscripterNode n in nodes.ToArray())
                {
                    if (n.selected)
                    {
                        //for each socket in this node's sockets
                        foreach (TriggerscripterSocket s in n.sockets.Values)
                        {
                            //if it is an input
                            if (s is TriggerscripterSocket_Input)
                            {
                                //foreach output connected to this input
                                foreach (TriggerscripterSocket cs in ((TriggerscripterSocket_Input)s).connectedSockets.ToArray())
                                {
                                    //disconnect this input from the output
                                    ((TriggerscripterSocket_Output)cs).Disconnect((TriggerscripterSocket_Input)s);
                                }
                            }

                            //if it is an output
                            else
                            {
                                //foreach input connected to this output
                                foreach (TriggerscripterSocket cs in s.connectedSockets.ToArray())
                                {
                                    ((TriggerscripterSocket_Output)s).Disconnect((TriggerscripterSocket_Input)cs);
                                }
                            }

                            nodes.Remove(n);
                        }
                    }
                }
            }
            if (keyboard.IsKeyDown(Key.ControlLeft) && keyboard.IsKeyDown(Key.C))
            {
                if (!copyPastePressed)
                {
                    CopyGraph();
                }
                copyPastePressed = true;
            }
            else if (keyboard.IsKeyDown(Key.ControlLeft) && keyboard.IsKeyDown(Key.V))
            {
                OnEdit?.Invoke(this, null);
                if (!copyPastePressed)
                {
                    PasteGraph();
                }
                copyPastePressed = true;
            }
            else
            {
                copyPastePressed = false;
            }

            Point min = PointToScreen(new Point(0, 0));
            Point max = PointToScreen(new Point(Width, Height));
            if (m.X >= min.X && m.X <= max.X &&
                m.Y >= min.Y && m.Y <= max.Y &&
                !suspendInput &&
                Focused)
            {
                //translated mouse pos
                Point[] p = new Point[] { PointToClient(new Point(m.X, m.Y)) };
                transformInv.TransformPoints(p);

                if (!lastClickedL)
                {
                    if (m.LeftButton == OpenTK.Input.ButtonState.Pressed)
                    {
                        OnClickL(p[0].X, p[0].Y);
                        lastClickedL = true;
                    }
                    else lastClickedL = false;
                }
                if (m.LeftButton == OpenTK.Input.ButtonState.Pressed)
                {
                    OnMouseHeldL(p[0].X, p[0].Y);
                }
                if (lastClickedL && m.LeftButton == OpenTK.Input.ButtonState.Released)
                {
                    OnMouseReleasedL(p[0].X, p[0].Y);
                }
                if (m.LeftButton == OpenTK.Input.ButtonState.Released)
                {
                    lastClickedL = false;
                }

                if (!lastClickedR)
                {
                    if (m.RightButton == OpenTK.Input.ButtonState.Pressed)
                    {
                        OnClickR(p[0].X, p[0].Y);
                        lastClickedR = true;
                    }
                    else lastClickedR = false;
                }
                if (m.RightButton == OpenTK.Input.ButtonState.Released)
                {
                    lastClickedR = false;
                }

                //zoom
                if (m.MiddleButton == OpenTK.Input.ButtonState.Pressed)
                {
                    x += (m.X - (int)lastX) * 1 / zoom;
                    y += (m.Y - (int)lastY) * 1 / zoom;
                }
                zoom += (m.Scroll.Y - lastM) / 50;
                zoom = zoom < zoomMin ? zoomMin : zoom;
                zoom = zoom > zoomMax ? zoomMax : zoom;
            }
            else
            {
                if (m.LeftButton == OpenTK.Input.ButtonState.Pressed)
                {
                    suspendInput = true;
                }
                else
                    suspendInput = false;
            }

            //must be done after the entire poll.
            lastX = m.X;
            lastY = m.Y;
            lastM = m.Scroll.Y;
        }
        void OnClickL(int mx, int my)
        {
            marqueeX1 = mx;
            marqueeY1 = my;
            marqueeX2 = mx;
            marqueeY2 = my;

            //reverse nodes so that the last to draw is on top
            var nodesReversed = nodes.ToList();
            nodesReversed.Reverse();

            //dragging socket
            foreach (TriggerscripterNode n in nodesReversed)
            {
                foreach (TriggerscripterSocket s in n.sockets.Values)
                {
                    if (s.PointIsIn(mx, my))
                    {
                        mouseState = CurrentMouseState.DraggingSocket;
                        if (!s.multiConnection)
                        {
                            if (s.connectedSockets.Count > 0)
                            {
                                selectedSocket = s.connectedSockets[0];
                                if (s is TriggerscripterSocket_Input)
                                {
                                    ((TriggerscripterSocket_Output)s.connectedSockets[0]).Disconnect((TriggerscripterSocket_Input)s);
                                }
                                else
                                {
                                    ((TriggerscripterSocket_Output)s).Disconnect((TriggerscripterSocket_Input)s.connectedSockets[0]);
                                }
                                return;
                            }
                        }

                        //deselect all nodes and select the socket
                        foreach (TriggerscripterNode n2 in nodesReversed)
                            n2.selected = false;
                        selectedSocket = s;
                        return;
                    }
                }
            }
            
            //doing nothing
            if (mouseState == CurrentMouseState.None)
            {
                foreach (TriggerscripterNode n in nodesReversed)
                {
                    //if mouse is in this node
                    if (n.PointIsIn(mx, my))
                    {
                        int ox, oy;
                        n.GetPointOffset(mx, my, out ox, out oy);
                        n.selectedX = ox;
                        n.selectedY = oy;
                        nodes.Remove(n);
                        nodes.Add(n);

                        foreach (TriggerscripterNode n2 in nodesReversed)
                            n2.selected = false;
                        n.selected = true;
                        OnNodeSelected(n);
                        
                        mouseState = CurrentMouseState.NodesSelected;
                        return;
                    }
                }
            }

            //node(s) selected
            if (mouseState == CurrentMouseState.NodesSelected)
            {
                bool somethingUnderMouse = false;
                bool selectedUnderMouse = false;
                foreach (TriggerscripterNode n in nodesReversed)
                {
                    //set select offset for this node
                    int ox, oy;
                    n.GetPointOffset(mx, my, out ox, out oy);
                    n.selectedX = ox;
                    n.selectedY = oy;

                    //determine if a selected node is under the mouse
                    if(n.selected && n.PointIsIn(mx, my))
                    {
                        selectedUnderMouse = true;
                    }

                    //if this node is not selected, but the mouse is inside the node, and the mouse is not in a selected node
                    if (!n.selected && n.PointIsIn(mx, my) && !selectedUnderMouse)
                    {
                        foreach (TriggerscripterNode n2 in nodesReversed)
                        {
                            n2.selected = false;
                        }
                        n.selected = true;

                        nodes.Remove(n);
                        nodes.Add(n);
                    }

                    //determine if there is anything under the mouse
                    if (n.PointIsIn(mx, my))
                        somethingUnderMouse = true;

                    if(n.selected)
                    {
                        //select the node
                        OnNodeSelected(n);
                    }
                }
                //nothing was under the mouse, deselect all and set state
                if (!somethingUnderMouse)
                {
                    foreach (TriggerscripterNode n in nodes)
                        n.selected = false;
                    mouseState = CurrentMouseState.None;
                    OnDeselect();
                }
            }

            //if there is no state still, start dragging marquee
            if (mouseState == CurrentMouseState.None)
            {
                foreach (TriggerscripterNode n in nodes)
                    n.selected = false;

                mouseState = CurrentMouseState.DraggingMarquee;
                marqueeX1 = mx;
                marqueeY1 = my;
                OnDeselect();
            }
        }
        void OnMouseHeldL(int mx, int my)
        {
            var nodesReversed = nodes.ToList();
            nodesReversed.Reverse();

            //nodes are selected, drag them
            if (mouseState == CurrentMouseState.NodesSelected)
            {
                foreach (TriggerscripterNode n in nodesReversed)
                {
                    if(n.selected)
                    {
                        n.SetPos(mx - n.selectedX, my - n.selectedY);
                        OnEdit?.Invoke(this, null);
                    }
                }
            }
            //marquee is dragging, update it
            if (mouseState == CurrentMouseState.DraggingMarquee)
            {
                marqueeX2 = mx;
                marqueeY2 = my;
                foreach (TriggerscripterNode n in nodesReversed)
                {
                    if (n.IsInRect(
                    Math.Min(marqueeX1, marqueeX2),
                    Math.Min(marqueeY1, marqueeY2),
                    Math.Abs(marqueeX2 - marqueeX1),
                    Math.Abs(marqueeY2 - marqueeY1)))
                    {
                        n.selected = true;
                        nodes.Remove(n);
                        nodes.Add(n);
                    }
                }
            }
        }
        void OnMouseReleasedL(int mx, int my)
        {
            //marquee was dragging. select nodes inside it
            if (mouseState == CurrentMouseState.DraggingMarquee)
            {
                foreach (TriggerscripterNode n in nodes)
                {
                    if (n.IsInRect(
                    Math.Min(marqueeX1, marqueeX2),
                    Math.Min(marqueeY1, marqueeY2),
                    Math.Abs(marqueeX2 - marqueeX1),
                    Math.Abs(marqueeY2 - marqueeY1)))
                    {
                        mouseState = CurrentMouseState.NodesSelected;
                    }
                }
                if (mouseState != CurrentMouseState.NodesSelected)
                    mouseState = CurrentMouseState.None;
            }
            //socket was dragging
            if (mouseState == CurrentMouseState.DraggingSocket)
            {
                //foreach node
                foreach(TriggerscripterNode n in nodes)
                {
                    //foreach socket in that node
                    foreach(TriggerscripterSocket s in n.sockets.Values)
                    {
                        if (s == selectedSocket) break;
                        //if mouse was in that socket
                        if (s.PointIsIn(mx,my))
                        {
                            //if selected socket is output
                            if(selectedSocket is TriggerscripterSocket_Output)
                            {
                                //if this socket is input (compatible)
                                if (s is TriggerscripterSocket_Input)
                                {
                                    ((TriggerscripterSocket_Output)selectedSocket).Connect(s as TriggerscripterSocket_Input);
                                    OnEdit?.Invoke(this, null);
                                    goto DraggingSocketFinish;
                                }
                            }
                            //selected socket is input
                            else
                            {
                                //if this socket is output (compatible)
                                if (s is TriggerscripterSocket_Output)
                                {
                                    ((TriggerscripterSocket_Output)s).Connect(selectedSocket as TriggerscripterSocket_Input);
                                    OnEdit?.Invoke(this, null);
                                    goto DraggingSocketFinish;
                                }
                            }
                        }
                    }
                }
DraggingSocketFinish:
                mouseState = CurrentMouseState.None;
                selectedSocket = null;
            }
        }
        void OnClickR(int mx, int my)
        {
        }

        void OnNodeSelected(TriggerscripterNode n)
        {

        }
        void OnDeselect()
        {

        }
        #endregion

        #region creation
        public static Color requiredVarColor = Color.FromArgb(255, 64, 130, 64);
        public static Color optionalVarColor = Color.FromArgb(255, 122, 130, 64);
        public static Color cndColor = Color.FromArgb(255, 130, 64, 64);
        public static Color trgColor = Color.FromArgb(255, 64, 117, 130);
        public static Color effColor = Color.FromArgb(255, 130, 64, 106);

        Point nodeAddLocation = new Point();
        public void CaptureMousePos(object o, EventArgs e)
        {
            nodeAddLocation = PointToClient(new Point(
                OpenTK.Input.Mouse.GetCursorState().X,
                OpenTK.Input.Mouse.GetCursorState().Y));

            Point[] nodeAddLocationArr = new Point[] { nodeAddLocation };
            transformInv.TransformPoints(nodeAddLocationArr);
            nodeAddLocation = nodeAddLocationArr[0];
        }
        
        //node id trackers
        public int trgID = 0, varID = 0;
        public int cndID = 0, effID = 0;

        //context menu callbacks
        public void CreateNewTriggerPressed(object o, EventArgs e)
        {
            SerializedTrigger t = new SerializedTrigger();
            t.name = "NewTrigger" + trgID;
            CreateTriggerNode(t, trgID, nodeAddLocation.X, nodeAddLocation.Y);
            trgID++;
        }
        public void CreateNewVarPressed(object o, EventArgs e)
        {
            SerializedVariable var = new SerializedVariable();
            var.type = (string)((MenuItem)o).Tag;
            var.name = "New" + var.type + varID.ToString();
            var.value = "";
            CreateVarNode(var, varID, nodeAddLocation.X, nodeAddLocation.Y);
            varID++;
        }
        public void CreateNewEffectPressed(object o, EventArgs e)
        {
            CreateEffectNode((Effect)((MenuItem)o).Tag, effID++, nodeAddLocation.X, nodeAddLocation.Y);
        }
        public void CreateNewConditionPressed(object o, EventArgs e)
        {
            CreateConditionNode((Condition)((MenuItem)o).Tag, cndID++, nodeAddLocation.X, nodeAddLocation.Y);
        }

        //actual creation functions
        public TriggerscripterNode CreateTriggerNode(SerializedTrigger t, int id, int x, int y)
        {
            TriggerscripterNode_Trigger n = new TriggerscripterNode_Trigger(this, x, y);
            n.headerColor = trgColor;

            n.id = id;
            n.data = t;

            n.Active = t.active;
            n.ConditionalOr = t.cndIsOr;
            n.Name = t.name;

            n.id = id;
            nodes.Add(n);
            return n;
        }
        public TriggerscripterNode CreateVarNode(SerializedVariable v, int id, int x, int y)
        {
            TriggerscripterNode_Variable n = new TriggerscripterNode_Variable(this, x, y);
            n.headerColor = requiredVarColor;

            n.data = v;
            n.id = id;

            n.nodeTitle = v.name;
            n.Name = v.name;
            n.Value = v.value;

            n.typeTitle = v.type;
            n.AddSocket(true, "Set", v.type, requiredVarColor, false);
            n.AddSocket(false, "Use", v.type, requiredVarColor, false);

            n.bottomPadding = 50;

            nodes.Add(n);
            return n;
        }
        public TriggerscripterNode CreateEffectNode(Effect e, int id, int x, int y)
        {
            TriggerscripterNode n = new TriggerscripterNode(this, x, y);
            n.headerColor = effColor;

            n.data = e;
            n.id = id;

            n.nodeTitle = e.name + " v" + e.version;
            n.typeTitle = "Effect";
            n.handleAs = "Effect";
            n.AddSocket(true, "Caller", "EFF", effColor, false, false);
            n.AddSocket(false, "Call", "EFF", effColor, false, false);

            if (!e.name.Contains("Trigger"))
            {
                foreach (Input i in e.inputs)
                {
                    Color color = i.optional ? optionalVarColor : requiredVarColor;
                    n.AddSocket(true, i.name, i.valueType, color, true, false);
                }
                foreach (Output ou in e.outputs)
                {
                    Color color = ou.optional ? optionalVarColor : requiredVarColor;
                    n.AddSocket(false, ou.name, ou.valueType, color, true, false);
                }
            }
            else
            {
                n.AddSocket(false, "Trigger", "TRG", trgColor, false, false);
            }

            nodes.Add(n);

            return n;
        }
        public TriggerscripterNode CreateConditionNode(Condition c, int id, int x, int y)
        {
            TriggerscripterNode_Condition n = new TriggerscripterNode_Condition(this, x, y);
            n.headerColor = cndColor;

            n.data = c;
            n.id = id;
            n.nodeTitle = c.name + " v" + c.version;
            n.handleAs = "Condition";

            n.AddSocket(false, "Result", "CND", cndColor, false, false);

            foreach (Input i in c.inputs)
            {
                Color color = i.optional ? optionalVarColor : requiredVarColor;
                n.AddSocket(true, i.name, i.valueType, color, true, false);
            }
            foreach (Output ou in c.outputs)
            {
                Color color = ou.optional ? optionalVarColor : requiredVarColor;
                n.AddSocket(false, ou.name, ou.valueType, color, true, false);
            }

            nodes.Add(n);

            return n;
        }
        #endregion

        #region serialization
        public SerializedTriggerscripter GetSerializedGraph()
        {
            SerializedTriggerscripter sts = new SerializedTriggerscripter();
            foreach (TriggerscripterNode n in nodes)
            {
                SerializableNode sn = new SerializableNode();
                sn.handleAs = n.handleAs;
                sn.selected = n.selected;
                if (n.handleAs == "Trigger")
                {
                    SerializedTrigger t = new SerializedTrigger();
                    t.active = ((TriggerscripterNode_Trigger)n).Active;
                    t.cndIsOr = ((TriggerscripterNode_Trigger)n).Conditional;
                    t.name = ((TriggerscripterNode_Trigger)n).Name;
                    sn.trigger = t;
                }
                if (n.handleAs == "Variable")
                {
                    SerializedVariable v = new SerializedVariable();
                    v.value = ((TriggerscripterNode_Variable)n).Value;
                    v.name = ((TriggerscripterNode_Variable)n).Name;
                    v.type = n.typeTitle;
                    sn.variable = v;
                }
                if (n.handleAs == "Effect")
                {
                    Effect e = (Effect)n.data;
                    sn.effect = e;
                }
                if (n.handleAs == "Condition")
                {
                    Condition c = (Condition)n.data;
                    Console.WriteLine(n.id);
                    sn.condition = c;
                }

                foreach (TriggerscripterSocket os in n.sockets.Values)
                {
                    if (os is TriggerscripterSocket_Output)
                    {
                        foreach (TriggerscripterSocket s in os.connectedSockets)
                        {
                            SerializedNodeLink link = new SerializedNodeLink();
                            link.sourceId = n.id;
                            link.sourceSocketName = os.text;

                            if (n is TriggerscripterNode_Variable)
                                link.sourceType = "Variable";
                            else
                                link.sourceType = n.typeTitle;

                            link.targetId = s.node.id;
                            link.targetSocketName = s.text;
                            if (s.node is TriggerscripterNode_Variable)
                                link.targetType = "Variable";
                            else
                                link.targetType = s.node.typeTitle;

                            sts.links.Add(link);
                        }


                    }
                }

                sn.x = n.x;
                sn.y = n.y;
                sn.id = n.id;

                sts.nodes.Add(sn);
            }
            sts.lastTrg = trgID++;
            sts.lastVar = varID++;
            sts.lastEff = effID++;
            sts.lastCnd = cndID++;
            return sts;
        }

        //TODO: rename -- this does not take a file as input.
        public bool Load(SerializedTriggerscripter sts)
        {
            nodes.Clear();
            Dictionary<int, TriggerscripterNode> triggers = new Dictionary<int, TriggerscripterNode>();
            Dictionary<int, TriggerscripterNode> variables = new Dictionary<int, TriggerscripterNode>();
            Dictionary<int, TriggerscripterNode> effects = new Dictionary<int, TriggerscripterNode>();
            Dictionary<int, TriggerscripterNode> conditions = new Dictionary<int, TriggerscripterNode>();

            trgID = sts.lastTrg;
            varID = sts.lastVar;
            effID = sts.lastEff;
            cndID = sts.lastCnd;

            foreach (SerializableNode n in sts.nodes)
            {
                try
                {
                    if (n.handleAs == "Trigger")
                    {
                        triggers.Add(n.id, CreateTriggerNode(n.trigger, n.id, n.x, n.y));
                    }
                    if (n.handleAs == "Variable")
                    {
                        variables.Add(n.id, CreateVarNode(n.variable, n.id, n.x, n.y));
                    }
                    if (n.handleAs == "Effect")
                    {
                        effects.Add(n.id, CreateEffectNode(n.effect, n.id, n.x, n.y));
                    }
                    if (n.handleAs == "Condition")
                    {
                        conditions.Add(n.id, CreateConditionNode(n.condition, n.id, n.x, n.y));
                    }
                }
                catch
                {
                    return false;
                }
            }
            
            foreach (SerializedNodeLink l in sts.links)
            {
                try
                {
                    if (l.sourceType == "Trigger")
                    {
                        switch (l.targetType)
                        {
                            case "Effect":
                                ((TriggerscripterSocket_Output)triggers[l.sourceId].sockets[l.sourceSocketName]).Connect((TriggerscripterSocket_Input)effects[l.targetId].sockets[l.targetSocketName]);
                                break;
                            default:
                                break;
                        }
                    }
                    if (l.sourceType == "Effect")
                    {
                        switch (l.targetType)
                        {
                            case "Effect":
                                ((TriggerscripterSocket_Output)effects[l.sourceId].sockets[l.sourceSocketName]).Connect((TriggerscripterSocket_Input)effects[l.targetId].sockets[l.targetSocketName]);
                                break;
                            case "Condition":
                                ((TriggerscripterSocket_Output)effects[l.sourceId].sockets[l.sourceSocketName]).Connect((TriggerscripterSocket_Input)conditions[l.targetId].sockets[l.targetSocketName]);
                                break;
                            case "Variable":
                                ((TriggerscripterSocket_Output)effects[l.sourceId].sockets[l.sourceSocketName]).Connect((TriggerscripterSocket_Input)variables[l.targetId].sockets[l.targetSocketName]);
                                break;
                            case "Trigger":
                                ((TriggerscripterSocket_Output)effects[l.sourceId].sockets[l.sourceSocketName]).Connect((TriggerscripterSocket_Input)triggers[l.targetId].sockets[l.targetSocketName]);
                                break;
                            default:
                                break;
                        }
                    }
                    if (l.sourceType == "Condition")
                    {
                        switch (l.targetType)
                        {
                            case "Effect":
                                ((TriggerscripterSocket_Output)conditions[l.sourceId].sockets[l.sourceSocketName]).Connect((TriggerscripterSocket_Input)effects[l.targetId].sockets[l.targetSocketName]);
                                break;
                            case "Condition":
                                ((TriggerscripterSocket_Output)conditions[l.sourceId].sockets[l.sourceSocketName]).Connect((TriggerscripterSocket_Input)conditions[l.targetId].sockets[l.targetSocketName]);
                                break;
                            case "Variable":
                                ((TriggerscripterSocket_Output)conditions[l.sourceId].sockets[l.sourceSocketName]).Connect((TriggerscripterSocket_Input)variables[l.targetId].sockets[l.targetSocketName]);
                                break;
                            case "Trigger":
                                ((TriggerscripterSocket_Output)conditions[l.sourceId].sockets[l.sourceSocketName]).Connect((TriggerscripterSocket_Input)triggers[l.targetId].sockets[l.targetSocketName]);
                                break;
                            default:
                                break;
                        }
                    }
                    if (l.sourceType == "Variable")
                    {
                        switch (l.targetType)
                        {
                            case "Effect":
                                ((TriggerscripterSocket_Output)variables[l.sourceId].sockets[l.sourceSocketName]).Connect((TriggerscripterSocket_Input)effects[l.targetId].sockets[l.targetSocketName]);
                                break;
                            case "Condition":
                                ((TriggerscripterSocket_Output)variables[l.sourceId].sockets[l.sourceSocketName]).Connect((TriggerscripterSocket_Input)conditions[l.targetId].sockets[l.targetSocketName]);
                                break;
                            default:
                                break;
                        }
                    }
                }
                catch 
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region copy/paste
        int pasteOffset = 50;
        SerializedTriggerscripter copyBuffer = new SerializedTriggerscripter();
        public void CopyGraph()
        {
            copyBuffer = GetSerializedGraph();
        }
        public void PasteGraph()
        {
            foreach (TriggerscripterNode n in nodes)
                n.selected = false;

            Dictionary<int, TriggerscripterNode> trgMap = new Dictionary<int, TriggerscripterNode>();
            Dictionary<int, TriggerscripterNode> effMap = new Dictionary<int, TriggerscripterNode>();
            Dictionary<int, TriggerscripterNode> varMap = new Dictionary<int, TriggerscripterNode>();
            Dictionary<int, TriggerscripterNode> cndMap = new Dictionary<int, TriggerscripterNode>();
            foreach (SerializableNode sn in copyBuffer.nodes)
            {
                if(sn.selected)
                {
                    if(sn.handleAs == "Trigger")
                    {
                        TriggerscripterNode n = CreateTriggerNode(sn.trigger, trgID, sn.x - pasteOffset, sn.y - pasteOffset);
                        n.selected = true;
                        trgMap.Add(sn.id, n);
                        trgID++;
                    }
                    if(sn.handleAs == "Effect")
                    {
                        TriggerscripterNode n = CreateEffectNode(sn.effect, effID, sn.x - pasteOffset, sn.y - pasteOffset);
                        n.selected = true;
                        effMap.Add(sn.id, n);
                        effID++;
                    }
                    if (sn.handleAs == "Condition")
                    {
                        TriggerscripterNode n = CreateConditionNode(sn.condition, cndID, sn.x - pasteOffset, sn.y - pasteOffset);
                        n.selected = true;
                        cndMap.Add(sn.id, n);
                        cndID++;
                    }
                    if (sn.handleAs == "Variable")
                    {
                        TriggerscripterNode n = CreateVarNode(sn.variable, varID, sn.x - pasteOffset, sn.y - pasteOffset);
                        n.selected = true;
                        varMap.Add(sn.id, n);
                        varID++;
                    }
                }
            }
            foreach (SerializedNodeLink sl in copyBuffer.links)
            {
                try
                {
                    if (sl.sourceType == "Trigger")
                    {
                        if (sl.targetType == "Effect")
                        {
                            ((TriggerscripterSocket_Output)trgMap[sl.sourceId].sockets[sl.sourceSocketName]).Connect(
                                (TriggerscripterSocket_Input)effMap[sl.targetId].sockets[sl.targetSocketName]);
                        }
                    }

                    if (sl.sourceType == "Effect")
                    {
                        if (sl.targetType == "Variable")
                        {
                            ((TriggerscripterSocket_Output)effMap[sl.sourceId].sockets[sl.sourceSocketName]).Connect(
                                (TriggerscripterSocket_Input)varMap[sl.targetId].sockets[sl.targetSocketName]);
                        }
                        if (sl.targetType == "Effect")
                        {
                            ((TriggerscripterSocket_Output)effMap[sl.sourceId].sockets[sl.sourceSocketName]).Connect(
                                (TriggerscripterSocket_Input)effMap[sl.targetId].sockets[sl.targetSocketName]);
                        }
                        if (sl.targetType == "Condition")
                        {
                            ((TriggerscripterSocket_Output)effMap[sl.sourceId].sockets[sl.sourceSocketName]).Connect(
                                (TriggerscripterSocket_Input)cndMap[sl.targetId].sockets[sl.targetSocketName]);
                        }
                        if (sl.targetType == "Trigger")
                        {
                            ((TriggerscripterSocket_Output)effMap[sl.sourceId].sockets[sl.sourceSocketName]).Connect(
                                (TriggerscripterSocket_Input)trgMap[sl.targetId].sockets[sl.targetSocketName]);
                        }
                    }

                    if (sl.sourceType == "Condition")
                    {
                        if (sl.targetType == "Variable")
                        {
                            ((TriggerscripterSocket_Output)cndMap[sl.sourceId].sockets[sl.sourceSocketName]).Connect(
                                (TriggerscripterSocket_Input)varMap[sl.targetId].sockets[sl.targetSocketName]);
                        }
                        if (sl.targetType == "Effect")
                        {
                            ((TriggerscripterSocket_Output)cndMap[sl.sourceId].sockets[sl.sourceSocketName]).Connect(
                                (TriggerscripterSocket_Input)effMap[sl.targetId].sockets[sl.targetSocketName]);
                        }
                        if (sl.targetType == "Condition")
                        {
                            ((TriggerscripterSocket_Output)cndMap[sl.sourceId].sockets[sl.sourceSocketName]).Connect(
                                (TriggerscripterSocket_Input)cndMap[sl.targetId].sockets[sl.targetSocketName]);
                        }
                        if (sl.targetType == "Trigger")
                        {
                            ((TriggerscripterSocket_Output)cndMap[sl.sourceId].sockets[sl.sourceSocketName]).Connect(
                                (TriggerscripterSocket_Input)trgMap[sl.targetId].sockets[sl.targetSocketName]);
                        }
                    }

                    if (sl.sourceType == "Variable")
                    {
                        if (sl.targetType == "Effect")
                        {
                            ((TriggerscripterSocket_Output)varMap[sl.sourceId].sockets[sl.sourceSocketName]).Connect(
                                (TriggerscripterSocket_Input)effMap[sl.targetId].sockets[sl.targetSocketName]);
                        }
                        if (sl.targetType == "Condition")
                        {
                            ((TriggerscripterSocket_Output)varMap[sl.sourceId].sockets[sl.sourceSocketName]).Connect(
                                (TriggerscripterSocket_Input)cndMap[sl.targetId].sockets[sl.targetSocketName]);
                        }
                    }
                }
                catch { }
            mouseState = CurrentMouseState.NodesSelected;
            }
        }
        #endregion

        #region import from a .triggerscript
        public void ImportScript(string file)
        {
            XDocument doc = XDocument.Load(file);
            XElement vars = doc.Element("TriggerSystem").Element("TriggerVars");
            XElement triggers = doc.Element("TriggerSystem").Element("Trigger");

        }
        #endregion
        #endregion
    }
}
