using Foundry;
using SharpDX;
using YAXLib;
using YAXLib.Attributes;
using YAXLib.Enums;

namespace Foundry.Data.Triggerscript
{
    public class TriggerscriptModule : BaseModule
    {
        public static ScriptLogicPrototype GetEffectLogicPrototype(int dbid, int version)
        {
            if(EffectItems.ContainsKey(dbid))
            {
                if(EffectItems[dbid].ContainsKey(version))
                    return EffectItems[dbid][version];

                if (EffectItems[dbid].ContainsKey(-1))
                    return EffectItems[dbid][-1];
            }
            return null;
        }
        public static ScriptLogicPrototype GetConditionLogicPrototype(int dbid, int version)
        {
            if (ConditionItems.ContainsKey(dbid))
            {
                if (ConditionItems[dbid].ContainsKey(version))
                    return ConditionItems[dbid][version];

                if (ConditionItems[dbid].ContainsKey(-1))
                    return ConditionItems[dbid][-1];
            }
            return null;
        }
        public static Dictionary<int, Dictionary<int, ScriptLogicPrototype>> EffectItems { get; private set; } = (Dictionary<int, Dictionary<int, ScriptLogicPrototype>>)new YAXSerializer(typeof(Dictionary<int, Dictionary<int, ScriptLogicPrototype>>)).Deserialize(File.ReadAllText("modules/effects.tsdef"));
        public static Dictionary<int, Dictionary<int, ScriptLogicPrototype>> ConditionItems { get; private set; } = (Dictionary<int, Dictionary<int, ScriptLogicPrototype>>)new YAXSerializer(typeof(Dictionary<int, Dictionary<int, ScriptLogicPrototype>>)).Deserialize(File.ReadAllText("modules/conditions.tsdef"));
        public static ScriptVarType GetTypeFromString(string varTypeName)
        {
            ScriptVarType varTypeEnum = ScriptVarType.Invalid;

            //this is taken straight from the source if anyones wondering why it isnt a dict.
            if (varTypeName == "Tech")
                varTypeEnum = ScriptVarType.Tech;
            else if (varTypeName == "TechStatus")
                varTypeEnum = ScriptVarType.TechStatus;
            else if (varTypeName == "Operator")
                varTypeEnum = ScriptVarType.Operator;
            else if (varTypeName == "ProtoObject")
                varTypeEnum = ScriptVarType.ProtoObject;
            else if (varTypeName == "ObjectType")
                varTypeEnum = ScriptVarType.ObjectType;
            else if (varTypeName == "ProtoSquad")
                varTypeEnum = ScriptVarType.ProtoSquad;
            else if (varTypeName == "Sound")
                varTypeEnum = ScriptVarType.Sound;
            else if (varTypeName == "Entity")
                varTypeEnum = ScriptVarType.Entity;
            else if (varTypeName == "EntityList")
                varTypeEnum = ScriptVarType.EntityList;
            else if (varTypeName == "Trigger")
                varTypeEnum = ScriptVarType.Trigger;
            else if (varTypeName == "Distance")
                varTypeEnum = ScriptVarType.Float;
            else if (varTypeName == "Time")
                varTypeEnum = ScriptVarType.Time;
            else if (varTypeName == "Player")
                varTypeEnum = ScriptVarType.Player;
            else if (varTypeName == "Count")
                varTypeEnum = ScriptVarType.Integer;
            else if (varTypeName == "Location")
                varTypeEnum = ScriptVarType.Vector;
            else if (varTypeName == "UILocation")
                varTypeEnum = ScriptVarType.UILocation;
            else if (varTypeName == "UIEntity")
                varTypeEnum = ScriptVarType.UIEntity;
            else if (varTypeName == "Cost")
                varTypeEnum = ScriptVarType.Cost;
            else if (varTypeName == "AnimType")
                varTypeEnum = ScriptVarType.AnimType;
            else if (varTypeName == "ActionStatus")
                varTypeEnum = ScriptVarType.ActionStatus;
            else if (varTypeName == "Percent")
                varTypeEnum = ScriptVarType.Float;
            else if (varTypeName == "Hitpoints")
                varTypeEnum = ScriptVarType.Float;
            else if (varTypeName == "Power")
                varTypeEnum = ScriptVarType.Power;
            else if (varTypeName == "Bool")
                varTypeEnum = ScriptVarType.Bool;
            else if (varTypeName == "Float")
                varTypeEnum = ScriptVarType.Float;
            else if (varTypeName == "Iterator")
                varTypeEnum = ScriptVarType.Iterator;
            else if (varTypeName == "Team")
                varTypeEnum = ScriptVarType.Team;
            else if (varTypeName == "PlayerList")
                varTypeEnum = ScriptVarType.PlayerList;
            else if (varTypeName == "TeamList")
                varTypeEnum = ScriptVarType.TeamList;
            else if (varTypeName == "PlayerState")
                varTypeEnum = ScriptVarType.PlayerState;
            else if (varTypeName == "Objective")
                varTypeEnum = ScriptVarType.Objective;
            else if (varTypeName == "Unit")
                varTypeEnum = ScriptVarType.Unit;
            else if (varTypeName == "UnitList")
                varTypeEnum = ScriptVarType.UnitList;
            else if (varTypeName == "Squad")
                varTypeEnum = ScriptVarType.Squad;
            else if (varTypeName == "SquadList")
                varTypeEnum = ScriptVarType.SquadList;
            else if (varTypeName == "UIUnit")
                varTypeEnum = ScriptVarType.UIUnit;
            else if (varTypeName == "UISquad")
                varTypeEnum = ScriptVarType.UISquad;
            else if (varTypeName == "UISquadList")
                varTypeEnum = ScriptVarType.UISquadList;
            else if (varTypeName == "String")
                varTypeEnum = ScriptVarType.String;
            else if (varTypeName == "MessageIndex")
                varTypeEnum = ScriptVarType.MessageIndex;
            else if (varTypeName == "MessageJustify")
                varTypeEnum = ScriptVarType.MessageJustify;
            else if (varTypeName == "MessagePoint")
                varTypeEnum = ScriptVarType.MessagePoint;
            else if (varTypeName == "Color")
                varTypeEnum = ScriptVarType.Color;
            else if (varTypeName == "ProtoObjectList")
                varTypeEnum = ScriptVarType.ProtoObjectList;
            else if (varTypeName == "ObjectTypeList")
                varTypeEnum = ScriptVarType.ObjectTypeList;
            else if (varTypeName == "ProtoSquadList")
                varTypeEnum = ScriptVarType.ProtoSquadList;
            else if (varTypeName == "TechList")
                varTypeEnum = ScriptVarType.TechList;
            else if (varTypeName == "MathOperator")
                varTypeEnum = ScriptVarType.MathOperator;
            else if (varTypeName == "ObjectDataType")
                varTypeEnum = ScriptVarType.ObjectDataType;
            else if (varTypeName == "ObjectDataRelative")
                varTypeEnum = ScriptVarType.ObjectDataRelative;
            else if (varTypeName == "Civ")
                varTypeEnum = ScriptVarType.Civ;
            else if (varTypeName == "ProtoObjectCollection")
                varTypeEnum = ScriptVarType.ProtoObjectCollection;
            else if (varTypeName == "Object")
                varTypeEnum = ScriptVarType.Object;
            else if (varTypeName == "ObjectList")
                varTypeEnum = ScriptVarType.ObjectList;
            else if (varTypeName == "Group")
                varTypeEnum = ScriptVarType.Group;
            else if (varTypeName == "LocationList")
                varTypeEnum = ScriptVarType.VectorList;
            else if (varTypeName == "RefCountType")
                varTypeEnum = ScriptVarType.RefCountType;
            else if (varTypeName == "UnitFlag")
                varTypeEnum = ScriptVarType.UnitFlag;
            else if (varTypeName == "LOSType")
                varTypeEnum = ScriptVarType.LOSType;
            else if (varTypeName == "EntityFilterSet")
                varTypeEnum = ScriptVarType.EntityFilterSet;
            else if (varTypeName == "PopBucket")
                varTypeEnum = ScriptVarType.PopBucket;
            else if (varTypeName == "ListPosition")
                varTypeEnum = ScriptVarType.ListPosition;
            else if (varTypeName == "RelationType")
                varTypeEnum = ScriptVarType.RelationType;
            else if (varTypeName == "Diplomacy")
                varTypeEnum = ScriptVarType.RelationType;
            else if (varTypeName == "ExposedAction")
                varTypeEnum = ScriptVarType.ExposedAction;
            else if (varTypeName == "SquadMode")
                varTypeEnum = ScriptVarType.SquadMode;
            else if (varTypeName == "ExposedScript")
                varTypeEnum = ScriptVarType.ExposedScript;
            else if (varTypeName == "KBBase")
                varTypeEnum = ScriptVarType.KBBase;
            else if (varTypeName == "KBBaseList")
                varTypeEnum = ScriptVarType.KBBaseList;
            else if (varTypeName == "DataScalar")
                varTypeEnum = ScriptVarType.DataScalar;
            else if (varTypeName == "KBBaseQuery")
                varTypeEnum = ScriptVarType.KBBaseQuery;
            else if (varTypeName == "DesignLine")
                varTypeEnum = ScriptVarType.DesignLine;
            else if (varTypeName == "LocStringID")
                varTypeEnum = ScriptVarType.LocStringID;
            else if (varTypeName == "Leader")
                varTypeEnum = ScriptVarType.Leader;
            else if (varTypeName == "Cinematic")
                varTypeEnum = ScriptVarType.Cinematic;
            else if (varTypeName == "TalkingHead")
                varTypeEnum = ScriptVarType.TalkingHead;
            else if (varTypeName == "Direction")
                varTypeEnum = ScriptVarType.Vector;
            else if (varTypeName == "FlareType")
                varTypeEnum = ScriptVarType.FlareType;
            else if (varTypeName == "CinematicTag")
                varTypeEnum = ScriptVarType.CinematicTag;
            else if (varTypeName == "IconType")
                varTypeEnum = ScriptVarType.IconType;
            else if (varTypeName == "Difficulty")
                varTypeEnum = ScriptVarType.Difficulty;
            else if (varTypeName == "Integer")
                varTypeEnum = ScriptVarType.Integer;
            else if (varTypeName == "HUDItem")
                varTypeEnum = ScriptVarType.HUDItem;
            else if (varTypeName == "UIItem")
                varTypeEnum = ScriptVarType.FlashableUIItem;
            else if (varTypeName == "FlashableUIItem")
                varTypeEnum = ScriptVarType.FlashableUIItem;
            else if (varTypeName == "ControlType")
                varTypeEnum = ScriptVarType.ControlType;
            else if (varTypeName == "UIButton")
                varTypeEnum = ScriptVarType.UIButton;
            else if (varTypeName == "MissionType")
                varTypeEnum = ScriptVarType.MissionType;
            else if (varTypeName == "MissionState")
                varTypeEnum = ScriptVarType.MissionState;
            else if (varTypeName == "MissionTargetType")
                varTypeEnum = ScriptVarType.MissionTargetType;
            else if (varTypeName == "IntegerList")
                varTypeEnum = ScriptVarType.IntegerList;
            else if (varTypeName == "BidType")
                varTypeEnum = ScriptVarType.BidType;
            else if (varTypeName == "BidState")
                varTypeEnum = ScriptVarType.BidState;
            else if (varTypeName == "BuildingCommandState")
                varTypeEnum = ScriptVarType.BuildingCommandState;
            else if (varTypeName == "Vector")
                varTypeEnum = ScriptVarType.Vector;
            else if (varTypeName == "VectorList")
                varTypeEnum = ScriptVarType.VectorList;
            else if (varTypeName == "PlacementRule")
                varTypeEnum = ScriptVarType.PlacementRule;
            else if (varTypeName == "KBSquad")
                varTypeEnum = ScriptVarType.KBSquad;
            else if (varTypeName == "KBSquadList")
                varTypeEnum = ScriptVarType.KBSquadList;
            else if (varTypeName == "KBSquadQuery")
                varTypeEnum = ScriptVarType.KBSquadQuery;
            else if (varTypeName == "AISquadAnalysis")
                varTypeEnum = ScriptVarType.AISquadAnalysis;
            else if (varTypeName == "AISquadAnalysisComponent")
                varTypeEnum = ScriptVarType.AISquadAnalysisComponent;
            else if (varTypeName == "KBSquadFilterSet")
                varTypeEnum = ScriptVarType.KBSquadFilterSet;
            else if (varTypeName == "ChatSpeaker")
                varTypeEnum = ScriptVarType.ChatSpeaker;
            else if (varTypeName == "RumbleType")
                varTypeEnum = ScriptVarType.RumbleType;
            else if (varTypeName == "RumbleMotor")
                varTypeEnum = ScriptVarType.RumbleMotor;
            else if (varTypeName == "TechDataCommandType")
                varTypeEnum = ScriptVarType.TechDataCommandType;
            else if (varTypeName == "CommandType")
                varTypeEnum = ScriptVarType.TechDataCommandType;
            else if (varTypeName == "SquadDataType")
                varTypeEnum = ScriptVarType.SquadDataType;
            else if (varTypeName == "EventType")
                varTypeEnum = ScriptVarType.EventType;
            else if (varTypeName == "TimeList")
                varTypeEnum = ScriptVarType.TimeList;
            else if (varTypeName == "DesignLineList")
                varTypeEnum = ScriptVarType.DesignLineList;
            else if (varTypeName == "GameStatePredicate")
                varTypeEnum = ScriptVarType.GameStatePredicate;
            else if (varTypeName == "FloatList")
                varTypeEnum = ScriptVarType.FloatList;
            else if (varTypeName == "UILocationMinigame")
                varTypeEnum = ScriptVarType.UILocationMinigame;
            else if (varTypeName == "SquadFlag")
                varTypeEnum = ScriptVarType.SquadFlag;
            else if (varTypeName == "Concept")
                varTypeEnum = ScriptVarType.Concept;
            else if (varTypeName == "ConceptList")
                varTypeEnum = ScriptVarType.ConceptList;
            else if (varTypeName == "UserClassType")
                varTypeEnum = ScriptVarType.UserClassType;

            return varTypeEnum;
        }
        public static Dictionary<string, string> EffectCategories { get; private set; } = new Dictionary<string, string>()
        {
            { "AIAddTeleporterZone", "AI" },
{ "AIAnalyzeKBSquadList", "AI" },
{ "AIAnalyzeOffenseAToB", "AI" },
{ "AIAnalyzeProtoSquadList", "AI" },
{ "AIAnalyzeSquadList", "AI" },
{ "AIBindLog", "AI" },
{ "AICalculateOffenseRatioAToB", "AI" },
{ "AICalculations", "AI" },
{ "AICalculations2", "AI" },
{ "AIChat", "AI" },
{ "AIClearOpportunityRequests", "AI" },
{ "AICreateAreaTarget", "AI" },
{ "AICreateWrapper", "AI" },
{ "AIDestroyWrapper", "AI" },
{ "AIFactoidSubmit", "AI" },
{ "AIGenerateMissionTargets", "AI" },
{ "AIGetAlertData", "AI" },
{ "AIGetAttackAlerts", "AI" },
{ "AIGetFlareAlerts", "AI" },
{ "AIGetLastAttackAlert", "AI" },
{ "AIGetLastFlareAlert", "AI" },
{ "AIGetMemory", "AI" },
{ "AIGetMissionTargets", "AI" },
{ "AIGetMissions", "AI" },
{ "AIGetOpportunityRequests", "AI" },
{ "AIGetTerminalMissions", "AI" },
{ "AIMissionAddSquads", "AI" },
{ "AIMissionCancel", "AI" },
{ "AIMissionCreate", "AI" },
{ "AIMissionGetLaunchScores", "AI" },
{ "AIMissionGetSquads", "AI" },
{ "AIMissionGetTarget", "AI" },
{ "AIMissionLaunch", "AI" },
{ "AIMissionModifyTickets", "AI" },
{ "AIMissionRemoveSquads", "AI" },
{ "AIMissionSetFlags", "AI" },
{ "AIMissionSetMoveAttack", "AI" },
{ "AIMissionTargetGetLocation", "AI" },
{ "AIMissionTargetGetScores", "AI" },
{ "AIQueryMissionTargets", "AI" },
{ "AIRegisterHook", "AI" },
{ "AIRemoveFromMissions", "AI" },
{ "AIReorderWrapper", "AI" },
{ "AISAGetComponent", "AI" },
{ "AIScnMissionAttackArea", "AI" },
{ "AIScnMissionDefendArea", "AI" },
{ "AIScoreMissionTargets", "AI" },
{ "AISetAssetMultipliers", "AI" },
{ "AISetBiases", "AI" },
{ "AISetFocus", "AI" },
{ "AISetPlayerAssetModifier", "AI" },
{ "AISetPlayerBuildSpeedModifiers", "AI" },
{ "AISetPlayerDamageModifiers", "AI" },
{ "AISetPlayerMultipliers", "AI" },
{ "AISetScoringParms", "AI" },
{ "AISetWinRange", "AI" },
{ "AISortMissionTargets", "AI" },
{ "AITopicCreate", "AI" },
{ "AITopicDestroy", "AI" },
{ "AITopicLotto", "AI" },
{ "AITopicModifyTickets", "AI" },
{ "AITopicPriorityRequest", "AI" },
{ "AITopicSetFocus", "AI" },
{ "AIUnopposedTimeToKill", "AI" },
{ "AIWrapperModifyFlags", "AI" },
{ "AIWrapperModifyParms", "AI" },
{ "AIWrapperModifyRadius", "AI" },
{ "BidAddToMissions", "AI" },
{ "BidClear", "AI" },
{ "BidCreateBlank", "AI" },
{ "BidCreateBuilding", "AI" },
{ "BidCreatePower", "AI" },
{ "BidCreateSquad", "AI" },
{ "BidCreateTech", "AI" },
{ "BidDelete", "AI" },
{ "BidGetData", "AI" },
{ "BidPurchase", "AI" },
{ "BidQuery", "AI" },
{ "BidRemoveFromMissions", "AI" },
{ "BidSetBlockedBuilders", "AI" },
{ "BidSetBuilder", "AI" },
{ "BidSetBuilding", "AI" },
{ "BidSetPadSupplies", "AI" },
{ "BidSetPower", "AI" },
{ "BidSetPriority", "AI" },
{ "BidSetQueueLimits", "AI" },
{ "BidSetSquad", "AI" },
{ "BidSetTargetLocation", "AI" },
{ "BidSetTech", "AI" },
{ "GetBidsMatching", "AI" },
{ "GetKBBaseLocation", "AI" },
{ "KBAddSquadsToKB", "AI|KB" },
{ "KBBQExecute", "AI|KB" },
{ "KBBQExecuteClosest", "AI|KB" },
{ "KBBQMaxStaleness", "AI|KB" },
{ "KBBQMinStaleness", "AI|KB" },
{ "KBBQPlayerRelation", "AI|KB" },
{ "KBBQPointRadius", "AI|KB" },
{ "KBBQReset", "AI|KB" },
{ "KBBaseGetDistance", "AI|KB" },
{ "KBBaseGetKBSquads", "AI|KB" },
{ "KBBaseGetMass", "AI|KB" },
{ "KBSFAddCurrentlyVisible", "AI|KB" },
{ "KBSFAddInList", "AI|KB" },
{ "KBSFAddMaxStaleness", "AI|KB" },
{ "KBSFAddMinStaleness", "AI|KB" },
{ "KBSFAddObjectTypes", "AI|KB" },
{ "KBSFAddPlayerRelation", "AI|KB" },
{ "KBSFAddPlayers", "AI|KB" },
{ "KBSQBase", "AI|KB" },
{ "KBSQCurrentlyVisible", "AI|KB" },
{ "KBSQExecute", "AI|KB" },
{ "KBSQExecuteClosest", "AI|KB" },
{ "KBSQInit", "AI|KB" },
{ "KBSQMaxStaleness", "AI|KB" },
{ "KBSQMinStaleness", "AI|KB" },
{ "KBSQObjectType", "AI|KB" },
{ "KBSQPlayerRelation", "AI|KB" },
{ "KBSQPointRadius", "AI|KB" },
{ "KBSQReset", "AI|KB" },
{ "KBSquadFilterClear", "AI|KB" },
{ "KBSquadGetLocation", "AI|KB" },
{ "KBSquadGetOwner", "AI|KB" },
{ "KBSquadGetProtoSquad", "AI|KB" },
{ "KBSquadListDiff", "AI|KB" },
{ "KBSquadListFilter", "AI|KB" },
{ "KBSquadListGetSize", "AI|KB" },
{ "ConceptClearSub", "Concept" },
{ "ConceptGetParameters", "Concept" },
{ "ConceptPermission", "Concept" },
{ "ConceptResetCooldown", "Concept" },
{ "ConceptSetParameters", "Concept" },
{ "ConceptSetPrecondition", "Concept" },
{ "ConceptSetState", "Concept" },
{ "ConceptSetttings", "Concept" },
{ "ConceptStartSub", "Concept" },
{ "AsCount", "Convert" },
{ "AsFloat", "Convert" },
{ "AsString", "Convert" },
{ "AsTime", "Convert" },
{ "CombineString", "Convert" },
{ "ConvertKBSquadsToSquads", "Convert" },
{ "CostToFloat", "Convert" },
{ "CountToInt", "Convert" },
{ "IntToCount", "Convert" },
{ "IntToPower", "Convert" },
{ "InvertBool", "Convert" },
{ "ObjectTypeToProtoObjects", "Convert" },
{ "PlayersToTeams", "Convert" },
{ "PowerToInt", "Convert" },
{ "TeamsToPlayers", "Convert" },
{ "TimeToFloat", "Convert" },
{ "CopyAISquadAnalysis", "Copy" },
{ "CopyBool", "Copy" },
{ "CopyChatSpeaker", "Copy" },
{ "CopyColor", "Copy" },
{ "CopyControlType", "Copy" },
{ "CopyCost", "Copy" },
{ "CopyCount", "Copy" },
{ "CopyDesignLine", "Copy" },
{ "CopyDesignLineList", "Copy" },
{ "CopyDirection", "Copy" },
{ "CopyDistance", "Copy" },
{ "CopyEntity", "Copy" },
{ "CopyEntityList", "Copy" },
{ "CopyFloat", "Copy" },
{ "CopyFloatList", "Copy" },
{ "CopyHitpoints", "Copy" },
{ "CopyIconType", "Copy" },
{ "CopyIntegerList", "Copy" },
{ "CopyKBBase", "Copy" },
{ "CopyKBSquad", "Copy" },
{ "CopyLOSType", "Copy" },
{ "CopyLocStringID", "Copy" },
{ "CopyLocation", "Copy" },
{ "CopyLocationList", "Copy" },
{ "CopyMessageIndex", "Copy" },
{ "CopyObject", "Copy" },
{ "CopyObjectList", "Copy" },
{ "CopyObjectType", "Copy" },
{ "CopyObjectTypeList", "Copy" },
{ "CopyObjective", "Copy" },
{ "CopyOperator", "Copy" },
{ "CopyPercent", "Copy" },
{ "CopyPlayer", "Copy" },
{ "CopyProtoObject", "Copy" },
{ "CopyProtoObjectList", "Copy" },
{ "CopyProtoSquad", "Copy" },
{ "CopyProtoSquadList", "Copy" },
{ "CopySound", "Copy" },
{ "CopySquad", "Copy" },
{ "CopySquadList", "Copy" },
{ "CopyString", "Copy" },
{ "CopyTech", "Copy" },
{ "CopyTechList", "Copy" },
{ "CopyTechStatus", "Copy" },
{ "CopyTime", "Copy" },
{ "CopyTimeList", "Copy" },
{ "CopyUnit", "Copy" },
{ "CopyUnitList", "Copy" },
{ "AirStrike", "Cut" },
{ "ConnectHitpointBar", "Cut" },
{ "GetPrimaryHealthComponent", "Cut" },
{ "GroupDeactivate", "Cut" },
{ "MKTest", "Cut" },
{ "OBSOLETEUsePower", "Cut" },
{ "SetFlagNearLayer", "Cut" },
{ "SetScenarioScore", "Cut" },
{ "ShowProgressBar", "Cut" },
{ "UpdateProgressBar", "Cut" },
{ "UITogglePowerOverlay", "Cut?" },
{ "Breakpoint", "Debug" },
{ "DebugDirection", "Debug" },
{ "DebugVarAnimType", "Debug" },
{ "DebugVarBool", "Debug" },
{ "DebugVarBuildingCommandState", "Debug" },
{ "DebugVarColor", "Debug" },
{ "DebugVarCost", "Debug" },
{ "DebugVarCount", "Debug" },
{ "DebugVarDesignLine", "Debug" },
{ "DebugVarDistance", "Debug" },
{ "DebugVarFloat", "Debug" },
{ "DebugVarHitpoints", "Debug" },
{ "DebugVarIterator", "Debug" },
{ "DebugVarLocation", "Debug" },
{ "DebugVarMathOperator", "Debug" },
{ "DebugVarObjectType", "Debug" },
{ "DebugVarObjectTypeList", "Debug" },
{ "DebugVarObjective", "Debug" },
{ "DebugVarOperator", "Debug" },
{ "DebugVarPercent", "Debug" },
{ "DebugVarPlayer", "Debug" },
{ "DebugVarPlayerList", "Debug" },
{ "DebugVarPlayerState", "Debug" },
{ "DebugVarPower", "Debug" },
{ "DebugVarProtoObject", "Debug" },
{ "DebugVarProtoObjectList", "Debug" },
{ "DebugVarProtoSquad", "Debug" },
{ "DebugVarProtoSquadList", "Debug" },
{ "DebugVarSound", "Debug" },
{ "DebugVarSquad", "Debug" },
{ "DebugVarSquadList", "Debug" },
{ "DebugVarString", "Debug" },
{ "DebugVarTeam", "Debug" },
{ "DebugVarTeamList", "Debug" },
{ "DebugVarTech", "Debug" },
{ "DebugVarTechList", "Debug" },
{ "DebugVarTechStatus", "Debug" },
{ "DebugVarTime", "Debug" },
{ "DebugVarUIButton", "Debug" },
{ "DebugVarUILocation", "Debug" },
{ "DebugVarUISquad", "Debug" },
{ "DebugVarUIUnit", "Debug" },
{ "DebugVarUnit", "Debug" },
{ "DebugVarUnitList", "Debug" },
{ "EventClearFilters", "Event" },
{ "EventDelete", "Event" },
{ "EventFilterCamera", "Event" },
{ "EventFilterEntity", "Event" },
{ "EventFilterEntityList", "Event" },
{ "EventFilterGameState", "Event" },
{ "EventFilterNumeric", "Event" },
{ "EventFilterType", "Event" },
{ "EventReset", "Event" },
{ "EventSetFilter", "Event" },
{ "EventSubscribe", "Event" },
{ "EventSubscribeUseCount", "Event" },
{ "GetPowerRadius", "Game|Player" },
{ "CustomCommandAdd", "Game|Command" },
{ "CustomCommandExecute", "Game|Command" },
{ "CustomCommandRemove", "Game|Command" },
{ "HideCircleMenu", "Game|Hint" },
{ "HintCalloutCreate", "Game|Hint" },
{ "HintCalloutDestroy", "Game|Hint" },
{ "HintGlowToggle", "Game|Hint" },
{ "HintMessageDestroy", "Game|Hint" },
{ "HintMessageShow", "Game|Hint" },
{ "IgnoreDpad", "Game|Input" },
{ "InputUIButton", "Game|Input" },
{ "InputUILocation", "Game|Input" },
{ "InputUILocationMinigame", "Game|Input" },
{ "InputUIPlaceSquads", "Game|Input" },
{ "InputUISquad", "Game|Input" },
{ "InputUISquadList", "Game|Input" },
{ "InputUIUnit", "Game|Input" },
{ "ActivateTentacle", "Game|Unit" },
{ "ChangeControlledPlayer", "Game|Singleplayer" },
{ "GetObjectiveStats", "Game|Objective" },
{ "ObjectiveComplete", "Game|Objective" },
{ "ObjectiveDecrementCounter", "Game|Objective" },
{ "ObjectiveDisplay", "Game|Objective" },
{ "ObjectiveGetCurrentCounter", "Game|Objective" },
{ "ObjectiveGetFinalCounter", "Game|Objective" },
{ "ObjectiveIncrementCounter", "Game|Objective" },
{ "ObjectiveUserMessage", "Game|Objective" },
{ "BlockLeaderPowers", "Game|Player" },
{ "BlockMinimap", "Game|Player" },
{ "CameraShake", "Game|Player" },
{ "EnableAttackNotifications", "Game|Player" },
{ "EnableChats", "Game|Player" },
{ "EnableFollowCam", "Game|Player" },
{ "EnableLetterBox", "Game|Player" },
{ "EnableScreenBlur", "Game|Player" },
{ "EnableUserMessage", "Game|Player" },
{ "FadeToColor", "Game|Player" },
{ "FadeTransition", "Game|Player" },
{ "FlareMinimapNormal", "Game|Player" },
{ "FlareMinimapSpoof", "Game|Player" },
{ "FlashUIElement", "Game|Player" },
{ "Forbid", "Game|Player" },
{ "GetLegalBuildings", "Game|Player" },
{ "GetLegalSquads", "Game|Player" },
{ "GetLegalTechs", "Game|Player" },
{ "GetNPCPlayersByName", "Game|Player" },
{ "GetPlayerCiv", "Game|Player" },
{ "GetPlayerColor", "Game|Player" },
{ "GetPlayerEconomy", "Game|Player" },
{ "GetPlayerLeader", "Game|Player" },
{ "GetPlayerMilitaryStats", "Game|Player" },
{ "GetPlayerPop", "Game|Player" },
{ "GetPlayerScore", "Game|Player" },
{ "GetPlayerTeam", "Game|Player" },
{ "GetPlayers", "Game|Player" },
{ "GetPlayers2", "Game|Player" },
{ "GetPopularSquadType", "Game|Player" },
{ "GetPrimaryUser", "Game|Player" },
{ "GetResources", "Game|Player" },
{ "GetResourcesTotals", "Game|Player" },
{ "GetSquads", "Game|Player" },
{ "GetTeamPlayers", "Game|Player" },
{ "GetTeams", "Game|Player" },
{ "GetTechResearcherType", "Game|Player" },
{ "GetTrickleRate", "Game|Player" },
{ "GetUTechBldings", "Game|Player" },
{ "GetUnits", "Game|Player" },
{ "GrantAchievement", "Game|Player" },
{ "HUDToggle", "Game|Player" },
{ "LockPlayerUser", "Game|Player" },
{ "PlayerSelectSquads", "Game|Player" },
{ "PowerClear", "Game|Player" },
{ "PowerGrant", "Game|Player" },
{ "PowerInvoke", "Game|Player" },
{ "PowerMenuEnable", "Game|Player" },
{ "PowerRevoke", "Game|Player" },
{ "PowerUsed", "Game|Player" },
{ "PowerUserShutdown", "Game|Player" },
{ "RallyPointClear", "Game|Player" },
{ "RallyPointGet", "Game|Player" },
{ "RallyPointSet", "Game|Player" },
{ "RefundCost", "Game|Player" },
{ "RumbleStart", "Game|Player" },
{ "RumbleStop", "Game|Player" },
{ "SetCamera", "Game|Player" },
{ "SetFollowCam", "Game|Player" },
{ "SetIgnoreUserInput", "Game|Player" },
{ "SetPlayableBounds", "Game|Player" },
{ "SetPlayerPop", "Game|Player" },
{ "SetPlayerState", "Game|Player" },
{ "SetPowerAvailableTime", "Game|Player" },
{ "SetResourceHandicap", "Game|Player" },
{ "SetResources", "Game|Player" },
{ "SetResourcesTotals", "Game|Player" },
{ "SetTransportPickUpLocations", "Game|Player" },
{ "SetTrickleRate", "Game|Player" },
{ "SetUIPowerRadius", "Game|Player" },
{ "ShowInfoDialog", "Game|Player" },
{ "ShowMessage", "Game|Player" },
{ "TeamSetDiplomacy", "Game|Player" },
{ "TechActivate", "Game|Player" },
{ "TechDeactivate", "Game|Player" },
{ "TimeUserMessage", "Game|Player" },
{ "UIUnlock", "Game|Player" },
{ "UserMessage", "Game|Player" },
{ "RefCountSquadAdd", "Game|Ref" },
{ "RefCountSquadRemove", "Game|Ref" },
{ "RefCountUnitAdd", "Game|Ref" },
{ "RefCountUnitRemove", "Game|Ref" },
{ "DesignFindSphere", "Game|Scenario" },
{ "DesignLineGetPoints", "Game|Scenario" },
{ "GetDifficulty", "Game|Scenario" },
{ "GetGameMode", "Game|Scenario" },
{ "GetGameTime", "Game|Scenario" },
{ "GetGameTimeRemaining", "Game|Scenario" },
{ "LaunchCinematic", "Game|Scenario" },
{ "LoadGame", "Game|Scenario" },
{ "MissionResult", "Game|Scenario" },
{ "SaveGame", "Game|Scenario" },
{ "SetCitizensSaved", "Game|Scenario" },
{ "SetGarrisonedCount", "Game|Scenario" },
{ "SetScenarioScoreInfo", "Game|Scenario" },
{ "ShowCitizensSaved", "Game|Scenario" },
{ "ShowGarrisonedCount", "Game|Scenario" },
{ "ShowObjectCounter", "Game|Scenario" },
{ "ShowObjectivePointer", "Game|Scenario" },
{ "UpdateObjectCounter", "Game|Scenario" },
{ "DestroyTimer", "Game|Timer" },
{ "TimerGet", "Game|Timer" },
{ "TimerSet", "Game|Timer" },
{ "TimerSetPaused", "Game|Timer" },
{ "AddXP", "Game|Unit" },
{ "AttachmentAddObject", "Game|Unit" },
{ "AttachmentAddType", "Game|Unit" },
{ "AttachmentAddUnit", "Game|Unit" },
{ "AttachmentRemoveAll", "Game|Unit" },
{ "AttachmentRemoveObject", "Game|Unit" },
{ "AttachmentRemoveType", "Game|Unit" },
{ "AttachmentRemoveUnit", "Game|Unit" },
{ "Attack", "Game|Unit" },
{ "BuildingCommand", "Game|Unit" },
{ "ChangeOwner", "Game|Unit" },
{ "ChangeSquadMode", "Game|Unit" },
{ "ClearBuildingCommandState", "Game|Unit" },
{ "Cloak", "Game|Unit" },
{ "CloakDetected", "Game|Unit" },
{ "CombatDamage", "Game|Unit" },
{ "CreateObject", "Game|Unit" },
{ "CreateObstructionUnit", "Game|Unit" },
{ "CreateSquad", "Game|Unit" },
{ "CreateSquads", "Game|Unit" },
{ "Damage", "Game|Unit" },
{ "Destroy", "Game|Unit" },
{ "EnableOverrideTint", "Game|Unit" },
{ "EnableShield", "Game|Unit" },
{ "FlashEntity", "Game|Unit" },
{ "GetAmmo", "Game|Unit" },
{ "GetBuildingTrainQueue", "Game|Unit" },
{ "GetChildUnits", "Game|Unit" },
{ "GetCost", "Game|Unit" },
{ "GetDirection", "Game|Unit" },
{ "GetGarrisonedSquads", "Game|Unit" },
{ "GetGarrisonedUnits", "Game|Unit" },
{ "GetHealth", "Game|Unit" },
{ "GetHitZoneHealth", "Game|Unit" },
{ "GetIdleDuration", "Game|Unit" },
{ "GetLOS", "Game|Unit" },
{ "GetLevel", "Game|Unit" },
{ "GetLocation", "Game|Unit" },
{ "GetNumTransports", "Game|Unit" },
{ "GetObstructionRadius", "Game|Unit" },
{ "GetOwner", "Game|Unit" },
{ "GetParentSquad", "Game|Unit" },
{ "GetPop", "Game|Unit" },
{ "GetProtoObject", "Game|Unit" },
{ "GetProtoSquad", "Game|Unit" },
{ "GetSquadTrainerType", "Game|Unit" },
{ "Infect", "Game|Unit" },
{ "Kill", "Game|Unit" },
{ "MegaTurretAttack", "Game|Unit" },
{ "ModifyDataScalar", "Game|Unit" },
{ "ModifyProtoData", "Game|Unit" },
{ "ModifyProtoSquadData", "Game|Unit" },
{ "Move", "Game|Unit" },
{ "MovePath", "Game|Unit" },
{ "MoveToFace", "Game|Unit" },
{ "ParkingLotSet", "Game|Unit" },
{ "PlayAnimationObject", "Game|Unit" },
{ "PlayAnimationSquad", "Game|Unit" },
{ "PlayAnimationUnit", "Game|Unit" },
{ "RecycleBuilding", "Game|Unit" },
{ "ReinforceSquad", "Game|Unit" },
{ "Repair", "Game|Unit" },
{ "RepairByCombatValue", "Game|Unit" },
{ "ResetAbilityTimer", "Game|Unit" },
{ "ReverseHotDrop", "Game|Unit" },
{ "SensorLock", "Game|Unit" },
{ "SetAmmo", "Game|Unit" },
{ "SetAutoAttackable", "Game|Unit" },
{ "SetBlockAttack", "Game|Unit" },
{ "SetDirection", "Game|Unit" },
{ "SetHitZoneActive", "Game|Unit" },
{ "SetHitZoneHealth", "Game|Unit" },
{ "SetLevel", "Game|Unit" },
{ "SetMobile", "Game|Unit" },
{ "SetOccluded", "Game|Unit" },
{ "SetOverrideTint", "Game|Unit" },
{ "SetPosition", "Game|Unit" },
{ "SetSelectable", "Game|Unit" },
{ "SetTeleporterDestination", "Game|Unit" },
{ "SetTowerWallDestination", "Game|Unit" },
{ "SetUnitAttackTarget", "Game|Unit" },
{ "Settle", "Game|Unit" },
{ "SquadFlagSet", "Game|Unit" },
{ "Teleport", "Game|Unit" },
{ "TeleportUnitsOffObstruction", "Game|Unit" },
{ "TransferGarrisoned", "Game|Unit" },
{ "TransferGarrisonedSquad", "Game|Unit" },
{ "Transform", "Math" },
{ "TransportSquads", "Game|Unit" },
{ "TransportSquads2", "Game|Unit" },
{ "UnitFlagSet", "Game|Unit" },
{ "Unload", "Game|Unit" },
{ "Unpack", "Game|Unit" },
{ "Work", "Game|Unit" },
{ "Blocker", "Game|World" },
{ "CarpetBomb", "Game|World" },
{ "ChatDestroy", "Game|Scenario" },
{ "ChatForceSubtitles", "Game|Scenario" },
{ "ClearBlackMap", "Game|World" },
{ "ClearCorpseUnits", "Game|World" },
{ "CreateIconObject", "Game|World" },
{ "CreateTimer", "Game|Timer" },
{ "CreateUnit", "Game|Unit" },
{ "EnableFogOfWar", "Game|World" },
{ "EnableMusicManager", "Game|World" },
{ "GetClosestPath", "Game|World" },
{ "GetClosestPowerSquad", "Game|World" },
{ "GetClosestSquad", "Game|World" },
{ "GetClosestUnit", "Game|World" },
{ "GetDeadSquadCount", "Game|World" },
{ "GetDeadUnitCount", "Game|World" },
{ "GetDirectionFromLocations", "Game|World" },
{ "GetDistanceLocationLocation", "Game|World" },
{ "GetDistanceUnitLocation", "Game|World" },
{ "GetDistanceUnitUnit", "Game|World" },
{ "GetMeanLocation", "Game|World" },
{ "LaunchProjectile", "Game|World" },
{ "LightsetAnimate", "Game|World" },
{ "LocationAdjust", "Game|World" },
{ "LocationAdjustDir", "Game|World" },
{ "LocationTieToGround", "Game|World" },
{ "PatherObstructionRebuild", "Game|World" },
{ "PatherObstructionUpdates", "Game|World" },
{ "PayCost", "Game|Player" },
{ "PlayChat", "Game|Scenario" },
{ "PlayRelationSound", "Game|World" },
{ "PlaySound", "Game|World" },
{ "PlayVideo", "Game|World" },
{ "PlayWorldSoundAtPosition", "Game|World" },
{ "PlayWorldSoundOnObject", "Game|World" },
{ "RandomLocation", "Game|World" },
{ "ResetBlackMap", "Game|World" },
{ "ResetDopple", "Game|World" },
{ "Revealer", "Game|World" },
{ "SetMinimapNorthPointerRotation", "Game|World" },
{ "SetMinimapSkirtMirroring", "Game|World" },
{ "SetRenderTerrainSkirt", "Game|World" },
{ "UsePower", "Game|Player" },
{ "DesignLineListAdd", "List" },
{ "DesignLineListGetSize", "List" },
{ "DesignLineListRemove", "List" },
{ "EntityFilterAddCanChangeOwner", "List|Filter" },
{ "EntityFilterAddDiplomacy", "List|Filter" },
{ "EntityFilterAddInList", "List|Filter" },
{ "EntityFilterAddIsAlive", "List|Filter" },
{ "EntityFilterAddIsIdle", "List|Filter" },
{ "EntityFilterAddIsSelected", "List|Filter" },
{ "EntityFilterAddJacking", "List|Filter" },
{ "EntityFilterAddMaxObjectType", "List|Filter" },
{ "EntityFilterAddObjectTypes", "List|Filter" },
{ "EntityFilterAddPlayers", "List|Filter" },
{ "EntityFilterAddProtoObjects", "List|Filter" },
{ "EntityFilterAddProtoSquads", "List|Filter" },
{ "EntityFilterAddRefCount", "List|Filter" },
{ "EntityFilterAddTeams", "List|Filter" },
{ "EntityFilterClear", "List|Filter" },
{ "EntityListShuffle", "List" },
{ "FloatListAdd", "List" },
{ "FloatListGetSize", "List" },
{ "FloatListRemove", "List" },
{ "IntegerListAdd", "List" },
{ "IntegerListGetSize", "List" },
{ "IntegerListRemove", "List" },
{ "IteratorKBBaseList", "List" },
{ "IteratorLocationList", "List" },
{ "IteratorObjectList", "List" },
{ "IteratorObjectTypeList", "List" },
{ "IteratorPlayerList", "List" },
{ "IteratorProtoObjectList", "List" },
{ "IteratorProtoSquadList", "List" },
{ "IteratorSquadList", "List" },
{ "IteratorTeamList", "List" },
{ "IteratorTechList", "List" },
{ "IteratorUnitList", "List" },
{ "LocationListAdd", "List" },
{ "LocationListGetByIndex", "List" },
{ "LocationListGetClosest", "List" },
{ "LocationListGetSize", "List" },
{ "LocationListPartition", "List" },
{ "LocationListRemove", "List" },
{ "LocationListShuffle", "List" },
{ "ObjectListAdd", "List" },
{ "ObjectListGetSize", "List" },
{ "ObjectListRemove", "List" },
{ "ObjectTypeListShuffle", "List" },
{ "PlayerListAdd", "List" },
{ "PlayerListGetSize", "List" },
{ "PlayerListRemove", "List" },
{ "PlayerListShuffle", "List" },
{ "ProtoObjectListAdd", "List" },
{ "ProtoObjectListRemove", "List" },
{ "ProtoObjectListShuffle", "List" },
{ "ProtoSquadListAdd", "List" },
{ "ProtoSquadListGetSize", "List" },
{ "ProtoSquadListRemove", "List" },
{ "ProtoSquadListShuffle", "List" },
{ "SquadListAdd", "List" },
{ "SquadListDiff", "List" },
{ "SquadListFilter", "List" },
{ "SquadListGetSize", "List" },
{ "SquadListPartition", "List" },
{ "SquadListRemove", "List" },
{ "SquadListShuffle", "List" },
{ "TeamListAdd", "List" },
{ "TeamListRemove", "List" },
{ "TeamListShuffle", "List" },
{ "TechListAdd", "List" },
{ "TechListRemove", "List" },
{ "TechListShuffle", "List" },
{ "TimeListAdd", "List" },
{ "TimeListGetSize", "List" },
{ "TimeListRemove", "List" },
{ "UnitListAdd", "List" },
{ "UnitListDiff", "List" },
{ "UnitListFilter", "List" },
{ "UnitListGetSize", "List" },
{ "UnitListPartition", "List" },
{ "UnitListRemove", "List" },
{ "UnitListShuffle", "List" },
{ "CalculatePercentCount", "Math" },
{ "CalculatePercentHitpoints", "Math" },
{ "CalculatePercentTime", "Math" },
{ "CountDecrement", "Math" },
{ "CountIncrement", "Math" },
{ "LerpColor", "Math" },
{ "LerpCount", "Math" },
{ "LerpLocation", "Math" },
{ "LerpPercent", "Math" },
{ "LerpTime", "Math" },
{ "MathCount", "Math" },
{ "MathDistance", "Math" },
{ "MathFloat", "Math" },
{ "MathHitpoints", "Math" },
{ "MathLocation", "Math" },
{ "MathPercent", "Math" },
{ "MathResources", "Math" },
{ "MathTime", "Math" },
{ "RandomCount", "Math" },
{ "RandomTime", "Math" },
{ "RoundFloat", "Math" },
{ "LaunchScript", "Special" },
{ "Shutdown", "Special" },
{ "TableLoad", "Special" },
{ "TriggerActivate", "Special" },
{ "TriggerDeactivate", "Special" },
        };
        public static Dictionary<string, string> ConditionCategories { get; private set; } = new Dictionary<string, string>()
        {

        };

