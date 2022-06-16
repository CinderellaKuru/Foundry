using ComponentFactory.Krypton.Navigator;
using ComponentFactory.Krypton.Toolkit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMHEditor.DockingModules.Triggerscripter
{
    public class TriggerScripterPage : KryptonPage
    {
        public static Dictionary<string, string> nodeSubCategories = new Dictionary<string, string>()
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

        TriggerscripterControl c;
        public TriggerScripterPage()
        {
            c = new TriggerscripterControl();
            c.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Top;
            c.Dock = System.Windows.Forms.DockStyle.Fill;
            Controls.Add(c);

            MenuStrip ms = new MenuStrip();
            Controls.Add(ms);
            ms.Items.Add("Save", null, SaveAs);
            ms.Items.Add("Open", null, OpenProject);
            ms.Items.Add("Compile", null, Compile);
            ms.BackColor = Program.window.darkmode.GetBackColor1(
                ComponentFactory.Krypton.Toolkit.PaletteBackStyle.PanelCustom1,
                ComponentFactory.Krypton.Toolkit.PaletteState.Normal);

            ContextMenu cm = new ContextMenu();
            ContextMenu = cm;
            cm.Popup += c.CaptureMousePos;
            MenuItem trg = new MenuItem("New Trigger");
            trg.Click += c.CreateNewTriggerPressed;
            cm.MenuItems.Add(trg);

            MenuItem cndMI = new MenuItem("Conditions");
            MenuItem effMI = new MenuItem("Effects");
            MenuItem varMI = new MenuItem("Variables");
            cm.MenuItems.Add(cndMI);
            cm.MenuItems.Add(effMI);
            cm.MenuItems.Add(varMI);

            Dictionary<string, MenuItem> hierarchy = new Dictionary<string, MenuItem>() {
                { "cnd|", cndMI },
                { "eff|", effMI },
                { "var|", varMI }
            };

            List<Tuple<Effect, string>> effsSorted = new List<Tuple<Effect, string>>();
            foreach (Effect e in JsonConvert.DeserializeObject<List<Effect>>(SMHEditor.Properties.Resources.eff))
                effsSorted.Add(new Tuple<Effect, string>(e, nodeSubCategories[e.name + "Eff"]));
            effsSorted = effsSorted.OrderBy(x => (x.Item2 + "|" + x.Item1.name)).ToList();

            foreach (var v in effsSorted)
            {
                string concat = "eff|";
                string[] split = v.Item2.Split('|');
                MenuItem last = hierarchy[concat];
                foreach(string s in split)
                {
                    concat += s + "|";
                    MenuItem item;
                    if (hierarchy.ContainsKey(concat))
                    {
                        item = hierarchy[concat];
                    }
                    else
                    {
                        MenuItem i = new MenuItem(s);
                        hierarchy.Add(concat, i);
                        item = i;
                    }
                    last.MenuItems.Add(item);
                    last = item;
                }
                MenuItem effI = new MenuItem(v.Item1.name);
                effI.Tag = v.Item1;
                effI.Click += c.CreateNewEffectPressed;
                last.MenuItems.Add(effI);
            }

            List<Tuple<Condition, string>> cndsSorted = new List<Tuple<Condition, string>>();
            foreach (Condition c in JsonConvert.DeserializeObject<List<Condition>>(SMHEditor.Properties.Resources.cnd))
                cndsSorted.Add(new Tuple<Condition, string>(c, nodeSubCategories[c.name + "Cnd"]));
            cndsSorted = cndsSorted.OrderBy(x => (x.Item2 + "|" + x.Item1.name)).ToList();

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
                        MenuItem i = new MenuItem(s);
                        hierarchy.Add(concat, i);
                        item = i;
                    }
                    last.MenuItems.Add(item);
                    last = item;
                }
                MenuItem cndI = new MenuItem(v.Item1.name);
                cndI.Tag = v.Item1;
                cndI.Click += c.CreateNewConditionPressed;
                last.MenuItems.Add(cndI);
            }

            List<string> vs = JsonConvert.DeserializeObject<List<string>>(SMHEditor.Properties.Resources.var);
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
                        MenuItem i = new MenuItem(s);
                        hierarchy.Add(concat, i);
                        last.MenuItems.Add(i);
                        last = i;
                    }
                }

                MenuItem varItem = new MenuItem(v);
                varItem.Tag = v;
                varItem.Click += c.CreateNewVarPressed;
                last.MenuItems.Add(varItem);
            }
        }

        void Compile(object o, EventArgs e)
        {
            TriggerscripterCompiler comp = new TriggerscripterCompiler();

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Triggerscript|*.triggerscript";
            if (sfd.ShowDialog() == DialogResult.OK)
                comp.Compile(c.nodes, c.varID, sfd.FileName);

        }
        void SaveAs(object o, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Triggerscript Project|*.tsp";

            if (sfd.ShowDialog() == DialogResult.OK)
                c.SaveToFile(sfd.FileName);
        }
        void OpenProject(object o, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Triggerscript Project|*.tsp";

            if (ofd.ShowDialog() == DialogResult.OK)
                c.LoadFromFile(ofd.FileName);
        }
    }
}
