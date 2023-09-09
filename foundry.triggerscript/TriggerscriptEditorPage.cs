using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.IO;
using Newtonsoft.Json;
using foundry;
using System.Numerics;
using foundry.Util;
using foundry.util;

namespace foundry.triggerscript
{
	//TODO: Clean this shit up!!!!!
    public class TriggerscriptEditorPage : BaseEditorPage
    {
		#region editor page
		private ContextMenuStrip contextMenu;

		//TODO: load this from the config.
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

		public TriggerscriptEditorPage()
        {
			SetRenderInterval(0);

            DoubleBuffered = true;

            #region Construct context menu
            contextMenu = new ContextMenuStrip();
            ContextMenuStrip = contextMenu;
            contextMenu.Opened += CaptureMousePos;
            ToolStripMenuItem trg = new ToolStripMenuItem("New Trigger");
            trg.Click += CreateNewTriggerPressed;
            contextMenu.Items.Add(trg);

            ToolStripMenuItem cndMI = new ToolStripMenuItem("Conditions");
            ToolStripMenuItem effMI = new ToolStripMenuItem("Effects");
            ToolStripMenuItem varMI = new ToolStripMenuItem("Variables");
            contextMenu.Items.Add(cndMI);
            contextMenu.Items.Add(effMI);
            contextMenu.Items.Add(varMI);

            //populate with main categories
            Dictionary<string, ToolStripMenuItem> hierarchy = new Dictionary<string, ToolStripMenuItem>() {
                { "cnd|", cndMI },
                { "eff|", effMI },
                { "var|", varMI }
            };

            //deserialize and sort effects.
            List<Tuple<SerializedEffect, string>> effsSorted = new List<Tuple<SerializedEffect, string>>();
            foreach (SerializedEffect e in JsonConvert.DeserializeObject<List<SerializedEffect>>(foundry.Properties.Resources.eff))
            {
                effsSorted.Add(new Tuple<SerializedEffect, string>(e, nodeSubCategories[e.name + "Eff"]));
            }
            effsSorted = effsSorted.OrderBy(x => (x.Item2 + "|" + x.Item1.name)).ToList();
            //create ToolStripMenuItems for effects.
            foreach (var v in effsSorted)
            {
                string concat = "eff|";
                string[] split = v.Item2.Split('|');
                ToolStripMenuItem last = hierarchy[concat];
                foreach (string s in split)
                {
                    concat += s + "|";
                    ToolStripMenuItem item;
                    if (hierarchy.ContainsKey(concat))
                    {
                        item = hierarchy[concat];
                    }
                    else
                    {
                        item = new ToolStripMenuItem(s);
                        hierarchy.Add(concat, item);
                    }
                    last.DropDownItems.Add(item);
                    last = item;
                }
                ToolStripMenuItem effI = new ToolStripMenuItem(v.Item1.name + " v" + v.Item1.version);
                effI.Tag = v.Item1;
                effI.Click += CreateNewEffectPressed;
                last.DropDownItems.Add(effI);
            }


            //deserialize and sort conditions.
            List<Tuple<SerializedCondition, string>> cndsSorted = new List<Tuple<SerializedCondition, string>>();
            foreach (SerializedCondition c in JsonConvert.DeserializeObject<List<SerializedCondition>>(foundry.Properties.Resources.cnd))
                cndsSorted.Add(new Tuple<SerializedCondition, string>(c, nodeSubCategories[c.name + "Cnd"]));
            cndsSorted = cndsSorted.OrderBy(x => (x.Item2 + "|" + x.Item1.name)).ToList();
            //create ToolStripMenuItems for conditions
            foreach (var v in cndsSorted)
            {
                string concat = "cnd|";
                string[] split = v.Item2.Split('|');
                ToolStripMenuItem last = hierarchy[concat];
                foreach (string s in split)
                {
                    concat += s + "|";
                    ToolStripMenuItem item;
                    if (hierarchy.ContainsKey(concat))
                    {
                        item = hierarchy[concat];
                    }
                    else
                    {
                        item = new ToolStripMenuItem(s);
                        hierarchy.Add(concat, item);
                    }
                    last.DropDownItems.Add(item);
                    last = item;
                }
                ToolStripMenuItem cndI = new ToolStripMenuItem(v.Item1.name + " v" + v.Item1.version);
                cndI.Tag = v.Item1;
                cndI.Click += CreateNewConditionPressed;
                last.DropDownItems.Add(cndI);
            }


            //deserialize and sort variable types.
            List<string> vs = JsonConvert.DeserializeObject<List<string>>(foundry.Properties.Resources.var);
            vs.Sort();
            foreach (string v in vs)
            {
                string vStr = nodeSubCategories[v + "Var"];
                string concat = "var|";
                ToolStripMenuItem last = varMI;
                foreach (string s in vStr.Split('|'))
                {
                    concat += s + "|";
                    if (hierarchy.Keys.Contains(concat))
                    {
                        last = hierarchy[concat];
                    }
                    else
                    {
                        ToolStripMenuItem item = new ToolStripMenuItem(s);
                        hierarchy.Add(concat, item);
                        last.DropDownItems.Add(item);
                        last = item;
                    }
                }

                ToolStripMenuItem varItem = new ToolStripMenuItem(v);
                varItem.Tag = v;
                varItem.Click += CreateNewVarPressed;
                last.DropDownItems.Add(varItem);
            }
			#endregion

			OnPageTick += OnTick;
			OnPageClickL += OnClickL;
			OnPageDragL += OnDragL;
			OnPageReleaseL += OnReleaseL;
			OnPageDraw += OnDraw;
		}
		#endregion


