using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEndProject.Migrations
{
    public partial class DeletephoneColumnInOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Orders");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Orders",
                type: "nvarchar(25)",
                maxLength: 25,
                nullable: false,
                defaultValue: "");
        }
    }
}
