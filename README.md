<p align="center">
    <img alt="Expense-Explorer-Logo" src="res/imgs/Expense-Explorer-Logo-512.png">
</p>

## Expense-Explorer

[![.net workflow](https://github.com/Frognar/Expense-Explorer/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/Frognar/Expense-Explorer/actions/workflows/dotnet.yml)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
![GitHub Repo stars](https://img.shields.io/github/stars/frognar/expense-explorer?style=flat)

Expense Explorer is a simple expense tracking application designed to run on a home server, allowing users to manage their receipts and track expenses conveniently.

## Note

The application is being rewritten as a Web API (ASP.NET Core, .NET 10) with a Blazor WebAssembly frontend.
The previous version is kept on the [`main-2609-archive`](https://github.com/Frognar/Expense-Explorer/tree/main-2609-archive) branch.

## Project layout

- `src/ExpenseExplorer.Domain` – receipt model; invalid states cannot be constructed
- `src/ExpenseExplorer.Api` – HTTP API, also serves the frontend
- `src/ExpenseExplorer.Web` – Blazor WebAssembly frontend (MudBlazor, mobile first)
- `src/aspire/AppHost` – local run with PostgreSQL (`dotnet run --project src/aspire/AppHost`)
- `tests/` – unit and API tests (`dotnet test --solution ExpenseExplorer.slnx`)

## Features

- [ ] **Receipt Management:** Add, edit, and delete receipts.
- [ ] **Purchase Management:** Add, edit, and delete purchases associated with receipts.
- [ ] **Browse Receipts:** View and search through receipts based on store, date, and total amount.
- [ ] **Browse Stores, Items, and Categories:** View and search through stores, items, and categories used in receipts and purchases.
- [ ] **Reporting:** Generate category-wise expense reports for a given date range.

## Contributing

Contributions to Expense Explorer are welcome! Whether it's bug fixes, new features, or enhancements, feel free to submit pull requests.

## License

This project is licensed under the [MIT License](LICENSE). Feel free to use, modify, and distribute as per the terms of the license.
