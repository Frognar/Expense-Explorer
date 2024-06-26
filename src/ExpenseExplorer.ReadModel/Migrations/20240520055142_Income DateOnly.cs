﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseExplorer.ReadModel.Migrations
{
    /// <inheritdoc />
    public partial class IncomeDateOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "ReceivedDate",
                table: "Incomes",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ReceivedDate",
                table: "Incomes",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");
        }
    }
}
