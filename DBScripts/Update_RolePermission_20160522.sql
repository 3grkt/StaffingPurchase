/* Delete old data */
DELETE RolePermission
DELETE Permission

/* Insert permissions */
INSERT [dbo].[Permission] ([Id], [Name], [Description]) VALUES (1, N'View Policy', N'Xem policy')
INSERT [dbo].[Permission] ([Id], [Name], [Description]) VALUES (2, N'Submit Order', N'Dat hang')
INSERT [dbo].[Permission] ([Id], [Name], [Description]) VALUES (3, N'View Order History', N'Xem lich su don hang')
INSERT [dbo].[Permission] ([Id], [Name], [Description]) VALUES (4, N'Update Order', N'Thay doi thong tin don hang')
INSERT [dbo].[Permission] ([Id], [Name], [Description]) VALUES (5, N'Approve Order', N'Duyet don hang')
INSERT [dbo].[Permission] ([Id], [Name], [Description]) VALUES (6, N'Pack Order', N'Dong goi don hang')
INSERT [dbo].[Permission] ([Id], [Name], [Description]) VALUES (7, N'Create Award', N'Tao giai thuong')
INSERT [dbo].[Permission] ([Id], [Name], [Description]) VALUES (8, N'Upload Award List', N'Upload danh sach nguoi trung giai')
INSERT [dbo].[Permission] ([Id], [Name], [Description]) VALUES (9, N'Maintain Employee List', N'Cap nhat danh sach NV')
INSERT [dbo].[Permission] ([Id], [Name], [Description]) VALUES (10, N'Maintain Product List', N'Cap nhat danh sach san pham')
INSERT [dbo].[Permission] ([Id], [Name], [Description]) VALUES (11, N'Maintain Level Group', N'Cau hinh level/group')
INSERT [dbo].[Permission] ([Id], [Name], [Description]) VALUES (12, N'Maintain Policy', N'Cau hinh policy')
INSERT [dbo].[Permission] ([Id], [Name], [Description]) VALUES (13, N'Maintain User', N'Tao user/cap quyen')
INSERT [dbo].[Permission] ([Id], [Name], [Description]) VALUES (14, N'Report Employee Orders', N'Report don hang cua NV')
INSERT [dbo].[Permission] ([Id], [Name], [Description]) VALUES (15, N'Report Packaged Orders', N'Report don hang da dong goi')
INSERT [dbo].[Permission] ([Id], [Name], [Description]) VALUES (16, N'Report All Orders', N'Repot tat ca cac don hang trong ky')
INSERT [dbo].[Permission] ([Id], [Name], [Description]) VALUES (17, N'Reset User Password', N'Reset password')
INSERT [dbo].[Permission] ([Id], [Name], [Description]) VALUES (18, N'View Batchjob Log', N'Xem batchjob log')
INSERT [dbo].[Permission] ([Id], [Name], [Description]) VALUES (19, N'Reject Order', N'Tu choi don hang')
INSERT [dbo].[Permission] ([Id], [Name], [Description]) VALUES (20, N'View PV Log', N'Xem PV log')
INSERT [dbo].[Permission] ([Id], [Name], [Description]) VALUES (21, N'View User''s PV Log', N'Xem PV log của user (HR Admin/Manager)')

/* Insert role-permission link */
-- Employee
INSERT [dbo].[RolePermission] ([RoleId], [PermissionId]) VALUES (1, 2)
INSERT [dbo].[RolePermission] ([RoleId], [PermissionId]) VALUES (1, 3)
INSERT [dbo].[RolePermission] ([RoleId], [PermissionId]) VALUES (1, 1)
INSERT [dbo].[RolePermission] ([RoleId], [PermissionId]) VALUES (1, 20)
-- Warehouse
INSERT [dbo].[RolePermission] ([RoleId], [PermissionId]) VALUES (2, 2)
INSERT [dbo].[RolePermission] ([RoleId], [PermissionId]) VALUES (2, 3)
INSERT [dbo].[RolePermission] ([RoleId], [PermissionId]) VALUES (2, 6)
INSERT [dbo].[RolePermission] ([RoleId], [PermissionId]) VALUES (2, 15)
INSERT [dbo].[RolePermission] ([RoleId], [PermissionId]) VALUES (2, 1)
INSERT [dbo].[RolePermission] ([RoleId], [PermissionId]) VALUES (2, 20)
-- HR Admin
INSERT [dbo].[RolePermission] ([RoleId], [PermissionId]) VALUES (3, 2)
INSERT [dbo].[RolePermission] ([RoleId], [PermissionId]) VALUES (3, 3)
INSERT [dbo].[RolePermission] ([RoleId], [PermissionId]) VALUES (3, 4)
INSERT [dbo].[RolePermission] ([RoleId], [PermissionId]) VALUES (3, 5)
INSERT [dbo].[RolePermission] ([RoleId], [PermissionId]) VALUES (3, 7)
INSERT [dbo].[RolePermission] ([RoleId], [PermissionId]) VALUES (3, 8)
INSERT [dbo].[RolePermission] ([RoleId], [PermissionId]) VALUES (3, 14)
INSERT [dbo].[RolePermission] ([RoleId], [PermissionId]) VALUES (3, 9)
INSERT [dbo].[RolePermission] ([RoleId], [PermissionId]) VALUES (3, 10)
INSERT [dbo].[RolePermission] ([RoleId], [PermissionId]) VALUES (3, 11)
INSERT [dbo].[RolePermission] ([RoleId], [PermissionId]) VALUES (3, 12)
INSERT [dbo].[RolePermission] ([RoleId], [PermissionId]) VALUES (3, 1)
INSERT [dbo].[RolePermission] ([RoleId], [PermissionId]) VALUES (3, 16)
INSERT [dbo].[RolePermission] ([RoleId], [PermissionId]) VALUES (3, 20)
INSERT [dbo].[RolePermission] ([RoleId], [PermissionId]) VALUES (3, 21)
-- HR Manager
INSERT [dbo].[RolePermission] ([RoleId], [PermissionId]) VALUES (4, 2)
INSERT [dbo].[RolePermission] ([RoleId], [PermissionId]) VALUES (4, 3)
INSERT [dbo].[RolePermission] ([RoleId], [PermissionId]) VALUES (4, 5)
INSERT [dbo].[RolePermission] ([RoleId], [PermissionId]) VALUES (4, 1)
INSERT [dbo].[RolePermission] ([RoleId], [PermissionId]) VALUES (4, 19)
INSERT [dbo].[RolePermission] ([RoleId], [PermissionId]) VALUES (4, 20)
INSERT [dbo].[RolePermission] ([RoleId], [PermissionId]) VALUES (4, 21)
-- IT
INSERT [dbo].[RolePermission] ([RoleId], [PermissionId]) VALUES (5, 1)
INSERT [dbo].[RolePermission] ([RoleId], [PermissionId]) VALUES (5, 2)
INSERT [dbo].[RolePermission] ([RoleId], [PermissionId]) VALUES (5, 3)
INSERT [dbo].[RolePermission] ([RoleId], [PermissionId]) VALUES (5, 13)
INSERT [dbo].[RolePermission] ([RoleId], [PermissionId]) VALUES (5, 17)
INSERT [dbo].[RolePermission] ([RoleId], [PermissionId]) VALUES (5, 18)
INSERT [dbo].[RolePermission] ([RoleId], [PermissionId]) VALUES (5, 20)
