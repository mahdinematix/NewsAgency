using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsAgency.Infra.Data.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class CreationDatePropAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CategoryTitle",
                table: "Categories",
                newName: "Title");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "Categories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "Authors",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "Articles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "Articles");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Categories",
                newName: "CategoryTitle");
        }
    }
}
