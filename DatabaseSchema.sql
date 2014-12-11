 IF NOT EXISTS(SELECT * FROM sys.schemas WHERE [name] = N'dbo')      
     EXEC (N'CREATE SCHEMA dbo')                                   
 GO                                                               
IF  EXISTS (SELECT * FROM sys.objects so JOIN sys.schemas sc ON so.schema_id = sc.schema_id WHERE so.name = N'Backlog'  AND sc.name=N'dbo'  AND type in (N'U'))
BEGIN

  DECLARE @drop_statement nvarchar(500)

  DECLARE drop_cursor CURSOR FOR
      SELECT 'alter table '+quotename(schema_name(ob.schema_id))+
      '.'+quotename(object_name(ob.object_id))+ ' drop constraint ' + quotename(fk.name) 
      FROM sys.objects ob INNER JOIN sys.foreign_keys fk ON fk.parent_object_id = ob.object_id
      WHERE fk.referenced_object_id = 
          (
             SELECT so.object_id 
             FROM sys.objects so JOIN sys.schemas sc
             ON so.schema_id = sc.schema_id
             WHERE so.name = N'Backlog'  AND sc.name=N'dbo'  AND type in (N'U')
           )

  OPEN drop_cursor

  FETCH NEXT FROM drop_cursor
  INTO @drop_statement

  WHILE @@FETCH_STATUS = 0
  BEGIN
     EXEC (@drop_statement)

     FETCH NEXT FROM drop_cursor
     INTO @drop_statement
  END

  CLOSE drop_cursor
  DEALLOCATE drop_cursor

  DROP TABLE [dbo].[Backlog]
END 
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE 
[dbo].[Backlog]
(
   [Backlog_ID] int IDENTITY(1, 1)  NOT NULL,
   [Project_ID] int  NOT NULL,
   [Swimlane_ID] int  NOT NULL,
   [Title] nvarchar(255)  NULL,
   [Description] nvarchar(255)  NULL,
   [Complexity] int  NULL,
   [Status] nvarchar(255)  NULL,
   [Color] nvarchar(255)  NULL,
   [Position] int  NULL,
   [Start_date] datetime2(0)  NULL,
   [Completion_date] datetime2(0)  NULL,
   [Relative_ID] int  NULL
)
GO
GO
IF  EXISTS (SELECT * FROM sys.objects so JOIN sys.schemas sc ON so.schema_id = sc.schema_id WHERE so.name = N'Backlog_User'  AND sc.name=N'dbo'  AND type in (N'U'))
BEGIN

  DECLARE @drop_statement nvarchar(500)

  DECLARE drop_cursor CURSOR FOR
      SELECT 'alter table '+quotename(schema_name(ob.schema_id))+
      '.'+quotename(object_name(ob.object_id))+ ' drop constraint ' + quotename(fk.name) 
      FROM sys.objects ob INNER JOIN sys.foreign_keys fk ON fk.parent_object_id = ob.object_id
      WHERE fk.referenced_object_id = 
          (
             SELECT so.object_id 
             FROM sys.objects so JOIN sys.schemas sc
             ON so.schema_id = sc.schema_id
             WHERE so.name = N'Backlog_User'  AND sc.name=N'dbo'  AND type in (N'U')
           )

  OPEN drop_cursor

  FETCH NEXT FROM drop_cursor
  INTO @drop_statement

  WHILE @@FETCH_STATUS = 0
  BEGIN
     EXEC (@drop_statement)

     FETCH NEXT FROM drop_cursor
     INTO @drop_statement
  END

  CLOSE drop_cursor
  DEALLOCATE drop_cursor

  DROP TABLE [dbo].[Backlog_User]
END 
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE 
[dbo].[Backlog_User]
(
   [Backlog_ID] int  NOT NULL,
   [User_ID] int  NOT NULL
)
GO
GO
IF  EXISTS (SELECT * FROM sys.objects so JOIN sys.schemas sc ON so.schema_id = sc.schema_id WHERE so.name = N'Project'  AND sc.name=N'dbo'  AND type in (N'U'))
BEGIN

  DECLARE @drop_statement nvarchar(500)

  DECLARE drop_cursor CURSOR FOR
      SELECT 'alter table '+quotename(schema_name(ob.schema_id))+
      '.'+quotename(object_name(ob.object_id))+ ' drop constraint ' + quotename(fk.name) 
      FROM sys.objects ob INNER JOIN sys.foreign_keys fk ON fk.parent_object_id = ob.object_id
      WHERE fk.referenced_object_id = 
          (
             SELECT so.object_id 
             FROM sys.objects so JOIN sys.schemas sc
             ON so.schema_id = sc.schema_id
             WHERE so.name = N'Project'  AND sc.name=N'dbo'  AND type in (N'U')
           )

  OPEN drop_cursor

  FETCH NEXT FROM drop_cursor
  INTO @drop_statement

  WHILE @@FETCH_STATUS = 0
  BEGIN
     EXEC (@drop_statement)

     FETCH NEXT FROM drop_cursor
     INTO @drop_statement
  END

  CLOSE drop_cursor
  DEALLOCATE drop_cursor

  DROP TABLE [dbo].[Project]
