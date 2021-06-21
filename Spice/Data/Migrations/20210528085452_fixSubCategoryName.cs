using Microsoft.EntityFrameworkCore.Migrations;

namespace Spice.Data.Migrations
{
    public partial class fixSubCategoryName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubCaegory_Category_CategoryId",
                table: "SubCaegory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubCaegory",
                table: "SubCaegory");

            migrationBuilder.RenameTable(
                name: "SubCaegory",
                newName: "SubCategory");

            migrationBuilder.RenameIndex(
                name: "IX_SubCaegory_CategoryId",
                table: "SubCategory",
                newName: "IX_SubCategory_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubCategory",
                table: "SubCategory",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubCategory_Category_CategoryId",
                table: "SubCategory",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubCategory_Category_CategoryId",
                table: "SubCategory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubCategory",
                table: "SubCategory");

            migrationBuilder.RenameTable(
                name: "SubCategory",
                newName: "SubCaegory");

            migrationBuilder.RenameIndex(
                name: "IX_SubCategory_CategoryId",
                table: "SubCaegory",
                newName: "IX_SubCaegory_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubCaegory",
                table: "SubCaegory",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubCaegory_Category_CategoryId",
                table: "SubCaegory",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
