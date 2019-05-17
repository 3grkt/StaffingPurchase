-- Allow null in GroupId
ALTER TABLE [Level]
ALTER COLUMN [GroupId] smallint null
GO

-- Add new column and copy value from old id
ALTER TABLE [Level]
ADD [NewId] smallint not null default(0)
GO

UPDATE [Level] SET [NewId] = [Id]
GO

-- Drop old id
DECLARE @PK_ConstraintName varchar(100)
DECLARE @DropConstraintSql varchar(300)

ALTER TABLE [User] DROP CONSTRAINT [FK_Employee_Level]

SELECT @PK_ConstraintName = CONSTRAINT_NAME
	FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
	WHERE TABLE_NAME = 'Level' AND CONSTRAINT_TYPE = 'PRIMARY KEY'

SELECT @DropConstraintSql = 'ALTER TABLE [Level] DROP CONSTRAINT ' + @PK_ConstraintName
EXEC (@DropConstraintSql)

ALTER TABLE [Level] DROP COLUMN [Id]
GO

-- Rename new id
EXEC sp_rename 'Level.NewId', 'Id', 'COLUMN';

ALTER TABLE [Level] ADD PRIMARY KEY (Id)
ALTER TABLE [User] ADD CONSTRAINT FK_Employee_Level FOREIGN KEY (LevelId) REFERENCES [Level](Id)
GO