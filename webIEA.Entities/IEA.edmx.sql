
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 08/04/2022 13:15:48
-- Generated from EDMX file: D:\Projects\DevTeam\Benipal\Projects\webIEA\webIEA.Entities\IEA.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [webIEA];
GO
IF SCHEMA_ID(N'IEA') IS NULL EXECUTE(N'CREATE SCHEMA [IEA]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[IEA].[FK_MemberDocuments_MemberDocuments]', 'F') IS NOT NULL
    ALTER TABLE [IEA].[MemberDocuments] DROP CONSTRAINT [FK_MemberDocuments_MemberDocuments];
GO
IF OBJECT_ID(N'[IEA].[FK_Members_MemberStatus]', 'F') IS NOT NULL
    ALTER TABLE [IEA].[MemberProfile] DROP CONSTRAINT [FK_Members_MemberStatus];
GO
IF OBJECT_ID(N'[IEA].[FK_MemberSpecifications_Members]', 'F') IS NOT NULL
    ALTER TABLE [IEA].[MemberSpecializations] DROP CONSTRAINT [FK_MemberSpecifications_Members];
GO
IF OBJECT_ID(N'[IEA].[FK_MemberTranieeCommission_Members]', 'F') IS NOT NULL
    ALTER TABLE [IEA].[MemberTranieeCommission] DROP CONSTRAINT [FK_MemberTranieeCommission_Members];
GO
IF OBJECT_ID(N'[IEA].[FK_MemberTranieeCommission_TrainingCourses]', 'F') IS NOT NULL
    ALTER TABLE [IEA].[MemberTranieeCommission] DROP CONSTRAINT [FK_MemberTranieeCommission_TrainingCourses];
GO
IF OBJECT_ID(N'[IEA].[FK_TrainingCourses_CourseType]', 'F') IS NOT NULL
    ALTER TABLE [IEA].[TrainingCourses] DROP CONSTRAINT [FK_TrainingCourses_CourseType];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[IEA].[CourseType]', 'U') IS NOT NULL
    DROP TABLE [IEA].[CourseType];
GO
IF OBJECT_ID(N'[IEA].[HistoryDataChanges]', 'U') IS NOT NULL
    DROP TABLE [IEA].[HistoryDataChanges];
GO
IF OBJECT_ID(N'[IEA].[MemberDocuments]', 'U') IS NOT NULL
    DROP TABLE [IEA].[MemberDocuments];
GO
IF OBJECT_ID(N'[IEA].[MemberEmploymentStatus]', 'U') IS NOT NULL
    DROP TABLE [IEA].[MemberEmploymentStatus];
GO
IF OBJECT_ID(N'[IEA].[MemberProfile]', 'U') IS NOT NULL
    DROP TABLE [IEA].[MemberProfile];
GO
IF OBJECT_ID(N'[IEA].[MemberSpecializations]', 'U') IS NOT NULL
    DROP TABLE [IEA].[MemberSpecializations];
GO
IF OBJECT_ID(N'[IEA].[MemberStatus]', 'U') IS NOT NULL
    DROP TABLE [IEA].[MemberStatus];
GO
IF OBJECT_ID(N'[IEA].[MemberTranieeCommission]', 'U') IS NOT NULL
    DROP TABLE [IEA].[MemberTranieeCommission];
GO
IF OBJECT_ID(N'[IEA].[TrainingCourses]', 'U') IS NOT NULL
    DROP TABLE [IEA].[TrainingCourses];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'CourseTypes'
CREATE TABLE [IEA].[CourseTypes] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(100)  NULL
);
GO

-- Creating table 'HistoryDataChanges'
CREATE TABLE [IEA].[HistoryDataChanges] (
    [ID] bigint IDENTITY(1,1) NOT NULL,
    [Type] char(1)  NULL,
    [TableName] varchar(128)  NULL,
    [PK] varchar(1000)  NULL,
    [FieldName] varchar(128)  NULL,
    [OldValue] varchar(1000)  NULL,
    [NewValue] varchar(1000)  NULL,
    [UpdateDate] datetime  NULL,
    [UserName] varchar(128)  NULL,
    [UniqueId] uniqueidentifier  NULL,
    [UpdatedBy] int  NULL
);
GO

