using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEndProject.Migrations
{
    public partial class AddColumnIsAdmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAdmin",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAdmin",
                table: "AspNetUsers");
        }
    }
}
