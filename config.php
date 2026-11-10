<?php
declare(strict_types=1);

function database(): PDO
{
    static $pdo = null;
    if ($pdo instanceof PDO) {
        return $pdo;
    }

    $host = getenv('SPENDWISE_DB_HOST') ?: '127.0.0.1';
    $port = getenv('SPENDWISE_DB_PORT') ?: '3306';
    $name = getenv('SPENDWISE_DB_NAME') ?: 'spendwise';
    $user = getenv('SPENDWISE_DB_USER') ?: 'root';
    $password = getenv('SPENDWISE_DB_PASSWORD') ?: '';

    $pdo = new PDO(
        "mysql:host={$host};port={$port};dbname={$name};charset=utf8mb4",
        $user,
        $password,
        [
            PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION,
            PDO::ATTR_DEFAULT_FETCH_MODE => PDO::FETCH_ASSOC,
            PDO::ATTR_EMULATE_PREPARES => false,
        ]
    );

    return $pdo;
}
