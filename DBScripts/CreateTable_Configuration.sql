CREATE TABLE Configuration
(
	Id int identity(1,1) not null primary key,
	Name varchar(100) not null,
	Value nvarchar(250) not null,
	[Description] nvarchar(1000) null,
	CreatedDate datetime not null,
	ModifiedDate datetime not null
)
GO
