ALTER TABLE [PVLog]
ADD LogTypeId smallint not null default(0),
	OrderSession char(8) null,
	CurrentPV float null
GO

/*** Migrate data ***/
-- Create temp function
CREATE FUNCTION dbo.[temp_GetOrderSession]
(
	@OrderDate datetime
)
RETURNS varchar(8)
AS
BEGIN
	DECLARE @month int,
			@day int,
			@year int,
			@sDate varchar(20)

	SELECT @month = DATEPART(M, @OrderDate), @day = DATEPART(D, @OrderDate), @year = DATEPART(YY, @OrderDate)

	IF @month % 2 = 0
		SET @month = @month - 1
	ELSE IF @day < 4
		SET @month = @month - 2

	IF @month < 0
	BEGIN
		SET @month = @month + 12
		SET @year = @year - 1
	END

	SET @sDate = CAST(@year as varchar) + '-' + CAST(@month as varchar) + '-01'
	RETURN SUBSTRING(DATENAME(month, CONVERT(datetime, @sDate)), 1, 3) + '-' + CAST(@year AS varchar)
END
GO

-- Update data
UPDATE PVLog
SET LogTypeId =
		CASE
			WHEN [Description] = N'Tặng PV cho sinh nhật' THEN 2 -- birthday
			WHEN [Description] LIKE N'Tặng điểm thắng giải%' THEN 3 -- award
			WHEN [Description] LIKE N'Reset PV cuối năm%' THEN 5 -- reset
			ELSE 4 -- ordering
		END,
	OrderSession = dbo.[temp_GetOrderSession](LogDate)
GO

-- Drop temp function
DROP FUNCTION dbo.[temp_GetOrderSession]
GO

-- Migrate CurrentPV from [User] table
UPDATE [PVLog]
SET CurrentPV = usr.CurrentPV
FROM [User] usr
JOIN (select UserId, MAX(Id) AS MaxId
	from PVLog
	group by UserId) tmp
	ON tmp.UserId = usr.Id
WHERE [PVLog].Id = tmp.MaxId
GO