END 
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE 
[dbo].[Project]
(
   [Project_ID] int IDENTITY(1, 1)  NOT NULL,
   [Name] nvarchar(1000)  NULL,
   [Description] nvarchar(max)  NULL,
   [Owner] int  NOT NULL,
   [Start_Date] datetime2(0)  NULL,
   [Modified_Date] datetime2(0)  NULL
)
GO
GO
IF  EXISTS (SELECT * FROM sys.objects so JOIN sys.schemas sc ON so.schema_id = sc.schema_id WHERE so.name = N'Project_Supervisor'  AND sc.name=N'dbo'  AND type in (N'U'))
BEGIN

  DECLARE @drop_statement nvarchar(500)

  DECLARE drop_cursor CURSOR FOR
      SELECT 'alter table '+quotename(schema_name(ob.schema_id))+
      '.'+quotename(object_name(ob.object_id))+ ' drop constraint ' + quotename(fk.name) 
      FROM sys.objects ob INNER JOIN sys.foreign_keys fk ON fk.parent_object_id = ob.object_id
      WHERE fk.referenced_object_id = 
          (
             SELECT so.object_id 
             FROM sys.objects so JOIN sys.schemas sc
             ON so.schema_id = sc.schema_id
             WHERE so.name = N'Project_Supervisor'  AND sc.name=N'dbo'  AND type in (N'U')
           )

  OPEN drop_cursor

  FETCH NEXT FROM drop_cursor
  INTO @drop_statement

  WHILE @@FETCH_STATUS = 0
  BEGIN
     EXEC (@drop_statement)

     FETCH NEXT FROM drop_cursor
     INTO @drop_statement
  END

  CLOSE drop_cursor
  DEALLOCATE drop_cursor

  DROP TABLE [dbo].[Project_Supervisor]
END 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE 
[dbo].[Project_Supervisor]
(
   [Project_ID] int  NOT NULL,
   [User_ID] int  NOT NULL
)
GO
GO
IF  EXISTS (SELECT * FROM sys.objects so JOIN sys.schemas sc ON so.schema_id = sc.schema_id WHERE so.name = N'Project_User'  AND sc.name=N'dbo'  AND type in (N'U'))
BEGIN

  DECLARE @drop_statement nvarchar(500)

  DECLARE drop_cursor CURSOR FOR
      SELECT 'alter table '+quotename(schema_name(ob.schema_id))+
      '.'+quotename(object_name(ob.object_id))+ ' drop constraint ' + quotename(fk.name) 
      FROM sys.objects ob INNER JOIN sys.foreign_keys fk ON fk.parent_object_id = ob.object_id
      WHERE fk.referenced_object_id = 
          (
             SELECT so.object_id 
             FROM sys.objects so JOIN sys.schemas sc
             ON so.schema_id = sc.schema_id
             WHERE so.name = N'Project_User'  AND sc.name=N'dbo'  AND type in (N'U')
           )

  OPEN drop_cursor

  FETCH NEXT FROM drop_cursor
  INTO @drop_statement

  WHILE @@FETCH_STATUS = 0
  BEGIN
     EXEC (@drop_statement)

     FETCH NEXT FROM drop_cursor
     INTO @drop_statement
  END

  CLOSE drop_cursor
  DEALLOCATE drop_cursor

  DROP TABLE [dbo].[Project_User]
END 
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE 
[dbo].[Project_User]
(
   [Project_ID] int  NOT NULL,
   [User_ID] int  NOT NULL
)
GO
GO
IF  EXISTS (SELECT * FROM sys.objects so JOIN sys.schemas sc ON so.schema_id = sc.schema_id WHERE so.name = N'Swimlane'  AND sc.name=N'dbo'  AND type in (N'U'))
BEGIN

  DECLARE @drop_statement nvarchar(500)

  DECLARE drop_cursor CURSOR FOR
      SELECT 'alter table '+quotename(schema_name(ob.schema_id))+
      '.'+quotename(object_name(ob.object_id))+ ' drop constraint ' + quotename(fk.name) 
      FROM sys.objects ob INNER JOIN sys.foreign_keys fk ON fk.parent_object_id = ob.object_id
      WHERE fk.referenced_object_id = 
          (
             SELECT so.object_id 
             FROM sys.objects so JOIN sys.schemas sc
             ON so.schema_id = sc.schema_id
             WHERE so.name = N'Swimlane'  AND sc.name=N'dbo'  AND type in (N'U')
           )

  OPEN drop_cursor

  FETCH NEXT FROM drop_cursor
  INTO @drop_statement

  WHILE @@FETCH_STATUS = 0
  BEGIN
     EXEC (@drop_statement)

     FETCH NEXT FROM drop_cursor
     INTO @drop_statement
  END

  CLOSE drop_cursor
  DEALLOCATE drop_cursor

  DROP TABLE [dbo].[Swimlane]
END 
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE 
[dbo].[Swimlane]
(
   [Swimlane_ID] int IDENTITY(1, 1)  NOT NULL,
   [Project_ID] int  NULL,
   [Name] nvarchar(255)  NULL,
   [Type] int  NULL,
   [Data_status] nvarchar(255)  NULL,
   [Position] int  NULL
)
GO
GO
IF  EXISTS (SELECT * FROM sys.objects so JOIN sys.schemas sc ON so.schema_id = sc.schema_id WHERE so.name = N'Task'  AND sc.name=N'dbo'  AND type in (N'U'))
BEGIN

  DECLARE @drop_statement nvarchar(500)

  DECLARE drop_cursor CURSOR FOR
      SELECT 'alter table '+quotename(schema_name(ob.schema_id))+
      '.'+quotename(object_name(ob.object_id))+ ' drop constraint ' + quotename(fk.name) 
      FROM sys.objects ob INNER JOIN sys.foreign_keys fk ON fk.parent_object_id = ob.object_id
      WHERE fk.referenced_object_id = 
          (
             SELECT so.object_id 
             FROM sys.objects so JOIN sys.schemas sc
             ON so.schema_id = sc.schema_id
             WHERE so.name = N'Task'  AND sc.name=N'dbo'  AND type in (N'U')
           )

  OPEN drop_cursor

  FETCH NEXT FROM drop_cursor
  INTO @drop_statement

  WHILE @@FETCH_STATUS = 0
  BEGIN
     EXEC (@drop_statement)

     FETCH NEXT FROM drop_cursor
     INTO @drop_statement
  END

  CLOSE drop_cursor
  DEALLOCATE drop_cursor

  DROP TABLE [dbo].[Task]
