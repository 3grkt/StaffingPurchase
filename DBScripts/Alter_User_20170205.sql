ALTER TABLE [User]
ADD [Language] varchar(20) not null default('vi')
GO

-- Updated on 9 Feb 2017
ALTER TABLE [User]
ADD [EmailAddress] varchar(50)
GO
