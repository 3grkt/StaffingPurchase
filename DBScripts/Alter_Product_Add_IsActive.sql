-- Add IsActive column
ALTER TABLE [Product]
ADD [IsActive] bit not null default(0)
GO

-- Update all records to active
UPDATE [Product] SET [IsActive] = 1
GO