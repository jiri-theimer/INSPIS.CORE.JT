CREATE TABLE [dbo].[o53TagGroup](
	[o53ID] [int] IDENTITY(1,1) NOT NULL,
	[o53Name] [varchar](100) NULL,
	[o53Entities] [varchar](100) NULL,
	[o53IsMultiSelect] [bit] NULL,
	[o53DateInsert] [datetime] NULL,
	[o53DateUpdate] [datetime] NULL,
	[o53UserInsert] [varchar](50) NULL,
	[o53UserUpdate] [varchar](50) NULL,
	[o53ValidFrom] [datetime] NULL,
	[o53ValidUntil] [datetime] NULL,
	[j02ID_Owner] [int] NULL,
	[o53Ordinary] [int] NULL,
	[o53Field] [varchar](50) NULL,
 CONSTRAINT [PK_o53TagGroup] PRIMARY KEY CLUSTERED 
(
	[o53ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[o53TagGroup] ADD  CONSTRAINT [DEF_o53TagGroup_DateInsert]  DEFAULT (getdate()) FOR [o53DateInsert]
GO

ALTER TABLE [dbo].[o53TagGroup] ADD  CONSTRAINT [DEF_o53TagGroup_ValidFrom]  DEFAULT (getdate()) FOR [o53ValidFrom]
GO

ALTER TABLE [dbo].[o53TagGroup] ADD  CONSTRAINT [DEF_o53TagGroup_ValidUntil]  DEFAULT (CONVERT([datetime],'01.01.3000',(104))) FOR [o53ValidUntil]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Název kategorie' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'o53TagGroup', @level2type=N'COLUMN',@level2name=N'o53Name'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Prázdné = pro všechny entity' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'o53TagGroup', @level2type=N'COLUMN',@level2name=N'o53Entities'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Kategorie (skupina položek)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'o53TagGroup'
GO


CREATE TABLE [dbo].[o51Tag](
	[o51ID] [int] IDENTITY(1,1) NOT NULL,
	[o53ID] [int] NULL,
	[o51Name] [varchar](100) NULL,
	[o51Code] [varchar](50) NULL,
	[o51Memo] [varchar](500) NULL,
	[o51IsColor] [bit] NULL,
	[o51BackColor] [varchar](50) NULL,
	[o51ForeColor] [varchar](50) NULL,
	[o51DateInsert] [datetime] NULL,
	[o51DateUpdate] [datetime] NULL,
	[o51UserInsert] [nvarchar](50) NULL,
	[o51UserUpdate] [nvarchar](50) NULL,
	[o51ValidFrom] [datetime] NULL,
	[o51ValidUntil] [datetime] NULL,
	[j02ID_Owner] [int] NULL,
	[o51Ordinary] [int] NULL,
 CONSTRAINT [PK_o51Tag] PRIMARY KEY CLUSTERED 
(
	[o51ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[o51Tag] ADD  CONSTRAINT [DEF_o51Tag_DateInsert]  DEFAULT (getdate()) FOR [o51DateInsert]
GO

ALTER TABLE [dbo].[o51Tag] ADD  CONSTRAINT [DEF_o51Tag_ValidFrom]  DEFAULT (getdate()) FOR [o51ValidFrom]
GO

ALTER TABLE [dbo].[o51Tag] ADD  CONSTRAINT [DEF_o51Tag_ValidUntil]  DEFAULT (CONVERT([datetime],'01.01.3000',(104))) FOR [o51ValidUntil]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Název položky' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'o51Tag', @level2type=N'COLUMN',@level2name=N'o51Name'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Položka kategorie k oštítkování záznamu entity' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'o51Tag'
GO


CREATE TABLE [dbo].[o52TagBinding](
	[o52ID] [int] IDENTITY(1,1) NOT NULL,
	[o51ID] [int] NULL,
	[o52RecordPid] [int] NULL,
	[o52RecordEntity] [varchar](50) NULL,
	[o52DateInsert] [datetime] NULL,
	[o52UserInsert] [nvarchar](50) NULL,
	[o52UserUpdate] [nvarchar](50) NULL,
	[o52DateUpdate] [datetime] NULL,
 CONSTRAINT [PK_o52TagBinding] PRIMARY KEY CLUSTERED 
(
	[o52ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[o52TagBinding] ADD  CONSTRAINT [DEF_o52TagBinding_DateInsert]  DEFAULT (getdate()) FOR [o52DateInsert]
GO

ALTER TABLE [dbo].[o52TagBinding]  WITH CHECK ADD  CONSTRAINT [o51Tag_o52TagBinding] FOREIGN KEY([o51ID])
REFERENCES [dbo].[o51Tag] ([o51ID])
GO

ALTER TABLE [dbo].[o52TagBinding] CHECK CONSTRAINT [o51Tag_o52TagBinding]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Položka kategorie' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'o52TagBinding', @level2type=N'COLUMN',@level2name=N'o51ID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ID záznamu entity' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'o52TagBinding', @level2type=N'COLUMN',@level2name=N'o52RecordPid'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Druh svázané entity' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'o52TagBinding', @level2type=N'COLUMN',@level2name=N'o52RecordEntity'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Vazba položky na záznamy entit' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'o52TagBinding'
GO

CREATE TABLE [dbo].[o54TagBindingInline](
	[o54ID] [int] IDENTITY(1,1) NOT NULL,
	[o54InlineText] [varchar](500) NULL,
	[o54InlineHtml] [varchar](500) NULL,
	[o54RecordPid] [int] NULL,
	[o54RecordEntity] [varchar](50) NULL,
	[o54DateInsert] [datetime] NULL,
	[o54UserInsert] [nvarchar](50) NULL,
	[o54UserUpdate] [nvarchar](50) NULL,
	[o54DateUpdate] [datetime] NULL,
	[o54Group01] [varchar](100) NULL,
	[o54Group02] [varchar](100) NULL,
	[o54Group03] [varchar](100) NULL,
	[o54Group04] [varchar](100) NULL,
	[o54Group05] [varchar](100) NULL,
	[o54Group06] [varchar](100) NULL,
	[o54Group07] [varchar](100) NULL,
	[o54Group08] [varchar](100) NULL,
	[o54Group09] [varchar](100) NULL,
	[o54Group10] [varchar](100) NULL,
	[o54Group11] [varchar](100) NULL,
	[o54Group12] [varchar](100) NULL,
	[o54Group13] [varchar](100) NULL,
	[o54Group14] [varchar](100) NULL,
	[o54Group15] [varchar](100) NULL,
	[o54Group16] [varchar](100) NULL,
	[o54Group17] [varchar](100) NULL,
	[o54Group18] [varchar](100) NULL,
	[o54Group19] [varchar](100) NULL,
	[o54Group20] [varchar](100) NULL,
	[o54Group21] [varchar](100) NULL,
	[o54Group22] [varchar](100) NULL,
	[o54Group23] [varchar](100) NULL,
	[o54Group24] [varchar](100) NULL,
	[o54Group25] [varchar](100) NULL,
	[o54Group26] [varchar](100) NULL,
	[o54Group27] [varchar](100) NULL,
	[o54Group28] [varchar](100) NULL,
	[o54Group29] [varchar](100) NULL,
	[o54Group30] [varchar](100) NULL,
	[o54Group31] [varchar](100) NULL,
 CONSTRAINT [PK_o54TagBindingInline] PRIMARY KEY CLUSTERED 
(
	[o54ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[o54TagBindingInline] ADD  CONSTRAINT [DEF_o54TagBindingInline_DateInsert]  DEFAULT (getdate()) FOR [o54DateInsert]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Čárkou oddělené názvy položek' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'o54TagBindingInline', @level2type=N'COLUMN',@level2name=N'o54InlineText'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Html názvy položek' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'o54TagBindingInline', @level2type=N'COLUMN',@level2name=N'o54InlineHtml'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ID záznamu entity' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'o54TagBindingInline', @level2type=N'COLUMN',@level2name=N'o54RecordPid'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Druh svázané entity' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'o54TagBindingInline', @level2type=N'COLUMN',@level2name=N'o54RecordEntity'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Vazba položky na záznamy entit' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'o54TagBindingInline'
GO