USE [@dbname]
GO
/****** Object:  Table [@schemaname].[ConditionNodeRelations]    Script Date: 2022-08-05 14:49:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[@schemaname].[ConditionNodeRelations]') AND type in (N'U'))
BEGIN
CREATE TABLE [@schemaname].[ConditionNodeRelations](
	[OwnerId] [bigint] NOT NULL,
	[ChildId] [bigint] NOT NULL,
 CONSTRAINT [PK_ConditionNodeRelations] PRIMARY KEY CLUSTERED 
(
	[OwnerId] ASC,
	[ChildId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [@schemaname].[ConditionNodes]    Script Date: 2022-08-05 14:49:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[@schemaname].[ConditionNodes]') AND type in (N'U'))
BEGIN
CREATE TABLE [@schemaname].[ConditionNodes](
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [@schemaname].[ConditionNodeTypes]    Script Date: 2022-08-05 14:49:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[@schemaname].[ConditionNodeTypes]') AND type in (N'U'))
BEGIN
CREATE TABLE [@schemaname].[ConditionNodeTypes](
	[Code] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_ConditionNodeTypes] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [@schemaname].[ConditionTypes]    Script Date: 2022-08-05 14:49:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[@schemaname].[ConditionTypes]') AND type in (N'U'))
BEGIN
CREATE TABLE [@schemaname].[ConditionTypes](
	[Code] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_ConditionTypes] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [@schemaname].[ContentTypes]    Script Date: 2022-08-05 14:49:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[@schemaname].[ContentTypes]') AND type in (N'U'))
BEGIN
CREATE TABLE [@schemaname].[ContentTypes](
	[Code] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_ContentTypes] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [@schemaname].[DataTypes]    Script Date: 2022-08-05 14:49:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[@schemaname].[DataTypes]') AND type in (N'U'))
BEGIN
CREATE TABLE [@schemaname].[DataTypes](
	[Code] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_DataTypes] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [@schemaname].[LogicalOperators]    Script Date: 2022-08-05 14:49:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[@schemaname].[LogicalOperators]') AND type in (N'U'))
BEGIN
CREATE TABLE [@schemaname].[LogicalOperators](
	[Code] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Symbol] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_LogicalOperators] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [@schemaname].[Operators]    Script Date: 2022-08-05 14:49:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[@schemaname].[Operators]') AND type in (N'U'))
BEGIN
CREATE TABLE [@schemaname].[Operators](
	[Code] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Symbol] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Operators] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [@schemaname].[Rules]    Script Date: 2022-08-05 14:49:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[@schemaname].[Rules]') AND type in (N'U'))
BEGIN
CREATE TABLE [@schemaname].[Rules](
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[@schemaname].[FK_ChildConditionNodeRelations_ConditionNodes]') AND parent_object_id = OBJECT_ID(N'[@schemaname].[ConditionNodeRelations]'))
ALTER TABLE [@schemaname].[ConditionNodeRelations]  WITH CHECK ADD  CONSTRAINT [FK_ChildConditionNodeRelations_ConditionNodes] FOREIGN KEY([ChildId])
REFERENCES [@schemaname].[ConditionNodes] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[@schemaname].[FK_ChildConditionNodeRelations_ConditionNodes]') AND parent_object_id = OBJECT_ID(N'[@schemaname].[ConditionNodeRelations]'))
ALTER TABLE [@schemaname].[ConditionNodeRelations] CHECK CONSTRAINT [FK_ChildConditionNodeRelations_ConditionNodes]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[@schemaname].[FK_OwnerConditionNodeRelations_ConditionNodes]') AND parent_object_id = OBJECT_ID(N'[@schemaname].[ConditionNodeRelations]'))
ALTER TABLE [@schemaname].[ConditionNodeRelations]  WITH CHECK ADD  CONSTRAINT [FK_OwnerConditionNodeRelations_ConditionNodes] FOREIGN KEY([OwnerId])
REFERENCES [@schemaname].[ConditionNodes] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[@schemaname].[FK_OwnerConditionNodeRelations_ConditionNodes]') AND parent_object_id = OBJECT_ID(N'[@schemaname].[ConditionNodeRelations]'))
ALTER TABLE [@schemaname].[ConditionNodeRelations] CHECK CONSTRAINT [FK_OwnerConditionNodeRelations_ConditionNodes]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[@schemaname].[FK_ConditionNodes_ConditionNodeTypes]') AND parent_object_id = OBJECT_ID(N'[@schemaname].[ConditionNodes]'))
ALTER TABLE [@schemaname].[ConditionNodes]  WITH CHECK ADD  CONSTRAINT [FK_ConditionNodes_ConditionNodeTypes] FOREIGN KEY([ConditionNodeTypeCode])
REFERENCES [@schemaname].[ConditionNodeTypes] ([Code])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[@schemaname].[FK_ConditionNodes_ConditionNodeTypes]') AND parent_object_id = OBJECT_ID(N'[@schemaname].[ConditionNodes]'))
ALTER TABLE [@schemaname].[ConditionNodes] CHECK CONSTRAINT [FK_ConditionNodes_ConditionNodeTypes]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[@schemaname].[FK_ConditionNodes_ConditionTypes]') AND parent_object_id = OBJECT_ID(N'[@schemaname].[ConditionNodes]'))
ALTER TABLE [@schemaname].[ConditionNodes]  WITH CHECK ADD  CONSTRAINT [FK_ConditionNodes_ConditionTypes] FOREIGN KEY([ConditionTypeCode])
REFERENCES [@schemaname].[ConditionTypes] ([Code])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[@schemaname].[FK_ConditionNodes_ConditionTypes]') AND parent_object_id = OBJECT_ID(N'[@schemaname].[ConditionNodes]'))
ALTER TABLE [@schemaname].[ConditionNodes] CHECK CONSTRAINT [FK_ConditionNodes_ConditionTypes]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[@schemaname].[FK_ConditionNodes_DataTypes]') AND parent_object_id = OBJECT_ID(N'[@schemaname].[ConditionNodes]'))
ALTER TABLE [@schemaname].[ConditionNodes]  WITH CHECK ADD  CONSTRAINT [FK_ConditionNodes_DataTypes] FOREIGN KEY([DataTypeCode])
REFERENCES [@schemaname].[DataTypes] ([Code])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[@schemaname].[FK_ConditionNodes_DataTypes]') AND parent_object_id = OBJECT_ID(N'[@schemaname].[ConditionNodes]'))
ALTER TABLE [@schemaname].[ConditionNodes] CHECK CONSTRAINT [FK_ConditionNodes_DataTypes]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[@schemaname].[FK_ConditionNodes_LogicalOperators]') AND parent_object_id = OBJECT_ID(N'[@schemaname].[ConditionNodes]'))
ALTER TABLE [@schemaname].[ConditionNodes]  WITH CHECK ADD  CONSTRAINT [FK_ConditionNodes_LogicalOperators] FOREIGN KEY([LogicalOperatorCode])
REFERENCES [@schemaname].[LogicalOperators] ([Code])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[@schemaname].[FK_ConditionNodes_LogicalOperators]') AND parent_object_id = OBJECT_ID(N'[@schemaname].[ConditionNodes]'))
ALTER TABLE [@schemaname].[ConditionNodes] CHECK CONSTRAINT [FK_ConditionNodes_LogicalOperators]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[@schemaname].[FK_ConditionNodes_Operators]') AND parent_object_id = OBJECT_ID(N'[@schemaname].[ConditionNodes]'))
ALTER TABLE [@schemaname].[ConditionNodes]  WITH CHECK ADD  CONSTRAINT [FK_ConditionNodes_Operators] FOREIGN KEY([OperatorCode])
REFERENCES [@schemaname].[Operators] ([Code])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[@schemaname].[FK_ConditionNodes_Operators]') AND parent_object_id = OBJECT_ID(N'[@schemaname].[ConditionNodes]'))
ALTER TABLE [@schemaname].[ConditionNodes] CHECK CONSTRAINT [FK_ConditionNodes_Operators]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[@schemaname].[FK_Rules_ConditionNodes]') AND parent_object_id = OBJECT_ID(N'[@schemaname].[Rules]'))
ALTER TABLE [@schemaname].[Rules]  WITH CHECK ADD  CONSTRAINT [FK_Rules_ConditionNodes] FOREIGN KEY([ConditionNodeId])
REFERENCES [@schemaname].[ConditionNodes] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[@schemaname].[FK_Rules_ConditionNodes]') AND parent_object_id = OBJECT_ID(N'[@schemaname].[Rules]'))
ALTER TABLE [@schemaname].[Rules] CHECK CONSTRAINT [FK_Rules_ConditionNodes]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[@schemaname].[FK_Rules_ContentTypes]') AND parent_object_id = OBJECT_ID(N'[@schemaname].[Rules]'))
ALTER TABLE [@schemaname].[Rules]  WITH CHECK ADD  CONSTRAINT [FK_Rules_ContentTypes] FOREIGN KEY([ContentTypeCode])
REFERENCES [@schemaname].[ContentTypes] ([Code])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[@schemaname].[FK_Rules_ContentTypes]') AND parent_object_id = OBJECT_ID(N'[@schemaname].[Rules]'))
ALTER TABLE [@schemaname].[Rules] CHECK CONSTRAINT [FK_Rules_ContentTypes]
GO
