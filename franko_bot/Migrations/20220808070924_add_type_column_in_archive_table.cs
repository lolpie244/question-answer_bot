using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace franko_bot.Migrations
{
    /// <inheritdoc />
    public partial class add_type_column_in_archive_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsQuestion",
                table: "Archive");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Archive",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Archive");

            migrationBuilder.AddColumn<bool>(
                name: "IsQuestion",
                table: "Archive",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
