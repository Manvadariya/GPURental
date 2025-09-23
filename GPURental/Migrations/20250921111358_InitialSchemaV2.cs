using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GPURental.Migrations
{
    public partial class InitialSchemaV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    FullName = table.Column<string>(nullable: true),
                    BalanceInCents = table.Column<int>(nullable: false),
                    Timezone = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    TagId = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.TagId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(maxLength: 128, nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GpuListings",
                columns: table => new
                {
                    ListingId = table.Column<string>(nullable: false),
                    ProviderId = table.Column<string>(nullable: false),
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
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ImagePath = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GpuListings", x => x.ListingId);
                    table.ForeignKey(
                        name: "FK_GpuListings_AspNetUsers_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ListingAvailabilities",
                columns: table => new
                {
                    AvailabilityId = table.Column<string>(nullable: false),
                    ListingId = table.Column<string>(nullable: false),
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
                name: "ListingTags",
                columns: table => new
                {
                    ListingId = table.Column<string>(nullable: false),
                    TagId = table.Column<string>(nullable: false)
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
                    RentalJobId = table.Column<string>(nullable: false),
                    ListingId = table.Column<string>(nullable: false),
                    RenterId = table.Column<string>(nullable: false),
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
                        name: "FK_RentalJobs_AspNetUsers_RenterId",
                        column: x => x.RenterId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Disputes",
                columns: table => new
                {
                    DisputeId = table.Column<string>(nullable: false),
                    RentalJobId = table.Column<string>(nullable: false),
                    RaisedByUserId = table.Column<string>(nullable: false),
                    Reason = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Disputes", x => x.DisputeId);
                    table.ForeignKey(
                        name: "FK_Disputes_AspNetUsers_RaisedByUserId",
                        column: x => x.RaisedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
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
                    InvoiceId = table.Column<string>(nullable: false),
                    RentalJobId = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
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
                        name: "FK_Invoices_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobLogs",
                columns: table => new
                {
                    LogId = table.Column<string>(nullable: false),
                    RentalJobId = table.Column<string>(nullable: false),
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
                    TelemetryId = table.Column<string>(nullable: false),
                    RentalJobId = table.Column<string>(nullable: false),
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
                    ReviewId = table.Column<string>(nullable: false),
                    RentalJobId = table.Column<string>(nullable: false),
                    ListingId = table.Column<string>(nullable: false),
                    AuthorId = table.Column<string>(nullable: false),
                    Rating = table.Column<int>(nullable: false),
                    Comment = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.ReviewId);
                    table.ForeignKey(
                        name: "FK_Reviews_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
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
                    LedgerId = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    RentalJobId = table.Column<string>(nullable: true),
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
                        name: "FK_WalletLedgerEntries_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

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
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

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
                name: "ListingTags");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "WalletLedgerEntries");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "RentalJobs");

            migrationBuilder.DropTable(
                name: "GpuListings");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
