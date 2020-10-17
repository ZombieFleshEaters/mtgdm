CREATE TABLE `comment` (
  `CommentID` varchar(36) COLLATE utf8mb4_bin NOT NULL,
  `ShowpieceID` varchar(36) COLLATE utf8mb4_bin NOT NULL,
  `UserID` varchar(36) COLLATE utf8mb4_bin NOT NULL,
  `Content` longtext COLLATE utf8mb4_bin NOT NULL,
  `Created` datetime NOT NULL,
  PRIMARY KEY (`CommentID`),
  KEY `Showpiece` (`ShowpieceID`) /*!80000 INVISIBLE */,
  KEY `UserID` (`UserID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin;
