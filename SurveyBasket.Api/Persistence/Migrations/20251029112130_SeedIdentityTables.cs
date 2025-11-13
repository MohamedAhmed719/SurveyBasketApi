using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SurveyBasket.Api.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedIdentityTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "IsDefault", "IsDeleted", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "8ce85fb2-4e25-4648-9023-8bda528e7912", "c93aca9d-1baf-4900-87e3-45723331d786", false, false, "Admin", "ADMIN" },
                    { "c2553eef-3a19-4d18-b647-c5e7faa98f69", "83fbd714-b5fb-49be-bce4-217056af3279", true, false, "Member", "MEMBER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "0a3b72df-5947-43a5-9dc7-b033a002776a", 0, "5e6e4bca-7b2a-48af-b16c-3ab861ed1be1", "Admin@survey-baskey.com", true, "Mohamed", "Ahmed", false, null, "ADMIN@SURVEY-BASKEY.COM", "ADMIN@SURVEY-BASKEY.COM", "AQAAAAIAAYagAAAAEA/5I27Ux3pBwyIzegs3bijI8PEVgqquVoOIQmLwC+488rRvQ8kpj7QnbP1Fo8HEhw==", null, false, "9B11B0CC788146C9907EA5E9D6581BB1", false, "Admin@survey-baskey.com" });

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { 1, "permissions", "polls:read", "8ce85fb2-4e25-4648-9023-8bda528e7912" },
                    { 2, "permissions", "polls:add", "8ce85fb2-4e25-4648-9023-8bda528e7912" },
                    { 3, "permissions", "polls:update", "8ce85fb2-4e25-4648-9023-8bda528e7912" },
                    { 4, "permissions", "polls:delete", "8ce85fb2-4e25-4648-9023-8bda528e7912" },
                    { 5, "permissions", "questions:read", "8ce85fb2-4e25-4648-9023-8bda528e7912" },
                    { 6, "permissions", "questions:add", "8ce85fb2-4e25-4648-9023-8bda528e7912" },
                    { 7, "permissions", "questions:update", "8ce85fb2-4e25-4648-9023-8bda528e7912" },
                    { 8, "permissions", "users:read", "8ce85fb2-4e25-4648-9023-8bda528e7912" },
                    { 9, "permissions", "users:add", "8ce85fb2-4e25-4648-9023-8bda528e7912" },
                    { 10, "permissions", "users:update", "8ce85fb2-4e25-4648-9023-8bda528e7912" },
                    { 11, "permissions", "roles:read", "8ce85fb2-4e25-4648-9023-8bda528e7912" },
                    { 12, "permissions", "roles:add", "8ce85fb2-4e25-4648-9023-8bda528e7912" },
                    { 13, "permissions", "roles:update", "8ce85fb2-4e25-4648-9023-8bda528e7912" },
                    { 14, "permissions", "results:read", "8ce85fb2-4e25-4648-9023-8bda528e7912" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "8ce85fb2-4e25-4648-9023-8bda528e7912", "0a3b72df-5947-43a5-9dc7-b033a002776a" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c2553eef-3a19-4d18-b647-c5e7faa98f69");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "8ce85fb2-4e25-4648-9023-8bda528e7912", "0a3b72df-5947-43a5-9dc7-b033a002776a" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8ce85fb2-4e25-4648-9023-8bda528e7912");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "0a3b72df-5947-43a5-9dc7-b033a002776a");
        }
    }
}
