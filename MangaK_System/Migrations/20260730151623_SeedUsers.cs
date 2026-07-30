using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MangaK_System.Migrations
{
    /// <inheritdoc />
    public partial class SeedUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AuthorName", "AvatarUrl", "Bio", "CreatedAt", "Email", "FirstName", "IsDeleted", "LastName", "PasswordHash", "Phone", "Role", "Status", "SupervisorId", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000000"), null, null, "Handles account management.", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "admin.mangak@gmail.com", "Admin", false, "System", "$2a$11$ulBAY43Pfpj.IP0WQIuFrOAm9ZmahfWnc.Ugybp4J8D67/CLniMC6", "0902234506", "Admin", "Active", null, null },
                    { new Guid("20000000-0000-0000-0000-000000000000"), null, null, "Handles account management.", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "mangaksystem.admin@gmail.com", "Mangaka", false, "System", "$2a$11$cLPq8siwUg2S5V4XNaiM8Oc6B1pe6BgB1gJdYHmUfgPkaODod1EIi", "0865326785", "Admin", "Active", null, null },
                    { new Guid("30000000-0000-0000-0000-000000000000"), null, null, "Oversees final approval and publication planning.", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "board.mangak@gmail.com", "Editorial", false, "Board", "$2a$11$z8YIdqtAEzYhflEdzA2BueXvvDhFkCrtQl2m.gN38c25UnQYsIyf.", "0902234507", "Editorial", "Active", null, null },
                    { new Guid("90000000-0000-0000-0000-000000000000"), null, null, "Provides editorial support and assists with manga publication workflows.", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "xuanhieubato@gmail.com", "Aoi", false, "Yamamoto", "$2a$11$8tNqQlqdW.qWeSlHD9bxkOzpJNt/gvJEPuHZuaU5WyyA9VZE6doTq", "0902234501", "Assistant", "Active", null, null },
                    { new Guid("a0000000-0000-0000-0000-000000000000"), null, null, "Coordinates schedules and communicates with artists and publishers.", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "daichi.mori@gmail.com", "Daichi", false, "Mori", "$2a$11$N.wGhUp5lKhnoafvp3JwWuHXF/IY/MWi4LEl2aErxJ704/RAgGdUG", "0902234502", "Assistant", "Active", null, null },
                    { new Guid("b0000000-0000-0000-0000-000000000000"), null, null, "Helps manage manuscripts, deadlines, and project documentation.", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "kimngan21102020@gmail.com", "Emi", false, "Shimizu", "$2a$11$XbJQdZPicvuHm.igcqBBl.sDGAIjWi1ZX8m3qtxxBqDPICTAx4eui", "0902234503", "Assistant", "Active", null, null },
                    { new Guid("c0000000-0000-0000-0000-000000000000"), null, null, "Supports creative teams by organizing assets and tracking progress.", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "riku.hayashi@gmail.com", "Riku", false, "Hayashi", "$2a$11$ln7.kBOWVJ8qWYtppfT59.GiIjiLAmxwzH1iRkDA/AJ7rt8t5E8Dy", "0902234504", "Assistant", "Active", null, null },
                    { new Guid("d0000000-0000-0000-0000-000000000000"), null, null, "Assists with content reviews and communication between departments.", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "nozomi.ishikawa@gmail.com", "Nozomi", false, "Ishikawa", "$2a$11$j2ng4tXNrfVPDciPtZtBmuLE5OkzsqYLoFbThEjZu9JXaYvUoAq/q", "0902234505", "Assistant", "Active", null, null },
                    { new Guid("e0000000-0000-0000-0000-000000000000"), null, null, "Handles administrative tasks and ensures smooth production coordination.", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "sora.okamoto@gmail.com", "Sora", false, "Okamoto", "$2a$11$zpqD/2IVAzfYzhO1whgO8uui3rAjlAriQt7TuJtA9PCddG9zIANjm", "0902234506", "Assistant", "Active", null, null },
                    { new Guid("e24a52c3-96cb-4034-88db-4e1bba697841"), null, null, "Official Tantou account.", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "tranmaiconghung@gmail.com", "Togoro", false, "Mori", "$2a$11$wqkWTRk76JOZgoNgmeaJvutSNAGJAhIDX3eu/8cLWYbSVFoyObrdu", "0901234567", "Tantou", "Active", null, null },
                    { new Guid("f39b61d2-74ba-42a1-9a7c-3b0acc786520"), null, null, "Fantasy editor and content quality.", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "michael.anderson@gmail.com", "Mika", false, "Ayashi", "$2a$11$k0aUHB80AaaAOoBnWAViwODti2AF4lKFw2JbsiBM/ucaYe9wnl.MS", "0975633219", "Tantou", "Active", null, null },
                    { new Guid("40000000-0000-0000-0000-000000000000"), "Akira Koba", null, "A versatile manga creator known for sci-fi adventures and immersive world-building.", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "nhathanhthinguyen@gmail.com", "Akira", false, "Kobayashi", "$2a$11$7N/oOICse8bsHRVjw0WiDejq8wFTicRnXEjhmBxd2N4Sl/Qco1pdC", "0901234565", "Mangaka", "Active", new Guid("f39b61d2-74ba-42a1-9a7c-3b0acc786520"), null },
                    { new Guid("50000000-0000-0000-0000-000000000000"), "Haru Sato", null, "A manga artist specializing in action and adventure stories with dynamic illustrations.", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "haruto.sato@gmail.com", "Haruto", false, "Sato", "$2a$11$tZME3QovDc/3FI4oOx6uvu7tmMBYbhtaNYvYGq7TGJvVDo0oKCgxa", "0901234561", "Mangaka", "Active", new Guid("f39b61d2-74ba-42a1-9a7c-3b0acc786520"), null },
                    { new Guid("60000000-0000-0000-0000-000000000000"), "Ren Taka", null, "Passionate about psychological mysteries and creating emotionally complex characters.", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "tuanle17082k5@gmail.com", "Ren", false, "Takahashi", "$2a$11$Ubj63s7D/C9GGx6WDgbz5.iXipfT1iW1pauB11LavweRUvRyI3v6m", "0901234562", "Mangaka", "Active", new Guid("f39b61d2-74ba-42a1-9a7c-3b0acc786520"), null },
                    { new Guid("70000000-0000-0000-0000-000000000000"), "Yuki Naka", null, "Creates heartwarming slice-of-life and romance manga inspired by everyday Japanese culture.", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "yuki.nakamura@gmail.com", "Yuki", false, "Nakamura", "$2a$11$QRutO6OZfCho8czGilByWuyRmRlQyxHSlGpZzP549OYToU2EjQeuG", "0901234563", "Mangaka", "Active", new Guid("e24a52c3-96cb-4034-88db-4e1bba697841"), null },
                    { new Guid("80000000-0000-0000-0000-000000000000"), "Kaito Fuji", null, "Focuses on dark fantasy and supernatural worlds with highly detailed artwork.", new DateTimeOffset(new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "kaito.fujimoto@gmail.com", "Kaito", false, "Fujimoto", "$2a$11$mivf3Nx71qxFZfi1bYt/Uu5IwYegHZXo8CI3oGES9yk2Wak20opLy", "0901234564", "Mangaka", "Active", new Guid("e24a52c3-96cb-4034-88db-4e1bba697841"), null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("80000000-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("90000000-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("b0000000-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("c0000000-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("d0000000-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("e0000000-0000-0000-0000-000000000000"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("e24a52c3-96cb-4034-88db-4e1bba697841"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("f39b61d2-74ba-42a1-9a7c-3b0acc786520"));
        }
    }
}
