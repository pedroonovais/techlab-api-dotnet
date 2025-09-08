using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LEITURARFID",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    RfidId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    SensorId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LEITURARFID", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PATIO",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    nome = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    localizacao = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    capacidadeTotal = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    vagasDisponiveis = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    descricao = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    dataCadastro = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    dataAtualizacao = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PATIO", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "SENSOR",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    codigoIdentificacao = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    tipo = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    localizacaoFisica = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    status = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    dtInstalacao = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    dtUltimaLeitura = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SENSOR", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "USUARIO",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    nome = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    email = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    senha = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    cpf = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    status = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    perfil = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    area = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    dtCriacao = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    dtAlteracao = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USUARIO", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "MOTO",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    marca = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    modelo = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    placa = table.Column<string>(type: "NVARCHAR2(10)", nullable: false),
                    chassi = table.Column<string>(type: "NVARCHAR2(200)", nullable: false),
                    motor = table.Column<string>(type: "NVARCHAR2(200)", nullable: false),
                    imeiIot = table.Column<string>(type: "NVARCHAR2(200)", nullable: false),
                    rfId = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    dataCadastro = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    dataAtualizacao = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MOTO", x => x.id);
                    table.ForeignKey(
                        name: "FK_MOTO_RFID_rfId",
                        column: x => x.rfId,
                        principalTable: "rfId",
                        principalColumn: "id");
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LEITURARFID");

            migrationBuilder.DropTable(
                name: "MOTO");

            migrationBuilder.DropTable(
                name: "SENSOR");

            migrationBuilder.DropTable(
                name: "USUARIO");

            migrationBuilder.DropTable(
                name: "PATIO");
        }
    }
}