-- Creating table 'MemberDocuments'
CREATE TABLE [IEA].[MemberDocuments] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [DocumentName] nvarchar(200)  NULL,
    [ContentType] nvarchar(50)  NULL,
    [MemberID] bigint  NULL,
    [AddedOn] datetime  NOT NULL,
    [AddedBy] bigint  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [ModifiedBy] bigint  NOT NULL
);
GO

-- Creating table 'MemberEmploymentStatus'
CREATE TABLE [IEA].[MemberEmploymentStatus] (
    [Id] bigint  NOT NULL,
    [StatusName] varchar(50)  NOT NULL
);
GO

-- Creating table 'MemberProfiles'
CREATE TABLE [IEA].[MemberProfiles] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [FirstName] nvarchar(200)  NOT NULL,
    [FirstNamePublic] bit  NOT NULL,
    [LastName] nvarchar(200)  NOT NULL,
    [LastNamePublic] bit  NOT NULL,
    [Email] nvarchar(200)  NOT NULL,
    [EmailPublic] bit  NOT NULL,
    [DOB] datetime  NULL,
    [DOBPublic] bit  NOT NULL,
    [BirthPlace] nvarchar(200)  NULL,
    [BirthPlacePublic] bit  NOT NULL,
    [Nationality] nvarchar(200)  NULL,
    [NationalityPublic] bit  NOT NULL,
    [LanguageID] nvarchar(50)  NULL,
    [LanguageIDPublic] bit  NOT NULL,
    [Phone] nvarchar(20)  NULL,
    [PhonePublic] bit  NOT NULL,
    [GSM] nvarchar(10)  NULL,
    [GSMPublic] bit  NOT NULL,
    [Street] nvarchar(max)  NULL,
    [StreetPublic] bit  NOT NULL,
    [PostalCode] nvarchar(10)  NULL,
    [PostalCodePublic] bit  NOT NULL,
    [Commune] nvarchar(100)  NULL,
    [CommunePublic] bit  NOT NULL,
    [PrivateAddress] nvarchar(max)  NULL,
    [PrivateAddressPublic] bit  NOT NULL,
    [PrivatePostalCode] nvarchar(10)  NULL,
    [PrivatePostalCodePublic] bit  NOT NULL,
    [StatusID] int  NULL,
    [StatusIDPublic] bit  NOT NULL,
    [AddedOn] datetime  NOT NULL,
    [AddedBy] bigint  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [ModifiedBy] bigint  NOT NULL
);
GO

-- Creating table 'MemberSpecializations'
CREATE TABLE [IEA].[MemberSpecializations] (
    [Id] bigint  NOT NULL,
    [MemberID] bigint  NULL,
    [SpecializationId] bigint  NULL,
    [AddedOn] datetime  NULL,
    [AddedBy] bigint  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [ModifiedBy] bigint  NOT NULL
);
GO

-- Creating table 'MemberStatus'
CREATE TABLE [IEA].[MemberStatus] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [StatusName] nvarchar(100)  NULL
);
GO

-- Creating table 'MemberTranieeCommissions'
CREATE TABLE [IEA].[MemberTranieeCommissions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [MemberID] bigint  NULL,
    [TrainingCourseId] int  NULL,
    [AddedOn] datetime  NOT NULL,
    [AddedBy] bigint  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [ModifiedBy] bigint  NOT NULL
);
GO

-- Creating table 'TrainingCourses'
CREATE TABLE [IEA].[TrainingCourses] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [TrainingName] nvarchar(200)  NOT NULL,
    [Description] nvarchar(max)  NULL,
    [ValidatedHours] int  NOT NULL,
    [IsShow] bit  NOT NULL,
    [TypeID] int  NULL,
    [Languages] nvarchar(50)  NULL,
    [StartDateTime] datetime  NULL,
    [EndDateTime] datetime  NULL,
    [Cost] decimal(19,4)  NOT NULL,
    [IsFullTime] bit  NOT NULL,
    [Location] nvarchar(200)  NULL,
    [IsApproved] bit  NOT NULL,
    [AddedOn] datetime  NOT NULL,
    [AddedBy] bigint  NOT NULL,
    [ModifiedOn] datetime  NOT NULL,
    [ModifiedBy] bigint  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [ID] in table 'CourseTypes'
