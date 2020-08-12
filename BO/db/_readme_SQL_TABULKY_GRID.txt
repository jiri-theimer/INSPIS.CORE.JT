

CREATE TABLE [dbo].[j72TheGridState](
	[j72ID] [int] IDENTITY(1,1) NOT NULL,
	[j03ID] [int] NULL,
	[j72Name] [varchar](100) NULL,
	[j72IsSystem] [bit] NULL,
	[j72Entity] [varchar](50) NULL,
	[j72MasterEntity] [varchar](50) NULL,
	[j72Columns] [varchar](800) NULL,
	[j72SplitterFlag] [tinyint] NULL,
	[j72HeightPanel1] [int] NULL,
	[j72HeightPanel2] [int] NULL,
	[j72MasterPID] [int] NULL,
	[j72DateInsert] [datetime] NULL,
	[j72UserInsert] [varchar](50) NULL,
	[j72DateUpdate] [datetime] NULL,
	[j72UserUpdate] [varchar](50) NULL,
	[j72PageSize] [int] NULL,
	[j72SortDataField] [varchar](50) NULL,
	[j72SortOrder] [varchar](50) NULL,
	[j72CurrentPagerIndex] [int] NULL,
	[j72CurrentRecordPid] [int] NULL,
	[j72GroupDataField] [varchar](50) NULL,
	[j72GroupFlag] [tinyint] NULL,
	[j72Filter] [varchar](500) NULL,
	[j72GroupPid] [varchar](255) NULL,
	[j72TableClass] [varchar](50) NULL,
	[j72TableBgColor] [varchar](50) NULL,
	[j72ColumnsGridWidth] [varchar](500) NULL,
	[j72ColumnsReportWidth] [varchar](500) NULL,
	[j72IsNoWrap] [bit] NULL,
	[j72TreePid] [int] NULL,
	[j72ValidFrom] [datetime] NULL,
	[j72ValidUntil] [datetime] NULL,
	[j72SelectableFlag] [tinyint] NULL,
	[j72HashJ73Query] [bit] NULL,
	[j72IsPublic] [bit] NULL,
 CONSTRAINT [PK_j72QueryTemplate_State] PRIMARY KEY CLUSTERED 
(
	[j72ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[j72TheGridState] ADD  CONSTRAINT [DEF_j72TheGridState_j72IsNoWrap]  DEFAULT ((0)) FOR [j72IsNoWrap]
GO

ALTER TABLE [dbo].[j72TheGridState] ADD  CONSTRAINT [DEF_j72TheGridState_ValidFrom]  DEFAULT (getdate()) FOR [j72ValidFrom]
GO

ALTER TABLE [dbo].[j72TheGridState] ADD  CONSTRAINT [DEF_j72TheGridState_ValidUntil]  DEFAULT (CONVERT([datetime],'01.01.3000',(104))) FOR [j72ValidUntil]
GO


CREATE TABLE [dbo].[j73TheGridQuery](
	[j73ID] [int] IDENTITY(1,1) NOT NULL,
	[j72ID] [int] NULL,
	[j73Column] [varchar](50) NULL,
	[j73Op] [varchar](5) NULL,
	[j73BracketLeft] [varchar](10) NULL,
	[j73BracketRight] [varchar](10) NULL,
	[j73Operator] [varchar](50) NULL,
	[j73Value] [nvarchar](300) NULL,
	[j73ValueAlias] [nvarchar](1000) NULL,
	[j73ComboValue] [int] NULL,
	[j73Date1] [datetime] NULL,
	[j73Date2] [datetime] NULL,
	[j73Num1] [float] NULL,
	[j73Num2] [float] NULL,
	[j73Ordinal] [int] NULL,
	[j73DateInsert] [datetime] NULL,
	[j73UserInsert] [varchar](50) NULL,
	[j73DateUpdate] [datetime] NULL,
	[j73UserUpdate] [varchar](50) NULL,
	[j73DatePeriodFlag] [tinyint] NULL,
 CONSTRAINT [PK_j73TheGridQuery] PRIMARY KEY CLUSTERED 
(
	[j73ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[j74TheGridReceiver](
	[j74ID] [int] IDENTITY(1,1) NOT NULL,
	[j72ID] [int] NULL,
	[j04ID] [int] NULL,
	[j11ID] [int] NULL,
 CONSTRAINT [PK_j74TheGridReceiver] PRIMARY KEY CLUSTERED 
(
	[j74ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

