USE [rules-framework-sample]
GO
/****** Object:  Table [dbo].[ConditionNodeRelations]    Script Date: 29/07/2022 15:14:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ConditionNodeRelations](
	[OwnerId] [bigint] NOT NULL,
	[ChildId] [bigint] NOT NULL,
 CONSTRAINT [PK_ConditionNodeRelations] PRIMARY KEY CLUSTERED 
(
	[OwnerId] ASC,
	[ChildId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ConditionNodes]    Script Date: 29/07/2022 15:14:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ConditionNodes](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ConditionNodeTypeCode] [int] NOT NULL,
	[ConditionTypeCode] [int] NOT NULL,
	[DataTypeCode] [int] NOT NULL,
	[OperatorCode] [int] NULL,
	[Operand] [nvarchar](50) NULL,
	[LogicalOperatorCode] [int] NULL,
 CONSTRAINT [PK_ConditionNodes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ConditionNodeTypes]    Script Date: 29/07/2022 15:14:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ConditionNodeTypes](
	[Code] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_ConditionNodeTypes] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ConditionTypes]    Script Date: 29/07/2022 15:14:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ConditionTypes](
	[Code] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_ConditionTypes] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ContentTypes]    Script Date: 29/07/2022 15:14:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ContentTypes](
	[Code] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_ContentTypes] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DataTypes]    Script Date: 29/07/2022 15:14:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DataTypes](
	[Code] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_DataTypes] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LogicalOperators]    Script Date: 29/07/2022 15:14:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LogicalOperators](
	[Code] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Symbol] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_LogicalOperators] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Operators]    Script Date: 29/07/2022 15:14:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Operators](
	[Code] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Symbol] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Operators] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Rules]    Script Date: 29/07/2022 15:14:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Rules](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Content] [nvarchar](100) NOT NULL,
	[ContentTypeCode] [int] NOT NULL,
	[DateBegin] [datetime2](7) NOT NULL,
	[DateEnd] [datetime2](7) NULL,
	[Name] [nvarchar](250) NOT NULL,
	[Priority] [int] NOT NULL,
	[ConditionNodeId] [bigint] NULL,
 CONSTRAINT [PK_Rules] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ConditionNodeRelations]  WITH CHECK ADD  CONSTRAINT [FK_ChildConditionNodeRelations_ConditionNodes] FOREIGN KEY([ChildId])
REFERENCES [dbo].[ConditionNodes] ([Id])
GO
ALTER TABLE [dbo].[ConditionNodeRelations] CHECK CONSTRAINT [FK_ChildConditionNodeRelations_ConditionNodes]
GO
ALTER TABLE [dbo].[ConditionNodeRelations]  WITH CHECK ADD  CONSTRAINT [FK_OwnerConditionNodeRelations_ConditionNodes] FOREIGN KEY([OwnerId])
REFERENCES [dbo].[ConditionNodes] ([Id])
GO
ALTER TABLE [dbo].[ConditionNodeRelations] CHECK CONSTRAINT [FK_OwnerConditionNodeRelations_ConditionNodes]
GO
ALTER TABLE [dbo].[ConditionNodes]  WITH CHECK ADD  CONSTRAINT [FK_ConditionNodes_ConditionNodeTypes] FOREIGN KEY([ConditionNodeTypeCode])
REFERENCES [dbo].[ConditionNodeTypes] ([Code])
GO
ALTER TABLE [dbo].[ConditionNodes] CHECK CONSTRAINT [FK_ConditionNodes_ConditionNodeTypes]
GO
ALTER TABLE [dbo].[ConditionNodes]  WITH CHECK ADD  CONSTRAINT [FK_ConditionNodes_ConditionTypes] FOREIGN KEY([ConditionTypeCode])
REFERENCES [dbo].[ConditionTypes] ([Code])
GO
ALTER TABLE [dbo].[ConditionNodes] CHECK CONSTRAINT [FK_ConditionNodes_ConditionTypes]
GO
ALTER TABLE [dbo].[ConditionNodes]  WITH CHECK ADD  CONSTRAINT [FK_ConditionNodes_DataTypes] FOREIGN KEY([DataTypeCode])
REFERENCES [dbo].[DataTypes] ([Code])
GO
ALTER TABLE [dbo].[ConditionNodes] CHECK CONSTRAINT [FK_ConditionNodes_DataTypes]
GO
ALTER TABLE [dbo].[ConditionNodes]  WITH CHECK ADD  CONSTRAINT [FK_ConditionNodes_LogicalOperators] FOREIGN KEY([LogicalOperatorCode])
REFERENCES [dbo].[LogicalOperators] ([Code])
GO
ALTER TABLE [dbo].[ConditionNodes] CHECK CONSTRAINT [FK_ConditionNodes_LogicalOperators]
GO
ALTER TABLE [dbo].[ConditionNodes]  WITH CHECK ADD  CONSTRAINT [FK_ConditionNodes_Operators] FOREIGN KEY([OperatorCode])
REFERENCES [dbo].[Operators] ([Code])
GO
ALTER TABLE [dbo].[ConditionNodes] CHECK CONSTRAINT [FK_ConditionNodes_Operators]
GO
ALTER TABLE [dbo].[Rules]  WITH CHECK ADD  CONSTRAINT [FK_Rules_ConditionNodes] FOREIGN KEY([ConditionNodeId])
REFERENCES [dbo].[ConditionNodes] ([Id])
GO
ALTER TABLE [dbo].[Rules] CHECK CONSTRAINT [FK_Rules_ConditionNodes]
GO
ALTER TABLE [dbo].[Rules]  WITH CHECK ADD  CONSTRAINT [FK_Rules_ContentTypes] FOREIGN KEY([ContentTypeCode])
REFERENCES [dbo].[ContentTypes] ([Code])
GO
ALTER TABLE [dbo].[Rules] CHECK CONSTRAINT [FK_Rules_ContentTypes]
GO