		#region tick
		private enum CurrentMouseState
		{
			None,
			DraggingNodes,
			DraggingMarquee,
			DraggingSocket
		}
		private CurrentMouseState mouseState;
		private TriggerscripterSocket selectedSocket = null;
		private float zoomMax = .55f, zoomMin = .005f;
		private float currentViewX = 0, currentViewY = 0, currentViewZoom = .25f;
        private Matrix viewMatrix = new Matrix();
        private Point markedMousePos = new Point();
        private Point GetTransformedMousePos()
		{
			var m = GetMouseState();
            Point[] p = new Point[] { new Point(m.X, m.Y) };
            viewMatrix.Inverted().TransformPoints(p);
			return p[0];
        }

		private List<TriggerscripterNode> SelectedNodes { get; set; } = new List<TriggerscripterNode>();
		private void SelectNode(TriggerscripterNode node)
        {
			if (SelectedNodes.Contains(node)) return;

            int mx = GetTransformedMousePos().X;
            int my = GetTransformedMousePos().Y;

			node.selected = true;
			SelectedNodes.Add(node);
        }
		private void DeselectNode(TriggerscripterNode node)
		{
			if (!SelectedNodes.Contains(node)) return;

			node.selected = false;
            SelectedNodes.Remove(node);
		}
		private void DeselectAllNodes()
		{
			foreach (var node in SelectedNodes)
				node.selected = false;

			SelectedNodes.Clear();
		}
        TriggerscripterNode? GetFirstNodeAtPoint(int mx, int my)
        {
            var nodesReversed = nodes.ToList();
            nodesReversed.Reverse();

            foreach (TriggerscripterNode n in nodesReversed)
			{
				if(n.PointIsIn(mx,my))
				{
					return n;
				}
			}

			return null;
        }


