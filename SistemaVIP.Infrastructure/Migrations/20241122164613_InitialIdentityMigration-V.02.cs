using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaVIP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialIdentityMigrationV02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoleClaims_Roles_RoleId",
                schema: "SistemaVIP",
                table: "RoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_UserClaims_Users_UserId",
                schema: "SistemaVIP",
                table: "UserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLogins_Users_UserId",
                schema: "SistemaVIP",
                table: "UserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Roles_RoleId",
                schema: "SistemaVIP",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Users_UserId",
                schema: "SistemaVIP",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTokens_Users_UserId",
                schema: "SistemaVIP",
                table: "UserTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserTokens",
                schema: "SistemaVIP",
                table: "UserTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                schema: "SistemaVIP",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRoles",
                schema: "SistemaVIP",
                table: "UserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserLogins",
                schema: "SistemaVIP",
                table: "UserLogins");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserClaims",
                schema: "SistemaVIP",
                table: "UserClaims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Roles",
                schema: "SistemaVIP",
                table: "Roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoleClaims",
                schema: "SistemaVIP",
                table: "RoleClaims");

            migrationBuilder.RenameTable(
                name: "UserTokens",
                schema: "SistemaVIP",
                newName: "AspNetUserTokens",
                newSchema: "SistemaVIP");

            migrationBuilder.RenameTable(
                name: "Users",
                schema: "SistemaVIP",
                newName: "AspNetUsers",
                newSchema: "SistemaVIP");

            migrationBuilder.RenameTable(
                name: "UserRoles",
                schema: "SistemaVIP",
                newName: "AspNetUserRoles",
                newSchema: "SistemaVIP");

            migrationBuilder.RenameTable(
                name: "UserLogins",
                schema: "SistemaVIP",
                newName: "AspNetUserLogins",
                newSchema: "SistemaVIP");

            migrationBuilder.RenameTable(
                name: "UserClaims",
                schema: "SistemaVIP",
                newName: "AspNetUserClaims",
                newSchema: "SistemaVIP");

            migrationBuilder.RenameTable(
                name: "Roles",
                schema: "SistemaVIP",
                newName: "AspNetRoles",
                newSchema: "SistemaVIP");

            migrationBuilder.RenameTable(
                name: "RoleClaims",
                schema: "SistemaVIP",
                newName: "AspNetRoleClaims",
                newSchema: "SistemaVIP");

            migrationBuilder.RenameIndex(
                name: "IX_UserRoles_RoleId",
                schema: "SistemaVIP",
                table: "AspNetUserRoles",
                newName: "IX_AspNetUserRoles_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_UserLogins_UserId",
                schema: "SistemaVIP",
                table: "AspNetUserLogins",
                newName: "IX_AspNetUserLogins_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserClaims_UserId",
                schema: "SistemaVIP",
                table: "AspNetUserClaims",
                newName: "IX_AspNetUserClaims_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_RoleClaims_RoleId",
                schema: "SistemaVIP",
                table: "AspNetRoleClaims",
                newName: "IX_AspNetRoleClaims_RoleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserTokens",
                schema: "SistemaVIP",
                table: "AspNetUserTokens",
                columns: new[] { "UserId", "LoginProvider", "Name" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUsers",
                schema: "SistemaVIP",
                table: "AspNetUsers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserRoles",
                schema: "SistemaVIP",
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserLogins",
                schema: "SistemaVIP",
                table: "AspNetUserLogins",
                columns: new[] { "LoginProvider", "ProviderKey" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUserClaims",
                schema: "SistemaVIP",
                table: "AspNetUserClaims",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetRoles",
                schema: "SistemaVIP",
                table: "AspNetRoles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetRoleClaims",
                schema: "SistemaVIP",
                table: "AspNetRoleClaims",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                schema: "SistemaVIP",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalSchema: "SistemaVIP",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                schema: "SistemaVIP",
                table: "AspNetUserClaims",
                column: "UserId",
                principalSchema: "SistemaVIP",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                schema: "SistemaVIP",
                table: "AspNetUserLogins",
                column: "UserId",
                principalSchema: "SistemaVIP",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                schema: "SistemaVIP",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalSchema: "SistemaVIP",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                schema: "SistemaVIP",
                table: "AspNetUserRoles",
                column: "UserId",
                principalSchema: "SistemaVIP",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                schema: "SistemaVIP",
                table: "AspNetUserTokens",
                column: "UserId",
                principalSchema: "SistemaVIP",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                schema: "SistemaVIP",
                table: "AspNetRoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                schema: "SistemaVIP",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                schema: "SistemaVIP",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                schema: "SistemaVIP",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                schema: "SistemaVIP",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                schema: "SistemaVIP",
                table: "AspNetUserTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserTokens",
                schema: "SistemaVIP",
                table: "AspNetUserTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUsers",
                schema: "SistemaVIP",
                table: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserRoles",
                schema: "SistemaVIP",
                table: "AspNetUserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserLogins",
                schema: "SistemaVIP",
                table: "AspNetUserLogins");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUserClaims",
                schema: "SistemaVIP",
                table: "AspNetUserClaims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetRoles",
                schema: "SistemaVIP",
                table: "AspNetRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetRoleClaims",
                schema: "SistemaVIP",
                table: "AspNetRoleClaims");

            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                schema: "SistemaVIP",
                newName: "UserTokens",
                newSchema: "SistemaVIP");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                schema: "SistemaVIP",
                newName: "Users",
                newSchema: "SistemaVIP");

            migrationBuilder.RenameTable(
                name: "AspNetUserRoles",
                schema: "SistemaVIP",
                newName: "UserRoles",
                newSchema: "SistemaVIP");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                schema: "SistemaVIP",
                newName: "UserLogins",
                newSchema: "SistemaVIP");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                schema: "SistemaVIP",
                newName: "UserClaims",
                newSchema: "SistemaVIP");

            migrationBuilder.RenameTable(
                name: "AspNetRoles",
                schema: "SistemaVIP",
                newName: "Roles",
                newSchema: "SistemaVIP");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaims",
                schema: "SistemaVIP",
                newName: "RoleClaims",
                newSchema: "SistemaVIP");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserRoles_RoleId",
                schema: "SistemaVIP",
                table: "UserRoles",
                newName: "IX_UserRoles_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserLogins_UserId",
                schema: "SistemaVIP",
                table: "UserLogins",
                newName: "IX_UserLogins_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUserClaims_UserId",
                schema: "SistemaVIP",
                table: "UserClaims",
                newName: "IX_UserClaims_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                schema: "SistemaVIP",
                table: "RoleClaims",
                newName: "IX_RoleClaims_RoleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserTokens",
                schema: "SistemaVIP",
                table: "UserTokens",
                columns: new[] { "UserId", "LoginProvider", "Name" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                schema: "SistemaVIP",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRoles",
                schema: "SistemaVIP",
                table: "UserRoles",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserLogins",
                schema: "SistemaVIP",
                table: "UserLogins",
                columns: new[] { "LoginProvider", "ProviderKey" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserClaims",
                schema: "SistemaVIP",
                table: "UserClaims",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Roles",
                schema: "SistemaVIP",
                table: "Roles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoleClaims",
                schema: "SistemaVIP",
                table: "RoleClaims",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleClaims_Roles_RoleId",
                schema: "SistemaVIP",
                table: "RoleClaims",
                column: "RoleId",
                principalSchema: "SistemaVIP",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserClaims_Users_UserId",
                schema: "SistemaVIP",
                table: "UserClaims",
                column: "UserId",
                principalSchema: "SistemaVIP",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLogins_Users_UserId",
                schema: "SistemaVIP",
                table: "UserLogins",
                column: "UserId",
                principalSchema: "SistemaVIP",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Roles_RoleId",
                schema: "SistemaVIP",
                table: "UserRoles",
                column: "RoleId",
                principalSchema: "SistemaVIP",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Users_UserId",
                schema: "SistemaVIP",
                table: "UserRoles",
                column: "UserId",
                principalSchema: "SistemaVIP",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserTokens_Users_UserId",
                schema: "SistemaVIP",
                table: "UserTokens",
                column: "UserId",
                principalSchema: "SistemaVIP",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