END 
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE 
[dbo].[Task]
(
   [Task_ID] int IDENTITY(1, 1)  NOT NULL,
   [Swimlane_ID] int  NOT NULL,
   [Project_ID] int  NOT NULL,
   [Backlog_ID] int  NULL,
   [Title] nvarchar(1000)  NULL,
   [Description] nvarchar(max)  NULL,
   [Work_estimation] int  NULL,
   [Status] nvarchar(16)  NULL,
   [Color] nvarchar(16)  NULL,
   [Position] int  NULL,
   [Due_date] datetime2(0)  NULL,
   [Completion_date] datetime2(0)  NULL,
   [Relative_ID] int  NULL,
   [Actual_work] int  NULL
)
GO
GO
IF  EXISTS (SELECT * FROM sys.objects so JOIN sys.schemas sc ON so.schema_id = sc.schema_id WHERE so.name = N'Task_Comment'  AND sc.name=N'dbo'  AND type in (N'U'))
BEGIN

  DECLARE @drop_statement nvarchar(500)

  DECLARE drop_cursor CURSOR FOR
      SELECT 'alter table '+quotename(schema_name(ob.schema_id))+
      '.'+quotename(object_name(ob.object_id))+ ' drop constraint ' + quotename(fk.name) 
      FROM sys.objects ob INNER JOIN sys.foreign_keys fk ON fk.parent_object_id = ob.object_id
      WHERE fk.referenced_object_id = 
          (
             SELECT so.object_id 
             FROM sys.objects so JOIN sys.schemas sc
             ON so.schema_id = sc.schema_id
             WHERE so.name = N'Task_Comment'  AND sc.name=N'dbo'  AND type in (N'U')
           )

  OPEN drop_cursor

  FETCH NEXT FROM drop_cursor
  INTO @drop_statement

  WHILE @@FETCH_STATUS = 0
  BEGIN
     EXEC (@drop_statement)

     FETCH NEXT FROM drop_cursor
     INTO @drop_statement
  END

  CLOSE drop_cursor
  DEALLOCATE drop_cursor

  DROP TABLE [dbo].[Task_Comment]
END 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE 
[dbo].[Task_Comment]
(
   [Comment_ID] bigint IDENTITY(1, 1)  NOT NULL,
   [Task_ID] int  NOT NULL,
   [User_ID] int  NOT NULL,
   [Content] nvarchar(max)  NULL,
   [Project_ID] int  NOT NULL
)
GO
GO
IF  EXISTS (SELECT * FROM sys.objects so JOIN sys.schemas sc ON so.schema_id = sc.schema_id WHERE so.name = N'Task_File'  AND sc.name=N'dbo'  AND type in (N'U'))
BEGIN

  DECLARE @drop_statement nvarchar(500)

  DECLARE drop_cursor CURSOR FOR
      SELECT 'alter table '+quotename(schema_name(ob.schema_id))+
      '.'+quotename(object_name(ob.object_id))+ ' drop constraint ' + quotename(fk.name) 
      FROM sys.objects ob INNER JOIN sys.foreign_keys fk ON fk.parent_object_id = ob.object_id
      WHERE fk.referenced_object_id = 
          (
             SELECT so.object_id 
             FROM sys.objects so JOIN sys.schemas sc
             ON so.schema_id = sc.schema_id
             WHERE so.name = N'Task_File'  AND sc.name=N'dbo'  AND type in (N'U')
           )

  OPEN drop_cursor

  FETCH NEXT FROM drop_cursor
  INTO @drop_statement

  WHILE @@FETCH_STATUS = 0
  BEGIN
     EXEC (@drop_statement)

     FETCH NEXT FROM drop_cursor
     INTO @drop_statement
  END

  CLOSE drop_cursor
  DEALLOCATE drop_cursor

  DROP TABLE [dbo].[Task_File]
END 
GO

SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE 
[dbo].[Task_File]
(
   [File_ID] int IDENTITY(1, 1)  NOT NULL,
   [Task_ID] int  NOT NULL,
   [Type] nvarchar(15)  NOT NULL,
   [Path] nvarchar(max)  NOT NULL,
   [Name] nvarchar(255)  NOT NULL,
   [User_ID] int  NOT NULL
)
GO
GO
IF  EXISTS (SELECT * FROM sys.objects so JOIN sys.schemas sc ON so.schema_id = sc.schema_id WHERE so.name = N'Task_User'  AND sc.name=N'dbo'  AND type in (N'U'))
BEGIN

  DECLARE @drop_statement nvarchar(500)

  DECLARE drop_cursor CURSOR FOR
      SELECT 'alter table '+quotename(schema_name(ob.schema_id))+
      '.'+quotename(object_name(ob.object_id))+ ' drop constraint ' + quotename(fk.name) 
      FROM sys.objects ob INNER JOIN sys.foreign_keys fk ON fk.parent_object_id = ob.object_id
      WHERE fk.referenced_object_id = 
          (
             SELECT so.object_id 
             FROM sys.objects so JOIN sys.schemas sc
             ON so.schema_id = sc.schema_id
             WHERE so.name = N'Task_User'  AND sc.name=N'dbo'  AND type in (N'U')
           )

  OPEN drop_cursor

  FETCH NEXT FROM drop_cursor
  INTO @drop_statement

  WHILE @@FETCH_STATUS = 0
  BEGIN
     EXEC (@drop_statement)

     FETCH NEXT FROM drop_cursor
     INTO @drop_statement
  END

  CLOSE drop_cursor
  DEALLOCATE drop_cursor

  DROP TABLE [dbo].[Task_User]