        protected override void OnInit()
        {
            TriggerscriptEditorPages = new List<TriggerscriptEditorPage>();
            Triggerscripts = new Dictionary<string, TriggerscriptEditorData>();
            UserClasses = new UserClassesXml();
            UserTables = new Dictionary<string, UserTablesXml>();


            Operator opOpenTriggerscript = new Operator("Open Triggerscript...");
            opOpenTriggerscript.OperatorActivated += (sender, e) =>
            {
                OpenFileDialog ofd = new OpenFileDialog();
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    TriggerscriptEditorData data = new TriggerscriptEditorData(Instance);
                    //if(data.TryLoad(ofd.FileName))
                    {
                        TriggerscriptEditorPage page = new TriggerscriptEditorPage(Instance);
                        //page.Data = data;
                        page.Show(Instance, WeifenLuo.WinFormsUI.Docking.DockState.Document);
                    }
                }
            };
            opOpenTriggerscript.Parent = Instance.Operator_File;
            Instance.RefreshToolstrip();
        }
        protected override void OnPostInit()
        {
        }
        protected override void OnWorkspaceOpened()
        {
            Instance.Browser.RootItems.Add(new RootBrowserItem(
                Instance.GetNamedWorkspaceDir(FoundryInstance.NamedWorkspaceDirNames.Triggerscripts)
                ));
            Instance.Browser.UpdateView();

            //IndexAllTriggerscripts();
            //IndexAllUserClasses();
            //IndexAllUserTables();
        }
        protected override void OnWorkspaceClosed()
        {
            TriggerscriptEditorPages.Clear();
            Triggerscripts.Clear();
            UserClasses = new UserClassesXml();
            UserTables.Clear();
        }

