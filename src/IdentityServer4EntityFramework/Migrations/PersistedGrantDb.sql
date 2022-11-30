CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

CREATE TABLE "DeviceCodes" (
    "UserCode" TEXT NOT NULL CONSTRAINT "PK_DeviceCodes" PRIMARY KEY,
    "DeviceCode" TEXT NOT NULL,
    "SubjectId" TEXT NULL,
    "SessionId" TEXT NULL,
    "ClientId" TEXT NOT NULL,
    "Description" TEXT NULL,
    "CreationTime" TEXT NOT NULL,
    "Expiration" TEXT NOT NULL,
    "Data" TEXT NOT NULL
);

CREATE TABLE "PersistedGrants" (
    "Key" TEXT NOT NULL CONSTRAINT "PK_PersistedGrants" PRIMARY KEY,
    "Type" TEXT NOT NULL,
    "SubjectId" TEXT NULL,
    "SessionId" TEXT NULL,
    "ClientId" TEXT NOT NULL,
    "Description" TEXT NULL,
    "CreationTime" TEXT NOT NULL,
    "Expiration" TEXT NULL,
    "ConsumedTime" TEXT NULL,
    "Data" TEXT NOT NULL
);

CREATE UNIQUE INDEX "IX_DeviceCodes_DeviceCode" ON "DeviceCodes" ("DeviceCode");

CREATE INDEX "IX_DeviceCodes_Expiration" ON "DeviceCodes" ("Expiration");

CREATE INDEX "IX_PersistedGrants_Expiration" ON "PersistedGrants" ("Expiration");

CREATE INDEX "IX_PersistedGrants_SubjectId_ClientId_Type" ON "PersistedGrants" ("SubjectId", "ClientId", "Type");

CREATE INDEX "IX_PersistedGrants_SubjectId_SessionId_Type" ON "PersistedGrants" ("SubjectId", "SessionId", "Type");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20200624171018_Grants', '3.1.0');

