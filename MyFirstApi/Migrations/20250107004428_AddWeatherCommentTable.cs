﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFirstApi.Migrations
{
    /// <inheritdoc />
    public partial class AddWeatherCommentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WeatherComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CommentMessage = table.Column<string>(type: "TEXT", nullable: false),
                    WeatherForecastId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeatherComments_WeatherForecasts_WeatherForecastId",
                        column: x => x.WeatherForecastId,
                        principalTable: "WeatherForecasts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WeatherComments_WeatherForecastId",
                table: "WeatherComments",
                column: "WeatherForecastId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WeatherComments");
        }
    }
}
