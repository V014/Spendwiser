# SpendWise

SpendWise is a responsive personal-finance web app built with HTML, CSS, vanilla JavaScript, PHP, and MySQL. The original C# desktop project remains in `SpendWise/`; the web app lives at the repository root.

## Features

- Overview of wallet balance, monthly income and spending, and savings
- Twelve-month income-versus-expense chart and selectable monthly summaries
- Annualized capital growth (CAGR) when there is at least one full year of positive capital history
- Searchable transaction history, income/expense recording, deletion, and CSV export
- Automatic savings contributions on income, with an adjustable savings rate
- Monthly spending budget and savings-goal progress
- Investment milestones with editable progress
- Editable wallet name and currency, plus persisted dark/light themes
- Responsive layout for desktop and mobile

## Run locally with XAMPP

1. Copy the project folder into `C:\xampp\htdocs\SpendWise` (or use the existing workspace location).
2. Start Apache and MySQL from the XAMPP Control Panel.
3. Open phpMyAdmin at `http://localhost/phpmyadmin`, import `schema.sql`, and confirm the `spendwise` database was created.
4. If your MySQL connection differs from the defaults, set `SPENDWISE_DB_HOST`, `SPENDWISE_DB_PORT`, `SPENDWISE_DB_NAME`, `SPENDWISE_DB_USER`, and `SPENDWISE_DB_PASSWORD`, or edit the corresponding local defaults in `config.php`.
5. Open `http://localhost/SpendWise/`.

The default XAMPP MySQL connection is `127.0.0.1:3306`, database `spendwise`, user `root`, and an empty password. The dashboard displays a connection/setup message if the API cannot reach MySQL.

## Data notes

The included `SpendWise/SpendWise.db` belongs to the legacy desktop app and is not modified by the web app. After importing `schema.sql`, run `php migrate-legacy.php` from the project folder to optionally copy the existing wallet, transactions, and investments into an empty MySQL ledger. The migration asks for confirmation, refuses to run over existing MySQL transactions or investments, and leaves the SQLite file unchanged. A migrated wallet balance is retained as its opening balance.

This version is designed for a single local user and does not include sign-in or multi-user access controls. Do not expose it directly to the public internet without adding authentication, authorization, HTTPS, and deployment-specific database credentials.
