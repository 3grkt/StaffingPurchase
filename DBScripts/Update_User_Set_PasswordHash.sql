-- Set password the same as username using SHA256 hashing
UPDATE [User] SET PasswordHash = CONVERT(NVARCHAR(MAX), HASHBYTES('SHA2_256', UserName), 2)
GO
