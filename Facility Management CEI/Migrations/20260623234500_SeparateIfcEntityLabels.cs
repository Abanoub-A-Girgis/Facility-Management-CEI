using Facility_Management_CEI.IdentityDb;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Facility_Management_CEI.Migrations
{
    [DbContext(typeof(ApplicationDBContext))]
    [Migration("20260623234500_SeparateIfcEntityLabels")]
    public partial class SeparateIfcEntityLabels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>("IfcEntityLabel", "Buildings", nullable: false, defaultValue: 0);
            migrationBuilder.AddColumn<int>("IfcEntityLabel", "Floors", nullable: false, defaultValue: 0);
            migrationBuilder.AddColumn<int>("IfcEntityLabel", "Spaces", nullable: false, defaultValue: 0);
            migrationBuilder.AddColumn<int>("IfcEntityLabel", "Assets", nullable: false, defaultValue: 0);
            migrationBuilder.AddColumn<int>("IfcEntityLabel", "Sensors", nullable: false, defaultValue: 0);

            migrationBuilder.Sql("UPDATE Buildings SET IfcEntityLabel = Id");
            migrationBuilder.Sql("UPDATE Floors SET IfcEntityLabel = Id");
            migrationBuilder.Sql("UPDATE Spaces SET IfcEntityLabel = Id");
            migrationBuilder.Sql("UPDATE Assets SET IfcEntityLabel = Id");
            migrationBuilder.Sql("UPDATE Sensors SET IfcEntityLabel = Id");

            migrationBuilder.CreateIndex(
                name: "IX_Floors_BuildingId_IfcEntityLabel",
                table: "Floors",
                columns: new[] { "BuildingId", "IfcEntityLabel" },
                unique: true);
            migrationBuilder.CreateIndex(
                name: "IX_Spaces_FloorId_IfcEntityLabel",
                table: "Spaces",
                columns: new[] { "FloorId", "IfcEntityLabel" });
            migrationBuilder.CreateIndex(
                name: "IX_Assets_FloorId_IfcEntityLabel",
                table: "Assets",
                columns: new[] { "FloorId", "IfcEntityLabel" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex("IX_Floors_BuildingId_IfcEntityLabel", "Floors");
            migrationBuilder.DropIndex("IX_Spaces_FloorId_IfcEntityLabel", "Spaces");
            migrationBuilder.DropIndex("IX_Assets_FloorId_IfcEntityLabel", "Assets");
            migrationBuilder.DropColumn("IfcEntityLabel", "Buildings");
            migrationBuilder.DropColumn("IfcEntityLabel", "Floors");
            migrationBuilder.DropColumn("IfcEntityLabel", "Spaces");
            migrationBuilder.DropColumn("IfcEntityLabel", "Assets");
            migrationBuilder.DropColumn("IfcEntityLabel", "Sensors");
        }
    }
}