END 
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE 
[dbo].[Task_User]
(
   [Task_ID] int  NOT NULL,
   [User_ID] int  NOT NULL
)
GO
GO
IF  EXISTS (SELECT * FROM sys.objects so JOIN sys.schemas sc ON so.schema_id = sc.schema_id WHERE so.name = N'Users'  AND sc.name=N'dbo'  AND type in (N'U'))
BEGIN

  DECLARE @drop_statement nvarchar(500)

  DECLARE drop_cursor CURSOR FOR
      SELECT 'alter table '+quotename(schema_name(ob.schema_id))+
      '.'+quotename(object_name(ob.object_id))+ ' drop constraint ' + quotename(fk.name) 
      FROM sys.objects ob INNER JOIN sys.foreign_keys fk ON fk.parent_object_id = ob.object_id
      WHERE fk.referenced_object_id = 
          (
             SELECT so.object_id 
             FROM sys.objects so JOIN sys.schemas sc
             ON so.schema_id = sc.schema_id
             WHERE so.name = N'Users'  AND sc.name=N'dbo'  AND type in (N'U')
           )

  OPEN drop_cursor

  FETCH NEXT FROM drop_cursor
  INTO @drop_statement

  WHILE @@FETCH_STATUS = 0
  BEGIN
     EXEC (@drop_statement)

     FETCH NEXT FROM drop_cursor
     INTO @drop_statement
  END

  CLOSE drop_cursor
  DEALLOCATE drop_cursor

  DROP TABLE [dbo].[Users]
END 
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE 
[dbo].[Users]
(
   [User_ID] int IDENTITY(1, 1)  NOT NULL,
   [Username] nvarchar(255)  NOT NULL,
   [Password] nvarchar(255)  NULL,
   [Name] nvarchar(255)  NULL,
   [Role] int  NULL,
   [Avatar] nvarchar(max)  NULL
)
GO
GO
IF EXISTS (SELECT * FROM sys.objects so JOIN sys.schemas sc ON so.schema_id = sc.schema_id WHERE so.name = N'Backlog$PrimaryKey'  AND sc.name=N'dbo'  AND type in (N'PK'))
ALTER TABLE [dbo].[Backlog] DROP CONSTRAINT [Backlog$PrimaryKey]
 GO



ALTER TABLE [dbo].[Backlog]
 ADD CONSTRAINT [Backlog$PrimaryKey]
 PRIMARY KEY 
   CLUSTERED ([Backlog_ID] ASC)

GO

IF EXISTS (SELECT * FROM sys.objects so JOIN sys.schemas sc ON so.schema_id = sc.schema_id WHERE so.name = N'Backlog_User$PrimaryKey'  AND sc.name=N'dbo'  AND type in (N'PK'))
ALTER TABLE [dbo].[Backlog_User] DROP CONSTRAINT [Backlog_User$PrimaryKey]
 GO



ALTER TABLE [dbo].[Backlog_User]
 ADD CONSTRAINT [Backlog_User$PrimaryKey]
 PRIMARY KEY 
   CLUSTERED ([Backlog_ID] ASC, [User_ID] ASC)

GO

IF EXISTS (SELECT * FROM sys.objects so JOIN sys.schemas sc ON so.schema_id = sc.schema_id WHERE so.name = N'Project$PrimaryKey'  AND sc.name=N'dbo'  AND type in (N'PK'))
ALTER TABLE [dbo].[Project] DROP CONSTRAINT [Project$PrimaryKey]
 GO



ALTER TABLE [dbo].[Project]
 ADD CONSTRAINT [Project$PrimaryKey]
 PRIMARY KEY 
   CLUSTERED ([Project_ID] ASC)

GO

IF EXISTS (SELECT * FROM sys.objects so JOIN sys.schemas sc ON so.schema_id = sc.schema_id WHERE so.name = N'PrimaryKey_652400ab-b301-41fe-81e5-bb74ec025f10'  AND sc.name=N'dbo'  AND type in (N'PK'))
ALTER TABLE [dbo].[Project_Supervisor] DROP CONSTRAINT [PrimaryKey_652400ab-b301-41fe-81e5-bb74ec025f10]
 GO



ALTER TABLE [dbo].[Project_Supervisor]
 ADD CONSTRAINT [PrimaryKey_652400ab-b301-41fe-81e5-bb74ec025f10]
 PRIMARY KEY 
   CLUSTERED ([Project_ID] ASC, [User_ID] ASC)

GO

IF EXISTS (SELECT * FROM sys.objects so JOIN sys.schemas sc ON so.schema_id = sc.schema_id WHERE so.name = N'Project_User$PrimaryKey'  AND sc.name=N'dbo'  AND type in (N'PK'))
ALTER TABLE [dbo].[Project_User] DROP CONSTRAINT [Project_User$PrimaryKey]
 GO



ALTER TABLE [dbo].[Project_User]
 ADD CONSTRAINT [Project_User$PrimaryKey]
 PRIMARY KEY 
   CLUSTERED ([Project_ID] ASC, [User_ID] ASC)

GO

IF EXISTS (SELECT * FROM sys.objects so JOIN sys.schemas sc ON so.schema_id = sc.schema_id WHERE so.name = N'Swimlane$PrimaryKey'  AND sc.name=N'dbo'  AND type in (N'PK'))
ALTER TABLE [dbo].[Swimlane] DROP CONSTRAINT [Swimlane$PrimaryKey]
 GO



ALTER TABLE [dbo].[Swimlane]
 ADD CONSTRAINT [Swimlane$PrimaryKey]
 PRIMARY KEY 
   CLUSTERED ([Swimlane_ID] ASC)

GO

IF EXISTS (SELECT * FROM sys.objects so JOIN sys.schemas sc ON so.schema_id = sc.schema_id WHERE so.name = N'Task$PrimaryKey'  AND sc.name=N'dbo'  AND type in (N'PK'))
ALTER TABLE [dbo].[Task] DROP CONSTRAINT [Task$PrimaryKey]
 GO



ALTER TABLE [dbo].[Task]
 ADD CONSTRAINT [Task$PrimaryKey]
 PRIMARY KEY 
   CLUSTERED ([Task_ID] ASC)

GO

IF EXISTS (SELECT * FROM sys.objects so JOIN sys.schemas sc ON so.schema_id = sc.schema_id WHERE so.name = N'PrimaryKey_4492f9fe-dfae-4365-a860-5463b031c9e1'  AND sc.name=N'dbo'  AND type in (N'PK'))
ALTER TABLE [dbo].[Task_Comment] DROP CONSTRAINT [PrimaryKey_4492f9fe-dfae-4365-a860-5463b031c9e1]
 GO



