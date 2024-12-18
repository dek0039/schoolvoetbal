using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolVoetbalAPI.Migrations
{
    /// <inheritdoc />
    public partial class rollbackasdaj : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedMatch",
                table: "Team");

            migrationBuilder.RenameColumn(
                name: "Score",
                table: "Match",
                newName: "ScoreTeam2");

            migrationBuilder.AddColumn<int>(
                name: "AssignedTeam1",
                table: "Match",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AssignedTeam2",
                table: "Match",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ScoreTeam1",
                table: "Match",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedTeam1",
                table: "Match");

            migrationBuilder.DropColumn(
                name: "AssignedTeam2",
                table: "Match");

            migrationBuilder.DropColumn(
                name: "ScoreTeam1",
                table: "Match");

            migrationBuilder.RenameColumn(
                name: "ScoreTeam2",
                table: "Match",
                newName: "Score");

            migrationBuilder.AddColumn<int>(
                name: "AssignedMatch",
                table: "Team",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
