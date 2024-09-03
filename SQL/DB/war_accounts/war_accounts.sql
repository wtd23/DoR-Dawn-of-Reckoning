/*
 Navicat MySQL Dump SQL

 Source Server         : warserver
 Source Server Type    : MySQL
 Source Server Version : 90001 (9.0.1)
 Source Host           : localhost:3306
 Source Schema         : war_accounts

 Target Server Type    : MySQL
 Target Server Version : 90001 (9.0.1)
 File Encoding         : 65001

 Date: 22/08/2024 01:39:12
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for accounts
-- ----------------------------
DROP TABLE IF EXISTS `accounts`;
CREATE TABLE `accounts`  (
  `AccountId` int NOT NULL AUTO_INCREMENT,
  `PacketLog` tinyint UNSIGNED NULL DEFAULT NULL,
  `Username` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `Password` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `CryptPassword` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `Ip` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `Token` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `GmLevel` tinyint NOT NULL,
  `Banned` int NOT NULL,
  `BanReason` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
  `AdviceBlockEnd` int NULL DEFAULT NULL,
  `StealthMuteEnd` int NULL DEFAULT NULL,
  `CoreLevel` int NULL DEFAULT NULL,
  `LastLogged` int NULL DEFAULT NULL,
  `LastNameChanged` int NULL DEFAULT NULL,
  `LastPatcherLog` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
  `InvalidPasswordCount` int UNSIGNED NOT NULL,
  `noSurname` tinyint NOT NULL,
  `Email` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
  PRIMARY KEY (`AccountId`) USING BTREE,
  UNIQUE INDEX `Username`(`Username` ASC) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of accounts
-- ----------------------------
INSERT INTO `accounts` VALUES (1, 0, 'test', '', '31f014b53e5861c8b28a8707a1d6a2a2737ce2c22fd671884173498510a063f0', '127.0.0.1', '', 40, 0, '', 0, 0, 0, 1725392787, 0, '', 0, 0, '123@fsmail.com');

-- ----------------------------
-- Table structure for accounts_pending
-- ----------------------------
DROP TABLE IF EXISTS `accounts_pending`;
CREATE TABLE `accounts_pending`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Username` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `Code` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `Expires` datetime NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  UNIQUE INDEX `Username`(`Username` ASC) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of accounts_pending
-- ----------------------------

-- ----------------------------
-- Table structure for accounts_sanction_logs
-- ----------------------------
DROP TABLE IF EXISTS `accounts_sanction_logs`;
CREATE TABLE `accounts_sanction_logs`  (
  `AccountId` int NULL DEFAULT NULL,
  `IssuedBy` varchar(24) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `ActionType` varchar(24) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `IssuerGmLevel` int NULL DEFAULT NULL,
  `ActionDuration` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
  `ActionLog` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `ActionTime` int NULL DEFAULT NULL,
  `accounts_sanction_logs_ID` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`accounts_sanction_logs_ID`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of accounts_sanction_logs
-- ----------------------------

-- ----------------------------
-- Table structure for accounts_value
-- ----------------------------
DROP TABLE IF EXISTS `accounts_value`;
CREATE TABLE `accounts_value`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `AccountId` int NULL DEFAULT NULL,
  `InstallId` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
  `IP` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
  `MAC` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
  `HDSerialHash` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
  `CPUIDHash` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
  `ModifyDate` datetime NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of accounts_value
-- ----------------------------

-- ----------------------------
-- Table structure for ip_bans
-- ----------------------------
DROP TABLE IF EXISTS `ip_bans`;
CREATE TABLE `ip_bans`  (
  `Ip` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Expire` int NULL DEFAULT NULL,
  PRIMARY KEY (`Ip`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of ip_bans
-- ----------------------------

-- ----------------------------
-- Table structure for launcher_myps
-- ----------------------------
DROP TABLE IF EXISTS `launcher_myps`;
CREATE TABLE `launcher_myps`  (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(2000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  `CRC32` int NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of launcher_myps
-- ----------------------------

-- ----------------------------
-- Table structure for realms
-- ----------------------------
DROP TABLE IF EXISTS `realms`;
CREATE TABLE `realms`  (
  `RealmId` tinyint UNSIGNED NOT NULL DEFAULT 0,
  `Name` varchar(255) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `Language` varchar(255) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `IP` varchar(255) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `Port` int NOT NULL,
  `AllowTrials` varchar(32) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `CharfxerAvailable` varchar(32) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `Legacy` varchar(32) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `BonusDestruction` varchar(32) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `BonusOrder` varchar(32) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `Redirect` varchar(32) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `Region` varchar(32) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `Retired` varchar(32) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `WaitingDestruction` varchar(32) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `WaitingOrder` varchar(32) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `DensityDestruction` varchar(32) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `DensityOrder` varchar(32) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `OpenRvr` varchar(32) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `Rp` varchar(32) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `Status` varchar(32) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL,
  `Online` tinyint UNSIGNED NOT NULL,
  `OnlineDate` datetime NULL DEFAULT NULL,
  `OnlinePlayers` int UNSIGNED NULL DEFAULT NULL,
  `OrderCount` int UNSIGNED NULL DEFAULT NULL,
  `DestructionCount` int UNSIGNED NULL DEFAULT NULL,
  `MaxPlayers` int UNSIGNED NULL DEFAULT NULL,
  `OrderCharacters` int UNSIGNED NULL DEFAULT NULL,
  `DestruCharacters` int UNSIGNED NULL DEFAULT NULL,
  `NextRotationTime` bigint NULL DEFAULT NULL,
  `MasterPassword` text CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL,
  `BootTime` int NULL DEFAULT NULL,
  PRIMARY KEY (`RealmId`) USING BTREE,
  UNIQUE INDEX `RealmId`(`RealmId` ASC) USING BTREE
) ENGINE = InnoDB CHARACTER SET = latin1 COLLATE = latin1_swedish_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of realms
-- ----------------------------
INSERT INTO `realms` VALUES (1, 'DoR', 'EN', '127.0.0.1', 10300, '0', '0', '0', '0', '0', '0', 'STR_REGION_NORTHAMERICA', '0', '0', '0', '0', '0', '0', '1', '0', 1, '2024-09-03 20:46:04', 1, 1, 0, 1000, 1, 0, 1532563200, '', 1725392764);

SET FOREIGN_KEY_CHECKS = 1;
