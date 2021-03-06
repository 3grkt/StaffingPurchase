USE [Cadena_Lite]
GO
/****** Object:  Table [dbo].[HR_Employee]    Script Date: 5/28/2016 3:32:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HR_Employee](
	[EmployeeID] [nvarchar](50) NULL,
	[FullName] [nvarchar](50) NULL,
	[DateOfBirth] [date] NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[HR_OrganizationLevel]    Script Date: 5/28/2016 3:32:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HR_OrganizationLevel](
	[OrganizationLevelID] [int] IDENTITY(1,1) NOT NULL,
	[OrganizationLevelName] [nvarchar](50) NULL,
	[OrganizationLevelNameEN] [nvarchar](50) NULL,
	[OrganizationLevelCode] [nvarchar](50) NULL,
	[IsUsing] [bit] NULL,
 CONSTRAINT [PK_HR_OrganizationLevel] PRIMARY KEY CLUSTERED 
(
	[OrganizationLevelID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[HR_ResignationEmployee]    Script Date: 5/28/2016 3:32:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HR_ResignationEmployee](
	[EmployeeID] [nvarchar](50) NULL,
	[ResignationDate] [date] NULL
) ON [PRIMARY]

GO
INSERT [dbo].[HR_Employee] ([EmployeeID], [FullName], [DateOfBirth]) VALUES (N'VNM00001', N'VNM00001', CAST(N'1980-01-01' AS Date))
INSERT [dbo].[HR_Employee] ([EmployeeID], [FullName], [DateOfBirth]) VALUES (N'VNM00002', N'VNM00002', CAST(N'1977-05-28' AS Date))
INSERT [dbo].[HR_Employee] ([EmployeeID], [FullName], [DateOfBirth]) VALUES (N'VNM00003', N'VNM00003', CAST(N'1986-10-20' AS Date))
SET IDENTITY_INSERT [dbo].[HR_OrganizationLevel] ON 

INSERT [dbo].[HR_OrganizationLevel] ([OrganizationLevelID], [OrganizationLevelName], [OrganizationLevelNameEN], [OrganizationLevelCode], [IsUsing]) VALUES (1, N'Level1', N'Level1', N'Location1', 1)
INSERT [dbo].[HR_OrganizationLevel] ([OrganizationLevelID], [OrganizationLevelName], [OrganizationLevelNameEN], [OrganizationLevelCode], [IsUsing]) VALUES (2, N'Level2', N'Level2', N'Location2', 1)
INSERT [dbo].[HR_OrganizationLevel] ([OrganizationLevelID], [OrganizationLevelName], [OrganizationLevelNameEN], [OrganizationLevelCode], [IsUsing]) VALUES (3, N'Department1', N'Department1', N'VNM_DP_Loc3', 1)
INSERT [dbo].[HR_OrganizationLevel] ([OrganizationLevelID], [OrganizationLevelName], [OrganizationLevelNameEN], [OrganizationLevelCode], [IsUsing]) VALUES (4, N'Department2', N'Department2', N'VNM_DP_Loc4', 1)
INSERT [dbo].[HR_OrganizationLevel] ([OrganizationLevelID], [OrganizationLevelName], [OrganizationLevelNameEN], [OrganizationLevelCode], [IsUsing]) VALUES (5, N'Level5', N'Level5', N'Location5', 1)
SET IDENTITY_INSERT [dbo].[HR_OrganizationLevel] OFF
INSERT [dbo].[HR_ResignationEmployee] ([EmployeeID], [ResignationDate]) VALUES (N'VNM00001', NULL)
INSERT [dbo].[HR_ResignationEmployee] ([EmployeeID], [ResignationDate]) VALUES (N'VNM00002', NULL)
INSERT [dbo].[HR_ResignationEmployee] ([EmployeeID], [ResignationDate]) VALUES (N'VNM00003', CAST(N'2016-05-30' AS Date))