        private void OnClickL(object o, EventArgs e)
		{
			int mx = GetTransformedMousePos().X;
			int my = GetTransformedMousePos().Y;

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
						if (!s.MultiConnection)
						{
							if (s.ConnectedSockets.Count > 0)
							{
								selectedSocket = s.ConnectedSockets[0];
								if (s is TriggerscripterSocket_Input)
								{
									((TriggerscripterSocket_Output)s.ConnectedSockets[0]).Disconnect((TriggerscripterSocket_Input)s);
								}
								else
								{
									((TriggerscripterSocket_Output)s).Disconnect((TriggerscripterSocket_Input)s.ConnectedSockets[0]);
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
				var node = GetFirstNodeAtPoint(mx, my);
				if (node != null)
				{
					if(!node.selected)
					{
						DeselectAllNodes();
						SelectNode(node);
                    }
                    mouseState = CurrentMouseState.DraggingNodes;
                }
			}

			if (mouseState == CurrentMouseState.DraggingNodes)
			{
				foreach(var node in SelectedNodes)
                {
                    //int ox, oy;
                    //node.GetPointOffset(mx, my, out ox, out oy);
                    //node.selectedX = ox;
                    //node.selectedY = oy;
                }
			}

			//if there is no state still, start dragging marquee
			if (mouseState == CurrentMouseState.None)
			{
				foreach (TriggerscripterNode n in nodes)
					n.selected = false;

				mouseState = CurrentMouseState.DraggingMarquee;
				markedMousePos.X = mx;
				markedMousePos.Y = my;
            }
		}
        private void OnDragL(object o, EventArgs e)
        {
            int mx = GetTransformedMousePos().X;
            int my = GetTransformedMousePos().Y;

            var nodesReversed = nodes.ToList();
			nodesReversed.Reverse();

			//nodes are selected, drag them
			if (mouseState == CurrentMouseState.DraggingNodes)
			{
				foreach (TriggerscripterNode n in SelectedNodes)
				{
					n.Move(GetMouseState().deltaX * (1/currentViewZoom), GetMouseState().deltaY * (1/currentViewZoom));
					TrySetEdited();
				}
			}

			//marquee is dragging, update it
			if (mouseState == CurrentMouseState.DraggingMarquee)
			{
				foreach (TriggerscripterNode n in nodesReversed)
				{
					if (n.IntersectsRect(
					Math.Min(markedMousePos.X, mx),
					Math.Min(markedMousePos.Y, my),
					Math.Abs(mx - markedMousePos.X),
					Math.Abs(my - markedMousePos.Y)))
					{
						SelectNode(n);
					}
					else
					{
						DeselectNode(n);
					}
				}
			}
		}
        private void OnReleaseL(object o, EventArgs e)
		{
            int mx = GetTransformedMousePos().X;
            int my = GetTransformedMousePos().Y;

			//socket was dragging
			if (mouseState == CurrentMouseState.DraggingSocket)
			{
				//foreach node
				foreach (TriggerscripterNode n in nodes)
				{
					//foreach socket in that node
					foreach (TriggerscripterSocket s in n.sockets.Values)
					{
						if (s == selectedSocket) break;
						//if mouse was in that socket
						if (s.PointIsIn(mx, my))
						{
							//if selected socket is output
							if (selectedSocket is TriggerscripterSocket_Output)
							{
								//if this socket is input (compatible)
								if (s is TriggerscripterSocket_Input)
								{
									((TriggerscripterSocket_Output)selectedSocket).Connect(s as TriggerscripterSocket_Input);
									TrySetEdited();
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
									TrySetEdited();
									goto DraggingSocketFinish;
								}
							}
						}
					}
				}
				DraggingSocketFinish:
				selectedSocket = null;
			}

			mouseState = CurrentMouseState.None;
        }
		private int pasteOffset = 50;

        private void OnTick(object o, EventArgs e)
		{
			var m = GetMouseState();

#if DEBUG
			if (GetKeyIsDown(Keys.F5) && !GetKeyWasDown(Keys.F5))
			{
				TriggerscriptCompiler c = new TriggerscriptCompiler();
				c.Compile(nodes, varID, "out.triggerscript");
			}
#endif


			#region delete
			if (GetKeyIsDown(Keys.Delete))
			{
				foreach (TriggerscripterNode n in SelectedNodes.ToList())
					//for each socket in this node's sockets
					foreach (TriggerscripterSocket s in n.sockets.Values)
					{
						//if it is an input
						if (s is TriggerscripterSocket_Input)
						{
							//foreach output connected to this input
							foreach (TriggerscripterSocket cs in ((TriggerscripterSocket_Input)s).ConnectedSockets.ToArray())
							{
								//disconnect this input from the output
								((TriggerscripterSocket_Output)cs).Disconnect((TriggerscripterSocket_Input)s);
							}
						}

						//if it is an output
						else
						{
							//foreach input connected to this output
							foreach (TriggerscripterSocket cs in s.ConnectedSockets.ToArray())
							{
								((TriggerscripterSocket_Output)s).Disconnect((TriggerscripterSocket_Input)cs);
							}
						}
						DeselectNode(n);
						nodes.Remove(n);
						TrySetEdited();
					}
			}
		
			#endregion


			#region copy/paste
			if (GetKeyIsDown(Keys.ControlKey) && GetKeyIsDown(Keys.C) && !GetKeyWasDown(Keys.C))
			{
				CopyGraph();
			}
			if (GetKeyIsDown(Keys.ControlKey) && GetKeyIsDown(Keys.V) && !GetKeyWasDown(Keys.V))
			{
				PasteGraph();
				TrySetEdited();
			}
			#endregion


			#region mouse
			//scroll wheel (pan)
			if (m.middleDown)
			{
				currentViewX += (m.deltaX) * 1 / currentViewZoom;
				currentViewY += (m.deltaY) * 1 / currentViewZoom;
			}
			//scroll wheel (zoom)
			currentViewZoom += m.deltaScroll / 5500f;
			currentViewZoom = currentViewZoom < zoomMin ? zoomMin : currentViewZoom;
			currentViewZoom = currentViewZoom > zoomMax ? zoomMax : currentViewZoom;
			#endregion


			//matrix stuff must be done at the very end.
			#region matrix
			//create the view matrix for the editor's page (order matters).
			viewMatrix.Reset();
			viewMatrix.Translate(Width / 2, Height / 2);
			viewMatrix.Scale(currentViewZoom, currentViewZoom);
			viewMatrix.Translate(currentViewX, currentViewY);
			#endregion
		}
		#endregion


		#region draw
		private Pen gridPen = new Pen(Color.FromArgb(255, 150, 150, 150));
		public int majorGridSpace = 350;
        public bool DrawDetail() { return currentViewZoom > .075; }
        private void OnDraw(object o, EventArgs e)
        {
			//cause the page to redraw using winforms events.
			Invalidate();
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

            int mx = GetTransformedMousePos().X;
            int my = GetTransformedMousePos().Y;

            Graphics g = e.Graphics;
			g.SmoothingMode = SmoothingMode.AntiAlias;
			g.InterpolationMode = InterpolationMode.HighQualityBicubic;
			g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
			g.Transform = viewMatrix;

			//TODO: replace with config (all colors)
			g.Clear(Color.DarkGray);

			//gets and transforms the viewable bounds by the inverted view matrix.
			PointF[] invertedMatrixPoints = new PointF[]
			{
				new PointF(e.ClipRectangle.Left, e.ClipRectangle.Top),
				new PointF(e.ClipRectangle.Right, e.ClipRectangle.Bottom)
			};
            viewMatrix.Inverted().TransformPoints(invertedMatrixPoints);
			float invLeft = invertedMatrixPoints[0].X;
			float invRight = invertedMatrixPoints[1].X;
			float invTop = invertedMatrixPoints[0].Y;
			float invBottom = invertedMatrixPoints[1].Y;
			
			//draw grid lines from the starting lines' offset.
			//top-most and left-most grid lines.
			int xOffs = (int)Math.Round(invLeft / majorGridSpace) * majorGridSpace;
			int yOffs = (int)Math.Round(invTop / majorGridSpace) * majorGridSpace;
			for (int x = xOffs; x < invRight; x += majorGridSpace)
			{
				g.DrawLine(gridPen, x, invTop, x, invBottom);
			}
			for (int y = yOffs; y < invBottom; y += majorGridSpace)
			{
				g.DrawLine(gridPen, invLeft, y, invRight, y);
			}
			

			//draw connections
			foreach (TriggerscripterNode n in nodes)
			{
				foreach (TriggerscripterSocket s in n.sockets.Values)
				{
					if (s is TriggerscripterSocket_Output)
					{
						//delegates the drawing to the socket itself.
						((TriggerscripterSocket_Output)s).DrawConnections(e);
					}
				}
			}

			//draw current temp connection
			if (mouseState == CurrentMouseState.DraggingSocket)
			{
				//just a line. TODO: make this more uniform with the actual socket connections. Perhaps make the socket itself draw this?
				e.Graphics.DrawLine(new Pen(selectedSocket.Color, 5.0f), 
					new PointF
						(
						selectedSocket.OwnerNode.PosX + selectedSocket.BoundingRect.X + selectedSocket.BoundingRect.Width / 2,
						selectedSocket.OwnerNode.PosY + selectedSocket.BoundingRect.Y + selectedSocket.BoundingRect.Height / 2
						),
					new PointF
						(
						mx, 
						my
						)
					);
			}

			//draw nodes
			foreach (TriggerscripterNode n in nodes)
			{
				//Console.WriteLine("{0} {1} {2} {3}", (int)invLeft, (int)invTop, (int)invRight, (int)invBottom);
				//delegates the drawing of the node to the node itself.
				if(n.IntersectsRect((int)invLeft, (int)invTop, (int)invRight - (int)invLeft, (int)invBottom - (int)invTop))
				{
					n.Draw(e);
				}
			}

			//draw marquee (current mouse dragged selection)
			if (mouseState == CurrentMouseState.DraggingMarquee)
            {
				//just a rectangle.
				e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(100, 10, 10, 10)),
					Math.Min(markedMousePos.X, mx),
					Math.Min(markedMousePos.Y, my),
					Math.Abs(mx - markedMousePos.X),
					Math.Abs(my - markedMousePos.Y));
			}
		}
		#endregion
	

