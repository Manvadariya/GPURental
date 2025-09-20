using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GPURental.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    TagId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.TagId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: false),
                    BalanceInCents = table.Column<int>(nullable: false),
                    Timezone = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "GpuListings",
                columns: table => new
                {
                    ListingId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProviderId = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    GpuModel = table.Column<string>(nullable: false),
                    VramInGB = table.Column<int>(nullable: false),
                    RamInGB = table.Column<int>(nullable: false),
                    DiskInGB = table.Column<int>(nullable: false),
                    CpuModel = table.Column<string>(nullable: true),
                    OperatingSystem = table.Column<string>(nullable: true),
                    Location = table.Column<string>(nullable: true),
                    PricePerHourInCents = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GpuListings", x => x.ListingId);
                    table.ForeignKey(
                        name: "FK_GpuListings_Users_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ListingAvailabilities",
                columns: table => new
                {
                    AvailabilityId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ListingId = table.Column<int>(nullable: false),
                    StartAt = table.Column<DateTime>(nullable: false),
                    EndAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListingAvailabilities", x => x.AvailabilityId);
                    table.ForeignKey(
                        name: "FK_ListingAvailabilities_GpuListings_ListingId",
                        column: x => x.ListingId,
                        principalTable: "GpuListings",
                        principalColumn: "ListingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ListingImages",
                columns: table => new
                {
                    ImageId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ListingId = table.Column<int>(nullable: false),
                    ImageUrl = table.Column<string>(nullable: false),
                    AltText = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListingImages", x => x.ImageId);
                    table.ForeignKey(
                        name: "FK_ListingImages_GpuListings_ListingId",
                        column: x => x.ListingId,
                        principalTable: "GpuListings",
                        principalColumn: "ListingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ListingTags",
                columns: table => new
                {
                    ListingId = table.Column<int>(nullable: false),
                    TagId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListingTags", x => new { x.ListingId, x.TagId });
                    table.ForeignKey(
                        name: "FK_ListingTags_GpuListings_ListingId",
                        column: x => x.ListingId,
                        principalTable: "GpuListings",
                        principalColumn: "ListingId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ListingTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "TagId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RentalJobs",
                columns: table => new
                {
                    RentalJobId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ListingId = table.Column<int>(nullable: false),
                    RenterId = table.Column<int>(nullable: false),
                    ActualStartAt = table.Column<DateTime>(nullable: true),
                    ActualEndAt = table.Column<DateTime>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    FinalChargeInCents = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentalJobs", x => x.RentalJobId);
                    table.ForeignKey(
                        name: "FK_RentalJobs_GpuListings_ListingId",
                        column: x => x.ListingId,
                        principalTable: "GpuListings",
                        principalColumn: "ListingId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RentalJobs_Users_RenterId",
                        column: x => x.RenterId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Disputes",
                columns: table => new
                {
                    DisputeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RentalJobId = table.Column<int>(nullable: false),
                    RaisedByUserId = table.Column<int>(nullable: false),
                    Reason = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Disputes", x => x.DisputeId);
                    table.ForeignKey(
                        name: "FK_Disputes_Users_RaisedByUserId",
                        column: x => x.RaisedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Disputes_RentalJobs_RentalJobId",
                        column: x => x.RentalJobId,
                        principalTable: "RentalJobs",
                        principalColumn: "RentalJobId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    InvoiceId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RentalJobId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    IssueDate = table.Column<DateTime>(nullable: false),
                    TotalInCents = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.InvoiceId);
                    table.ForeignKey(
                        name: "FK_Invoices_RentalJobs_RentalJobId",
                        column: x => x.RentalJobId,
                        principalTable: "RentalJobs",
                        principalColumn: "RentalJobId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Invoices_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobLogs",
                columns: table => new
                {
                    LogId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RentalJobId = table.Column<int>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    Message = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobLogs", x => x.LogId);
                    table.ForeignKey(
                        name: "FK_JobLogs_RentalJobs_RentalJobId",
                        column: x => x.RentalJobId,
                        principalTable: "RentalJobs",
                        principalColumn: "RentalJobId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobTelemetries",
                columns: table => new
                {
                    TelemetryId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RentalJobId = table.Column<int>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    GpuUtilizationPercent = table.Column<int>(nullable: false),
                    GpuMemoryUsedMB = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobTelemetries", x => x.TelemetryId);
                    table.ForeignKey(
                        name: "FK_JobTelemetries_RentalJobs_RentalJobId",
                        column: x => x.RentalJobId,
                        principalTable: "RentalJobs",
                        principalColumn: "RentalJobId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    ReviewId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RentalJobId = table.Column<int>(nullable: false),
                    ListingId = table.Column<int>(nullable: false),
                    AuthorId = table.Column<int>(nullable: false),
                    Rating = table.Column<int>(nullable: false),
                    Comment = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.ReviewId);
                    table.ForeignKey(
                        name: "FK_Reviews_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reviews_GpuListings_ListingId",
                        column: x => x.ListingId,
                        principalTable: "GpuListings",
                        principalColumn: "ListingId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_RentalJobs_RentalJobId",
                        column: x => x.RentalJobId,
                        principalTable: "RentalJobs",
                        principalColumn: "RentalJobId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WalletLedgerEntries",
                columns: table => new
                {
                    LedgerId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: false),
                    RentalJobId = table.Column<int>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    AmountInCents = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletLedgerEntries", x => x.LedgerId);
                    table.ForeignKey(
                        name: "FK_WalletLedgerEntries_RentalJobs_RentalJobId",
                        column: x => x.RentalJobId,
                        principalTable: "RentalJobs",
                        principalColumn: "RentalJobId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WalletLedgerEntries_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Disputes_RaisedByUserId",
                table: "Disputes",
                column: "RaisedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Disputes_RentalJobId",
                table: "Disputes",
                column: "RentalJobId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GpuListings_ProviderId",
                table: "GpuListings",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_RentalJobId",
                table: "Invoices",
                column: "RentalJobId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_UserId",
                table: "Invoices",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_JobLogs_RentalJobId",
                table: "JobLogs",
                column: "RentalJobId");

            migrationBuilder.CreateIndex(
                name: "IX_JobTelemetries_RentalJobId",
                table: "JobTelemetries",
                column: "RentalJobId");

            migrationBuilder.CreateIndex(
                name: "IX_ListingAvailabilities_ListingId",
                table: "ListingAvailabilities",
                column: "ListingId");

            migrationBuilder.CreateIndex(
                name: "IX_ListingImages_ListingId",
                table: "ListingImages",
                column: "ListingId");

            migrationBuilder.CreateIndex(
                name: "IX_ListingTags_TagId",
                table: "ListingTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_RentalJobs_ListingId",
                table: "RentalJobs",
                column: "ListingId");

            migrationBuilder.CreateIndex(
                name: "IX_RentalJobs_RenterId",
                table: "RentalJobs",
                column: "RenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_AuthorId",
                table: "Reviews",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ListingId",
                table: "Reviews",
                column: "ListingId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_RentalJobId",
                table: "Reviews",
                column: "RentalJobId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WalletLedgerEntries_RentalJobId",
                table: "WalletLedgerEntries",
                column: "RentalJobId");

            migrationBuilder.CreateIndex(
                name: "IX_WalletLedgerEntries_UserId",
                table: "WalletLedgerEntries",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Disputes");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "JobLogs");

            migrationBuilder.DropTable(
                name: "JobTelemetries");

            migrationBuilder.DropTable(
                name: "ListingAvailabilities");

            migrationBuilder.DropTable(
                name: "ListingImages");

            migrationBuilder.DropTable(
                name: "ListingTags");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "WalletLedgerEntries");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "RentalJobs");

            migrationBuilder.DropTable(
                name: "GpuListings");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
