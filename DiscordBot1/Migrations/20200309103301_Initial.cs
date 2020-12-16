using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DiscordBot1.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SetUser",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    UserCreated = table.Column<string>(nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserModified = table.Column<string>(nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExtensionClass = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    NickName = table.Column<string>(nullable: true),
                    UserID = table.Column<decimal>(nullable: false),
                    SLChannel = table.Column<string>(nullable: true),
                    SLChannelID = table.Column<decimal>(nullable: false),
                    CharakterID = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetUser", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SetCharakterblatt",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    UserCreated = table.Column<string>(nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserModified = table.Column<string>(nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExtensionClass = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    UserID = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetCharakterblatt", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SetCharakterblatt_SetUser_UserID",
                        column: x => x.UserID,
                        principalTable: "SetUser",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SetCharakterwert",
                columns: table => new
                {
                    ID = table.Column<Guid>(nullable: false),
                    UserCreated = table.Column<string>(nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserModified = table.Column<string>(nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExtensionClass = table.Column<string>(nullable: true),
                    name = table.Column<string>(nullable: true),
                    wert = table.Column<int>(nullable: false),
                    characterID = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SetCharakterwert", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SetCharakterwert_SetCharakterblatt_characterID",
                        column: x => x.characterID,
                        principalTable: "SetCharakterblatt",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SetCharakterblatt_UserID",
                table: "SetCharakterblatt",
                column: "UserID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SetCharakterwert_characterID",
                table: "SetCharakterwert",
                column: "characterID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SetCharakterwert");

            migrationBuilder.DropTable(
                name: "SetCharakterblatt");

            migrationBuilder.DropTable(
                name: "SetUser");
        }
    }
}
