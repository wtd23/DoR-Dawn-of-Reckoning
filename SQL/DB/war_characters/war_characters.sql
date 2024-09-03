/*
 Navicat MySQL Dump SQL

 Source Server         : warserver
 Source Server Type    : MySQL
 Source Server Version : 90001 (9.0.1)
 Source Host           : localhost:3306
 Source Schema         : war_characters

 Target Server Type    : MySQL
 Target Server Version : 90001 (9.0.1)
 File Encoding         : 65001

 Date: 22/08/2024 01:37:44
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for auctions
-- ----------------------------
DROP TABLE IF EXISTS `auctions`;
CREATE TABLE `auctions`  (
  `AuctionId` bigint UNSIGNED NOT NULL,
  `Realm` tinyint UNSIGNED NOT NULL,
  `SellerId` int UNSIGNED NOT NULL,
  `ItemId` int UNSIGNED NOT NULL,
  `SellPrice` int UNSIGNED NOT NULL,
  `Count` smallint UNSIGNED NOT NULL,
  `StartTime` int UNSIGNED NOT NULL,
  `Talismans` varchar(40) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `PrimaryDye` smallint UNSIGNED NOT NULL,
  `SecondaryDye` smallint UNSIGNED NOT NULL,
  PRIMARY KEY (`AuctionId`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of auctions
-- ----------------------------

-- ----------------------------
-- Table structure for banned_names
-- ----------------------------
DROP TABLE IF EXISTS `banned_names`;
CREATE TABLE `banned_names`  (
  `NameString` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `FilterTypeString` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
  PRIMARY KEY (`NameString`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of banned_names
-- ----------------------------

-- ----------------------------
-- Table structure for bugs_reports
-- ----------------------------
DROP TABLE IF EXISTS `bugs_reports`;
CREATE TABLE `bugs_reports`  (
  `AccountId` int UNSIGNED NOT NULL,
  `CharacterId` int UNSIGNED NOT NULL,
  `ZoneId` smallint UNSIGNED NOT NULL,
  `X` smallint UNSIGNED NOT NULL,
  `Y` smallint UNSIGNED NOT NULL,
  `Time` int UNSIGNED NOT NULL,
  `Type` tinyint UNSIGNED NOT NULL,
  `Category` tinyint UNSIGNED NOT NULL,
  `Message` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ReportType` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `FieldSting` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Assigned` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
  `bugs_reports_ID` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`bugs_reports_ID`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of bugs_reports
-- ----------------------------

-- ----------------------------
-- Table structure for characters
-- ----------------------------
DROP TABLE IF EXISTS `characters`;
CREATE TABLE `characters`  (
  `CharacterId` int UNSIGNED NOT NULL,
  `Name` varchar(24) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Surname` varchar(24) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `RealmId` int NOT NULL,
  `AccountId` int NOT NULL,
  `SlotId` tinyint UNSIGNED NOT NULL,
  `ModelId` tinyint UNSIGNED NOT NULL,
  `Career` tinyint UNSIGNED NOT NULL,
  `CareerLine` tinyint UNSIGNED NOT NULL,
  `Realm` tinyint UNSIGNED NOT NULL,
  `HeldLeft` int NOT NULL,
  `Race` tinyint UNSIGNED NOT NULL,
  `Traits` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Sex` tinyint UNSIGNED NOT NULL,
  `Anonymous` tinyint UNSIGNED NOT NULL,
  `Hidden` tinyint UNSIGNED NOT NULL,
  `OldName` varchar(24) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `PetName` varchar(24) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `PetModel` smallint UNSIGNED NOT NULL,
  `HonorPoints` smallint UNSIGNED NOT NULL,
  `HonorRank` smallint UNSIGNED NOT NULL,
  PRIMARY KEY (`CharacterId`) USING BTREE,
  UNIQUE INDEX `Name`(`Name` ASC) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of characters
-- ----------------------------

-- ----------------------------
-- Table structure for characters_abilities
-- ----------------------------
DROP TABLE IF EXISTS `characters_abilities`;
CREATE TABLE `characters_abilities`  (
  `CharacterID` int NULL DEFAULT NULL,
  `AbilityID` smallint UNSIGNED NULL DEFAULT NULL,
  `LastCast` int NULL DEFAULT NULL,
  `characters_abilities_ID` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`characters_abilities_ID`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of characters_abilities
-- ----------------------------

-- ----------------------------
-- Table structure for characters_bag_bonus
-- ----------------------------
DROP TABLE IF EXISTS `characters_bag_bonus`;
CREATE TABLE `characters_bag_bonus`  (
  `BonusId` bigint NOT NULL AUTO_INCREMENT,
  `GoldBag` int NOT NULL,
  `PurpleBag` int NOT NULL,
  `BlueBag` int NOT NULL,
  `GreenBag` int NOT NULL,
  `WhiteBag` int NOT NULL,
  `Timestamp` datetime NOT NULL,
  `CharacterId` int NOT NULL,
  `CharacterName` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`BonusId`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of characters_bag_bonus
-- ----------------------------

-- ----------------------------
-- Table structure for characters_bag_pools
-- ----------------------------
DROP TABLE IF EXISTS `characters_bag_pools`;
CREATE TABLE `characters_bag_pools`  (
  `CharacterId` int NOT NULL,
  `Bag_Type` int NOT NULL,
  `BagPool_Value` int NOT NULL,
  PRIMARY KEY (`CharacterId`, `Bag_Type`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of characters_bag_pools
-- ----------------------------

-- ----------------------------
-- Table structure for characters_client_data
-- ----------------------------
DROP TABLE IF EXISTS `characters_client_data`;
CREATE TABLE `characters_client_data`  (
  `CharacterId` int UNSIGNED NOT NULL,
  `ClientDataString` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`CharacterId`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of characters_client_data
-- ----------------------------

-- ----------------------------
-- Table structure for characters_deletions
-- ----------------------------
DROP TABLE IF EXISTS `characters_deletions`;
CREATE TABLE `characters_deletions`  (
  `Guid` int NOT NULL AUTO_INCREMENT,
  `DeletionIP` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
  `AccountID` int NULL DEFAULT NULL,
  `AccountName` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
  `CharacterID` int UNSIGNED NULL DEFAULT NULL,
  `CharacterName` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
  `DeletionTimeSeconds` int NULL DEFAULT NULL,
  PRIMARY KEY (`Guid`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of characters_deletions
-- ----------------------------

-- ----------------------------
-- Table structure for characters_honor_reward_cooldown
-- ----------------------------
DROP TABLE IF EXISTS `characters_honor_reward_cooldown`;
CREATE TABLE `characters_honor_reward_cooldown`  (
  `CharacterId` int UNSIGNED NOT NULL,
  `ItemId` int NOT NULL,
  `Cooldown` bigint NOT NULL,
  PRIMARY KEY (`CharacterId`, `ItemId`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of characters_honor_reward_cooldown
-- ----------------------------

-- ----------------------------
-- Table structure for characters_influences
-- ----------------------------
DROP TABLE IF EXISTS `characters_influences`;
CREATE TABLE `characters_influences`  (
  `CharacterId` int NOT NULL,
  `InfluenceId` smallint UNSIGNED NOT NULL,
  `InfluenceCount` int UNSIGNED NOT NULL,
  `Tier_1_Itemtaken` tinyint UNSIGNED NOT NULL,
  `Tier_2_Itemtaken` tinyint UNSIGNED NOT NULL,
  `Tier_3_Itemtaken` tinyint UNSIGNED NOT NULL,
  PRIMARY KEY (`CharacterId`, `InfluenceId`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of characters_influences
-- ----------------------------

-- ----------------------------
-- Table structure for characters_items
-- ----------------------------
DROP TABLE IF EXISTS `characters_items`;
CREATE TABLE `characters_items`  (
  `Guid` bigint NOT NULL,
  `CharacterId` int UNSIGNED NOT NULL,
  `Entry` int UNSIGNED NOT NULL,
  `SlotId` smallint UNSIGNED NOT NULL,
  `ModelId` int UNSIGNED NOT NULL,
  `Counts` smallint UNSIGNED NOT NULL,
  `Talismans` varchar(40) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `PrimaryDye` smallint UNSIGNED NOT NULL,
  `SecondaryDye` smallint UNSIGNED NOT NULL,
  `BoundtoPlayer` tinyint UNSIGNED NOT NULL,
  `Alternate_AppereanceEntry` int UNSIGNED NOT NULL,
  `characters_items_ID` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`characters_items_ID`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of characters_items
-- ----------------------------

-- ----------------------------
-- Table structure for characters_mails
-- ----------------------------
DROP TABLE IF EXISTS `characters_mails`;
CREATE TABLE `characters_mails`  (
  `Guid` int NOT NULL AUTO_INCREMENT,
  `AuctionType` tinyint UNSIGNED NOT NULL,
  `CharacterId` int UNSIGNED NOT NULL,
  `CharacterIdSender` int UNSIGNED NOT NULL,
  `SenderName` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ReceiverName` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `SendDate` int UNSIGNED NOT NULL,
  `ReadDate` int UNSIGNED NOT NULL,
  `Title` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Content` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Money` int UNSIGNED NOT NULL,
  `Cr` tinyint UNSIGNED NOT NULL,
  `Opened` tinyint UNSIGNED NOT NULL,
  `ItemsString` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`Guid`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of characters_mails
-- ----------------------------

-- ----------------------------
-- Table structure for characters_quests
-- ----------------------------
DROP TABLE IF EXISTS `characters_quests`;
CREATE TABLE `characters_quests`  (
  `CharacterId` int UNSIGNED NOT NULL,
  `QuestID` smallint UNSIGNED NOT NULL,
  `Objectives` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Done` tinyint UNSIGNED NOT NULL,
  PRIMARY KEY (`CharacterId`, `QuestID`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of characters_quests
-- ----------------------------

-- ----------------------------
-- Table structure for characters_saved_buffs
-- ----------------------------
DROP TABLE IF EXISTS `characters_saved_buffs`;
CREATE TABLE `characters_saved_buffs`  (
  `CharacterId` int UNSIGNED NOT NULL,
  `BuffId` smallint UNSIGNED NOT NULL,
  `Level` tinyint UNSIGNED NULL DEFAULT NULL,
  `StackLevel` tinyint UNSIGNED NULL DEFAULT NULL,
  `EndTimeSeconds` int UNSIGNED NULL DEFAULT NULL,
  PRIMARY KEY (`CharacterId`, `BuffId`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of characters_saved_buffs
-- ----------------------------

-- ----------------------------
-- Table structure for characters_socials
-- ----------------------------
DROP TABLE IF EXISTS `characters_socials`;
CREATE TABLE `characters_socials`  (
  `CharacterId` int UNSIGNED NOT NULL,
  `DistCharacterId` int UNSIGNED NOT NULL,
  `DistName` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Friend` tinyint UNSIGNED NOT NULL,
  `Ignore` tinyint UNSIGNED NOT NULL,
  PRIMARY KEY (`CharacterId`, `DistCharacterId`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of characters_socials
-- ----------------------------

-- ----------------------------
-- Table structure for characters_toks
-- ----------------------------
DROP TABLE IF EXISTS `characters_toks`;
CREATE TABLE `characters_toks`  (
  `CharacterId` int UNSIGNED NOT NULL,
  `TokEntry` smallint UNSIGNED NOT NULL,
  `Count` int UNSIGNED NULL DEFAULT NULL,
  PRIMARY KEY (`CharacterId`, `TokEntry`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of characters_toks
-- ----------------------------

-- ----------------------------
-- Table structure for characters_toks_kills
-- ----------------------------
DROP TABLE IF EXISTS `characters_toks_kills`;
CREATE TABLE `characters_toks_kills`  (
  `CharacterId` int UNSIGNED NOT NULL,
  `NPCEntry` smallint UNSIGNED NOT NULL,
  `Count` int UNSIGNED NULL DEFAULT NULL,
  PRIMARY KEY (`CharacterId`, `NPCEntry`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of characters_toks_kills
-- ----------------------------

-- ----------------------------
-- Table structure for characters_value
-- ----------------------------
DROP TABLE IF EXISTS `characters_value`;
CREATE TABLE `characters_value`  (
  `CharacterId` int UNSIGNED NOT NULL,
  `Level` tinyint UNSIGNED NOT NULL,
  `Xp` int UNSIGNED NOT NULL,
  `XpMode` int NOT NULL,
  `RestXp` int UNSIGNED NOT NULL,
  `Renown` int UNSIGNED NOT NULL,
  `RenownRank` tinyint UNSIGNED NOT NULL,
  `Money` int UNSIGNED NOT NULL,
  `Speed` int NOT NULL,
  `PlayedTime` int UNSIGNED NOT NULL,
  `LastSeen` int NULL DEFAULT NULL,
  `RegionId` int NOT NULL,
  `ZoneId` smallint UNSIGNED NOT NULL,
  `WorldX` int NOT NULL,
  `WorldY` int NOT NULL,
  `WorldZ` int NOT NULL,
  `WorldO` int NOT NULL,
  `RallyPoint` smallint UNSIGNED NOT NULL,
  `BagBuy` tinyint UNSIGNED NOT NULL,
  `BankBuy` tinyint UNSIGNED NOT NULL,
  `Skills` int UNSIGNED NOT NULL,
  `Online` tinyint UNSIGNED NOT NULL,
  `GearShow` tinyint UNSIGNED NOT NULL,
  `TitleId` smallint UNSIGNED NOT NULL,
  `RenownSkills` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `MasterySkills` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Morale1` smallint UNSIGNED NULL DEFAULT NULL,
  `Morale2` smallint UNSIGNED NULL DEFAULT NULL,
  `Morale3` smallint UNSIGNED NULL DEFAULT NULL,
  `Morale4` smallint UNSIGNED NULL DEFAULT NULL,
  `Tactic1` smallint UNSIGNED NULL DEFAULT NULL,
  `Tactic2` smallint UNSIGNED NULL DEFAULT NULL,
  `Tactic3` smallint UNSIGNED NULL DEFAULT NULL,
  `Tactic4` smallint UNSIGNED NULL DEFAULT NULL,
  `GatheringSkill` tinyint UNSIGNED NOT NULL,
  `GatheringSkillLevel` tinyint UNSIGNED NOT NULL,
  `CraftingSkill` tinyint UNSIGNED NOT NULL,
  `CraftingSkillLevel` tinyint UNSIGNED NOT NULL,
  `ExperimentalMode` tinyint UNSIGNED NOT NULL,
  `RVRKills` int UNSIGNED NOT NULL,
  `RVRDeaths` int UNSIGNED NOT NULL,
  `CraftingBags` tinyint UNSIGNED NOT NULL,
  `PendingXp` int UNSIGNED NULL DEFAULT NULL,
  `PendingRenown` int UNSIGNED NULL DEFAULT NULL,
  `Lockouts` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `DisconcetTime` int NOT NULL,
  PRIMARY KEY (`CharacterId`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of characters_value
-- ----------------------------

-- ----------------------------
-- Table structure for gm_commands_logs
-- ----------------------------
DROP TABLE IF EXISTS `gm_commands_logs`;
CREATE TABLE `gm_commands_logs`  (
  `Guid` int NOT NULL AUTO_INCREMENT,
  `AccountId` int UNSIGNED NULL DEFAULT NULL,
  `PlayerName` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `Command` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
  `Date` datetime NULL DEFAULT NULL,
  PRIMARY KEY (`Guid`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of gm_commands_logs
-- ----------------------------

-- ----------------------------
-- Table structure for guild_alliance_info
-- ----------------------------
DROP TABLE IF EXISTS `guild_alliance_info`;
CREATE TABLE `guild_alliance_info`  (
  `AllianceId` int UNSIGNED NOT NULL,
  `Name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`AllianceId`) USING BTREE,
  UNIQUE INDEX `Name`(`Name` ASC) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of guild_alliance_info
-- ----------------------------

-- ----------------------------
-- Table structure for guild_event
-- ----------------------------
DROP TABLE IF EXISTS `guild_event`;
CREATE TABLE `guild_event`  (
  `SlotId` tinyint UNSIGNED NOT NULL,
  `GuildId` int UNSIGNED NOT NULL,
  `CharacterId` int UNSIGNED NOT NULL,
  `Begin` int UNSIGNED NOT NULL,
  `End` int UNSIGNED NOT NULL,
  `Name` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Description` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Alliance` tinyint UNSIGNED NOT NULL,
  `Locked` tinyint UNSIGNED NOT NULL,
  `Signups` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `guild_event_ID` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`guild_event_ID`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of guild_event
-- ----------------------------

-- ----------------------------
-- Table structure for guild_info
-- ----------------------------
DROP TABLE IF EXISTS `guild_info`;
CREATE TABLE `guild_info`  (
  `GuildId` int UNSIGNED NOT NULL,
  `Name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Level` tinyint UNSIGNED NOT NULL,
  `Realm` tinyint UNSIGNED NOT NULL,
  `LeaderId` int UNSIGNED NOT NULL,
  `CreateDate` int NOT NULL,
  `Motd` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `AboutUs` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Xp` int UNSIGNED NOT NULL,
  `Renown` bigint UNSIGNED NOT NULL,
  `BriefDescription` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Summary` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `PlayStyle` tinyint UNSIGNED NOT NULL,
  `Atmosphere` tinyint UNSIGNED NOT NULL,
  `CareersNeeded` int UNSIGNED NOT NULL,
  `Interests` tinyint UNSIGNED NOT NULL,
  `ActivelyRecruiting` tinyint UNSIGNED NOT NULL,
  `RanksNeeded` tinyint UNSIGNED NOT NULL,
  `Tax` tinyint UNSIGNED NOT NULL,
  `Money` bigint UNSIGNED NOT NULL,
  `guildvaultpurchased` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Banners` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Heraldry` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `GuildTacticsPurchased` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `AllianceId` int UNSIGNED NULL DEFAULT NULL,
  PRIMARY KEY (`GuildId`) USING BTREE,
  UNIQUE INDEX `Name`(`Name` ASC) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of guild_info
-- ----------------------------

-- ----------------------------
-- Table structure for guild_logs
-- ----------------------------
DROP TABLE IF EXISTS `guild_logs`;
CREATE TABLE `guild_logs`  (
  `GuildId` int UNSIGNED NOT NULL,
  `Time` int UNSIGNED NOT NULL,
  `Type` tinyint UNSIGNED NOT NULL,
  `Text` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `guild_logs_ID` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`guild_logs_ID`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of guild_logs
-- ----------------------------

-- ----------------------------
-- Table structure for guild_members
-- ----------------------------
DROP TABLE IF EXISTS `guild_members`;
CREATE TABLE `guild_members`  (
  `GuildId` int UNSIGNED NOT NULL,
  `CharacterId` int UNSIGNED NOT NULL,
  `RankId` tinyint UNSIGNED NOT NULL,
  `PublicNote` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `OfficerNote` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `JoinDate` int UNSIGNED NOT NULL,
  `LastSeen` int UNSIGNED NOT NULL,
  `RealmCaptain` tinyint UNSIGNED NOT NULL,
  `StandardBearer` tinyint UNSIGNED NOT NULL,
  `GuildRecruiter` tinyint UNSIGNED NOT NULL,
  `RenownContributed` bigint UNSIGNED NOT NULL,
  `Tithe` tinyint UNSIGNED NOT NULL,
  `TitheContributed` bigint UNSIGNED NOT NULL,
  PRIMARY KEY (`CharacterId`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of guild_members
-- ----------------------------

-- ----------------------------
-- Table structure for guild_ranks
-- ----------------------------
DROP TABLE IF EXISTS `guild_ranks`;
CREATE TABLE `guild_ranks`  (
  `GuildId` int UNSIGNED NOT NULL,
  `RankId` tinyint UNSIGNED NOT NULL,
  `Name` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Permissions` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Enabled` tinyint UNSIGNED NOT NULL,
  PRIMARY KEY (`GuildId`, `RankId`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of guild_ranks
-- ----------------------------

-- ----------------------------
-- Table structure for guild_vault_item
-- ----------------------------
DROP TABLE IF EXISTS `guild_vault_item`;
CREATE TABLE `guild_vault_item`  (
  `GuildId` int UNSIGNED NOT NULL,
  `Entry` int UNSIGNED NOT NULL,
  `VaultId` tinyint UNSIGNED NOT NULL,
  `SlotId` smallint UNSIGNED NOT NULL,
  `Counts` smallint UNSIGNED NOT NULL,
  `Talismans` varchar(40) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `PrimaryDye` smallint UNSIGNED NOT NULL,
  `SecondaryDye` smallint UNSIGNED NOT NULL,
  PRIMARY KEY (`GuildId`, `VaultId`, `SlotId`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of guild_vault_item
-- ----------------------------

-- ----------------------------
-- Table structure for scenarios_durations
-- ----------------------------
DROP TABLE IF EXISTS `scenarios_durations`;
CREATE TABLE `scenarios_durations`  (
  `Guid` int NOT NULL AUTO_INCREMENT,
  `ScenarioId` smallint UNSIGNED NULL DEFAULT NULL,
  `Tier` tinyint UNSIGNED NULL DEFAULT NULL,
  `StartTime` bigint NULL DEFAULT NULL,
  `DurationSeconds` int UNSIGNED NULL DEFAULT NULL,
  PRIMARY KEY (`Guid`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of scenarios_durations
-- ----------------------------

SET FOREIGN_KEY_CHECKS = 1;
