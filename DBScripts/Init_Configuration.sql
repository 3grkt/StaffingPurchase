-- Init configuration
DELETE FROM [Configuration]
INSERT INTO [Configuration] ([Name], [Description], [Value], [CreatedDate], [ModifiedDate]) VALUES ('BirthDayAwardedPV', N'Điểm PV cho tháng sinh nhật', N'15', GETDATE(), GETDATE())
INSERT INTO [Configuration] ([Name], [Description], [Value], [CreatedDate], [ModifiedDate]) VALUES ('HighValueProductLimit', N'Số sản phẩm giá trị cao trong năm', N'2', GETDATE(), GETDATE())
INSERT INTO [Configuration] ([Name], [Description], [Value], [CreatedDate], [ModifiedDate]) VALUES ('HighValueProductPrice', N'Giá tiền của sản phẩm giá trị cao', N'5000000', GETDATE(), GETDATE())
INSERT INTO [Configuration] ([Name], [Description], [Value], [CreatedDate], [ModifiedDate]) VALUES ('OrderSessionStartDayOfMonth', N'Ngày mở chương trình', N'15', GETDATE(), GETDATE())
INSERT INTO [Configuration] ([Name], [Description], [Value], [CreatedDate], [ModifiedDate]) VALUES ('OrderSessionEndDayOfMonth', N'Ngày đóng chương trình', N'3', GETDATE(), GETDATE())
INSERT INTO [Configuration] ([Name], [Description], [Value], [CreatedDate], [ModifiedDate]) VALUES ('PolicyDocumentFile', N'File policy', N'Policy.docx', GETDATE(), GETDATE())
