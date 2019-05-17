-- Alter Product table
ALTER TABLE [Product]
ADD [CreatedDate] datetime not null default(getdate()),
	[ModifiedDate] datetime not null default(getdate())
GO

-- Alter Product table
ALTER TABLE [ProductCategory]
ADD [CreatedDate] datetime not null default(getdate()),
	[ModifiedDate] datetime not null default(getdate())
GO
