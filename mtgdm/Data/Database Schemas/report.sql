CREATE TABLE `report` (
  `ReportID` varchar(36) COLLATE utf8mb4_bin NOT NULL,
  `Source` varchar(12) COLLATE utf8mb4_bin NOT NULL,
  `SourceID` varchar(36) COLLATE utf8mb4_bin NOT NULL,
  `Comment` varchar(2000) COLLATE utf8mb4_bin NOT NULL,
  `ReportedBy` varchar(36) COLLATE utf8mb4_bin NOT NULL,
  `Reported` datetime NOT NULL,
  `ResolvedBy` varchar(45) COLLATE utf8mb4_bin NOT NULL,
  `Resolved` datetime NOT NULL,
  PRIMARY KEY (`ReportID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin;
