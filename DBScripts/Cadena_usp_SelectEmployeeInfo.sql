IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_SelectEmployeeInfo]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_SelectEmployeeInfo]
GO

-- =============================================
-- Author:		Tri Nguyen
-- Description:	Select employee info from Cadena to support sync job in StaffingPurchase app
-- Log:
--		05/29/2016: created
-- =============================================
CREATE PROCEDURE [dbo].[usp_SelectEmployeeInfo] 
(
	@SomeParams varchar(50) -- TODO: remove if not used
)
AS
SELECT
	 emp.EmployeeID
	,emp.FullName
	,emp.DateOfBirth
	,re.ResignationDate
FROM 
	[HR_Employee] emp
LEFT JOIN (SELECT EmployeeID, MAX(ResignationDate) AS [ResignationDate]
			FROM [HR_ResignationEmployee]
			GROUP BY EmployeeID) re
	 ON re.EmployeeID = emp.EmployeeID
--WHERE xxx -- TODO: add where if needed

RETURN

GO


