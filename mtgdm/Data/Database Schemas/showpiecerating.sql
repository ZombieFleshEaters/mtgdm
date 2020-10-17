CREATE TABLE `showpiecerating` (
  `ShowpieceRatingID` varchar(36) COLLATE utf8mb4_bin NOT NULL,
  `ShowpieceID` varchar(36) COLLATE utf8mb4_bin NOT NULL,
  `UserID` varchar(36) COLLATE utf8mb4_bin NOT NULL,
  `Rating` decimal(13,2) NOT NULL,
  PRIMARY KEY (`ShowpieceRatingID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin;