		#region creation
		//TODO: load from config (and all colors too).
		public static Color requiredVarColor = Color.FromArgb(255, 64, 130, 64);
        public static Color optionalVarColor = Color.FromArgb(255, 122, 130, 64);
        public static Color cndColor = Color.FromArgb(255, 130, 64, 64);
        public static Color trgColor = Color.FromArgb(255, 64, 117, 130);
        public static Color effColor = Color.FromArgb(255, 130, 64, 106);

		/// <summary>
		/// Gets the serialized representation of the current graph.
		/// </summary>
		/// <returns>The serialized object.</returns>
		public SerializedTriggerscript GetSerializedGraph()
		{
			SerializedTriggerscript sts = new SerializedTriggerscript();
			foreach (TriggerscripterNode n in nodes)
			{
				SerializedNode sn = new SerializedNode();
				sn.handleAs = n.HandleAs;
				sn.selected = n.selected;
				if (n.HandleAs == "Trigger")
				{
					SerializedTrigger t = new SerializedTrigger();
					t.active = ((TriggerscripterNode_Trigger)n).Active;
					t.cndIsOr = ((TriggerscripterNode_Trigger)n).Conditional;
					t.name = ((TriggerscripterNode_Trigger)n).Name;
					sn.trigger = t;
				}
				if (n.HandleAs == "Variable")
				{
					SerializedVariable v = new SerializedVariable();
					v.value = ((TriggerscripterNode_Variable)n).Value;
					v.name = ((TriggerscripterNode_Variable)n).Name;
					v.type = n.Type;
					sn.variable = v;
				}
				if (n.HandleAs == "Effect")
				{
					SerializedEffect e = (SerializedEffect)n.Data;
					sn.effect = e;
				}
				if (n.HandleAs == "Condition")
				{
					SerializedCondition c = (SerializedCondition)n.Data;
					sn.condition = c;
				}

				foreach (TriggerscripterSocket os in n.sockets.Values)
				{
					if (os is TriggerscripterSocket_Output)
					{
						foreach (TriggerscripterSocket s in os.ConnectedSockets)
						{
							SerializedNodeLink link = new SerializedNodeLink();
							link.sourceId = n.Id;
							link.sourceSocketName = os.Text;

							if (n is TriggerscripterNode_Variable)
								link.sourceType = "Variable";
							else
								link.sourceType = n.Type;

							link.targetId = s.OwnerNode.Id;
							link.targetSocketName = s.Text;
							if (s.OwnerNode is TriggerscripterNode_Variable)
								link.targetType = "Variable";
							else
								link.targetType = s.OwnerNode.Type;

							sts.links.Add(link);
						}


					}
				}

				sn.x = n.PosX;
				sn.y = n.PosY;
				sn.id = n.Id;

				sts.nodes.Add(sn);
			}
			sts.lastTrg = trgID++;
			sts.lastVar = varID++;
			sts.lastEff = effID++;
			sts.lastCnd = cndID++;
			return sts;
		}
		/// <summary>
		/// Populates the editor with data from a serialized graph.
		/// </summary>
		/// <returns>True if the data was fully loaded without error.</returns>
		public bool LoadFromSerializedGraph(SerializedTriggerscript sts)
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

