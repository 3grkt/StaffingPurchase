ALTER TABLE [dbo].[OrderBatch]
ADD [TypeId] smallint not null default(0),
	[HrAdminApproverId] int null,
	[HrManagerApproverId] int null,
	[HrAdminApprovalDate] datetime null,
	[HrManagerApprovalDate] datetime null
GO

ALTER TABLE [dbo].[OrderBatch]  WITH CHECK ADD  CONSTRAINT [FK_OrderBatch_User_1] FOREIGN KEY([HrAdminApproverId])
REFERENCES [dbo].[User] ([Id])
GO

ALTER TABLE [dbo].[OrderBatch]  WITH CHECK ADD  CONSTRAINT [FK_OrderBatch_User_2] FOREIGN KEY([HrManagerApproverId])
REFERENCES [dbo].[User] ([Id])
GO