ALTER TABLE [dbo].[Task_Comment]
 ADD CONSTRAINT [PrimaryKey_4492f9fe-dfae-4365-a860-5463b031c9e1]
 PRIMARY KEY 
   CLUSTERED ([Comment_ID] ASC)

GO

IF EXISTS (SELECT * FROM sys.objects so JOIN sys.schemas sc ON so.schema_id = sc.schema_id WHERE so.name = N'PrimaryKey_5ba6c526-049d-4913-8b55-953a9d355751'  AND sc.name=N'dbo'  AND type in (N'PK'))
ALTER TABLE [dbo].[Task_File] DROP CONSTRAINT [PrimaryKey_5ba6c526-049d-4913-8b55-953a9d355751]
 GO



ALTER TABLE [dbo].[Task_File]
 ADD CONSTRAINT [PrimaryKey_5ba6c526-049d-4913-8b55-953a9d355751]
 PRIMARY KEY 
   CLUSTERED ([File_ID] ASC)

GO

IF EXISTS (SELECT * FROM sys.objects so JOIN sys.schemas sc ON so.schema_id = sc.schema_id WHERE so.name = N'Task_User$PrimaryKey'  AND sc.name=N'dbo'  AND type in (N'PK'))
ALTER TABLE [dbo].[Task_User] DROP CONSTRAINT [Task_User$PrimaryKey]
 GO



ALTER TABLE [dbo].[Task_User]
 ADD CONSTRAINT [Task_User$PrimaryKey]
 PRIMARY KEY 
   CLUSTERED ([Task_ID] ASC, [User_ID] ASC)

GO

IF EXISTS (SELECT * FROM sys.objects so JOIN sys.schemas sc ON so.schema_id = sc.schema_id WHERE so.name = N'Users$PrimaryKey'  AND sc.name=N'dbo'  AND type in (N'PK'))
ALTER TABLE [dbo].[Users] DROP CONSTRAINT [Users$PrimaryKey]
 GO



ALTER TABLE [dbo].[Users]
 ADD CONSTRAINT [Users$PrimaryKey]
 PRIMARY KEY 
   CLUSTERED ([User_ID] ASC)

GO

IF  EXISTS (
       SELECT * FROM sys.objects  so JOIN sys.indexes si
       ON so.object_id = si.object_id
       JOIN sys.schemas sc
       ON so.schema_id = sc.schema_id
       WHERE so.name = N'Backlog'  AND sc.name = N'dbo'  AND si.name = N'Backlog$Project_ID' AND so.type in (N'U'))
   DROP INDEX [Backlog$Project_ID] ON [dbo].[Backlog] 
GO
CREATE NONCLUSTERED INDEX [Backlog$Project_ID] ON [dbo].[Backlog]
(
   [Project_ID] ASC
)
WITH (DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF)
GO
GO
IF  EXISTS (
       SELECT * FROM sys.objects  so JOIN sys.indexes si
       ON so.object_id = si.object_id
       JOIN sys.schemas sc
       ON so.schema_id = sc.schema_id
       WHERE so.name = N'Backlog'  AND sc.name = N'dbo'  AND si.name = N'Backlog$Relative_ID' AND so.type in (N'U'))
   DROP INDEX [Backlog$Relative_ID] ON [dbo].[Backlog] 
GO
CREATE NONCLUSTERED INDEX [Backlog$Relative_ID] ON [dbo].[Backlog]
(
   [Relative_ID] ASC
)
WITH (DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF)
GO
GO
IF  EXISTS (
       SELECT * FROM sys.objects  so JOIN sys.indexes si
       ON so.object_id = si.object_id
       JOIN sys.schemas sc
       ON so.schema_id = sc.schema_id
       WHERE so.name = N'Backlog'  AND sc.name = N'dbo'  AND si.name = N'Backlog$Swimlane_iD' AND so.type in (N'U'))
   DROP INDEX [Backlog$Swimlane_iD] ON [dbo].[Backlog] 
GO
CREATE NONCLUSTERED INDEX [Backlog$Swimlane_iD] ON [dbo].[Backlog]
(
   [Swimlane_ID] ASC
)
WITH (DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF)
GO
GO
IF  EXISTS (
       SELECT * FROM sys.objects  so JOIN sys.indexes si
       ON so.object_id = si.object_id
       JOIN sys.schemas sc
       ON so.schema_id = sc.schema_id
       WHERE so.name = N'Backlog_User'  AND sc.name = N'dbo'  AND si.name = N'Backlog_User$User_ID' AND so.type in (N'U'))
   DROP INDEX [Backlog_User$User_ID] ON [dbo].[Backlog_User] 
GO
CREATE NONCLUSTERED INDEX [Backlog_User$User_ID] ON [dbo].[Backlog_User]
(
   [User_ID] ASC
)
WITH (DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF)
GO
GO
IF  EXISTS (
       SELECT * FROM sys.objects  so JOIN sys.indexes si
       ON so.object_id = si.object_id
       JOIN sys.schemas sc
       ON so.schema_id = sc.schema_id
       WHERE so.name = N'Project_User'  AND sc.name = N'dbo'  AND si.name = N'Project_User$User_ID' AND so.type in (N'U'))
   DROP INDEX [Project_User$User_ID] ON [dbo].[Project_User] 
GO
CREATE NONCLUSTERED INDEX [Project_User$User_ID] ON [dbo].[Project_User]
(
   [User_ID] ASC
)
WITH (DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF)
GO
GO
IF  EXISTS (
       SELECT * FROM sys.objects  so JOIN sys.indexes si
       ON so.object_id = si.object_id
       JOIN sys.schemas sc
       ON so.schema_id = sc.schema_id
       WHERE so.name = N'Swimlane'  AND sc.name = N'dbo'  AND si.name = N'Swimlane$Project_ID' AND so.type in (N'U'))
   DROP INDEX [Swimlane$Project_ID] ON [dbo].[Swimlane] 
