using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalesCommissionsAPI.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SMSS",
                columns: table => new
                {
                    Salesrep = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Year = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Cmth = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GPBudget = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Site = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SMSS", x => new { x.Salesrep, x.Year, x.Cmth });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SMSS");
        }
    }
}
