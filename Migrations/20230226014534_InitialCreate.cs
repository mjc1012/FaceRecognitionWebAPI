using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FaceRecognitionWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FaceExpressions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageFile = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaceExpressions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FacesToRecognize",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageFile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoggedTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacesToRecognize", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValidIdNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FaceRecognitionStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsRecognized = table.Column<bool>(type: "bit", nullable: false),
                    FaceToRecognizeId = table.Column<int>(type: "int", nullable: false),
                    PredictedPersonId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaceRecognitionStatuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FaceRecognitionStatuses_FacesToRecognize_FaceToRecognizeId",
                        column: x => x.FaceToRecognizeId,
                        principalTable: "FacesToRecognize",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FaceRecognitionStatuses_Persons_PredictedPersonId",
                        column: x => x.PredictedPersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FacesToTrain",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageFile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    FaceExpressionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacesToTrain", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacesToTrain_FaceExpressions_FaceExpressionId",
                        column: x => x.FaceExpressionId,
                        principalTable: "FaceExpressions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FacesToTrain_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AugmentedFaces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageFile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FaceToTrainId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AugmentedFaces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AugmentedFaces_FacesToTrain_FaceToTrainId",
                        column: x => x.FaceToTrainId,
                        principalTable: "FacesToTrain",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "FaceExpressions",
                columns: new[] { "Id", "ImageFile", "Name" },
                values: new object[,]
                {
                    { 1, "face_1.jpg", "Resting Face" },
                    { 2, "face_2.jpg", "Resting Face with Eyes Closed" },
                    { 3, "face_3.jpg", "Open Mouth Face" },
                    { 4, "face_4.jpg", "Open Mouth Face with Eyes Closed" },
                    { 5, "face_5.jpg", "Smiling Face" },
                    { 6, "face_6.jpg", "Smiling Face with Eyes Closed" },
                    { 7, "face_7.jpg", "Duck Face" },
                    { 8, "face_8.jpg", "Duck Face with Eyes Closed" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AugmentedFaces_FaceToTrainId",
                table: "AugmentedFaces",
                column: "FaceToTrainId");

            migrationBuilder.CreateIndex(
                name: "IX_FaceRecognitionStatuses_FaceToRecognizeId",
                table: "FaceRecognitionStatuses",
                column: "FaceToRecognizeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FaceRecognitionStatuses_PredictedPersonId",
                table: "FaceRecognitionStatuses",
                column: "PredictedPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_FacesToTrain_FaceExpressionId",
                table: "FacesToTrain",
                column: "FaceExpressionId");

            migrationBuilder.CreateIndex(
                name: "IX_FacesToTrain_PersonId",
                table: "FacesToTrain",
                column: "PersonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AugmentedFaces");

            migrationBuilder.DropTable(
                name: "FaceRecognitionStatuses");

            migrationBuilder.DropTable(
                name: "FacesToTrain");

            migrationBuilder.DropTable(
                name: "FacesToRecognize");

            migrationBuilder.DropTable(
                name: "FaceExpressions");

            migrationBuilder.DropTable(
                name: "Persons");
        }
    }
}