GO
CREATE NONCLUSTERED INDEX [Swimlane$Project_ID] ON [dbo].[Swimlane]
(
   [Project_ID] ASC
)
WITH (DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF)
GO
GO
IF  EXISTS (
       SELECT * FROM sys.objects  so JOIN sys.indexes si
       ON so.object_id = si.object_id
       JOIN sys.schemas sc
       ON so.schema_id = sc.schema_id
       WHERE so.name = N'Task'  AND sc.name = N'dbo'  AND si.name = N'Task$Backlog_Id' AND so.type in (N'U'))
   DROP INDEX [Task$Backlog_Id] ON [dbo].[Task] 
GO
CREATE NONCLUSTERED INDEX [Task$Backlog_Id] ON [dbo].[Task]
(
   [Backlog_ID] ASC
)
WITH (DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF)
GO
GO
IF  EXISTS (
       SELECT * FROM sys.objects  so JOIN sys.indexes si
       ON so.object_id = si.object_id
       JOIN sys.schemas sc
       ON so.schema_id = sc.schema_id
       WHERE so.name = N'Task'  AND sc.name = N'dbo'  AND si.name = N'Task$Position' AND so.type in (N'U'))
   DROP INDEX [Task$Position] ON [dbo].[Task] 
GO
CREATE NONCLUSTERED INDEX [Task$Position] ON [dbo].[Task]
(
   [Position] ASC
)
WITH (DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF)
GO
GO
IF  EXISTS (
       SELECT * FROM sys.objects  so JOIN sys.indexes si
       ON so.object_id = si.object_id
       JOIN sys.schemas sc
       ON so.schema_id = sc.schema_id
       WHERE so.name = N'Task'  AND sc.name = N'dbo'  AND si.name = N'Task$Project_ID' AND so.type in (N'U'))
   DROP INDEX [Task$Project_ID] ON [dbo].[Task] 
GO
CREATE NONCLUSTERED INDEX [Task$Project_ID] ON [dbo].[Task]
(
   [Project_ID] ASC
)
WITH (DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF)
GO
GO
IF  EXISTS (
       SELECT * FROM sys.objects  so JOIN sys.indexes si
       ON so.object_id = si.object_id
       JOIN sys.schemas sc
       ON so.schema_id = sc.schema_id
       WHERE so.name = N'Task'  AND sc.name = N'dbo'  AND si.name = N'Task$Swimlane_ID' AND so.type in (N'U'))
   DROP INDEX [Task$Swimlane_ID] ON [dbo].[Task] 
GO
CREATE NONCLUSTERED INDEX [Task$Swimlane_ID] ON [dbo].[Task]
(
   [Swimlane_ID] ASC
)
WITH (DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF)
GO
GO
IF  EXISTS (
       SELECT * FROM sys.objects  so JOIN sys.indexes si
       ON so.object_id = si.object_id
       JOIN sys.schemas sc
       ON so.schema_id = sc.schema_id
       WHERE so.name = N'Task_Comment'  AND sc.name = N'dbo'  AND si.name = N'Task_Comment$Task_ID' AND so.type in (N'U'))
   DROP INDEX [Task_Comment$Task_ID] ON [dbo].[Task_Comment] 
GO
CREATE NONCLUSTERED INDEX [Task_Comment$Task_ID] ON [dbo].[Task_Comment]
(
   [Task_ID] ASC
)
WITH (DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF)
GO
GO
IF  EXISTS (
       SELECT * FROM sys.objects  so JOIN sys.indexes si
       ON so.object_id = si.object_id
       JOIN sys.schemas sc
       ON so.schema_id = sc.schema_id
       WHERE so.name = N'Task_File'  AND sc.name = N'dbo'  AND si.name = N'Task_File$Task_ID' AND so.type in (N'U'))
   DROP INDEX [Task_File$Task_ID] ON [dbo].[Task_File] 
GO
CREATE NONCLUSTERED INDEX [Task_File$Task_ID] ON [dbo].[Task_File]
(
   [Task_ID] ASC
)
WITH (DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF)
GO
GO
IF  EXISTS (
       SELECT * FROM sys.objects  so JOIN sys.indexes si
       ON so.object_id = si.object_id
       JOIN sys.schemas sc
       ON so.schema_id = sc.schema_id
       WHERE so.name = N'Task_User'  AND sc.name = N'dbo'  AND si.name = N'Task_User$Task_iD' AND so.type in (N'U'))
   DROP INDEX [Task_User$Task_iD] ON [dbo].[Task_User] 
GO
CREATE NONCLUSTERED INDEX [Task_User$Task_iD] ON [dbo].[Task_User]
(
   [Task_ID] ASC
)
WITH (DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF)
GO
GO
IF EXISTS (SELECT * FROM sys.objects so JOIN sys.schemas sc ON so.schema_id = sc.schema_id WHERE so.name = N'FK_Backlog_0'  AND sc.name=N'dbo'  AND type in (N'F'))
ALTER TABLE [dbo].[Backlog] DROP CONSTRAINT [FK_Backlog_0]
 GO



ALTER TABLE [dbo].[Backlog]
 ADD CONSTRAINT [FK_Backlog_0]
 FOREIGN KEY 
   ([Project_ID])
 REFERENCES 
   [dbo].[Project]     ([Project_ID])
    ON DELETE CASCADE
    ON UPDATE NO ACTION

GO

IF EXISTS (SELECT * FROM sys.objects so JOIN sys.schemas sc ON so.schema_id = sc.schema_id WHERE so.name = N'FK_Backlog_1'  AND sc.name=N'dbo'  AND type in (N'F'))
ALTER TABLE [dbo].[Backlog] DROP CONSTRAINT [FK_Backlog_1]
 GO



ALTER TABLE [dbo].[Backlog]
 ADD CONSTRAINT [FK_Backlog_1]
 FOREIGN KEY 
   ([Swimlane_ID])
 REFERENCES 
   [dbo].[Swimlane]     ([Swimlane_ID])
    ON DELETE NO ACTION
    ON UPDATE NO ACTION

