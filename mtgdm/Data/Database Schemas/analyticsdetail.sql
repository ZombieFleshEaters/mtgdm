CREATE TABLE `analyticsdetail` (
  `AnalyticsDetailID` varchar(36) COLLATE utf8mb4_bin NOT NULL,
  `AnalyticsSummaryID` varchar(36) COLLATE utf8mb4_bin NOT NULL,
  `PagePath` varchar(500) COLLATE utf8mb4_bin NOT NULL,
  `Visitors` int(11) NOT NULL,
  `Views` int(11) NOT NULL,
  PRIMARY KEY (`AnalyticsDetailID`),
  KEY `Index` (`PagePath`) /*!80000 INVISIBLE */
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_bin;