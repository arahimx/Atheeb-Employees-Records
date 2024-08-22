using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class sfsfs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserInfo_Skills_SkillId",
                table: "UserInfo");

            migrationBuilder.DropForeignKey(
                name: "FK_UserInfo_UserCertificates_CertificatesId",
                table: "UserInfo");

            migrationBuilder.DropIndex(
                name: "IX_UserInfo_CertificatesId",
                table: "UserInfo");

            migrationBuilder.DropIndex(
                name: "IX_UserInfo_SkillId",
                table: "UserInfo");

            migrationBuilder.DropColumn(
                name: "CertificatesId",
                table: "UserInfo");

            migrationBuilder.DropColumn(
                name: "SkillId",
                table: "UserInfo");

            migrationBuilder.AddColumn<string>(
                name: "CertificateAttachment",
                table: "UserInfo",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CertificateName",
                table: "UserInfo",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CertificateAttachment",
                table: "UserInfo");

            migrationBuilder.DropColumn(
                name: "CertificateName",
                table: "UserInfo");

            migrationBuilder.AddColumn<int>(
                name: "CertificatesId",
                table: "UserInfo",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SkillId",
                table: "UserInfo",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UserInfo_CertificatesId",
                table: "UserInfo",
                column: "CertificatesId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInfo_SkillId",
                table: "UserInfo",
                column: "SkillId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserInfo_Skills_SkillId",
                table: "UserInfo",
                column: "SkillId",
                principalTable: "Skills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserInfo_UserCertificates_CertificatesId",
                table: "UserInfo",
                column: "CertificatesId",
                principalTable: "UserCertificates",
                principalColumn: "Id");
        }
    }
}
