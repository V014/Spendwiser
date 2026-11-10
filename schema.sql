CREATE DATABASE IF NOT EXISTS spendwise
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;

USE spendwise;

CREATE TABLE IF NOT EXISTS wallet (
  id TINYINT UNSIGNED NOT NULL PRIMARY KEY,
  owner VARCHAR(60) NOT NULL DEFAULT 'Your wallet',
  currency VARCHAR(5) NOT NULL DEFAULT '$',
  theme ENUM('dark', 'light') NOT NULL DEFAULT 'dark',
  savings_rate TINYINT UNSIGNED NOT NULL DEFAULT 10,
  monthly_budget DECIMAL(14, 2) UNSIGNED NOT NULL DEFAULT 0,
  savings_goal DECIMAL(14, 2) UNSIGNED NOT NULL DEFAULT 0,
  savings DECIMAL(14, 2) NOT NULL DEFAULT 0,
  opening_balance DECIMAL(14, 2) NOT NULL DEFAULT 0,
  CONSTRAINT chk_wallet_singleton CHECK (id = 1),
  CONSTRAINT chk_wallet_savings_rate CHECK (savings_rate <= 100)
) ENGINE=InnoDB;

INSERT IGNORE INTO wallet (id, owner, currency) VALUES (1, 'Your wallet', '$');

CREATE TABLE IF NOT EXISTS transactions (
  id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
  description VARCHAR(120) NOT NULL,
  category VARCHAR(60) NOT NULL DEFAULT 'General',
  type ENUM('income', 'expense') NOT NULL,
  amount DECIMAL(14, 2) UNSIGNED NOT NULL,
  transaction_date DATE NOT NULL,
  savings_contribution DECIMAL(14, 2) UNSIGNED NOT NULL DEFAULT 0,
  created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  INDEX idx_transactions_date (transaction_date, id),
  INDEX idx_transactions_type_date (type, transaction_date),
  INDEX idx_transactions_category (category)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS investments (
  id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
  title VARCHAR(100) NOT NULL,
  amount DECIMAL(14, 2) UNSIGNED NOT NULL,
  description VARCHAR(255) NOT NULL DEFAULT '',
  progress TINYINT UNSIGNED NOT NULL DEFAULT 0,
  created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT chk_investment_progress CHECK (progress <= 100)
) ENGINE=InnoDB;