        private List<TriggerscriptEditorPage> TriggerscriptEditorPages;
        private Dictionary<string, TriggerscriptEditorData> Triggerscripts;
        private void IndexAllTriggerscripts()
        {
            var scriptsdir = Instance.GetNamedWorkspaceDir(FoundryInstance.NamedWorkspaceDirNames.Triggerscripts);
            YAXSerializer<TriggerscriptXmlData> ser = new YAXSerializer<TriggerscriptXmlData>();
            foreach (var file in Directory.EnumerateFiles(
                scriptsdir.FullPath,
                "*.triggerscript",
                SearchOption.AllDirectories))
            {
                if(!Triggerscripts.ContainsKey(file))
                {
                    var tsdata = new TriggerscriptEditorData(Instance);
                    //if (!tsdata.TryLoad(file)) continue;

                    Triggerscripts.Add(Path.GetRelativePath(scriptsdir.FullPath, file), tsdata);
                }
            }
        }
        public bool OpenNewTriggerscriptEditorPage(string script)
        {
            if (!Triggerscripts.ContainsKey(script)) return false;

            TriggerscriptEditorPage page = new TriggerscriptEditorPage(Instance);
            //page.Data = Triggerscripts[script];
            page.Show(Instance, WeifenLuo.WinFormsUI.Docking.DockState.Document);
            
            TriggerscriptEditorPages.Add(page);
            page.ViewClosed += (sender, e) =>
            {
                TriggerscriptEditorPages.Remove(page);
            };

            return true;
        }
        
        private UserClassesXml UserClasses;
        private void IndexAllUserClasses()
        {
            string userclassFile = Instance.GetNamedWorkspaceDir(FoundryInstance.NamedWorkspaceDirNames.Triggerscripts).FullPath + "userclasses.xml";
            if(File.Exists(userclassFile))
            {
                string userclassesContent = File.ReadAllText(userclassFile);
                YAXSerializer<UserClassesXml> ser = new YAXSerializer<UserClassesXml>();
                UserClasses = ser.Deserialize(userclassesContent);
            }
        }
        
        
        private Dictionary<string, UserTablesXml> UserTables;
        private void IndexAllUserTables()
        {
             
        }
    }
}