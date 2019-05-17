-- Author:	Minh Tan
-- Date:	07 Jun 2016
-- Description: Get infomation follow bug 0015723: HR\Staffing\TRansaction
-- Revisions:
--	09 Feb 2017 - Tri Nguyen: Add EmailAddress
--	15 Mar 2017 - Tri Nguyen: Add more logic to select RegistrationDate
--	19 Mar 2017 - Tri Nguyen: Add DateOfBirth

SELECT		he.EmployeeID,
			he.FullName,
			he.EmailAddress,
			he.DateOfBirth,
			Sec.OrganizationLevelNameEN AS Section,
			Dep.OrganizationLevelNameEN AS Department,
			Loc.OrganizationLevelNameEN AS Location,
			hte.ServiceStartDate,
			CASE WHEN hte.TransactionActionID = 7 THEN hre.ResignationDate ELSE NULL END AS ResignationDate,
			CAST(hl.LevelID AS smallint) AS LevelScale,
			hcc.CodeCenterCode
FROM		HR_Employee AS he
INNER JOIN	HR_TransactionEmployee AS hte ON hte.EmployeeID = he.EmployeeID AND hte.IsActivePosition = 1
INNER JOIN	HR_Organization AS ho ON ho.OrganizationID = hte.OrganizationID
LEFT JOIN	HR_OrganizationLevel AS Sec ON Sec.OrganizationLevelID = ho.OrganizationSectionID
LEFT JOIN	HR_OrganizationLevel AS Dep ON Dep.OrganizationLevelID = ho.OrganizationDepartmentID
LEFT JOIN	HR_OrganizationLevel AS Loc ON Loc.OrganizationLevelID = ho.OrganizationLocationID
LEFT JOIN	HR_Level AS hl ON hl.LevelID = hte.LevelID
LEFT JOIN	HR_CodeCenter AS hcc ON hcc.CodeCenterID = hte.CodeCenterID
LEFT JOIN	HR_ResignationEmployee AS hre ON hre.EmployeeID = he.EmployeeID
LEFT JOIN	HR_WorkingType AS hwt ON hwt.WorkingTypeID = hte.WorkingTypeID
