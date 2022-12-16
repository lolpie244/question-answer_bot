using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace franko_bot.Migrations
{
    /// <inheritdoc />
    public partial class add_user_settings_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Faculty",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Users",
                newName: "UserSettingsId");

            migrationBuilder.AddColumn<string>(
                name: "UserSettingsCode",
                table: "Users",
                type: "character varying(7)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "UsersData",
                columns: table => new
                {
                    Code = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    Faculty = table.Column<string>(type: "text", nullable: false),
                    Speciality = table.Column<string>(type: "text", nullable: false),
                    Course = table.Column<int>(type: "integer", nullable: false),
                    Group = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersData", x => x.Code);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserSettingsCode",
                table: "Users",
                column: "UserSettingsCode");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_UsersData_UserSettingsCode",
                table: "Users",
                column: "UserSettingsCode",
                principalTable: "UsersData",
                principalColumn: "Code",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_UsersData_UserSettingsCode",
                table: "Users");

            migrationBuilder.DropTable(
                name: "UsersData");

            migrationBuilder.DropIndex(
                name: "IX_Users_UserSettingsCode",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserSettingsCode",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "UserSettingsId",
                table: "Users",
                newName: "Status");

            migrationBuilder.AddColumn<string>(
                name: "Faculty",
                table: "Users",
                type: "text",
                nullable: true);
        }
    }
}
