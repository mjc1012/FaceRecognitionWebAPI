using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FaceRecognitionWebAPI.Migrations
{
    public partial class changetoken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Token",
                table: "Persons",
                newName: "AccessToken");

            migrationBuilder.InsertData(
                table: "Persons",
                columns: new[] { "Id", "AccessToken", "FirstName", "LastName", "MiddleName", "Password", "RefreshToken", "RefreshTokenExpiryTime", "ValidIdNumber" },
                values: new object[] { 1, null, "Admin", "Admin", "", "g66lU9EglPcZo3wRuGi9ZNO63eutXzEG+WxTPvyOLOE=", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Admin" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.RenameColumn(
                name: "AccessToken",
                table: "Persons",
                newName: "Token");
        }
    }
}
