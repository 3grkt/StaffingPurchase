IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ELMAH_Search]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[ELMAH_Search]
GO

-- =============================================
-- Author:		Tri Nguyen
-- Description:	Search with paging on ELMAH_Error table
-- Log:
--		08/09/2016: created
-- =============================================
CREATE PROCEDURE [dbo].[ELMAH_Search]
	@Application NVARCHAR(60),
	@MessageFilter NVARCHAR(2000) = NULL,
	@StartDate DATETIME = NULL,
	@EndDate DATETIME = NULL,
    @PageIndex INT = 1,
    @PageSize INT = 10,
    @TotalCount INT OUTPUT
AS

BEGIN
	
	DECLARE @tempTable TABLE
	(
		[RowNumber] int,
		[ErrorId] uniqueidentifier,
		[Type] nvarchar(100),
		[Message] nvarchar(500),
		[TimeUtc] datetime
	)
	DECLARE @Lower int
	DECLARE @Upper int

	Set @Lower = (@PageIndex - 1) * @PageSize
	Set @Upper = (@PageIndex * @PageSize) + 1

	-- Insert data to temp table
	INSERT INTO @tempTable
	SELECT 
		ROW_NUMBER() OVER (ORDER BY TimeUtc DESC),
		[ErrorId],
		[Type],
		[Message],
		[TimeUtc]
	FROM [ELMAH_Error]
	WHERE 
		(@MessageFilter IS NULL OR [Message] LIKE '%' + @MessageFilter + '%' ESCAPE '\')
		AND (@StartDate IS NULL OR @StartDate <= TimeUtc)
		AND (@EndDate IS NULL OR @EndDate >= TimeUtc)

	-- Select total count
	SELECT @TotalCount = COUNT(1) FROM @tempTable

	-- Select records
	SELECT * FROM @tempTable WHERE [RowNumber] > @Lower AND [RowNumber] < @Upper

END


GO


