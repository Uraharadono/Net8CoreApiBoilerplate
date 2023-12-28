using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Net8CoreApiBoilerplate.DbContext.Migrations
{
    /// <inheritdoc />
    public partial class LoggingTableMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "LoggingSeq",
                startValue: 2000000L,
                minValue: 2000000L);

            migrationBuilder.CreateTable(
                name: "Logging",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    LogDatum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LogType = table.Column<int>(type: "int", nullable: false),
                    LogValue = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: false),
                    LogText = table.Column<string>(type: "VARCHAR(500)", maxLength: 500, nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logging", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Logging_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Logging_UserId",
                table: "Logging",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Logging");

            migrationBuilder.DropSequence(
                name: "LoggingSeq");
        }
    }
}
