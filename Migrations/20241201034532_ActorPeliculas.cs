using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinimalAPIPelicula.Migrations
{
    /// <inheritdoc />
    public partial class ActorPeliculas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActorPeliculas",
                columns: table => new
                {
                    PeliculaId = table.Column<int>(type: "int", nullable: false),
                    Actorid = table.Column<int>(type: "int", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    Personaje = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActorPeliculas", x => new { x.Actorid, x.PeliculaId });
                    table.ForeignKey(
                        name: "FK_ActorPeliculas_Actores_Actorid",
                        column: x => x.Actorid,
                        principalTable: "Actores",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActorPeliculas_Peliculas_PeliculaId",
                        column: x => x.PeliculaId,
                        principalTable: "Peliculas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActorPeliculas_PeliculaId",
                table: "ActorPeliculas",
                column: "PeliculaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActorPeliculas");
        }
    }
}
