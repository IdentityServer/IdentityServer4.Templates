CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

CREATE TABLE "DeviceCodes" (
    "UserCode" TEXT NOT NULL CONSTRAINT "PK_DeviceCodes" PRIMARY KEY,
    "DeviceCode" TEXT NOT NULL,
    "SubjectId" TEXT NULL,
    "ClientId" TEXT NOT NULL,
    "CreationTime" TEXT NOT NULL,
    "Expiration" TEXT NOT NULL,
    "Data" TEXT NOT NULL
);

CREATE TABLE "PersistedGrants" (
    "Key" TEXT NOT NULL CONSTRAINT "PK_PersistedGrants" PRIMARY KEY,
    "Type" TEXT NOT NULL,
    "SubjectId" TEXT NULL,
    "ClientId" TEXT NOT NULL,
    "CreationTime" TEXT NOT NULL,
    "Expiration" TEXT NULL,
    "Data" TEXT NOT NULL
);

CREATE UNIQUE INDEX "IX_DeviceCodes_DeviceCode" ON "DeviceCodes" ("DeviceCode");

CREATE INDEX "IX_DeviceCodes_Expiration" ON "DeviceCodes" ("Expiration");

CREATE INDEX "IX_PersistedGrants_Expiration" ON "PersistedGrants" ("Expiration");

CREATE INDEX "IX_PersistedGrants_SubjectId_ClientId_Type" ON "PersistedGrants" ("SubjectId", "ClientId", "Type");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20190927145451_Grants', '3.0.0');