GO

IF EXISTS (SELECT * FROM sys.objects so JOIN sys.schemas sc ON so.schema_id = sc.schema_id WHERE so.name = N'FK_Backlog_User_1'  AND sc.name=N'dbo'  AND type in (N'F'))
ALTER TABLE [dbo].[Backlog_User] DROP CONSTRAINT [FK_Backlog_User_1]
 GO



ALTER TABLE [dbo].[Backlog_User]
 ADD CONSTRAINT [FK_Backlog_User_1]
 FOREIGN KEY 
   ([User_ID])
 REFERENCES 
   [dbo].[Users]     ([User_ID])
    ON DELETE NO ACTION
    ON UPDATE NO ACTION

GO

IF EXISTS (SELECT * FROM sys.objects so JOIN sys.schemas sc ON so.schema_id = sc.schema_id WHERE so.name = N'FK_Project_0'  AND sc.name=N'dbo'  AND type in (N'F'))
ALTER TABLE [dbo].[Project] DROP CONSTRAINT [FK_Project_0]
 GO



ALTER TABLE [dbo].[Project]
 ADD CONSTRAINT [FK_Project_0]
 FOREIGN KEY 
   ([Owner])
 REFERENCES 
   [dbo].[Users]     ([User_ID])
    ON DELETE NO ACTION
    ON UPDATE NO ACTION

GO

IF EXISTS (SELECT * FROM sys.objects so JOIN sys.schemas sc ON so.schema_id = sc.schema_id WHERE so.name = N'FK_Project_Supervisor_0'  AND sc.name=N'dbo'  AND type in (N'F'))
ALTER TABLE [dbo].[Project_Supervisor] DROP CONSTRAINT [FK_Project_Supervisor_0]
 GO



ALTER TABLE [dbo].[Project_Supervisor]
 ADD CONSTRAINT [FK_Project_Supervisor_0]
 FOREIGN KEY 
   ([Project_ID])
 REFERENCES 
   [dbo].[Project]     ([Project_ID])
    ON DELETE CASCADE
    ON UPDATE NO ACTION

GO

IF EXISTS (SELECT * FROM sys.objects so JOIN sys.schemas sc ON so.schema_id = sc.schema_id WHERE so.name = N'FK_Project_Supervisor_1'  AND sc.name=N'dbo'  AND type in (N'F'))
ALTER TABLE [dbo].[Project_Supervisor] DROP CONSTRAINT [FK_Project_Supervisor_1]
 GO



ALTER TABLE [dbo].[Project_Supervisor]
 ADD CONSTRAINT [FK_Project_Supervisor_1]
 FOREIGN KEY 
   ([User_ID])
 REFERENCES 
   [dbo].[Users]     ([User_ID])
    ON DELETE NO ACTION
    ON UPDATE NO ACTION

GO

IF EXISTS (SELECT * FROM sys.objects so JOIN sys.schemas sc ON so.schema_id = sc.schema_id WHERE so.name = N'FK_Project_User_1'  AND sc.name=N'dbo'  AND type in (N'F'))
ALTER TABLE [dbo].[Project_User] DROP CONSTRAINT [FK_Project_User_1]
 GO



ALTER TABLE [dbo].[Project_User]
 ADD CONSTRAINT [FK_Project_User_1]
 FOREIGN KEY 
   ([Project_ID])
 REFERENCES 
   [dbo].[Project]     ([Project_ID])
    ON DELETE CASCADE
    ON UPDATE NO ACTION

GO

IF EXISTS (SELECT * FROM sys.objects so JOIN sys.schemas sc ON so.schema_id = sc.schema_id WHERE so.name = N'FK_Project_User_0'  AND sc.name=N'dbo'  AND type in (N'F'))
ALTER TABLE [dbo].[Project_User] DROP CONSTRAINT [FK_Project_User_0]
 GO



ALTER TABLE [dbo].[Project_User]
 ADD CONSTRAINT [FK_Project_User_0]
 FOREIGN KEY 
   ([User_ID])
 REFERENCES 
   [dbo].[Users]     ([User_ID])
    ON DELETE CASCADE
    ON UPDATE NO ACTION

GO

IF EXISTS (SELECT * FROM sys.objects so JOIN sys.schemas sc ON so.schema_id = sc.schema_id WHERE so.name = N'FK_Swimlane_0'  AND sc.name=N'dbo'  AND type in (N'F'))
ALTER TABLE [dbo].[Swimlane] DROP CONSTRAINT [FK_Swimlane_0]
 GO



ALTER TABLE [dbo].[Swimlane]
 ADD CONSTRAINT [FK_Swimlane_0]
 FOREIGN KEY 
   ([Project_ID])
 REFERENCES 
   [dbo].[Project]     ([Project_ID])
    ON DELETE CASCADE
    ON UPDATE NO ACTION

GO

IF EXISTS (SELECT * FROM sys.objects so JOIN sys.schemas sc ON so.schema_id = sc.schema_id WHERE so.name = N'FK_Task_2'  AND sc.name=N'dbo'  AND type in (N'F'))
ALTER TABLE [dbo].[Task] DROP CONSTRAINT [FK_Task_2]
 GO



ALTER TABLE [dbo].[Task]
 ADD CONSTRAINT [FK_Task_2]
 FOREIGN KEY 
   ([Project_ID])
 REFERENCES 
   [dbo].[Project]     ([Project_ID])
    ON DELETE NO ACTION
    ON UPDATE NO ACTION

GO

IF EXISTS (SELECT * FROM sys.objects so JOIN sys.schemas sc ON so.schema_id = sc.schema_id WHERE so.name = N'FK_Task_1'  AND sc.name=N'dbo'  AND type in (N'F'))
ALTER TABLE [dbo].[Task] DROP CONSTRAINT [FK_Task_1]
 GO



