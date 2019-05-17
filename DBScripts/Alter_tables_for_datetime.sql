ALTER TABLE [Order]
ALTER COLUMN [OrderDate] datetime not null
GO

ALTER TABLE [OrderBatch]
ALTER COLUMN [ActionDate] datetime null
GO

ALTER TABLE [PackageLog]
ALTER COLUMN [PackedDate] datetime not null
GO

ALTER TABLE [PVLog]
ALTER COLUMN [LogDate] datetime not null
GO
