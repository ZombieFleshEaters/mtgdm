CREATE TABLE `showpiecetogenre` (
  `ShowpieceToGenreID` varchar(36) COLLATE utf8mb4_bin NOT NULL,
  `ShowpieceID` varchar(36) COLLATE utf8mb4_bin NOT NULL,
  `GenreID` varchar(36) COLLATE utf8mb4_bin NOT NULL,
  PRIMARY KEY (`ShowpieceToGenreID`),
  KEY `ShowpieceID` (`ShowpieceID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin;
