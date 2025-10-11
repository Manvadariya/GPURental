using Microsoft.EntityFrameworkCore.Migrations;

namespace GPURental.Migrations
{
    public partial class RefactorToINRAndSeconds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountInCents",
                table: "WalletLedgerEntries");

            migrationBuilder.DropColumn(
                name: "FinalChargeInCents",
                table: "RentalJobs");

            migrationBuilder.DropColumn(
                name: "PricePerHourInCents",
                table: "GpuListings");

            migrationBuilder.DropColumn(
                name: "BalanceInCents",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<decimal>(
                name: "AmountInINR",
                table: "WalletLedgerEntries",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FinalChargeInINR",
                table: "RentalJobs",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerHourInINR",
                table: "GpuListings",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "BalanceInINR",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a18be9c0-aa65-4af8-bd17-00bd9344e575",
                column: "ConcurrencyStamp",
                value: "eb7bdfbf-41a9-4740-ad91-768abe365681");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a18be9c0-aa65-4af8-bd17-00bd9344e576",
                column: "ConcurrencyStamp",
                value: "7e79a2d5-d835-46ae-951f-8d8349776b71");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a18be9c0-aa65-4af8-bd17-00bd9344e577",
                column: "ConcurrencyStamp",
                value: "6b80e6e9-cfa8-4111-a2e0-6b0b4619020a");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountInINR",
                table: "WalletLedgerEntries");

            migrationBuilder.DropColumn(
                name: "FinalChargeInINR",
                table: "RentalJobs");

            migrationBuilder.DropColumn(
                name: "PricePerHourInINR",
                table: "GpuListings");

            migrationBuilder.DropColumn(
                name: "BalanceInINR",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "AmountInCents",
                table: "WalletLedgerEntries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FinalChargeInCents",
                table: "RentalJobs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PricePerHourInCents",
                table: "GpuListings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BalanceInCents",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a18be9c0-aa65-4af8-bd17-00bd9344e575",
                column: "ConcurrencyStamp",
                value: "0383763f-ede1-47ab-8e29-c0b1bdf774dc");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a18be9c0-aa65-4af8-bd17-00bd9344e576",
                column: "ConcurrencyStamp",
                value: "3c44d0ce-838b-48ec-9d55-e0182ca864fd");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a18be9c0-aa65-4af8-bd17-00bd9344e577",
                column: "ConcurrencyStamp",
                value: "d8e1ba03-09de-4b81-8d49-186b2ba38d86");
        }
    }
}
