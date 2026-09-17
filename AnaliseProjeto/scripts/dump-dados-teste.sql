CREATE DATABASE  IF NOT EXISTS `medica` /*!40100 DEFAULT CHARACTER SET utf8mb3 */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `medica`;
-- MySQL dump 10.13  Distrib 8.0.45, for Win64 (x86_64)
--
-- Host: localhost    Database: medica
-- ------------------------------------------------------
-- Server version	8.0.46

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `__efmigrationshistory`
--

DROP TABLE IF EXISTS `__efmigrationshistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) NOT NULL,
  `ProductVersion` varchar(32) NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `__efmigrationshistory`
--

LOCK TABLES `__efmigrationshistory` WRITE;
/*!40000 ALTER TABLE `__efmigrationshistory` DISABLE KEYS */;
/*!40000 ALTER TABLE `__efmigrationshistory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `alergia`
--

DROP TABLE IF EXISTS `alergia`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `alergia` (
  `id` int unsigned NOT NULL AUTO_INCREMENT,
  `descricao` varchar(200) NOT NULL,
  `tipo` enum('MEDICAMENTO','ALIMENTAR','RESPIRATORIA','CONTATO','OUTROS') NOT NULL,
  `idPaciente` int unsigned NOT NULL,
  `idMedicamento` int unsigned DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_Alergia_Paciente1_idx` (`idPaciente`),
  KEY `fk_Alergia_Medicamento1_idx` (`idMedicamento`),
  CONSTRAINT `fk_Alergia_Medicamento1` FOREIGN KEY (`idMedicamento`) REFERENCES `medicamento` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `fk_Alergia_Paciente1` FOREIGN KEY (`idPaciente`) REFERENCES `paciente` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `alergia`
--

LOCK TABLES `alergia` WRITE;
/*!40000 ALTER TABLE `alergia` DISABLE KEYS */;
INSERT INTO `alergia` VALUES (5,'Não pode comer camarão.','ALIMENTAR',8,NULL);
/*!40000 ALTER TABLE `alergia` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetroleclaims`
--

DROP TABLE IF EXISTS `aspnetroleclaims`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetroleclaims` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `RoleId` varchar(255) NOT NULL,
  `ClaimType` longtext,
  `ClaimValue` longtext,
  PRIMARY KEY (`Id`),
  KEY `IX_AspNetRoleClaims_RoleId` (`RoleId`),
  CONSTRAINT `FK_AspNetRoleClaims_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `aspnetroles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetroleclaims`
--

LOCK TABLES `aspnetroleclaims` WRITE;
/*!40000 ALTER TABLE `aspnetroleclaims` DISABLE KEYS */;
/*!40000 ALTER TABLE `aspnetroleclaims` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetroles`
--

DROP TABLE IF EXISTS `aspnetroles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetroles` (
  `Id` varchar(255) NOT NULL,
  `Name` varchar(256) DEFAULT NULL,
  `NormalizedName` varchar(256) DEFAULT NULL,
  `ConcurrencyStamp` longtext,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `RoleNameIndex` (`NormalizedName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetroles`
--

LOCK TABLES `aspnetroles` WRITE;
/*!40000 ALTER TABLE `aspnetroles` DISABLE KEYS */;
INSERT INTO `aspnetroles` VALUES ('76a1e9b2-b297-11f1-9622-a8a1593b3445','Administrador','ADMINISTRADOR','76a1e9fc-b297-11f1-9622-a8a1593b3445'),('76a1ee86-b297-11f1-9622-a8a1593b3445','Cuidador','CUIDADOR','76a1ee8d-b297-11f1-9622-a8a1593b3445'),('76a1ef75-b297-11f1-9622-a8a1593b3445','Paciente','PACIENTE','76a1ef7a-b297-11f1-9622-a8a1593b3445');
/*!40000 ALTER TABLE `aspnetroles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetuserclaims`
--

DROP TABLE IF EXISTS `aspnetuserclaims`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetuserclaims` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `UserId` varchar(255) NOT NULL,
  `ClaimType` longtext,
  `ClaimValue` longtext,
  PRIMARY KEY (`Id`),
  KEY `IX_AspNetUserClaims_UserId` (`UserId`),
  CONSTRAINT `FK_AspNetUserClaims_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetuserclaims`
--

LOCK TABLES `aspnetuserclaims` WRITE;
/*!40000 ALTER TABLE `aspnetuserclaims` DISABLE KEYS */;
/*!40000 ALTER TABLE `aspnetuserclaims` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetuserlogins`
--

DROP TABLE IF EXISTS `aspnetuserlogins`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetuserlogins` (
  `LoginProvider` varchar(128) NOT NULL,
  `ProviderKey` varchar(128) NOT NULL,
  `ProviderDisplayName` longtext,
  `UserId` varchar(255) NOT NULL,
  PRIMARY KEY (`LoginProvider`,`ProviderKey`),
  KEY `IX_AspNetUserLogins_UserId` (`UserId`),
  CONSTRAINT `FK_AspNetUserLogins_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetuserlogins`
--

LOCK TABLES `aspnetuserlogins` WRITE;
/*!40000 ALTER TABLE `aspnetuserlogins` DISABLE KEYS */;
/*!40000 ALTER TABLE `aspnetuserlogins` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetuserroles`
--

DROP TABLE IF EXISTS `aspnetuserroles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetuserroles` (
  `UserId` varchar(255) NOT NULL,
  `RoleId` varchar(255) NOT NULL,
  PRIMARY KEY (`UserId`,`RoleId`),
  KEY `IX_AspNetUserRoles_RoleId` (`RoleId`),
  CONSTRAINT `FK_AspNetUserRoles_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `aspnetroles` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_AspNetUserRoles_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetuserroles`
--

LOCK TABLES `aspnetuserroles` WRITE;
/*!40000 ALTER TABLE `aspnetuserroles` DISABLE KEYS */;
INSERT INTO `aspnetuserroles` VALUES ('4288173e-6a90-41e1-9f61-8f9195e3777a','76a1e9b2-b297-11f1-9622-a8a1593b3445'),('6381d7c7-0b19-4b55-86b5-540d00818ab6','76a1e9b2-b297-11f1-9622-a8a1593b3445'),('85c61dce-e530-4df0-b856-f91b4175a1bf','76a1ee86-b297-11f1-9622-a8a1593b3445'),('ad9483ef-7ad0-44be-9a67-d3f2ff8da974','76a1ee86-b297-11f1-9622-a8a1593b3445'),('f37bddf0-6a13-4de0-bb13-11ac137705cf','76a1ee86-b297-11f1-9622-a8a1593b3445');
/*!40000 ALTER TABLE `aspnetuserroles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetusers`
--

DROP TABLE IF EXISTS `aspnetusers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetusers` (
  `Id` varchar(255) NOT NULL,
  `UserName` varchar(256) DEFAULT NULL,
  `NormalizedUserName` varchar(256) DEFAULT NULL,
  `Email` varchar(256) DEFAULT NULL,
  `NormalizedEmail` varchar(256) DEFAULT NULL,
  `EmailConfirmed` tinyint(1) NOT NULL,
  `PasswordHash` longtext,
  `SecurityStamp` longtext,
  `ConcurrencyStamp` longtext,
  `PhoneNumber` longtext,
  `PhoneNumberConfirmed` tinyint(1) NOT NULL,
  `TwoFactorEnabled` tinyint(1) NOT NULL,
  `LockoutEnd` datetime DEFAULT NULL,
  `LockoutEnabled` tinyint(1) NOT NULL,
  `AccessFailedCount` int NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UserNameIndex` (`NormalizedUserName`),
  KEY `EmailIndex` (`NormalizedEmail`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetusers`
--

LOCK TABLES `aspnetusers` WRITE;
/*!40000 ALTER TABLE `aspnetusers` DISABLE KEYS */;
INSERT INTO `aspnetusers` VALUES ('4288173e-6a90-41e1-9f61-8f9195e3777a','34192324032','34192324032','guilh2rm2lima@gmail.com','GUILH2RM2LIMA@GMAIL.COM',0,'AQAAAAIAAYagAAAAEOTP6RcnX7QsKwJl8PdryRbMwSAqOd7rO6NJ80zXPOiq5JEeL7Ab4XuftLSolslQcw==','LO5MMNVTH3X625PZBE6KA7WZ3CTE3RLN','8e5c116f-be12-4119-becf-b16dca41bd19',NULL,0,0,NULL,1,0),('6381d7c7-0b19-4b55-86b5-540d00818ab6','46241021001','46241021001','talysson.pestinha@gmail.com','TALYSSON.PESTINHA@GMAIL.COM',0,'AQAAAAIAAYagAAAAEGmEsbWtBeS/VsFxGVw6J/VpKif7aJMW7o7YvNjQd9urBtPCCgxTAmQ/6V8imZpUFQ==','NV32FZTGMWPZWG3BKA5IC4NCXYF7F3WY','4df3cf9a-bae6-4e08-80f6-1051d920884f',NULL,0,0,NULL,1,0),('85c61dce-e530-4df0-b856-f91b4175a1bf','36871380035','36871380035','caio.em15@gmail.com','CAIO.EM15@GMAIL.COM',0,'AQAAAAIAAYagAAAAEI5p+hfI2phDmu4i2pAavt/aGyazsa4QovPX05CB9V6n99pwNLx5fMiSyCALT/qw8g==','L5ND6HDCS7EXYMDDZEHHXCMIJY4Y76U7','1d7e2d58-efc8-45f1-afaa-a5813ab03505',NULL,0,0,NULL,1,0),('ad9483ef-7ad0-44be-9a67-d3f2ff8da974','05969457019','05969457019','valmir.ita1@gmail.com','VALMIR.ITA1@GMAIL.COM',0,'AQAAAAIAAYagAAAAEP8uxERaiUn3JUtCno6A9UptCZwNOnxKsuajKxR6L2ICH6i5LYrr5ObGbNE4bTWM8A==','BJTIU7FK4HWHFV3MTL722YDIT4YJXAB7','ed22cba6-c9ae-4bc3-aaf7-cb64c83d6a2e',NULL,0,0,NULL,1,0),('f37bddf0-6a13-4de0-bb13-11ac137705cf','20076318060','20076318060','carla1535@gmail.com','CARLA1535@GMAIL.COM',0,'AQAAAAIAAYagAAAAEJB2xQauErb99NP1FqA95hLGATfWstaeQrj/bCsBr/lTsBBOTbEt/IUJkSOJ43RyhA==','CPFM7OL7VCIDN4Q3D62URJR3RBRFR2WR','a29cc13d-a0ce-459f-930d-73fabb6ae827',NULL,0,0,NULL,1,0);
/*!40000 ALTER TABLE `aspnetusers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetusertokens`
--

DROP TABLE IF EXISTS `aspnetusertokens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetusertokens` (
  `UserId` varchar(255) NOT NULL,
  `LoginProvider` varchar(128) NOT NULL,
  `Name` varchar(128) NOT NULL,
  `Value` longtext,
  PRIMARY KEY (`UserId`,`LoginProvider`,`Name`),
  CONSTRAINT `FK_AspNetUserTokens_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetusertokens`
--

LOCK TABLES `aspnetusertokens` WRITE;
/*!40000 ALTER TABLE `aspnetusertokens` DISABLE KEYS */;
/*!40000 ALTER TABLE `aspnetusertokens` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `cuidador`
--

DROP TABLE IF EXISTS `cuidador`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cuidador` (
  `id` int unsigned NOT NULL AUTO_INCREMENT,
  `nome` varchar(60) NOT NULL,
  `cpf` varchar(11) NOT NULL,
  `foto` blob,
  `ativo` enum('S','N') NOT NULL DEFAULT 'S' COMMENT 'Campo onde o administrador pode gerenciar o usuário.',
  PRIMARY KEY (`id`),
  UNIQUE KEY `cpf_UNIQUE` (`cpf`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cuidador`
--

LOCK TABLES `cuidador` WRITE;
/*!40000 ALTER TABLE `cuidador` DISABLE KEYS */;
INSERT INTO `cuidador` VALUES (2,'Guilherme Lima','34192324032',NULL,'S'),(3,'Carla Santos','20076318060',NULL,'S'),(4,'Valmir de Leitoa','05969457019',NULL,'S'),(5,'Talysson Santos','46241021001',NULL,'S'),(6,'Caio Emannuel','36871380035',NULL,'S');
/*!40000 ALTER TABLE `cuidador` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `dispositivopaciente`
--

DROP TABLE IF EXISTS `dispositivopaciente`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `dispositivopaciente` (
  `id` int NOT NULL,
  `fcmToken` text NOT NULL,
  `dataAtualizacao` datetime NOT NULL,
  `idPaciente` int unsigned NOT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_dispositivopaciente_paciente1_idx` (`idPaciente`),
  CONSTRAINT `fk_dispositivopaciente_paciente1` FOREIGN KEY (`idPaciente`) REFERENCES `paciente` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `dispositivopaciente`
--

LOCK TABLES `dispositivopaciente` WRITE;
/*!40000 ALTER TABLE `dispositivopaciente` DISABLE KEYS */;
/*!40000 ALTER TABLE `dispositivopaciente` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `execucao`
--

DROP TABLE IF EXISTS `execucao`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `execucao` (
  `id` int unsigned NOT NULL AUTO_INCREMENT COMMENT 'Esta tabela guarda informações da execucao do planejamento das medicações.',
  `dataConfirmacao` date NOT NULL,
  `horaConfirmacao` time DEFAULT NULL,
  `latitude` decimal(10,8) DEFAULT NULL,
  `longitude` decimal(11,8) DEFAULT NULL,
  `status` enum('SUCESSO','ATRASO','FALHA') NOT NULL,
  `idPlanejamento` int NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `latitude_UNIQUE` (`latitude`),
  KEY `fk_Execucao_Planejamento1_idx` (`idPlanejamento`),
  CONSTRAINT `fk_Execucao_Planejamento1` FOREIGN KEY (`idPlanejamento`) REFERENCES `planejamento` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `execucao`
--

LOCK TABLES `execucao` WRITE;
/*!40000 ALTER TABLE `execucao` DISABLE KEYS */;
INSERT INTO `execucao` VALUES (1,'2026-09-17','11:58:00',NULL,NULL,'SUCESSO',11);
/*!40000 ALTER TABLE `execucao` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `medicamento`
--

DROP TABLE IF EXISTS `medicamento`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `medicamento` (
  `id` int unsigned NOT NULL AUTO_INCREMENT,
  `nome` varchar(60) NOT NULL,
  `apelido` varchar(60) DEFAULT NULL,
  `quantidade` int NOT NULL,
  `formaFarmaceutica` enum('COMPRIMIDO','CAPSULA','SOLUCAO_ORAL','CREME','POMADA','INJETAVEL','SUPOSITORIO') NOT NULL,
  `foto` blob,
  `idCuidador` int unsigned NOT NULL,
  `ativo` enum('S','N') NOT NULL DEFAULT 'S',
  PRIMARY KEY (`id`),
  KEY `fk_Medicamento_Cuidador1_idx` (`idCuidador`),
  CONSTRAINT `fk_Medicamento_Cuidador1` FOREIGN KEY (`idCuidador`) REFERENCES `cuidador` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `medicamento`
--

LOCK TABLES `medicamento` WRITE;
/*!40000 ALTER TABLE `medicamento` DISABLE KEYS */;
INSERT INTO `medicamento` VALUES (12,'Dipirona Sódica',NULL,50,'COMPRIMIDO','',2,'S');
/*!40000 ALTER TABLE `medicamento` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `paciente`
--

DROP TABLE IF EXISTS `paciente`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `paciente` (
  `id` int unsigned NOT NULL AUTO_INCREMENT,
  `nome` varchar(60) NOT NULL,
  `cpf` varchar(11) NOT NULL,
  `cartaoSus` varchar(15) DEFAULT NULL,
  `dataNascimento` date DEFAULT NULL,
  `tipoSanguineo` enum('A_POSITIVO','A_NEGATIVO','B_POSITIVO','B_NEGATIVO','AB_POSITIVO','AB_NEGATIVO','O_POSITIVO','O_NEGATIVO') DEFAULT NULL,
  `peso` float DEFAULT NULL,
  `altura` float DEFAULT NULL,
  `sexo` enum('M','F') NOT NULL COMMENT '''M'' = Masculino;\\n''F'' = Feminino.',
  `apelido` varchar(60) DEFAULT NULL,
  `alergiaMedicamento` tinyint NOT NULL,
  `escolaridade` enum('ANALFABETO','FUNDAMENTAL_INCOMPLETO','FUNDAMENTAL_COMPLETO','MEDIO_INCOMPLETO','MEDIO_COMPLETO','SUPERIOR_INCOMPLETO','SUPERIOR_COMPLETO','POS_GRADUACAO','MESTRADO','DOUTORADO') NOT NULL,
  `possuiDeficiencia` tinyint NOT NULL,
  `cep` varchar(8) NOT NULL,
  `rua` varchar(60) NOT NULL,
  `bairro` varchar(60) NOT NULL,
  `identificador` varchar(30) DEFAULT NULL,
  `cidade` varchar(30) NOT NULL,
  `estado` varchar(2) NOT NULL,
  `complemento` varchar(60) DEFAULT NULL,
  `ddd` varchar(2) NOT NULL,
  `telefone` varchar(9) NOT NULL,
  `dddResponsavel` varchar(2) DEFAULT NULL,
  `telefoneResponsavel` varchar(9) DEFAULT NULL,
  `nomeResponsavel` varchar(60) DEFAULT NULL COMMENT 'O nome do telefone responsável',
  `foto` blob,
  `deficiencia` varchar(200) DEFAULT NULL,
  `ativo` enum('S','N') NOT NULL DEFAULT 'S',
  PRIMARY KEY (`id`),
  UNIQUE KEY `cpf_UNIQUE` (`cpf`),
  UNIQUE KEY `cartaoSus_UNIQUE` (`cartaoSus`),
  KEY `idx_nome` (`nome`) /*!80000 INVISIBLE */
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `paciente`
--

LOCK TABLES `paciente` WRITE;
/*!40000 ALTER TABLE `paciente` DISABLE KEYS */;
INSERT INTO `paciente` VALUES (5,'João Santos','24718033004',NULL,'1968-09-03',NULL,NULL,NULL,'M',NULL,0,'ANALFABETO',0,'35300027','Vila Maria do Carmo Ribeiro','Centro',NULL,'Caratinga','MG',NULL,'79','999999999',NULL,NULL,NULL,'',NULL,'S'),(6,'Maria Santos','63129067019',NULL,'1976-01-01',NULL,NULL,NULL,'F',NULL,0,'FUNDAMENTAL_COMPLETO',0,'78060668','Rua Otawa','Jardim das Américas',NULL,'Cuiabá','MT',NULL,'79','999999999',NULL,NULL,NULL,'',NULL,'S'),(7,'Bruno Santos','28509793093',NULL,'2001-01-01',NULL,NULL,NULL,'M',NULL,0,'MEDIO_COMPLETO',0,'49500552','Rua José Wilson dos Reis','São Cristóvão',NULL,'Itabaiana','SE',NULL,'79','999999999',NULL,NULL,NULL,'',NULL,'S'),(8,'Maria Aparecida Lima Santos','27820208031','724532326956777','1972-09-30','B_POSITIVO',60,1.72,'F','Cida',1,'MEDIO_COMPLETO',1,'49500552','Rua José Wilson dos Reis','São Cristóvão',NULL,'Itabaiana','SE',NULL,'79','999999999',NULL,NULL,NULL,'','Dificuldade para levantar o braço esquerdo.','S'),(9,'José Gilson','19393413029',NULL,'1968-11-03',NULL,NULL,NULL,'M',NULL,0,'FUNDAMENTAL_INCOMPLETO',0,'49500552','Rua José Wilson dos Reis','São Cristóvão',NULL,'Itabaiana','SE',NULL,'79','999999999',NULL,NULL,NULL,'',NULL,'S');
/*!40000 ALTER TABLE `paciente` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `planejamento`
--

DROP TABLE IF EXISTS `planejamento`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `planejamento` (
  `id` int NOT NULL AUTO_INCREMENT,
  `idPaciente` int unsigned NOT NULL,
  `idMedicamento` int unsigned NOT NULL,
  `dataInicio` date NOT NULL,
  `dataFim` date NOT NULL,
  `diaSemana` varchar(7) NOT NULL COMMENT 'DOM,SEG,TER,QUA,QUI,SEX,SAB',
  `hora` time NOT NULL,
  `intervaloExecucao` time NOT NULL,
  `dosagem` int NOT NULL,
  `unidadeDosagem` enum('ML','MG','G','UI') NOT NULL,
  `ativo` enum('S','N') DEFAULT 'S',
  `status` enum('NAO_INICIADO','EM_ANDAMENTO','CONCLUIDO','INTERROMPIDO') NOT NULL DEFAULT 'NAO_INICIADO',
  PRIMARY KEY (`id`),
  KEY `fk_Paciente_has_Medicamento_Medicamento1_idx` (`idMedicamento`),
  KEY `fk_Paciente_has_Medicamento_Paciente1_idx` (`idPaciente`),
  CONSTRAINT `fk_Paciente_has_Medicamento_Medicamento1` FOREIGN KEY (`idMedicamento`) REFERENCES `medicamento` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `fk_Paciente_has_Medicamento_Paciente1` FOREIGN KEY (`idPaciente`) REFERENCES `paciente` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `planejamento`
--

LOCK TABLES `planejamento` WRITE;
/*!40000 ALTER TABLE `planejamento` DISABLE KEYS */;
INSERT INTO `planejamento` VALUES (10,8,12,'2026-09-18','9999-12-31','XSTXXSS','02:00:00','00:30:00',2,'G','S','NAO_INICIADO'),(11,9,12,'2026-09-17','2027-01-01','DSTQQSS','12:00:00','00:30:00',1,'G','S','EM_ANDAMENTO');
/*!40000 ALTER TABLE `planejamento` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `vinculo`
--

DROP TABLE IF EXISTS `vinculo`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `vinculo` (
  `idPaciente` int unsigned NOT NULL,
  `idCuidador` int unsigned NOT NULL,
  `parentesco` enum('PAI','MAE','FILHO','CONJUGE','IRMAO','AVO','TIO','SOBRINHO','OUTROS') NOT NULL,
  PRIMARY KEY (`idPaciente`,`idCuidador`),
  KEY `fk_Paciente_has_Cuidador_Cuidador1_idx` (`idCuidador`),
  KEY `fk_Paciente_has_Cuidador_Paciente_idx` (`idPaciente`),
  CONSTRAINT `fk_Paciente_has_Cuidador_Cuidador1` FOREIGN KEY (`idCuidador`) REFERENCES `cuidador` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `fk_Paciente_has_Cuidador_Paciente` FOREIGN KEY (`idPaciente`) REFERENCES `paciente` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `vinculo`
--

LOCK TABLES `vinculo` WRITE;
/*!40000 ALTER TABLE `vinculo` DISABLE KEYS */;
INSERT INTO `vinculo` VALUES (5,3,'PAI'),(6,3,'MAE'),(7,4,'IRMAO'),(8,2,'MAE'),(9,2,'PAI');
/*!40000 ALTER TABLE `vinculo` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-09-17 10:47:23