			foreach (SerializedNode n in sts.nodes)
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
			foreach (SerializedNodeLink l in sts.links)
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
			return true;
		}
		public bool LoadFromSerializedGraph(TriggerscriptClass ts)
		{
            Dictionary<int, TriggerscripterNode_Variable> variables = new Dictionary<int, TriggerscripterNode_Variable>();
			foreach(var v in ts.TriggerVars)
			{
				var editorData = v.EditorNodeData;
				float x = 0, y = 0;
				if (editorData != null)
				{
					x = editorData.X;
					y = editorData.Y;
                }
				TriggerscripterNode_Variable var = new TriggerscripterNode_Variable(this, v, x, y, v.ID);
                nodes.Add(var);
				variables.Add(v.ID, var);
            }

			foreach(var t in ts.Triggers)
			{
                var editorData = t.EditorNodeData;
				float x = t.X * 10;
				float y = t.Y * 10;
                if (editorData != null)
                {
                    x = editorData.X;
                    y = editorData.Y;
                }
				TriggerscripterNode_Trigger trigger = new TriggerscripterNode_Trigger(this, t.Name, x, y, t.ID);
				nodes.Add(trigger);

				int ytacker = 0;
                foreach (var e in t.TriggerEffectsOnTrue)
				{
					TriggerscripterNode_Effect effect = new TriggerscripterNode_Effect(this, e, x, y, t.ID);
                    nodes.Add(effect);

					if (e.Inputs != null)
					{
						foreach (var inp in e.Inputs)
						{
							if (variables.ContainsKey(inp.Value))
							{
								variables[inp.Value].PosX = x;
								variables[inp.Value].PosY = y + ytacker;
								ytacker += variables[inp.Value].Height;
							}
						}
					}
                }
            }

			return true;
        }

