CREATE TABLE [dbo].[x51HelpCore](
	[x51ID] [int] IDENTITY(1,1) NOT NULL,
	[x51Name] [nvarchar](255) NOT NULL,
	[x51ViewUrl] [varchar](255) NULL,
	[x51ExternalUrl] [varchar](255) NULL,
	[x51Html] [ntext] NULL,
	[x51PlainText] [ntext] NULL,
	[x51UserInsert] [nvarchar](50) NOT NULL,
	[x51UserUpdate] [nvarchar](50) NULL,
	[x51DateInsert] [datetime2](7) NOT NULL,
	[x51DateUpdate] [datetime2](7) NULL,
	[x51ValidFrom] [datetime2](7) NULL,
	[x51ValidUntil] [datetime2](7) NULL,
 CONSTRAINT [PK_x51HelpCore] PRIMARY KEY CLUSTERED 
(
	[x51ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


CREATE TABLE [dbo].[x91Translate](
	[x91ID] [int] IDENTITY(1,1) NOT NULL,
	[x91Code] [varchar](50) NULL,
	[x91Orig] [nvarchar](300) NULL,
	[x91Lang1] [nvarchar](300) NULL,
	[x91Lang2] [nvarchar](300) NULL,
	[x91Lang3] [nvarchar](300) NULL,
	[x91Lang4] [nvarchar](300) NULL,
	[x91Page] [varchar](50) NULL,
 CONSTRAINT [PK_x91Translate] PRIMARY KEY CLUSTERED 
(
	[x91ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'nepovinné' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'x91Translate', @level2type=N'COLUMN',@level2name=N'x91Code'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'originál v češtině' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'x91Translate', @level2type=N'COLUMN',@level2name=N'x91Orig'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'anglicky' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'x91Translate', @level2type=N'COLUMN',@level2name=N'x91Lang1'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'UA' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'x91Translate', @level2type=N'COLUMN',@level2name=N'x91Lang2'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'slovensky' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'x91Translate', @level2type=N'COLUMN',@level2name=N'x91Lang4'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Překlady do ostatních jazyků v rámci fungování sqlserveru' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'x91Translate'
GO


