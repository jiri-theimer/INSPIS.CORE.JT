

CREATE TABLE [dbo].[j26Holiday](
	[j26ID] [int] IDENTITY(1,1) NOT NULL,	
	[j26Name] [nvarchar](255) NULL,
	[j26Date] [datetime] NULL,
	[j26DateInsert] [datetime] NULL,
	[j26UserInsert] [nvarchar](50) NULL,
	[j26DateUpdate] [datetime] NULL,
	[j26UserUpdate] [nvarchar](50) NULL,
	[j26ValidFrom] [datetime] NULL,
	[j26ValidUntil] [datetime] NULL,
 CONSTRAINT [PK_j26Holiday] PRIMARY KEY CLUSTERED 
(
	[j26ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[j26Holiday] ADD  CONSTRAINT [DEF_j26Holiday_j26DateInsert]  DEFAULT (getdate()) FOR [j26DateInsert]
GO

ALTER TABLE [dbo].[j26Holiday] ADD  CONSTRAINT [DEF_j26Holiday_j26ValidFrom]  DEFAULT (getdate()) FOR [j26ValidFrom]
GO

ALTER TABLE [dbo].[j26Holiday] ADD  CONSTRAINT [DEF_j26Holiday_j26ValidUntil]  DEFAULT (CONVERT([datetime],'01.01.3000',(104))) FOR [j26ValidUntil]
GO



EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Název svátku' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'j26Holiday', @level2type=N'COLUMN',@level2name=N'j26Name'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Den svátku' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'j26Holiday', @level2type=N'COLUMN',@level2name=N'j26Date'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Tým osob' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'j26Holiday'
GO
