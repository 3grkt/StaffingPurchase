-- Allow decimal value in Value column
ALTER TABLE [Order]
ALTER COLUMN [Value] decimal(18,2)
GO