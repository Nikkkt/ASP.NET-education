using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP.NET_Classwork.Migrations
{
    /// <inheritdoc />
    public partial class ProductChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Picture",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Picture",
                table: "Products");
        }
    }
}