ALTER TABLE [dbo].[Task]
 ADD CONSTRAINT [FK_Task_1]
 FOREIGN KEY 
   ([Swimlane_ID])
 REFERENCES 
   [dbo].[Swimlane]     ([Swimlane_ID])
    ON DELETE NO ACTION
    ON UPDATE CASCADE

GO

IF EXISTS (SELECT * FROM sys.objects so JOIN sys.schemas sc ON so.schema_id = sc.schema_id WHERE so.name = N'FK_Task_Comment_1'  AND sc.name=N'dbo'  AND type in (N'F'))
ALTER TABLE [dbo].[Task_Comment] DROP CONSTRAINT [FK_Task_Comment_1]
 GO



ALTER TABLE [dbo].[Task_Comment]
 ADD CONSTRAINT [FK_Task_Comment_1]
 FOREIGN KEY 
   ([User_ID])
 REFERENCES 
   [dbo].[Users]     ([User_ID])
    ON DELETE NO ACTION
    ON UPDATE NO ACTION

GO

IF EXISTS (SELECT * FROM sys.objects so JOIN sys.schemas sc ON so.schema_id = sc.schema_id WHERE so.name = N'FK_Task_Comment_0'  AND sc.name=N'dbo'  AND type in (N'F'))
ALTER TABLE [dbo].[Task_Comment] DROP CONSTRAINT [FK_Task_Comment_0]
 GO



ALTER TABLE [dbo].[Task_Comment]
 ADD CONSTRAINT [FK_Task_Comment_0]
 FOREIGN KEY 
   ([Task_ID])
 REFERENCES 
   [dbo].[Task]     ([Task_ID])
    ON DELETE CASCADE
    ON UPDATE NO ACTION

GO

IF EXISTS (SELECT * FROM sys.objects so JOIN sys.schemas sc ON so.schema_id = sc.schema_id WHERE so.name = N'FK_Task_File_1'  AND sc.name=N'dbo'  AND type in (N'F'))
ALTER TABLE [dbo].[Task_File] DROP CONSTRAINT [FK_Task_File_1]
 GO



ALTER TABLE [dbo].[Task_File]
 ADD CONSTRAINT [FK_Task_File_1]
 FOREIGN KEY 
   ([User_ID])
 REFERENCES 
   [dbo].[Users]     ([User_ID])
    ON DELETE NO ACTION
    ON UPDATE NO ACTION

GO

IF EXISTS (SELECT * FROM sys.objects so JOIN sys.schemas sc ON so.schema_id = sc.schema_id WHERE so.name = N'FK_Task_File_0'  AND sc.name=N'dbo'  AND type in (N'F'))
ALTER TABLE [dbo].[Task_File] DROP CONSTRAINT [FK_Task_File_0]
 GO



ALTER TABLE [dbo].[Task_File]
 ADD CONSTRAINT [FK_Task_File_0]
 FOREIGN KEY 
   ([Task_ID])
 REFERENCES 
   [dbo].[Task]     ([Task_ID])
    ON DELETE CASCADE
    ON UPDATE NO ACTION

GO

IF EXISTS (SELECT * FROM sys.objects so JOIN sys.schemas sc ON so.schema_id = sc.schema_id WHERE so.name = N'FK_Task_User_1'  AND sc.name=N'dbo'  AND type in (N'F'))
ALTER TABLE [dbo].[Task_User] DROP CONSTRAINT [FK_Task_User_1]
 GO



ALTER TABLE [dbo].[Task_User]
 ADD CONSTRAINT [FK_Task_User_1]
 FOREIGN KEY 
   ([User_ID])
 REFERENCES 
   [dbo].[Users]     ([User_ID])
    ON DELETE NO ACTION
    ON UPDATE NO ACTION

GO

IF EXISTS (SELECT * FROM sys.objects so JOIN sys.schemas sc ON so.schema_id = sc.schema_id WHERE so.name = N'FK_Task_User_0'  AND sc.name=N'dbo'  AND type in (N'F'))
ALTER TABLE [dbo].[Task_User] DROP CONSTRAINT [FK_Task_User_0]
 GO



ALTER TABLE [dbo].[Task_User]
 ADD CONSTRAINT [FK_Task_User_0]
 FOREIGN KEY 
   ([Task_ID])
 REFERENCES 
   [dbo].[Task]     ([Task_ID])
    ON DELETE CASCADE
    ON UPDATE NO ACTION

GO

IF  EXISTS (SELECT * FROM sys.objects so JOIN sys.schemas sc on so.schema_id = sc.schema_id WHERE so.name = N'addOwnerAsUser'  AND sc.name=N'dbo'  AND type in (N'TR'))
 DROP TRIGGER [dbo].[addOwnerAsUser]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER addOwnerAsUser ON [dbo].[Project] 
FOR INSERT
AS
DECLARE @projectID int;
DECLARE @userID int;

SELECT @projectID = Project_ID FROM Inserted;
SELECT @userID = Owner FROM Inserted;

INSERT INTO [dbo].[Project_User] (Project_ID, User_ID) VALUES (@projectID, @userID);


GO
GO
IF  EXISTS (SELECT * FROM sys.objects so JOIN sys.schemas sc on so.schema_id = sc.schema_id WHERE so.name = N'createSwimlanesForProject'  AND sc.name=N'dbo'  AND type in (N'TR'))
 DROP TRIGGER [dbo].[createSwimlanesForProject]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER createSwimlanesForProject ON [dbo].[Project] 
FOR INSERT
AS
DECLARE @projectID int;

SELECT @projectID = Project_ID FROM Inserted;

INSERT INTO [dbo].[Swimlane] (Project_ID, Name, Type, Data_status, Position) 
VALUES 
(@projectID, 'Product Backlog', 1, 'Standby', 0),
(@projectID, 'Sprint Backlog', 1, 'Ongoing', 1),
(@projectID, 'To-Do', 2, 'Standby', 2),
(@projectID, 'Work in Progress', 2, 'Ongoing', 3),
(@projectID, 'Done', 3, 'Done', 4);

GO
GO
