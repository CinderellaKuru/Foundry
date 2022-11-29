using hwFoundry.Modules.TriggerScripter;
using hwFoundry.Modules.TriggerScripter.Nodes;
using hwFoundry.Properties;
using Newtonsoft.Json;
using ST.Library.UI.NodeEditor;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

using ContextMenu = System.Windows.Forms.ContextMenuStrip;
using MenuItem = System.Windows.Forms.ToolStripMenuItem;

namespace hwFoundry.GUI
{
    public partial class TriggerScripter : DockContent
    {
        // Members
        public bool IsDead { get; set; }
        public ContextMenu CM;
        private static readonly Dictionary<string, string> nodeSubCategories = new()
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

        // Node ID Trackers
        public int trgID = 0, varID = 0;
        public int cndID = 0, effID = 0;

        // Constructor
        public TriggerScripter()
        {
            InitializeComponent();
            CM = LoadContextMenuItems();
            ContextMenuStrip = CM;
            //nodeEditor.ContextMenuStrip.Opening += ContextMenuStrip_Opening;
        }

        private void ContextMenuStrip_Opening(object? sender, CancelEventArgs e)
            => e.Cancel = nodeEditor.ActiveNode != null;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            nodeEditor.ActiveChanged += (s, ea) => Program.mainWindow.propertyEditor.SetSelectedObject(nodeEditor.ActiveNode);
            nodeEditor.OptionConnected += (s, ea) => nodeEditor.ShowAlert(ea.Status.ToString(), Color.White, ea.Status == ConnectionStatus.Connected ? Color.FromArgb(125, Color.Green) : Color.FromArgb(125, Color.Red));
            nodeEditor.CanvasScaled += (s, ea) => nodeEditor.ShowAlert($"x{nodeEditor.CanvasScale:F2}", Color.White, Color.FromArgb(125, Color.Yellow));
            nodeEditor.NodeAdded += (s, ea) => ea.Node.ContextMenuStrip = nodeContextMenuStrip;
            nodeEditor.KeyDown += HotKeyHandler;
        }


        public void SaveToFile(string path)
        {
            JsonSerializerSettings settings = new()
            {
                TypeNameHandling = TypeNameHandling.None,
                Formatting       = Formatting.Indented
            };

            File.WriteAllText(path, JsonConvert.SerializeObject(GetSerializedGraph(), settings));
        }

        public void LoadFromFile(SerializedGraph graph)
            => LoadSerializedGraph(graph, true);

        public SerializedGraph GetSerializedGraph(bool onlyGetSelected = false)
        {
            SerializedGraph graph = new();
            Point center = GetCenterOfSelection(onlyGetSelected ? nodeEditor.GetSelectedNode() : nodeEditor.Nodes.ToArray());
            foreach (BaseNode node in (onlyGetSelected ? nodeEditor.GetSelectedNode() : nodeEditor.Nodes.ToArray()))
            {
                SerializedNode sNode = new()
                {
                    handleAs = node.handleAs,
                    selected = node.IsSelected
                };

                switch (node.handleAs)
                {
                    case "Trigger":
                        SerializedTrigger trg = new()
                        {
                            active  = ((TriggerNode)node).Active,
                            cndIsOr = ((TriggerNode)node).Conditional,
                            name    = ((TriggerNode)node).Name
                        };
                        sNode.trigger = trg;
                        break;

                    case "Variable":
                        SerializedVariable var = new()
                        {
                            value = ((VariableNode)node).Value,
                            name  = ((VariableNode)node).Name,
                            type  = node.typeTitle
                        };
                        sNode.variable = var;
                        break;

                    case "Effect":
                        SerializedEffect eff = (SerializedEffect)node.data;
                        sNode.effect = eff;
                        break;

                    case "Condition":
                        SerializedCondition cnd = (SerializedCondition)node.data;
                        sNode.condition = cnd;
                        break;
                }

                foreach (STNodeOption socket in node.OutputOptions)
                {
                    foreach (STNodeOption link in socket.GetConnectedSockets())
                    {
                        SerializedNodeLink sLink = new()
                        {
                            sourceId = node.id,
                            sourceSocketName = socket.Text,
                            sourceType = node.GetType() == typeof(VariableNode) ? "Variable" : node.typeTitle,

                            targetId = ((BaseNode)link.Owner).id,
                            targetSocketName = link.Text,
                            targetType = link.Owner.GetType() == typeof(VariableNode) ? "Variable" : ((BaseNode)link.Owner).typeTitle
                        };
                        graph.links.Add(sLink);
                    }
                }

                // Set node location and ID
                sNode.x  = center.X - node.Location.X;
                sNode.y  = center.Y - node.Location.Y;
                sNode.id = node.id;

                // Add the node to the serialized graph
                graph.nodes.Add(sNode);
            }

            // Set all new IDs
            graph.lastTrg = trgID++;
            graph.lastVar = varID++;
            graph.lastEff = effID++;
            graph.lastCnd = cndID++;
            return graph;
        }

