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

go