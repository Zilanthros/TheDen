// SPDX-FileCopyrightText: 2024 Pierson Arnold <greyalphawolf7@gmail.com>
// SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Content.Server.Database.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class ConsentSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "consent_settings",
                columns: table => new
                {
                    consent_settings_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    user_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    consent_freetext = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_consent_settings", x => x.consent_settings_id);
                });

            migrationBuilder.CreateTable(
                name: "consent_toggle",
                columns: table => new
                {
                    consent_toggle_id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    consent_settings_id = table.Column<int>(type: "INTEGER", nullable: false),
                    toggle_proto_id = table.Column<string>(type: "TEXT", nullable: false),
                    toggle_proto_state = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_consent_toggle", x => x.consent_toggle_id);
                    table.ForeignKey(
                        name: "FK_consent_toggle_consent_settings_consent_settings_id",
                        column: x => x.consent_settings_id,
                        principalTable: "consent_settings",
                        principalColumn: "consent_settings_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_consent_settings_user_id",
                table: "consent_settings",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_consent_toggle_consent_settings_id_toggle_proto_id",
                table: "consent_toggle",
                columns: new[] { "consent_settings_id", "toggle_proto_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "consent_toggle");

            migrationBuilder.DropTable(
                name: "consent_settings");
        }
    }
}
