CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

CREATE TABLE "PersistedGrants" (
    "Key" TEXT NOT NULL CONSTRAINT "PK_PersistedGrants" PRIMARY KEY,
    "ClientId" TEXT NOT NULL,
    "CreationTime" TEXT NOT NULL,
    "Data" TEXT NOT NULL,
    "Expiration" TEXT NULL,
    "SubjectId" TEXT NULL,
    "Type" TEXT NOT NULL
);

CREATE INDEX "IX_PersistedGrants_SubjectId_ClientId_Type" ON "PersistedGrants" ("SubjectId", "ClientId", "Type");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20171119164341_Grants', '2.0.0-rtm-26452');