		private Point nodeAddLocation = new Point();
		/// <summary>
		/// Callback that captures the mouse's position when it is fired.
		/// This is called when the context menu is first opened to capture the mouse's position for copy/paste.
		/// </summary>
        public void CaptureMousePos(object o, EventArgs e)
        {
            Point[] nodeAddLocationArr = new Point[] { new Point(GetMouseState().X, GetMouseState().Y) };
            viewMatrix.Inverted().TransformPoints(nodeAddLocationArr);
            nodeAddLocation = nodeAddLocationArr[0];
        }
		private SerializedTriggerscript copyBuffer = new SerializedTriggerscript();
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
			foreach (SerializedNode sn in copyBuffer.nodes)
			{
				if (sn.selected)
				{
					if (sn.handleAs == "Trigger")
					{
						TriggerscripterNode n = CreateTriggerNode(sn.trigger, trgID, sn.x - pasteOffset, sn.y - pasteOffset);
						n.selected = true;
						trgMap.Add(sn.id, n);
						trgID++;
					}
					if (sn.handleAs == "Effect")
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
			}
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
            var.type = (string)((ToolStripMenuItem)o).Tag;
            var.name = "New" + var.type + varID.ToString();
            var.value = "";
            CreateVarNode(var, varID, nodeAddLocation.X, nodeAddLocation.Y);
            varID++;
        }
        public void CreateNewEffectPressed(object o, EventArgs e)
        {
            CreateEffectNode((SerializedEffect)((ToolStripMenuItem)o).Tag, effID++, nodeAddLocation.X, nodeAddLocation.Y);
        }
        public void CreateNewConditionPressed(object o, EventArgs e)
        {
            CreateConditionNode((SerializedCondition)((ToolStripMenuItem)o).Tag, cndID++, nodeAddLocation.X, nodeAddLocation.Y);
        }

        //actual creation functions
        public TriggerscripterNode CreateTriggerNode(SerializedTrigger t, int id, float x, float y)
        {
            TriggerscripterNode_Trigger n = new TriggerscripterNode_Trigger(this, t, x, y, id);
            nodes.Add(n);
            return n;
        }
        public TriggerscripterNode CreateVarNode(SerializedVariable v, int id, float x, float y)
        {
            TriggerscripterNode_Variable n = new TriggerscripterNode_Variable(this, v, x, y, id);
            nodes.Add(n);
            return n;
        }
        public TriggerscripterNode CreateEffectNode(SerializedEffect e, int id, float x, float y)
        {
			TriggerscripterNode_Effect n = new TriggerscripterNode_Effect(this, e, x, y, id);
            nodes.Add(n);
            return n;
        }
        public TriggerscripterNode CreateConditionNode(SerializedCondition c, int id, float x, float y)
        {
            TriggerscripterNode_Condition n = new TriggerscripterNode_Condition(this, c, x, y, id);
            nodes.Add(n);
            return n;
        }
		#endregion
	}
}
