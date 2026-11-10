<?php
declare(strict_types=1);

require_once __DIR__ . '/config.php';

if (PHP_SAPI !== 'cli') {
    http_response_code(404);
    exit("Not found.\n");
}

$source = $argv[1] ?? (__DIR__ . DIRECTORY_SEPARATOR . 'SpendWise' . DIRECTORY_SEPARATOR . 'SpendWise.db');
if (!is_file($source)) {
    fwrite(STDERR, "Legacy database not found: {$source}\n");
    exit(1);
}

try {
    $legacy = new PDO('sqlite:' . $source, null, null, [
        PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION,
        PDO::ATTR_DEFAULT_FETCH_MODE => PDO::FETCH_ASSOC,
    ]);
    $pdo = database();

    $wallet = $legacy->query('SELECT owner, currency, money, savings, state, save FROM wallet WHERE id = 1')->fetch();
    if (!$wallet) {
        throw new RuntimeException('The legacy database does not contain wallet id 1.');
    }

    $targetRows = (int)$pdo->query('SELECT (SELECT COUNT(*) FROM transactions) + (SELECT COUNT(*) FROM investments)')->fetchColumn();
    if ($targetRows !== 0) {
        throw new RuntimeException('The MySQL wallet already contains transactions or investments. Migration only runs against an empty ledger.');
    }

    $transactions = $legacy->query('SELECT description, amount, date, action FROM transactions ORDER BY id')->fetchAll();
    $investments = $legacy->query('SELECT title, amount, description, progress FROM investments ORDER BY id')->fetchAll();
    $legacyNetTransactions = (float)$legacy->query("SELECT COALESCE(SUM(CASE WHEN action = '+' THEN amount WHEN action = '-' THEN -amount ELSE 0 END), 0) FROM transactions")->fetchColumn();
    $openingBalance = (float)$wallet['money'] - $legacyNetTransactions;
    if (!is_finite($openingBalance) || abs($openingBalance) > 999999999999.99) {
        throw new RuntimeException('The legacy opening balance is outside the supported range.');
    }
    printf(
        "Ready to import %d transaction(s), %d investment(s), and the legacy wallet settings.\n",
        count($transactions),
        count($investments)
    );
    fwrite(STDOUT, "This reads the SQLite file and writes to MySQL. Type yes to continue: ");
    $confirmation = fgets(STDIN);
    if (strtolower(trim((string)$confirmation)) !== 'yes') {
        fwrite(STDOUT, "Migration cancelled.\n");
        exit(0);
    }

    $owner = trim((string)$wallet['owner']);
    $currency = trim((string)$wallet['currency']);
    $theme = strtolower(trim((string)$wallet['state']));
    $rate = max(0, min(100, (int)$wallet['save']));
    if (!in_array($theme, ['dark', 'light'], true)) {
        $theme = 'dark';
    }

    $pdo->beginTransaction();
    $settings = $pdo->prepare('UPDATE wallet SET owner = :owner, currency = :currency, theme = :theme, savings_rate = :rate, savings = :savings, opening_balance = :opening_balance WHERE id = 1');
    $settings->execute([
        'owner' => $owner !== '' ? mb_substr($owner, 0, 60) : 'Your wallet',
        'currency' => $currency !== '' ? mb_substr($currency, 0, 5) : '$',
        'theme' => $theme,
        'rate' => $rate,
        'savings' => max(0, (float)$wallet['savings']),
        'opening_balance' => number_format($openingBalance, 2, '.', ''),
    ]);

    $insertTransaction = $pdo->prepare('INSERT INTO transactions (description, category, type, amount, transaction_date, savings_contribution) VALUES (:description, :category, :type, :amount, :date, :contribution)');
    foreach ($transactions as $transaction) {
        $action = (string)$transaction['action'];
        if (!in_array($action, ['+', '-'], true)) {
            throw new RuntimeException('A legacy transaction has an unsupported action; no data was imported.');
        }
        $amount = (float)$transaction['amount'];
        if (!is_finite($amount) || $amount <= 0 || $amount > 999999999999.99) {
            throw new RuntimeException('A legacy transaction has an invalid amount; no data was imported.');
        }
        $timestamp = strtotime((string)$transaction['date']);
        if ($timestamp === false) {
            throw new RuntimeException('A legacy transaction has an invalid date; no data was imported.');
        }
        $type = $action === '+' ? 'income' : 'expense';
        $contribution = $type === 'income' ? round($amount * $rate / 100, 2) : 0;
        $description = trim((string)$transaction['description']);
        $insertTransaction->execute([
            'description' => $description !== '' ? mb_substr($description, 0, 120) : 'Imported transaction',
            'category' => 'General',
            'type' => $type,
            'amount' => number_format($amount, 2, '.', ''),
            'date' => date('Y-m-d', $timestamp),
            'contribution' => number_format($contribution, 2, '.', ''),
        ]);
    }

    $insertInvestment = $pdo->prepare('INSERT INTO investments (title, amount, description, progress) VALUES (:title, :amount, :description, :progress)');
    foreach ($investments as $investment) {
        $amount = (float)$investment['amount'];
        if (!is_finite($amount) || $amount <= 0 || $amount > 999999999999.99) {
            throw new RuntimeException('A legacy investment has an invalid amount; no data was imported.');
        }
        $progress = max(0, min(100, (int)$investment['progress']));
        $title = trim((string)$investment['title']);
        $insertInvestment->execute([
            'title' => $title !== '' ? mb_substr($title, 0, 100) : 'Imported milestone',
            'amount' => number_format($amount, 2, '.', ''),
            'description' => mb_substr(trim((string)$investment['description']), 0, 255),
            'progress' => $progress,
        ]);
    }

    $pdo->commit();
    fwrite(STDOUT, "Migration complete. The original SQLite file was left unchanged.\n");
} catch (Throwable $error) {
    if (isset($pdo) && $pdo instanceof PDO && $pdo->inTransaction()) {
        $pdo->rollBack();
    }
    fwrite(STDERR, "Migration failed: {$error->getMessage()}\n");
    exit(1);
}
