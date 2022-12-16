using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace franko_bot.Migrations
{
    /// <inheritdoc />
    public partial class change_asker_id_to_related_user_id : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AskerId",
                table: "Archive");

            migrationBuilder.AddColumn<long>(
                name: "RelatedUserId",
                table: "Archive",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RelatedUserId",
                table: "Archive");

            migrationBuilder.AddColumn<long>(
                name: "AskerId",
                table: "Archive",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