        private void LoadSerializedGraph(SerializedGraph graph, bool fromFile = false)
        {
            // Deselect everything
            foreach (STNode node in nodeEditor.GetSelectedNode())
                nodeEditor.RemoveSelectedNode(node);

            if (fromFile)
            {
                // Clear graph and set last used IDs
                nodeEditor.Nodes.Clear();
                trgID = graph.lastTrg;
                varID = graph.lastVar;
                effID = graph.lastEff;
                cndID = graph.lastCnd;
            }

            // Load all copied nodes
            Point pasteOffset = GetMouseCoords();
            Dictionary<int, BaseNode> trgMap = new(), effMap = new(), varMap = new(), cndMap = new();
            foreach (SerializedNode sNode in graph.nodes)
            {
                // Set location of new point
                Point newNodeLocation = fromFile ? new Point(sNode.x, sNode.y) :
                    new(pasteOffset.X - sNode.x, pasteOffset.Y - sNode.y);

                // Add appropriate nodes to graph
                if (sNode.selected || fromFile)
                {
                    switch (sNode.handleAs)
                    {
                        case "Trigger":
                            trgMap.Add(sNode.id, CreateTriggerNode(sNode.trigger,
                                fromFile ? sNode.id : trgID++, newNodeLocation.X, newNodeLocation.Y));
                            break;

                        case "Effect":
                            effMap.Add(sNode.id, CreateEffectNode(sNode.effect,
                                fromFile ? sNode.id : effID++, newNodeLocation.X, newNodeLocation.Y));
                            break;

                        case "Condition":
                            cndMap.Add(sNode.id, CreateConditionNode(sNode.condition,
                                fromFile ? sNode.id : cndID++, newNodeLocation.X, newNodeLocation.Y));
                            break;

                        case "Variable":
                            varMap.Add(sNode.id, CreateVarNode(sNode.variable,
                                fromFile ? sNode.id : varID++, newNodeLocation.X, newNodeLocation.Y));
                            break;
                    }
                }
            }

            // Add nodes to graph (has to come before the connections)
            nodeEditor.Nodes.AddRange(trgMap.Values.ToArray());
            nodeEditor.Nodes.AddRange(effMap.Values.ToArray());
            nodeEditor.Nodes.AddRange(varMap.Values.ToArray());
            nodeEditor.Nodes.AddRange(cndMap.Values.ToArray());

            if (!fromFile)
            {
                // Make all added nodes selected
                nodeEditor.AddSelectedNodeRange(trgMap.Values.ToArray());
                nodeEditor.AddSelectedNodeRange(effMap.Values.ToArray());
                nodeEditor.AddSelectedNodeRange(varMap.Values.ToArray());
                nodeEditor.AddSelectedNodeRange(cndMap.Values.ToArray());
            }

            // Form all stored connections
            foreach (SerializedNodeLink sLink in graph.links)
            {
                try
                {
                    switch (sLink.sourceType)
                    {
                        case "Trigger":
                            if (sLink.targetType == "Effect")
                                trgMap[sLink.sourceId].Sockets[sLink.sourceSocketName].ConnectOption(
                                    effMap[sLink.targetId].Sockets[sLink.targetSocketName]);
                            break;


                        case "Variable":
                            if (sLink.targetType == "Effect")
                                varMap[sLink.sourceId].Sockets[sLink.sourceSocketName].ConnectOption(
                                    effMap[sLink.targetId].Sockets[sLink.targetSocketName]);
                            
                            else if (sLink.targetType == "Condition")
                                varMap[sLink.sourceId].Sockets[sLink.sourceSocketName].ConnectOption(
                                    cndMap[sLink.targetId].Sockets[sLink.targetSocketName]);
                            break;


                        case "Effect":
                            if (sLink.targetType == "Variable")
                                effMap[sLink.sourceId].Sockets[sLink.sourceSocketName].ConnectOption(
                                    varMap[sLink.targetId].Sockets[sLink.targetSocketName]);
                            
                            else if (sLink.targetType == "Effect")
                                effMap[sLink.sourceId].Sockets[sLink.sourceSocketName].ConnectOption(
                                    effMap[sLink.targetId].Sockets[sLink.targetSocketName]);
                            
                            else if (sLink.targetType == "Condition")
                                effMap[sLink.sourceId].Sockets[sLink.sourceSocketName].ConnectOption(
                                    cndMap[sLink.targetId].Sockets[sLink.targetSocketName]);
                            
                            else if (sLink.targetType == "Trigger")
                                effMap[sLink.sourceId].Sockets[sLink.sourceSocketName].ConnectOption(
                                    trgMap[sLink.targetId].Sockets[sLink.targetSocketName]);
                            break;


                        case "Condition":
                            if (sLink.targetType == "Variable")
                                cndMap[sLink.sourceId].Sockets[sLink.sourceSocketName].ConnectOption(
                                    varMap[sLink.targetId].Sockets[sLink.targetSocketName]);

                            else if (sLink.targetType == "Effect")
                                cndMap[sLink.sourceId].Sockets[sLink.sourceSocketName].ConnectOption(
                                    effMap[sLink.targetId].Sockets[sLink.targetSocketName]);

                            else if (sLink.targetType == "Condition")
                                cndMap[sLink.sourceId].Sockets[sLink.sourceSocketName].ConnectOption(
                                    cndMap[sLink.targetId].Sockets[sLink.targetSocketName]);

                            else if (sLink.targetType == "Trigger")
                                cndMap[sLink.sourceId].Sockets[sLink.sourceSocketName].ConnectOption(
                                    trgMap[sLink.targetId].Sockets[sLink.targetSocketName]);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    nodeEditor.ShowAlert(ex.Message, Color.White, Color.FromArgb(125, Color.Red));
                }
            }
        }

        #region Utility

        // General
        private Point GetMouseCoords()
            => nodeEditor.ControlToCanvas(PointToClient(MousePosition));

        // Node Generation
        public static TriggerNode CreateTriggerNode(SerializedTrigger trg, int id, int x, int y)
            => new(trg, id, x, y);

        public static VariableNode CreateVarNode(SerializedVariable var, int id, int x, int y)
            => new(var, id, x, y);

        public static EffectNode CreateEffectNode(SerializedEffect eff, int id, int x, int y)
            => new(eff, id, x, y);

        public static ConditionNode CreateConditionNode(SerializedCondition cnd, int id, int x, int y)
            => new(cnd, id, x, y);

        public static Point GetCenterOfSelection(STNode[] nodes)
        {
            Point center = new(0,0);
            foreach (STNode node in nodes)
            {
                center.X += node.Location.X;
                center.Y += node.Location.Y;
            }
            return new Point(center.X/nodes.Length, center.Y/nodes.Length);
        }

        public static Point GetTopLeftOfSelection(STNode[] nodes)
        {
            // smallest X, largest Y
            STNode[] orderByX = nodes.OrderBy(x => x.Location.X).ToArray();
            STNode[] orderByY = nodes.OrderBy(x => x.Location.Y).ToArray();
            return new Point(orderByX.Last().Location.X, orderByY.First().Location.Y);
        }

        // Context Menu Stuff
        private static void BuildSubCM(dynamic sortedList, string concatStr, ref Dictionary<string, MenuItem> hierarchy, EventHandler clickHandler, bool isVar = false)
        {
            // Loop through each item in the provided list
            foreach (var sortedItem in sortedList)
            {
                // Each MenuItem is labeled by a specific path that is concatenated
                string concat = concatStr + '|';
                MenuItem currentMI = hierarchy[concat];
                string subCats = isVar ? nodeSubCategories[sortedItem + "Var"] : sortedItem.Item2;

                foreach (string subCat in subCats.Split('|'))
                {
                    concat += subCat + '|';
                    if (!hierarchy.ContainsKey(concat))
                    {
                        // Hierarchy needs a new page to be made
                        MenuItem i = new() { Text = subCat };
                        hierarchy.Add(concat, i);
                        currentMI.DropDownItems.Add(i);
                        currentMI = i;
                        continue;
                    }

                    // Hierarchy contains the needed page
                    currentMI = hierarchy[concat];
                }

                // Create a new option to select in the current sub-menu page
                MenuItem item = new()
                {
                    Text = isVar ? sortedItem : sortedItem.Item1.name + " v" + sortedItem.Item1.version,
                    Tag = isVar ? sortedItem : sortedItem.Item1
                };
                item.Click += clickHandler;
                currentMI.DropDownItems.Add(item);
            }
        }

        private ContextMenu LoadContextMenuItems()
        {
            // The main CM to return
            ContextMenu cm = new();

            // Event to grab where the CM is opening at
            cm.Opening += CaptureMousePosition;

            // Add a "New Trigger option to the top of the CM
            MenuItem trg = new() { Text = "New Trigger" };
            trg.Click += CreateNewTrigger_Pressed;
            cm.Items.Add(trg);

            // Create and add the sub-menus to our CM
            MenuItem cndMI = new() { Text = "Conditions" };
            MenuItem effMI = new() { Text = "Effects" };
            MenuItem varMI = new() { Text = "Variables" };
            cm.Items.Add(cndMI);
            cm.Items.Add(effMI);
            cm.Items.Add(varMI);

            // Assign sub-menus to a hierarchy
            Dictionary<string, MenuItem> hierarchy = new() {
                { "cnd|", cndMI },
                { "eff|", effMI },
                { "var|", varMI }
            };

#pragma warning disable CS8604, CS8602, CS8600 // Possible null reference argument.

            // Build Effects Container
            List<Tuple<SerializedEffect, string>> effsSorted = new();
            effsSorted.AddRange(JsonConvert.DeserializeObject<List<SerializedEffect>>(Resources.eff)
                .Select(e => new Tuple<SerializedEffect, string>(e, nodeSubCategories[e.name + "Eff"]))              // The "foreach => Add" loop
                    .OrderBy(x => (x.Item2 + "|" + x.Item1.name)).ToList());

            // Build Condition Container
            List<Tuple<SerializedCondition, string>> cndsSorted = new();
            cndsSorted.AddRange(JsonConvert.DeserializeObject<List<SerializedCondition>>(Resources.cnd)
                .Select(c => new Tuple<SerializedCondition, string>(c, nodeSubCategories[c.name + "Cnd"]))           // The "foreach => Add" loop
                    .OrderBy(x => (x.Item2 + "|" + x.Item1.name)).ToList());

            // Build Variable Container
            List<string> varsSorted = JsonConvert.DeserializeObject<List<string>>(Resources.var);
            varsSorted.Sort();

            // Build Each Sub-Menu
            BuildSubCM(cndsSorted, "cnd", ref hierarchy, CreateNewCondition_Pressed);
            BuildSubCM(effsSorted, "eff", ref hierarchy, CreateNewEffect_Pressed);
            BuildSubCM(varsSorted, "var", ref hierarchy, CreateNewVar_Pressed, true);

#pragma warning restore CS8604, CS8602, CS8600 // Possible null reference argument.

            return cm;
        }

        #endregion

        #region Event Handlers

        // General
        // TODO: Is this event needed now?
        Point nodeAddLocation = new();
        private void CaptureMousePosition(object? sender, CancelEventArgs e)
            => nodeAddLocation = GetMouseCoords();

        private void HotKeyHandler(object? sender, KeyEventArgs e)
        {
            // Copy
            if (e.KeyCode == Keys.C && e.Modifiers == Keys.Control)
            {
                var data = new DataObject();
                data.SetData(GetSerializedGraph(true));
                Clipboard.SetDataObject(data);
            }

            // Paste
            if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
            {
                DataObject retrievedData = (DataObject)Clipboard.GetDataObject();
                if (retrievedData.GetDataPresent(typeof(SerializedGraph)))
                {
                    SerializedGraph graph = (SerializedGraph)retrievedData.GetData(typeof(SerializedGraph));

                    if (graph != null)
                        LoadSerializedGraph(graph);
                }
            }
        }

        // Node Generation
        private void CreateNewTrigger_Pressed(object sender, EventArgs e)
        {
            SerializedTrigger t = new() { name = $"NewTrigger{trgID}" };
            nodeEditor.Nodes.Add(CreateTriggerNode(t, trgID++, nodeAddLocation.X, nodeAddLocation.Y));
        }

        private void CreateNewVar_Pressed(object sender, EventArgs e)
        {
            SerializedVariable v = new()
            {
                type = (string)((MenuItem)sender).Tag,
                value = string.Empty
            };
            v.name = $"New{v.type}{varID}";
            
            nodeEditor.Nodes.Add(CreateVarNode(v, varID++, nodeAddLocation.X, nodeAddLocation.Y));
        }

        private void CreateNewEffect_Pressed(object sender, EventArgs e)
        {
            SerializedEffect eff = (SerializedEffect)((MenuItem)sender).Tag;
            nodeEditor.Nodes.Add(CreateEffectNode(eff, effID++, nodeAddLocation.X, nodeAddLocation.Y));
        }

        private void CreateNewCondition_Pressed(object sender, EventArgs e)
        {
            SerializedCondition cnd = (SerializedCondition)((MenuItem)sender).Tag;
            nodeEditor.Nodes.Add(CreateConditionNode(cnd, cndID++, nodeAddLocation.X, nodeAddLocation.Y));
        }


        // STNodeGraph Events
        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (nodeEditor.ActiveNode != null)
                nodeEditor.Nodes.Remove(nodeEditor.ActiveNode);
        }

        private void uLockLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (nodeEditor.ActiveNode != null)
                nodeEditor.ActiveNode.LockLocation = !nodeEditor.ActiveNode.LockLocation;
        }

        private void uLockConnectionToolStripMenuItem_Click(object sender, EventArgs e)
            => nodeEditor.ActiveNode.LockOption = !nodeEditor.ActiveNode.LockOption;
        
        #endregion
    }
}
