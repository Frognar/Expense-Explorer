using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseExplorer.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class NameAliases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "name_aliases",
                schema: "expense",
                columns: table => new
                {
                    kind = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    alias = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_name_aliases", x => new { x.kind, x.alias });
                    table.CheckConstraint("ck_name_aliases_kind", "kind in ('store', 'item', 'category')");
                    table.CheckConstraint("ck_name_aliases_name_not_blank", "btrim(name) <> ''");
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "name_aliases",
                schema: "expense");
        }
    }
}
