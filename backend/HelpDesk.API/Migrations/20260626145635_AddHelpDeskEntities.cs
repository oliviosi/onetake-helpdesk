using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpDesk.API.Migrations
{
    /// <inheritdoc />
    public partial class AddHelpDeskEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cliente",
                columns: table => new
                {
                    IdEmpresa = table.Column<int>(type: "int", nullable: false),
                    IdCliente = table.Column<int>(type: "int", nullable: false),
                    DscCliente = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Ativo = table.Column<string>(type: "varchar(1)", maxLength: 1, nullable: false, defaultValue: "S")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CodClienteERP = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cliente", x => new { x.IdEmpresa, x.IdCliente });
                    table.ForeignKey(
                        name: "FK_Cliente_Empresa_IdEmpresa",
                        column: x => x.IdEmpresa,
                        principalTable: "Empresa",
                        principalColumn: "IdEmpresa",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Produto",
                columns: table => new
                {
                    IdEmpresa = table.Column<int>(type: "int", nullable: false),
                    IdProduto = table.Column<int>(type: "int", nullable: false),
                    DscProduto = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Ativo = table.Column<string>(type: "varchar(1)", maxLength: 1, nullable: false, defaultValue: "S")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produto", x => new { x.IdEmpresa, x.IdProduto });
                    table.ForeignKey(
                        name: "FK_Produto_Empresa_IdEmpresa",
                        column: x => x.IdEmpresa,
                        principalTable: "Empresa",
                        principalColumn: "IdEmpresa",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StatusTicket",
                columns: table => new
                {
                    IdEmpresa = table.Column<int>(type: "int", nullable: false),
                    IdStatusTicket = table.Column<int>(type: "int", nullable: false),
                    DscStatusTicket = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TipoTicket = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusTicket", x => new { x.IdEmpresa, x.IdStatusTicket });
                    table.ForeignKey(
                        name: "FK_StatusTicket_Empresa_IdEmpresa",
                        column: x => x.IdEmpresa,
                        principalTable: "Empresa",
                        principalColumn: "IdEmpresa",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Tecnico",
                columns: table => new
                {
                    IdEmpresa = table.Column<int>(type: "int", nullable: false),
                    IdTecnico = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NotificarNovosChamados = table.Column<string>(type: "varchar(1)", maxLength: 1, nullable: false, defaultValue: "N")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdUsuario = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tecnico", x => new { x.IdEmpresa, x.IdTecnico });
                    table.ForeignKey(
                        name: "FK_Tecnico_Empresa_IdEmpresa",
                        column: x => x.IdEmpresa,
                        principalTable: "Empresa",
                        principalColumn: "IdEmpresa",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tecnico_Usuario_IdEmpresa_IdUsuario",
                        columns: x => new { x.IdEmpresa, x.IdUsuario },
                        principalTable: "Usuario",
                        principalColumns: new[] { "IdEmpresa", "IdUsuario" });
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TipoOcorrencia",
                columns: table => new
                {
                    IdEmpresa = table.Column<int>(type: "int", nullable: false),
                    IdTipoOcorrencia = table.Column<int>(type: "int", nullable: false),
                    DscTipoOcorrencia = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FiltroChamado = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EmailCopiaChamado = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Ativo = table.Column<string>(type: "varchar(1)", maxLength: 1, nullable: false, defaultValue: "S")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoOcorrencia", x => new { x.IdEmpresa, x.IdTipoOcorrencia });
                    table.ForeignKey(
                        name: "FK_TipoOcorrencia_Empresa_IdEmpresa",
                        column: x => x.IdEmpresa,
                        principalTable: "Empresa",
                        principalColumn: "IdEmpresa",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ProdutoXCliente",
                columns: table => new
                {
                    IdEmpresa = table.Column<int>(type: "int", nullable: false),
                    IdProduto = table.Column<int>(type: "int", nullable: false),
                    IdCliente = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProdutoXCliente", x => new { x.IdEmpresa, x.IdProduto, x.IdCliente });
                    table.ForeignKey(
                        name: "FK_ProdutoXCliente_Cliente_IdEmpresa_IdCliente",
                        columns: x => new { x.IdEmpresa, x.IdCliente },
                        principalTable: "Cliente",
                        principalColumns: new[] { "IdEmpresa", "IdCliente" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProdutoXCliente_Empresa_IdEmpresa",
                        column: x => x.IdEmpresa,
                        principalTable: "Empresa",
                        principalColumn: "IdEmpresa",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProdutoXCliente_Produto_IdEmpresa_IdProduto",
                        columns: x => new { x.IdEmpresa, x.IdProduto },
                        principalTable: "Produto",
                        principalColumns: new[] { "IdEmpresa", "IdProduto" },
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Ticket",
                columns: table => new
                {
                    IdEmpresa = table.Column<int>(type: "int", nullable: false),
                    IdTicket = table.Column<int>(type: "int", nullable: false),
                    DtHrAbertura = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IdCliente = table.Column<int>(type: "int", nullable: false),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    Assunto = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdTipoOcorrencia = table.Column<int>(type: "int", nullable: false),
                    DscTicket = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Prioridade = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, defaultValue: "Normal")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdStatusTicket = table.Column<int>(type: "int", nullable: false),
                    DtHrFinalizacao = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IdTecnicoFinalizacao = table.Column<int>(type: "int", nullable: true),
                    TicketCancelado = table.Column<string>(type: "varchar(1)", maxLength: 1, nullable: false, defaultValue: "N")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ticket", x => new { x.IdEmpresa, x.IdTicket });
                    table.ForeignKey(
                        name: "FK_Ticket_Cliente_IdEmpresa_IdCliente",
                        columns: x => new { x.IdEmpresa, x.IdCliente },
                        principalTable: "Cliente",
                        principalColumns: new[] { "IdEmpresa", "IdCliente" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ticket_Empresa_IdEmpresa",
                        column: x => x.IdEmpresa,
                        principalTable: "Empresa",
                        principalColumn: "IdEmpresa",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ticket_StatusTicket_IdEmpresa_IdStatusTicket",
                        columns: x => new { x.IdEmpresa, x.IdStatusTicket },
                        principalTable: "StatusTicket",
                        principalColumns: new[] { "IdEmpresa", "IdStatusTicket" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ticket_Tecnico_IdEmpresa_IdTecnicoFinalizacao",
                        columns: x => new { x.IdEmpresa, x.IdTecnicoFinalizacao },
                        principalTable: "Tecnico",
                        principalColumns: new[] { "IdEmpresa", "IdTecnico" });
                    table.ForeignKey(
                        name: "FK_Ticket_TipoOcorrencia_IdEmpresa_IdTipoOcorrencia",
                        columns: x => new { x.IdEmpresa, x.IdTipoOcorrencia },
                        principalTable: "TipoOcorrencia",
                        principalColumns: new[] { "IdEmpresa", "IdTipoOcorrencia" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ticket_Usuario_IdEmpresa_IdUsuario",
                        columns: x => new { x.IdEmpresa, x.IdUsuario },
                        principalTable: "Usuario",
                        principalColumns: new[] { "IdEmpresa", "IdUsuario" },
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TicketDocumento",
                columns: table => new
                {
                    IdEmpresa = table.Column<int>(type: "int", nullable: false),
                    IdTicketDocumento = table.Column<int>(type: "int", nullable: false),
                    IdTicket = table.Column<int>(type: "int", nullable: false),
                    NomeArquivo = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CaminhoArquivo = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ContentType = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TamanhoBytes = table.Column<long>(type: "bigint", nullable: true),
                    DtHrUpload = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketDocumento", x => new { x.IdEmpresa, x.IdTicketDocumento });
                    table.ForeignKey(
                        name: "FK_TicketDocumento_Empresa_IdEmpresa",
                        column: x => x.IdEmpresa,
                        principalTable: "Empresa",
                        principalColumn: "IdEmpresa",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TicketDocumento_Ticket_IdEmpresa_IdTicket",
                        columns: x => new { x.IdEmpresa, x.IdTicket },
                        principalTable: "Ticket",
                        principalColumns: new[] { "IdEmpresa", "IdTicket" },
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TicketInteracao",
                columns: table => new
                {
                    IdEmpresa = table.Column<int>(type: "int", nullable: false),
                    IdTicketInteracao = table.Column<int>(type: "int", nullable: false),
                    IdTicket = table.Column<int>(type: "int", nullable: false),
                    DtHrInteracao = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DscInteracao = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdTecnico = table.Column<int>(type: "int", nullable: true),
                    IdUsuario = table.Column<int>(type: "int", nullable: true),
                    Privativo = table.Column<string>(type: "varchar(1)", maxLength: 1, nullable: false, defaultValue: "N")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AguardarInteracaoUsuario = table.Column<string>(type: "varchar(1)", maxLength: 1, nullable: false, defaultValue: "N")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketInteracao", x => new { x.IdEmpresa, x.IdTicketInteracao });
                    table.ForeignKey(
                        name: "FK_TicketInteracao_Empresa_IdEmpresa",
                        column: x => x.IdEmpresa,
                        principalTable: "Empresa",
                        principalColumn: "IdEmpresa",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TicketInteracao_Tecnico_IdEmpresa_IdTecnico",
                        columns: x => new { x.IdEmpresa, x.IdTecnico },
                        principalTable: "Tecnico",
                        principalColumns: new[] { "IdEmpresa", "IdTecnico" });
                    table.ForeignKey(
                        name: "FK_TicketInteracao_Ticket_IdEmpresa_IdTicket",
                        columns: x => new { x.IdEmpresa, x.IdTicket },
                        principalTable: "Ticket",
                        principalColumns: new[] { "IdEmpresa", "IdTicket" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TicketInteracao_Usuario_IdEmpresa_IdUsuario",
                        columns: x => new { x.IdEmpresa, x.IdUsuario },
                        principalTable: "Usuario",
                        principalColumns: new[] { "IdEmpresa", "IdUsuario" });
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_IdEmpresa_IdCliente",
                table: "Usuario",
                columns: new[] { "IdEmpresa", "IdCliente" });

            migrationBuilder.CreateIndex(
                name: "IX_ProdutoXCliente_IdEmpresa_IdCliente",
                table: "ProdutoXCliente",
                columns: new[] { "IdEmpresa", "IdCliente" });

            migrationBuilder.CreateIndex(
                name: "IX_Tecnico_IdEmpresa_IdUsuario",
                table: "Tecnico",
                columns: new[] { "IdEmpresa", "IdUsuario" });

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_IdEmpresa_DtHrAbertura",
                table: "Ticket",
                columns: new[] { "IdEmpresa", "DtHrAbertura" });

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_IdEmpresa_IdCliente",
                table: "Ticket",
                columns: new[] { "IdEmpresa", "IdCliente" });

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_IdEmpresa_IdStatusTicket",
                table: "Ticket",
                columns: new[] { "IdEmpresa", "IdStatusTicket" });

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_IdEmpresa_IdTecnicoFinalizacao",
                table: "Ticket",
                columns: new[] { "IdEmpresa", "IdTecnicoFinalizacao" });

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_IdEmpresa_IdTipoOcorrencia",
                table: "Ticket",
                columns: new[] { "IdEmpresa", "IdTipoOcorrencia" });

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_IdEmpresa_IdUsuario",
                table: "Ticket",
                columns: new[] { "IdEmpresa", "IdUsuario" });

            migrationBuilder.CreateIndex(
                name: "IX_TicketDocumento_IdEmpresa_IdTicket",
                table: "TicketDocumento",
                columns: new[] { "IdEmpresa", "IdTicket" });

            migrationBuilder.CreateIndex(
                name: "IX_TicketInteracao_IdEmpresa_IdTecnico",
                table: "TicketInteracao",
                columns: new[] { "IdEmpresa", "IdTecnico" });

            migrationBuilder.CreateIndex(
                name: "IX_TicketInteracao_IdEmpresa_IdTicket",
                table: "TicketInteracao",
                columns: new[] { "IdEmpresa", "IdTicket" });

            migrationBuilder.CreateIndex(
                name: "IX_TicketInteracao_IdEmpresa_IdUsuario",
                table: "TicketInteracao",
                columns: new[] { "IdEmpresa", "IdUsuario" });

            migrationBuilder.AddForeignKey(
                name: "FK_Usuario_Cliente_IdEmpresa_IdCliente",
                table: "Usuario",
                columns: new[] { "IdEmpresa", "IdCliente" },
                principalTable: "Cliente",
                principalColumns: new[] { "IdEmpresa", "IdCliente" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuario_Cliente_IdEmpresa_IdCliente",
                table: "Usuario");

            migrationBuilder.DropTable(
                name: "ProdutoXCliente");

            migrationBuilder.DropTable(
                name: "TicketDocumento");

            migrationBuilder.DropTable(
                name: "TicketInteracao");

            migrationBuilder.DropTable(
                name: "Produto");

            migrationBuilder.DropTable(
                name: "Ticket");

            migrationBuilder.DropTable(
                name: "Cliente");

            migrationBuilder.DropTable(
                name: "StatusTicket");

            migrationBuilder.DropTable(
                name: "Tecnico");

            migrationBuilder.DropTable(
                name: "TipoOcorrencia");

            migrationBuilder.DropIndex(
                name: "IX_Usuario_IdEmpresa_IdCliente",
                table: "Usuario");
        }
    }
}
