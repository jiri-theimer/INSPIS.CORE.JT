CREATE TABLE [dbo].[j40MailAccount](
	[j40ID] [int] IDENTITY(1,1) NOT NULL,
	[j02ID_Owner] [int] NULL,
	[j40Name] [varchar](100) NULL,
	[j40UsageFlag] [tinyint] NULL,
	[j40SmtpHost] [varchar](100) NULL,
	[j40SmtpPort] [int] NULL,
	[j40SmtpName] [varchar](100) NULL,
	[j40SmtpEmail] [varchar](50) NULL,
	[j40SmtpLogin] [varchar](50) NULL,
	[j40SmtpPassword] [varchar](50) NULL,
	[j40SmtpUsePersonalReply] [bit] NULL,
	[j40SmtpUseDefaultCredentials] [bit] NULL,
	[j40SmtpEnableSsl] [bit] NULL,
	[j40ImapHost] [varchar](100) NULL,
	[j40ImapLogin] [varchar](50) NULL,
	[j40ImapPassword] [varchar](50) NULL,
	[j40ImapPort] [int] NULL,
	[j40DateInsert] [datetime] NULL,
	[j40UserInsert] [varchar](50) NULL,
	[j40DateUpdate] [datetime] NULL,
	[j40UserUpdate] [varchar](50) NULL,
	[j40ValidFrom] [datetime] NULL,
	[j40ValidUntil] [datetime] NULL,
 CONSTRAINT [PK_j40MailAccount] PRIMARY KEY CLUSTERED 
(
	[j40ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[j40MailAccount] ADD  CONSTRAINT [DEF_j40MailAccount_j40SmtpPort]  DEFAULT ((0)) FOR [j40SmtpPort]
GO

ALTER TABLE [dbo].[j40MailAccount] ADD  CONSTRAINT [DEF_j40MailAccount_DateInsert]  DEFAULT (getdate()) FOR [j40DateInsert]
GO

ALTER TABLE [dbo].[j40MailAccount] ADD  CONSTRAINT [DEF_j40MailAccount_ValidFrom]  DEFAULT (getdate()) FOR [j40ValidFrom]
GO

ALTER TABLE [dbo].[j40MailAccount] ADD  CONSTRAINT [DEF_j40MailAccount_ValidUntil]  DEFAULT (CONVERT([datetime],'01.01.3000',(104))) FOR [j40ValidUntil]
GO

ALTER TABLE [dbo].[j40MailAccount]  WITH CHECK ADD  CONSTRAINT [j02Person_j40MailAccount] FOREIGN KEY([j02ID_Owner])
REFERENCES [dbo].[j02Person] ([j02ID])
GO

ALTER TABLE [dbo].[j40MailAccount] CHECK CONSTRAINT [j02Person_j40MailAccount]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'1-globální účet' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'j40MailAccount', @level2type=N'COLUMN',@level2name=N'j40UsageFlag'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'host adresa' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'j40MailAccount', @level2type=N'COLUMN',@level2name=N'j40SmtpHost'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'smtp port' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'j40MailAccount', @level2type=N'COLUMN',@level2name=N'j40SmtpPort'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'smtp login' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'j40MailAccount', @level2type=N'COLUMN',@level2name=N'j40SmtpLogin'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'smtp heslo' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'j40MailAccount', @level2type=N'COLUMN',@level2name=N'j40SmtpPassword'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'1 - bez hesla a loginu' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'j40MailAccount', @level2type=N'COLUMN',@level2name=N'j40SmtpUseDefaultCredentials'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'imap server' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'j40MailAccount', @level2type=N'COLUMN',@level2name=N'j40ImapHost'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Poštovní účet' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'j40MailAccount'
GO


