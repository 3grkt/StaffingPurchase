ALTER TABLE dbo.[User]
ALTER COLUMN CurrentPV float NOT NULL
GO

ALTER TABLE dbo.[User]
ADD CONSTRAINT DF_CurrentPV
DEFAULT 0 FOR CurrentPV
GO