CREATE TABLE [dbo].[j92PingLog](
	[j92ID] [int] IDENTITY(1,1) NOT NULL,
	[j03ID] [int] NULL,
	[j92Date] [datetime] NULL,
	[j92BrowserUserAgent] [varchar](1000) NULL,
	[j92BrowserFamily] [varchar](50) NULL,
	[j92BrowserOS] [varchar](50) NULL,
	[j92BrowserDeviceType] [varchar](10) NULL,
	[j92BrowserDeviceFamily] [varchar](50) NULL,
	[j92BrowserAvailWidth] [int] NULL,
	[j92BrowserAvailHeight] [int] NULL,
	[j92BrowserInnerWidth] [int] NULL,
	[j92BrowserInnerHeight] [int] NULL,
	[j92RequestURL] [varchar](255) NULL,
	[j92ValidFrom] [datetime] NULL,
	[j92ValidUntil] [datetime] NULL,
 CONSTRAINT [PK_j92PingLog] PRIMARY KEY CLUSTERED 
(
	[j92ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[j92PingLog] ADD  CONSTRAINT [DEF_j92PingLog_j92Date]  DEFAULT (getdate()) FOR [j92Date]
GO

ALTER TABLE [dbo].[j92PingLog] ADD  CONSTRAINT [DEF_j92PingLog_ValidFrom]  DEFAULT (getdate()) FOR [j92ValidFrom]
GO

ALTER TABLE [dbo].[j92PingLog] ADD  CONSTRAINT [DEF_j92PingLog_ValidUntil]  DEFAULT (CONVERT([datetime],'01.01.3000',(104))) FOR [j92ValidUntil]
GO

ALTER TABLE [dbo].[j92PingLog]  WITH CHECK ADD  CONSTRAINT [j03User_j92PingLog] FOREIGN KEY([j03ID])
REFERENCES [dbo].[j03User] ([j03ID])
GO

ALTER TABLE [dbo].[j92PingLog] CHECK CONSTRAINT [j03User_j92PingLog]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'useragent prohlížeče' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'j92PingLog', @level2type=N'COLUMN',@level2name=N'j92BrowserUserAgent'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'OS' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'j92PingLog', @level2type=N'COLUMN',@level2name=N'j92BrowserOS'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Desktop/Phone' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'j92PingLog', @level2type=N'COLUMN',@level2name=N'j92BrowserDeviceType'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'URL stránky, na které se uživatel vyskytuje' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'j92PingLog', @level2type=N'COLUMN',@level2name=N'j92RequestURL'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'PING log' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'j92PingLog'
GO

