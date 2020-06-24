CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

CREATE TABLE "ApiResources" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ApiResources" PRIMARY KEY AUTOINCREMENT,
    "Enabled" INTEGER NOT NULL,
    "Name" TEXT NOT NULL,
    "DisplayName" TEXT NULL,
    "Description" TEXT NULL,
    "AllowedAccessTokenSigningAlgorithms" TEXT NULL,
    "ShowInDiscoveryDocument" INTEGER NOT NULL,
    "Created" TEXT NOT NULL,
    "Updated" TEXT NULL,
    "LastAccessed" TEXT NULL,
    "NonEditable" INTEGER NOT NULL
);

CREATE TABLE "ApiScopes" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ApiScopes" PRIMARY KEY AUTOINCREMENT,
    "Enabled" INTEGER NOT NULL,
    "Name" TEXT NOT NULL,
    "DisplayName" TEXT NULL,
    "Description" TEXT NULL,
    "Required" INTEGER NOT NULL,
    "Emphasize" INTEGER NOT NULL,
    "ShowInDiscoveryDocument" INTEGER NOT NULL
);

CREATE TABLE "Clients" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Clients" PRIMARY KEY AUTOINCREMENT,
    "Enabled" INTEGER NOT NULL,
    "ClientId" TEXT NOT NULL,
    "ProtocolType" TEXT NOT NULL,
    "RequireClientSecret" INTEGER NOT NULL,
    "ClientName" TEXT NULL,
    "Description" TEXT NULL,
    "ClientUri" TEXT NULL,
    "LogoUri" TEXT NULL,
    "RequireConsent" INTEGER NOT NULL,
    "AllowRememberConsent" INTEGER NOT NULL,
    "AlwaysIncludeUserClaimsInIdToken" INTEGER NOT NULL,
    "RequirePkce" INTEGER NOT NULL,
    "AllowPlainTextPkce" INTEGER NOT NULL,
    "RequireRequestObject" INTEGER NOT NULL,
    "AllowAccessTokensViaBrowser" INTEGER NOT NULL,
    "FrontChannelLogoutUri" TEXT NULL,
    "FrontChannelLogoutSessionRequired" INTEGER NOT NULL,
    "BackChannelLogoutUri" TEXT NULL,
    "BackChannelLogoutSessionRequired" INTEGER NOT NULL,
    "AllowOfflineAccess" INTEGER NOT NULL,
    "IdentityTokenLifetime" INTEGER NOT NULL,
    "AllowedIdentityTokenSigningAlgorithms" TEXT NULL,
    "AccessTokenLifetime" INTEGER NOT NULL,
    "AuthorizationCodeLifetime" INTEGER NOT NULL,
    "ConsentLifetime" INTEGER NULL,
    "AbsoluteRefreshTokenLifetime" INTEGER NOT NULL,
    "SlidingRefreshTokenLifetime" INTEGER NOT NULL,
    "RefreshTokenUsage" INTEGER NOT NULL,
    "UpdateAccessTokenClaimsOnRefresh" INTEGER NOT NULL,
    "RefreshTokenExpiration" INTEGER NOT NULL,
    "AccessTokenType" INTEGER NOT NULL,
    "EnableLocalLogin" INTEGER NOT NULL,
    "IncludeJwtId" INTEGER NOT NULL,
    "AlwaysSendClientClaims" INTEGER NOT NULL,
    "ClientClaimsPrefix" TEXT NULL,
    "PairWiseSubjectSalt" TEXT NULL,
    "Created" TEXT NOT NULL,
    "Updated" TEXT NULL,
    "LastAccessed" TEXT NULL,
    "UserSsoLifetime" INTEGER NULL,
    "UserCodeType" TEXT NULL,
    "DeviceCodeLifetime" INTEGER NOT NULL,
    "NonEditable" INTEGER NOT NULL
);

CREATE TABLE "IdentityResources" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_IdentityResources" PRIMARY KEY AUTOINCREMENT,
    "Enabled" INTEGER NOT NULL,
    "Name" TEXT NOT NULL,
    "DisplayName" TEXT NULL,
    "Description" TEXT NULL,
    "Required" INTEGER NOT NULL,
    "Emphasize" INTEGER NOT NULL,
    "ShowInDiscoveryDocument" INTEGER NOT NULL,
    "Created" TEXT NOT NULL,
    "Updated" TEXT NULL,
    "NonEditable" INTEGER NOT NULL
);

