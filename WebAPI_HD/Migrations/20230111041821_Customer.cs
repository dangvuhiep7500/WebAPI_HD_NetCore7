using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPIHD.Migrations
{
    /// <inheritdoc />
    public partial class Customer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    customerid = table.Column<int>(name: "customer_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    customercode = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    address = table.Column<string>(type: "varchar(40)", unicode: false, maxLength: 40, nullable: true),
                    phone = table.Column<string>(type: "char(12)", unicode: false, fixedLength: true, maxLength: 12, nullable: false, defaultValueSql: "('UNKNOWN')"),
                    emailaddress = table.Column<string>(name: "email_address", type: "varchar(100)", unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.customerid);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Customer");
        }
    }
}
