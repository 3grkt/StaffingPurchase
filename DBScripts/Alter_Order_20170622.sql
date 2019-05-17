alter table [Order] add LocationId int null, DepartmentId int null
go
ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_Order_Location] FOREIGN KEY([LocationId])
REFERENCES [dbo].[Location] ([Id])
go
ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_Order_Department] FOREIGN KEY([DepartmentId])
REFERENCES [dbo].[Department] ([Id])