CREATE TABLE "ApiResourceClaims" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ApiResourceClaims" PRIMARY KEY AUTOINCREMENT,
    "Type" TEXT NOT NULL,
    "ApiResourceId" INTEGER NOT NULL,
    CONSTRAINT "FK_ApiResourceClaims_ApiResources_ApiResourceId" FOREIGN KEY ("ApiResourceId") REFERENCES "ApiResources" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ApiResourceProperties" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ApiResourceProperties" PRIMARY KEY AUTOINCREMENT,
    "Key" TEXT NOT NULL,
    "Value" TEXT NOT NULL,
    "ApiResourceId" INTEGER NOT NULL,
    CONSTRAINT "FK_ApiResourceProperties_ApiResources_ApiResourceId" FOREIGN KEY ("ApiResourceId") REFERENCES "ApiResources" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ApiResourceScopes" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ApiResourceScopes" PRIMARY KEY AUTOINCREMENT,
    "Scope" TEXT NOT NULL,
    "ApiResourceId" INTEGER NOT NULL,
    CONSTRAINT "FK_ApiResourceScopes_ApiResources_ApiResourceId" FOREIGN KEY ("ApiResourceId") REFERENCES "ApiResources" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ApiResourceSecrets" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ApiResourceSecrets" PRIMARY KEY AUTOINCREMENT,
    "Description" TEXT NULL,
    "Value" TEXT NOT NULL,
    "Expiration" TEXT NULL,
    "Type" TEXT NOT NULL,
    "Created" TEXT NOT NULL,
    "ApiResourceId" INTEGER NOT NULL,
    CONSTRAINT "FK_ApiResourceSecrets_ApiResources_ApiResourceId" FOREIGN KEY ("ApiResourceId") REFERENCES "ApiResources" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ApiScopeClaims" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ApiScopeClaims" PRIMARY KEY AUTOINCREMENT,
    "Type" TEXT NOT NULL,
    "ScopeId" INTEGER NOT NULL,
    CONSTRAINT "FK_ApiScopeClaims_ApiScopes_ScopeId" FOREIGN KEY ("ScopeId") REFERENCES "ApiScopes" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ApiScopeProperties" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ApiScopeProperties" PRIMARY KEY AUTOINCREMENT,
    "Key" TEXT NOT NULL,
    "Value" TEXT NOT NULL,
    "ScopeId" INTEGER NOT NULL,
    CONSTRAINT "FK_ApiScopeProperties_ApiScopes_ScopeId" FOREIGN KEY ("ScopeId") REFERENCES "ApiScopes" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ClientClaims" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ClientClaims" PRIMARY KEY AUTOINCREMENT,
    "Type" TEXT NOT NULL,
    "Value" TEXT NOT NULL,
    "ClientId" INTEGER NOT NULL,
    CONSTRAINT "FK_ClientClaims_Clients_ClientId" FOREIGN KEY ("ClientId") REFERENCES "Clients" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ClientCorsOrigins" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ClientCorsOrigins" PRIMARY KEY AUTOINCREMENT,
    "Origin" TEXT NOT NULL,
    "ClientId" INTEGER NOT NULL,
    CONSTRAINT "FK_ClientCorsOrigins_Clients_ClientId" FOREIGN KEY ("ClientId") REFERENCES "Clients" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ClientGrantTypes" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ClientGrantTypes" PRIMARY KEY AUTOINCREMENT,
    "GrantType" TEXT NOT NULL,
    "ClientId" INTEGER NOT NULL,
    CONSTRAINT "FK_ClientGrantTypes_Clients_ClientId" FOREIGN KEY ("ClientId") REFERENCES "Clients" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ClientIdPRestrictions" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ClientIdPRestrictions" PRIMARY KEY AUTOINCREMENT,
    "Provider" TEXT NOT NULL,
    "ClientId" INTEGER NOT NULL,
    CONSTRAINT "FK_ClientIdPRestrictions_Clients_ClientId" FOREIGN KEY ("ClientId") REFERENCES "Clients" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ClientPostLogoutRedirectUris" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ClientPostLogoutRedirectUris" PRIMARY KEY AUTOINCREMENT,
    "PostLogoutRedirectUri" TEXT NOT NULL,
    "ClientId" INTEGER NOT NULL,
    CONSTRAINT "FK_ClientPostLogoutRedirectUris_Clients_ClientId" FOREIGN KEY ("ClientId") REFERENCES "Clients" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ClientProperties" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ClientProperties" PRIMARY KEY AUTOINCREMENT,
    "Key" TEXT NOT NULL,
    "Value" TEXT NOT NULL,
    "ClientId" INTEGER NOT NULL,
    CONSTRAINT "FK_ClientProperties_Clients_ClientId" FOREIGN KEY ("ClientId") REFERENCES "Clients" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ClientRedirectUris" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ClientRedirectUris" PRIMARY KEY AUTOINCREMENT,
    "RedirectUri" TEXT NOT NULL,
    "ClientId" INTEGER NOT NULL,
    CONSTRAINT "FK_ClientRedirectUris_Clients_ClientId" FOREIGN KEY ("ClientId") REFERENCES "Clients" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ClientScopes" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ClientScopes" PRIMARY KEY AUTOINCREMENT,
    "Scope" TEXT NOT NULL,
    "ClientId" INTEGER NOT NULL,
    CONSTRAINT "FK_ClientScopes_Clients_ClientId" FOREIGN KEY ("ClientId") REFERENCES "Clients" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ClientSecrets" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ClientSecrets" PRIMARY KEY AUTOINCREMENT,
    "Description" TEXT NULL,
    "Value" TEXT NOT NULL,
    "Expiration" TEXT NULL,
    "Type" TEXT NOT NULL,
    "Created" TEXT NOT NULL,
    "ClientId" INTEGER NOT NULL,
    CONSTRAINT "FK_ClientSecrets_Clients_ClientId" FOREIGN KEY ("ClientId") REFERENCES "Clients" ("Id") ON DELETE CASCADE
);

