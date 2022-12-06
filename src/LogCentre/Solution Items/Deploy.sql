
drop table [Log].[Line];
drop table [Log].[File];
drop table [dbo].[LogSource];
drop table [dbo].[Provider];
drop table [dbo].[Host];
drop schema [Log];


if not exists (select 1 from sys.schemas where name = 'Log')
begin
	print N'Adding schema: Log'
	exec ('create schema [Log] authorization [dbo];')
end
else
begin
	print N'Schema already exists: Log'
end

if not exists (select 1 from information_schema.tables where table_name = 'Host' and table_schema = 'dbo')
begin
	print N'Add table: Host'
	create table [dbo].[Host] (
		[Id] bigint not null primary key identity,
		[Name] nvarchar(100) not null,
		[Description] nvarchar(1000) not null default N'',
		[Active] nvarchar(1) not null default N'Y',
		[Deleted] nvarchar(1) not null default N'N',
		[LastUpdatedBy] nvarchar(256) not null,
		[RowVersion] datetime not null default getutcdate(),
		constraint CK_Host_Active_YesNo check ([Active] in (N'Y', N'N')),
		constraint CK_Host_Deleted_YesNo check ([Deleted] in (N'Y', N'N')),
	)
end
else
begin
	print N'Table exists: Host'
end

if not exists (select 1 from information_schema.tables where table_name = 'Provider' and table_schema = 'dbo')
begin
	print N'Add table: Provider'
	create table [dbo].[Provider] (
		[Id] bigint not null primary key identity,
		[Name] nvarchar(100) not null,
		[Description] nvarchar(1000) not null default N'',
		[Regex] nvarchar(1000) not null default N'',
		[Active] nvarchar(1) not null default N'Y',
		[Deleted] nvarchar(1) not null default N'N',
		[LastUpdatedBy] nvarchar(256) not null,
		[RowVersion] datetime not null default getutcdate(),
		constraint CK_Provider_Active_YesNo check ([Active] in (N'Y', N'N')),
		constraint CK_Provider_Deleted_YesNo check ([Deleted] in (N'Y', N'N')),
	)
end
else
begin
	print N'Table exists: Provider'
end

if not exists (select 1 from information_schema.tables where table_name = 'LogSource' and table_schema = 'dbo')
begin
	print N'Add table: LogSource'
	create table [dbo].[LogSource] (
		[Id] bigint not null primary key identity,
		[HostId] bigint not null,
		[ProviderId] bigint not null,
		[Name] nvarchar(100) not null,
		[Path] nvarchar(500) not null default N'',
		[Active] nvarchar(1) not null default N'Y',
		[Deleted] nvarchar(1) not null default N'N',
		[LastUpdatedBy] nvarchar(256) not null,
		[RowVersion] datetime not null default getutcdate(),
		constraint CK_LogSource_Active_YesNo check ([Active] in (N'Y', N'N')),
		constraint CK_LogSource_Deleted_YesNo check ([Deleted] in (N'Y', N'N')),
		foreign key ([HostId]) references [dbo].[Host]([Id]),
		foreign key ([ProviderId]) references [dbo].[Provider]([Id])
	)

	create index [IX_LogSource_HostId] on [dbo].[LogSource]([HostId])
	create index [IX_LogSource_ProviderId] on [dbo].[LogSource]([ProviderId])
end
else
begin
	print N'Table exists: Provider'
end

if not exists (select 1 from information_schema.tables where table_name = 'File' and table_schema = 'Log')
begin
	print N'Add table: Log File'
	create table [Log].[File] (
		[Id] bigint not null primary key identity,
		[LogSourceId] bigint not null,
		[Name] nvarchar(100) not null,
		[FileComplete] nvarchar(1) not null default N'N',
		[Active] nvarchar(1) not null default N'Y',
		[Deleted] nvarchar(1) not null default N'N',
		[LastUpdatedBy] nvarchar(256) not null,
		[RowVersion] datetime not null default getutcdate(),
		constraint CK_LogFile_FileComplete_YesNo check ([FileComplete] in (N'Y', N'N')),
		constraint CK_LogFile_Active_YesNo check ([Active] in (N'Y', N'N')),
		constraint CK_LogFile_Deleted_YesNo check ([Deleted] in (N'Y', N'N')),
		foreign key ([LogSourceId]) references [dbo].[LogSource]([Id])
	)

	create index [IX_LogFile_LogSourceID] on [Log].[File]([LogSourceId])
end
else
begin
	print N'Table exists: Log.File'
end

if not exists (select 1 from information_schema.tables where table_name = 'Line' and table_schema = 'Log')
begin
	print N'Add table: Log.Line'
	create table [Log].[Line] (
		[Id] bigint not null primary key identity,
		[FileId] bigint not null,
		[LogLine] nvarchar(4000) not null,
		[Grouping] uniqueidentifier not null,
		[Active] nvarchar(1) not null default N'Y',
		[Deleted] nvarchar(1) not null default N'N',
		[LastUpdatedBy] nvarchar(256) not null,
		[RowVersion] datetime not null default getutcdate(),
		constraint CK_LogLine_Active_YesNo check ([Active] in (N'Y', N'N')),
		constraint CK_LogLine_Deleted_YesNo check ([Deleted] in (N'Y', N'N')),
		foreign key ([FileId]) references [Log].[File]([Id])
	)

	create index [IX_LogLine_FileId] on [Log].[Line]([FileId])
end
else
begin
	print N'Table exists: Log.Line'
end

go