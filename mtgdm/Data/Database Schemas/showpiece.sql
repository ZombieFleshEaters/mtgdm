CREATE TABLE `showpiece` (
  `ShowpieceID` varchar(36) COLLATE utf8mb4_bin NOT NULL,
  `Title` varchar(255) COLLATE utf8mb4_bin NOT NULL,
  `UserID` varchar(36) COLLATE utf8mb4_bin NOT NULL,
  `URL` varchar(255) COLLATE utf8mb4_bin NOT NULL,
  `Synopsis` varchar(2000) COLLATE utf8mb4_bin NOT NULL,
  `Created` datetime NOT NULL,
  `Published` tinyint(1) NOT NULL,
  PRIMARY KEY (`ShowpieceID`),
  KEY `UserID` (`UserID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin;