CREATE TABLE "IdentityResourceClaims" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_IdentityResourceClaims" PRIMARY KEY AUTOINCREMENT,
    "Type" TEXT NOT NULL,
    "IdentityResourceId" INTEGER NOT NULL,
    CONSTRAINT "FK_IdentityResourceClaims_IdentityResources_IdentityResourceId" FOREIGN KEY ("IdentityResourceId") REFERENCES "IdentityResources" ("Id") ON DELETE CASCADE
);

CREATE TABLE "IdentityResourceProperties" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_IdentityResourceProperties" PRIMARY KEY AUTOINCREMENT,
    "Key" TEXT NOT NULL,
    "Value" TEXT NOT NULL,
    "IdentityResourceId" INTEGER NOT NULL,
    CONSTRAINT "FK_IdentityResourceProperties_IdentityResources_IdentityResourceId" FOREIGN KEY ("IdentityResourceId") REFERENCES "IdentityResources" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_ApiResourceClaims_ApiResourceId" ON "ApiResourceClaims" ("ApiResourceId");

CREATE INDEX "IX_ApiResourceProperties_ApiResourceId" ON "ApiResourceProperties" ("ApiResourceId");

CREATE UNIQUE INDEX "IX_ApiResources_Name" ON "ApiResources" ("Name");

CREATE INDEX "IX_ApiResourceScopes_ApiResourceId" ON "ApiResourceScopes" ("ApiResourceId");

CREATE INDEX "IX_ApiResourceSecrets_ApiResourceId" ON "ApiResourceSecrets" ("ApiResourceId");

CREATE INDEX "IX_ApiScopeClaims_ScopeId" ON "ApiScopeClaims" ("ScopeId");

CREATE INDEX "IX_ApiScopeProperties_ScopeId" ON "ApiScopeProperties" ("ScopeId");

CREATE UNIQUE INDEX "IX_ApiScopes_Name" ON "ApiScopes" ("Name");

CREATE INDEX "IX_ClientClaims_ClientId" ON "ClientClaims" ("ClientId");

CREATE INDEX "IX_ClientCorsOrigins_ClientId" ON "ClientCorsOrigins" ("ClientId");

CREATE INDEX "IX_ClientGrantTypes_ClientId" ON "ClientGrantTypes" ("ClientId");

CREATE INDEX "IX_ClientIdPRestrictions_ClientId" ON "ClientIdPRestrictions" ("ClientId");

CREATE INDEX "IX_ClientPostLogoutRedirectUris_ClientId" ON "ClientPostLogoutRedirectUris" ("ClientId");

CREATE INDEX "IX_ClientProperties_ClientId" ON "ClientProperties" ("ClientId");

CREATE INDEX "IX_ClientRedirectUris_ClientId" ON "ClientRedirectUris" ("ClientId");

CREATE UNIQUE INDEX "IX_Clients_ClientId" ON "Clients" ("ClientId");

CREATE INDEX "IX_ClientScopes_ClientId" ON "ClientScopes" ("ClientId");

CREATE INDEX "IX_ClientSecrets_ClientId" ON "ClientSecrets" ("ClientId");

CREATE INDEX "IX_IdentityResourceClaims_IdentityResourceId" ON "IdentityResourceClaims" ("IdentityResourceId");

CREATE INDEX "IX_IdentityResourceProperties_IdentityResourceId" ON "IdentityResourceProperties" ("IdentityResourceId");

CREATE UNIQUE INDEX "IX_IdentityResources_Name" ON "IdentityResources" ("Name");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20200624171023_Config', '3.1.0');

