using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace franko_bot.Migrations
{
    /// <inheritdoc />
    public partial class remove_user_settings_id_column_from_user_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_UsersData_UserSettingsCode",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserSettingsId",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "UserSettingsCode",
                table: "Users",
                type: "character varying(7)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(7)");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_UsersData_UserSettingsCode",
                table: "Users",
                column: "UserSettingsCode",
                principalTable: "UsersData",
                principalColumn: "Code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_UsersData_UserSettingsCode",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "UserSettingsCode",
                table: "Users",
                type: "character varying(7)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(7)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserSettingsId",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_UsersData_UserSettingsCode",
                table: "Users",
                column: "UserSettingsCode",
                principalTable: "UsersData",
                principalColumn: "Code",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
