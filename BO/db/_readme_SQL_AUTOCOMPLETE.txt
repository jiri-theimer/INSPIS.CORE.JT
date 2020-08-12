CREATE TABLE [dbo].[o15AutoComplete](
	[o15ID] [int] IDENTITY(1,1) NOT NULL,
	[o15Flag] [int] NULL,
	[o15Value] [nvarchar](255) NULL,
	[o15Ordinary] [int] NULL,
 CONSTRAINT [PK_o15AutoComplete] PRIMARY KEY CLUSTERED 
(
	[o15ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Explicitní pořadí' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'o15AutoComplete', @level2type=N'COLUMN',@level2name=N'o15Ordinary'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Konstanty pro autocomplete' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'o15AutoComplete'
GO


