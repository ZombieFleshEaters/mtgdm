CREATE TABLE `genre` (
  `GenreID` varchar(36) COLLATE utf8mb4_bin NOT NULL,
  `Name` varchar(100) COLLATE utf8mb4_bin NOT NULL,
  `Normalized` varchar(100) COLLATE utf8mb4_bin NOT NULL,
  PRIMARY KEY (`GenreID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin;
