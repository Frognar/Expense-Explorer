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

## Deployment

The app runs on the home server with Docker Compose (`docker-compose.yaml`): PostgreSQL and the API, which also serves the frontend on port 8080.
It is meant to sit behind a reverse proxy that terminates HTTPS (Caddy on the Pi); the API trusts its `X-Forwarded-Proto` and `X-Forwarded-For` headers.

Deploying is manual: **Actions > .NET > Run workflow** on `main`. The job on the self-hosted runner:

1. dumps the database to `~/expense-explorer-backups/<timestamp>.sql.gz` (the 20 newest are kept),
2. runs `docker compose up -d --build --remove-orphans`.

On the first start the API creates its tables in the `expense` schema and copies receipts from the previous version's `public.receipts` and `public.receipt_items` (once; the old tables are left untouched).
Then create the accounts (see below).

To restore a backup: `gunzip -c <file>.sql.gz | docker exec -i expense-explorer-db psql -U user -d expense_explorer` (into an empty database).

## Accounts

There is no sign-up. Accounts are managed from the command line, which asks for the password:

```sh
docker exec -it expense-explorer-api dotnet ExpenseExplorer.Api.dll users add jan editor
docker exec -it expense-explorer-api dotnet ExpenseExplorer.Api.dll users list
```

Commands: `users list`, `users add <name> <reader|editor>`, `users password <name>`, `users role <name> <reader|editor>`, `users remove <name>`.
A reader can browse everything; an editor can also change data. Locally, run the same commands with `dotnet run --project src/ExpenseExplorer.Api -- users ...`.

## Features

- [ ] **Receipt Management:** Add, edit, and delete receipts.
- [ ] **Purchase Management:** Add, edit, and delete purchases associated with receipts.
- [ ] **Browse Receipts:** View and search through receipts based on store, date, and total amount.
- [ ] **Browse Stores, Items, and Categories:** View and search through stores, items, and categories used in receipts and purchases.
- [x] **Reporting:** Generate category-wise expense reports for a given date range.
- [x] **Browse Purchases:** Every receipt line with filters on store, item, category, date, price, quantity, discount, total and description.
- [x] **Export and Import:** Export a receipt to CSV; import a Biedronka e-receipt (JSON).
- [x] **Accounts:** Sign-in with read-only and full access roles.

## Contributing

Contributions to Expense Explorer are welcome! Whether it's bug fixes, new features, or enhancements, feel free to submit pull requests.

## License

This project is licensed under the [MIT License](LICENSE). Feel free to use, modify, and distribute as per the terms of the license.