ALTER TABLE [IEA].[CourseTypes]
ADD CONSTRAINT [PK_CourseTypes]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'HistoryDataChanges'
ALTER TABLE [IEA].[HistoryDataChanges]
ADD CONSTRAINT [PK_HistoryDataChanges]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'MemberDocuments'
ALTER TABLE [IEA].[MemberDocuments]
ADD CONSTRAINT [PK_MemberDocuments]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [Id] in table 'MemberEmploymentStatus'
ALTER TABLE [IEA].[MemberEmploymentStatus]
ADD CONSTRAINT [PK_MemberEmploymentStatus]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'MemberProfiles'
ALTER TABLE [IEA].[MemberProfiles]
ADD CONSTRAINT [PK_MemberProfiles]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'MemberSpecializations'
ALTER TABLE [IEA].[MemberSpecializations]
ADD CONSTRAINT [PK_MemberSpecializations]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [ID] in table 'MemberStatus'
ALTER TABLE [IEA].[MemberStatus]
ADD CONSTRAINT [PK_MemberStatus]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [Id] in table 'MemberTranieeCommissions'
ALTER TABLE [IEA].[MemberTranieeCommissions]
ADD CONSTRAINT [PK_MemberTranieeCommissions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [ID] in table 'TrainingCourses'
ALTER TABLE [IEA].[TrainingCourses]
ADD CONSTRAINT [PK_TrainingCourses]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [TypeID] in table 'TrainingCourses'
ALTER TABLE [IEA].[TrainingCourses]
ADD CONSTRAINT [FK_TrainingCourses_CourseType]
    FOREIGN KEY ([TypeID])
    REFERENCES [IEA].[CourseTypes]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TrainingCourses_CourseType'
CREATE INDEX [IX_FK_TrainingCourses_CourseType]
ON [IEA].[TrainingCourses]
    ([TypeID]);
GO

-- Creating foreign key on [MemberID] in table 'MemberDocuments'
ALTER TABLE [IEA].[MemberDocuments]
ADD CONSTRAINT [FK_MemberDocuments_MemberDocuments]
    FOREIGN KEY ([MemberID])
    REFERENCES [IEA].[MemberProfiles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MemberDocuments_MemberDocuments'
CREATE INDEX [IX_FK_MemberDocuments_MemberDocuments]
ON [IEA].[MemberDocuments]
    ([MemberID]);
GO

-- Creating foreign key on [StatusID] in table 'MemberProfiles'
ALTER TABLE [IEA].[MemberProfiles]
ADD CONSTRAINT [FK_Members_MemberStatus]
    FOREIGN KEY ([StatusID])
    REFERENCES [IEA].[MemberStatus]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Members_MemberStatus'
CREATE INDEX [IX_FK_Members_MemberStatus]
ON [IEA].[MemberProfiles]
    ([StatusID]);
GO

-- Creating foreign key on [MemberID] in table 'MemberSpecializations'
ALTER TABLE [IEA].[MemberSpecializations]
ADD CONSTRAINT [FK_MemberSpecifications_Members]
    FOREIGN KEY ([MemberID])
    REFERENCES [IEA].[MemberProfiles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MemberSpecifications_Members'
CREATE INDEX [IX_FK_MemberSpecifications_Members]
ON [IEA].[MemberSpecializations]
    ([MemberID]);
GO

-- Creating foreign key on [MemberID] in table 'MemberTranieeCommissions'
ALTER TABLE [IEA].[MemberTranieeCommissions]
ADD CONSTRAINT [FK_MemberTranieeCommission_Members]
    FOREIGN KEY ([MemberID])
    REFERENCES [IEA].[MemberProfiles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MemberTranieeCommission_Members'
CREATE INDEX [IX_FK_MemberTranieeCommission_Members]
ON [IEA].[MemberTranieeCommissions]
    ([MemberID]);
GO

-- Creating foreign key on [TrainingCourseId] in table 'MemberTranieeCommissions'
ALTER TABLE [IEA].[MemberTranieeCommissions]
ADD CONSTRAINT [FK_MemberTranieeCommission_TrainingCourses]
    FOREIGN KEY ([TrainingCourseId])
    REFERENCES [IEA].[TrainingCourses]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MemberTranieeCommission_TrainingCourses'
CREATE INDEX [IX_FK_MemberTranieeCommission_TrainingCourses]
ON [IEA].[MemberTranieeCommissions]
    ([TrainingCourseId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------