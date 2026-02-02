using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetCare.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddRegistroVacina : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RegistrosVacinas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PetId = table.Column<int>(type: "int", nullable: false),
                    NomeVacina = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DataAplicacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IntervaloDias = table.Column<int>(type: "int", nullable: false),
                    ProximaDose = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Observacao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosVacinas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrosVacinas_Pets_PetId",
                        column: x => x.PetId,
                        principalTable: "Pets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosVacinas_PetId",
                table: "RegistrosVacinas",
                column: "PetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegistrosVacinas");
        }
    }
}
