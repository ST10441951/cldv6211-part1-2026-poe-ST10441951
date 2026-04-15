IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Booking] (
    [BookingID] int NOT NULL IDENTITY,
    [EventID] int NOT NULL,
    [BookingStartDate] date NOT NULL,
    [BookingEndDate] date NOT NULL,
    [BookingStatus] nvarchar(max) NULL,
    CONSTRAINT [PK_Booking] PRIMARY KEY ([BookingID])
);

CREATE TABLE [Venue] (
    [VenueID] int NOT NULL IDENTITY,
    [VenueName] nvarchar(max) NOT NULL,
    [VenueLocation] nvarchar(max) NOT NULL,
    [VenueCapacity] int NOT NULL,
    [ImageUrl] nvarchar(max) NULL,
    CONSTRAINT [PK_Venue] PRIMARY KEY ([VenueID])
);

CREATE TABLE [Event] (
    [EventID] int NOT NULL IDENTITY,
    [BookingID] int NOT NULL,
    [EventStartDate] date NOT NULL,
    [EventEndDate] date NOT NULL,
    [EventName] nvarchar(max) NOT NULL,
    [EventDescription] nvarchar(max) NULL,
    [VenueID] int NULL,
    CONSTRAINT [PK_Event] PRIMARY KEY ([EventID]),
    CONSTRAINT [FK_Event_Booking_BookingID] FOREIGN KEY ([BookingID]) REFERENCES [Booking] ([BookingID]) ON DELETE CASCADE,
    CONSTRAINT [FK_Event_Venue_VenueID] FOREIGN KEY ([VenueID]) REFERENCES [Venue] ([VenueID])
);

CREATE INDEX [IX_Event_BookingID] ON [Event] ([BookingID]);

CREATE INDEX [IX_Event_VenueID] ON [Event] ([VenueID]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260414160632_InitialCreate', N'10.0.2');

COMMIT;
GO

BEGIN TRANSACTION;
DECLARE @var nvarchar(max);
SELECT @var = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Booking]') AND [c].[name] = N'EventID');
IF @var IS NOT NULL EXEC(N'ALTER TABLE [Booking] DROP CONSTRAINT ' + @var + ';');
ALTER TABLE [Booking] ALTER COLUMN [EventID] int NULL;

ALTER TABLE [Booking] ADD [VenueID] int NOT NULL DEFAULT 0;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260415054437_UpdateModelsAndFixVenue', N'10.0.2');

COMMIT;
GO

BEGIN TRANSACTION;
CREATE INDEX [IX_Booking_VenueID] ON [Booking] ([VenueID]);

ALTER TABLE [Booking] ADD CONSTRAINT [FK_Booking_Venue_VenueID] FOREIGN KEY ([VenueID]) REFERENCES [Venue] ([VenueID]) ON DELETE CASCADE;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260415055311_AddVenueNavigationToBooking', N'10.0.2');

COMMIT;
GO

BEGIN TRANSACTION;
ALTER TABLE [Booking] DROP CONSTRAINT [FK_Booking_Venue_VenueID];

ALTER TABLE [Event] DROP CONSTRAINT [FK_Event_Booking_BookingID];

ALTER TABLE [Event] ADD [BookingID1] int NULL;

CREATE INDEX [IX_Event_BookingID1] ON [Event] ([BookingID1]);

CREATE INDEX [IX_Booking_EventID] ON [Booking] ([EventID]);

ALTER TABLE [Booking] ADD CONSTRAINT [FK_Booking_Event_EventID] FOREIGN KEY ([EventID]) REFERENCES [Event] ([EventID]) ON DELETE NO ACTION;

ALTER TABLE [Booking] ADD CONSTRAINT [FK_Booking_Venue_VenueID] FOREIGN KEY ([VenueID]) REFERENCES [Venue] ([VenueID]) ON DELETE NO ACTION;

ALTER TABLE [Event] ADD CONSTRAINT [FK_Event_Booking_BookingID] FOREIGN KEY ([BookingID]) REFERENCES [Booking] ([BookingID]) ON DELETE NO ACTION;

ALTER TABLE [Event] ADD CONSTRAINT [FK_Event_Booking_BookingID1] FOREIGN KEY ([BookingID1]) REFERENCES [Booking] ([BookingID]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260415060505_FixRelationships', N'10.0.2');

COMMIT;
GO

BEGIN TRANSACTION;
ALTER TABLE [Event] DROP CONSTRAINT [FK_Event_Booking_BookingID1];

ALTER TABLE [Event] DROP CONSTRAINT [FK_Event_Venue_VenueID];

DROP INDEX [IX_Event_BookingID1] ON [Event];

DROP INDEX [IX_Event_VenueID] ON [Event];

DECLARE @var1 nvarchar(max);
SELECT @var1 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Event]') AND [c].[name] = N'BookingID1');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Event] DROP CONSTRAINT ' + @var1 + ';');
ALTER TABLE [Event] DROP COLUMN [BookingID1];

DECLARE @var2 nvarchar(max);
SELECT @var2 = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Event]') AND [c].[name] = N'VenueID');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Event] DROP CONSTRAINT ' + @var2 + ';');
ALTER TABLE [Event] DROP COLUMN [VenueID];

ALTER TABLE [Booking] ADD [VenueID1] int NULL;

CREATE INDEX [IX_Booking_VenueID1] ON [Booking] ([VenueID1]);

ALTER TABLE [Booking] ADD CONSTRAINT [FK_Booking_Venue_VenueID1] FOREIGN KEY ([VenueID1]) REFERENCES [Venue] ([VenueID]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260415064944_FinalModelCleanup', N'10.0.2');

COMMIT;
GO

