using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dig.API.Migrations
{
    public partial class Auth2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SystemUsers_UserAuthKeys_AuthKeyId",
                table: "SystemUsers");

            migrationBuilder.DropIndex(
                name: "IX_SystemUsers_AuthKeyId",
                table: "SystemUsers");

            migrationBuilder.DropColumn(
                name: "AuthKeyId",
                table: "SystemUsers");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "SystemUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAuthKeys_SystemUsers_Id",
                table: "UserAuthKeys",
                column: "Id",
                principalTable: "SystemUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAuthKeys_SystemUsers_Id",
                table: "UserAuthKeys");

            migrationBuilder.AddColumn<Guid>(
                name: "AuthKeyId",
                table: "SystemUsers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "SystemUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_SystemUsers_AuthKeyId",
                table: "SystemUsers",
                column: "AuthKeyId");

            migrationBuilder.AddForeignKey(
                name: "FK_SystemUsers_UserAuthKeys_AuthKeyId",
                table: "SystemUsers",
                column: "AuthKeyId",
                principalTable: "UserAuthKeys",
                principalColumn: "Id");
        }
    }
}
