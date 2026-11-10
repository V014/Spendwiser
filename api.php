<?php
declare(strict_types=1);

require_once __DIR__ . '/config.php';

function respond(array $data, int $status = 200): never
{
    http_response_code($status);
    header('Content-Type: application/json; charset=utf-8');
    header('Cache-Control: no-store');
    echo json_encode($data, JSON_THROW_ON_ERROR);
    exit;
}

function request_data(): array
{
    $body = file_get_contents('php://input');
    if ($body === false || $body === '') {
        return [];
    }
    $data = json_decode($body, true);
    if (!is_array($data)) {
        respond(['error' => 'The request body must be valid JSON.'], 400);
    }
    return $data;
}

function required_string(array $data, string $key, int $maxLength): string
{
    $value = trim((string)($data[$key] ?? ''));
    if ($value === '' || mb_strlen($value) > $maxLength) {
        respond(['error' => "Please provide a valid {$key} (up to {$maxLength} characters)."], 422);
    }
    return $value;
}

function valid_amount(mixed $value, bool $allowZero = false): string
{
    if (!is_numeric($value)) {
        respond(['error' => 'Enter a valid amount.'], 422);
    }
    $amount = (float)$value;
    if (!is_finite($amount) || $amount < ($allowZero ? 0 : 0.01) || $amount > 999999999999.99) {
        respond(['error' => 'The amount is outside the allowed range.'], 422);
    }
    return number_format($amount, 2, '.', '');
}

function selected_month(): string
{
    $month = (string)($_GET['month'] ?? date('Y-m'));
    if (!preg_match('/^\d{4}-(0[1-9]|1[0-2])$/', $month)) {
        respond(['error' => 'Choose a valid month.'], 422);
    }
    return $month;
}

function dashboard(PDO $pdo, string $selectedMonth): array
{
    $monthStart = new DateTimeImmutable($selectedMonth . '-01');
    $monthEnd = $monthStart->modify('+1 month');
    $monthStartValue = $monthStart->format('Y-m-d');
    $monthEndValue = $monthEnd->format('Y-m-d');
    $wallet = $pdo->query('SELECT owner, currency, theme, savings_rate, monthly_budget, savings_goal, savings, opening_balance FROM wallet WHERE id = 1')->fetch();
    if (!$wallet) {
        throw new RuntimeException('Wallet settings are missing. Import schema.sql and try again.');
    }

    $totalsStatement = $pdo->prepare("
        SELECT
            COALESCE(SUM(CASE WHEN type = 'income' THEN amount ELSE 0 END), 0) AS income,
            COALESCE(SUM(CASE WHEN type = 'expense' THEN amount ELSE 0 END), 0) AS expenses,
            COALESCE(SUM(CASE WHEN type = 'income' AND transaction_date >= :income_start AND transaction_date < :income_end THEN amount ELSE 0 END), 0) AS month_income,
            COALESCE(SUM(CASE WHEN type = 'expense' AND transaction_date >= :expense_start AND transaction_date < :expense_end THEN amount ELSE 0 END), 0) AS month_expenses,
            COALESCE((SELECT amount FROM transactions WHERE type = 'income' ORDER BY transaction_date, id LIMIT 1), 0) AS first_income_amount,
            MIN(CASE WHEN type = 'income' THEN transaction_date END) AS first_income,
            COUNT(*) AS transaction_count
        FROM transactions
    ");
    $totalsStatement->execute([
        'income_start' => $monthStartValue,
        'income_end' => $monthEndValue,
        'expense_start' => $monthStartValue,
        'expense_end' => $monthEndValue,
    ]);
    $totals = $totalsStatement->fetch();

    $monthly = $pdo->query("
        SELECT DATE_FORMAT(months.month_start, '%Y-%m') AS month,
            COALESCE(SUM(CASE WHEN t.type = 'income' THEN t.amount ELSE 0 END), 0) AS income,
            COALESCE(SUM(CASE WHEN t.type = 'expense' THEN t.amount ELSE 0 END), 0) AS expenses
        FROM (
            SELECT DATE_SUB(DATE_FORMAT(CURDATE(), '%Y-%m-01'), INTERVAL seq.n MONTH) AS month_start
            FROM (
                SELECT 0 AS n UNION ALL SELECT 1 UNION ALL SELECT 2 UNION ALL
                SELECT 3 UNION ALL SELECT 4 UNION ALL SELECT 5 UNION ALL
                SELECT 6 UNION ALL SELECT 7 UNION ALL SELECT 8 UNION ALL
                SELECT 9 UNION ALL SELECT 10 UNION ALL SELECT 11
            ) seq
        ) months
        LEFT JOIN transactions t ON t.transaction_date >= months.month_start
            AND t.transaction_date < DATE_ADD(months.month_start, INTERVAL 1 MONTH)
        GROUP BY months.month_start
        ORDER BY months.month_start
    ")->fetchAll();

    $recentStatement = $pdo->prepare('SELECT id, description, category, type, amount, transaction_date FROM transactions WHERE transaction_date >= :start_date AND transaction_date < :end_date ORDER BY transaction_date DESC, id DESC LIMIT 8');
    $recentStatement->execute(['start_date' => $monthStartValue, 'end_date' => $monthEndValue]);
    $recent = $recentStatement->fetchAll();
    $investments = $pdo->query('SELECT id, title, amount, description, progress, created_at FROM investments ORDER BY created_at DESC, id DESC')->fetchAll();

    $transactionsCount = (int)$totals['transaction_count'];
    $income = (float)$totals['income'];
    $firstIncome = $totals['first_income'];
    $endingCapital = (float)$wallet['opening_balance'] + $income - (float)$totals['expenses'];
    $starting = (float)$wallet['opening_balance'] > 0
        ? (float)$wallet['opening_balance']
        : (float)$totals['first_income_amount'];
    $firstIncomeDate = $firstIncome ? new DateTimeImmutable((string)$firstIncome) : null;
    $today = new DateTimeImmutable('today');
    $elapsedDays = $firstIncomeDate && $firstIncomeDate <= $today
        ? (int)$firstIncomeDate->diff($today)->days
        : 0;
    $years = $elapsedDays / 365.2425;
    $growthRate = ($starting > 0 && $endingCapital > 0 && $years >= 1)
        ? (pow($endingCapital / $starting, 1 / $years) - 1) * 100
        : null;

    return [
        'wallet' => $wallet,
        'totals' => [
            'balance' => $endingCapital,
            'income' => $income,
            'expenses' => (float)$totals['expenses'],
            'month_income' => (float)$totals['month_income'],
            'month_expenses' => (float)$totals['month_expenses'],
            'saved' => (float)$wallet['savings'],
            'transaction_count' => $transactionsCount,
            'starting_capital' => $starting,
            'growth_rate' => $growthRate,
            'first_income' => $firstIncome,
        ],
        'monthly' => $monthly,
        'transactions' => $recent,
        'investments' => $investments,
    ];
}

$action = (string)($_GET['action'] ?? '');

try {
    $pdo = database();

    if ($_SERVER['REQUEST_METHOD'] === 'GET' && $action === 'dashboard') {
        respond(dashboard($pdo, selected_month()));
    }

    if ($_SERVER['REQUEST_METHOD'] === 'GET' && $action === 'transactions') {
        $search = trim((string)($_GET['q'] ?? ''));
        $type = (string)($_GET['type'] ?? 'all');
        $period = (string)($_GET['period'] ?? 'all');
        $where = [];
        $params = [];
        if ($search !== '') {
            $where[] = '(description LIKE :search_description OR category LIKE :search_category)';
            $params['search_description'] = '%' . $search . '%';
            $params['search_category'] = '%' . $search . '%';
        }
        if (in_array($type, ['income', 'expense'], true)) {
            $where[] = 'type = :type';
            $params['type'] = $type;
        }
        if ($period === 'month') {
            $month = (string)($_GET['month'] ?? date('Y-m'));
            if (!preg_match('/^\d{4}-(0[1-9]|1[0-2])$/', $month)) {
                respond(['error' => 'Choose a valid month.'], 422);
            }
            $monthStart = new DateTimeImmutable($month . '-01');
            $where[] = 'transaction_date >= :month_start AND transaction_date < :month_end';
            $params['month_start'] = $monthStart->format('Y-m-d');
            $params['month_end'] = $monthStart->modify('+1 month')->format('Y-m-d');
        }
        $sql = 'SELECT id, description, category, type, amount, transaction_date FROM transactions'
            . ($where ? ' WHERE ' . implode(' AND ', $where) : '')
            . ' ORDER BY transaction_date DESC, id DESC LIMIT 200';
        $statement = $pdo->prepare($sql);
        $statement->execute($params);
        respond(['transactions' => $statement->fetchAll()]);
    }

    if ($_SERVER['REQUEST_METHOD'] === 'GET' && $action === 'export') {
        $statement = $pdo->query('SELECT id, description, category, type, amount, transaction_date FROM transactions ORDER BY transaction_date DESC, id DESC');
        header('Content-Type: text/csv; charset=utf-8');
        header('Content-Disposition: attachment; filename="spendwise-transactions-' . date('Y-m-d') . '.csv"');
        $output = fopen('php://output', 'w');
        if ($output === false) {
            throw new RuntimeException('Could not prepare the CSV export.');
        }
        fwrite($output, "\xEF\xBB\xBF");
        fputcsv($output, ['ID', 'Description', 'Category', 'Type', 'Amount', 'Date']);
        while ($row = $statement->fetch()) {
            foreach (['description', 'category'] as $field) {
                if (preg_match('/^[=+\-@\t\r]/', (string)$row[$field])) {
                    $row[$field] = "'" . $row[$field];
                }
            }
            fputcsv($output, [$row['id'], $row['description'], $row['category'], $row['type'], $row['amount'], $row['transaction_date']]);
        }
        fclose($output);
        exit;
    }

    if ($_SERVER['REQUEST_METHOD'] === 'POST' && $action === 'transaction') {
        $data = request_data();
        $description = required_string($data, 'description', 120);
        $category = required_string($data, 'category', 60);
        $type = (string)($data['type'] ?? '');
        if (!in_array($type, ['income', 'expense'], true)) {
            respond(['error' => 'Choose income or expense.'], 422);
        }
        $amount = valid_amount($data['amount']);
        $date = (string)($data['date'] ?? '');
        $parsedDate = DateTime::createFromFormat('!Y-m-d', $date);
        if (!$parsedDate || $parsedDate->format('Y-m-d') !== $date) {
            respond(['error' => 'Choose a valid transaction date.'], 422);
        }

        $pdo->beginTransaction();
        $wallet = $pdo->query('SELECT savings_rate FROM wallet WHERE id = 1 FOR UPDATE')->fetch();
        if (!$wallet) {
            throw new RuntimeException('Wallet settings are missing. Import schema.sql and try again.');
        }
        $saved = $type === 'income' ? round((float)$amount * (int)$wallet['savings_rate'] / 100, 2) : 0;
        $insert = $pdo->prepare('INSERT INTO transactions (description, category, type, amount, transaction_date, savings_contribution) VALUES (:description, :category, :type, :amount, :date, :contribution)');
        $insert->execute([
            'description' => $description,
            'category' => $category,
            'type' => $type,
            'amount' => $amount,
            'date' => $date,
            'contribution' => number_format($saved, 2, '.', ''),
        ]);
        if ($type === 'income') {
            $update = $pdo->prepare('UPDATE wallet SET savings = savings + :saved WHERE id = 1');
            $update->execute(['saved' => number_format($saved, 2, '.', '')]);
        }
        $pdo->commit();
        respond(['success' => true, 'dashboard' => dashboard($pdo, selected_month())], 201);
    }

    if ($_SERVER['REQUEST_METHOD'] === 'DELETE' && $action === 'transaction') {
        $id = filter_var($_GET['id'] ?? null, FILTER_VALIDATE_INT);
        if (!$id || $id < 1) {
            respond(['error' => 'Choose a valid transaction.'], 422);
        }
        $pdo->beginTransaction();
        $statement = $pdo->prepare('SELECT type, amount, savings_contribution FROM transactions WHERE id = :id FOR UPDATE');
        $statement->execute(['id' => $id]);
        $transaction = $statement->fetch();
        if (!$transaction) {
            $pdo->rollBack();
            respond(['error' => 'That transaction no longer exists.'], 404);
        }
        $delete = $pdo->prepare('DELETE FROM transactions WHERE id = :id');
        $delete->execute(['id' => $id]);
        if ($transaction['type'] === 'income') {
            $update = $pdo->prepare('UPDATE wallet SET savings = GREATEST(0, savings - :saved) WHERE id = 1');
            $update->execute(['saved' => $transaction['savings_contribution']]);
        }
        $pdo->commit();
        respond(['success' => true, 'dashboard' => dashboard($pdo, selected_month())]);
    }

    if ($_SERVER['REQUEST_METHOD'] === 'POST' && $action === 'settings') {
        $data = request_data();
        $owner = required_string($data, 'owner', 60);
        $currency = required_string($data, 'currency', 5);
        $rate = filter_var($data['savings_rate'] ?? null, FILTER_VALIDATE_INT);
        if ($rate === false || $rate < 0 || $rate > 100) {
            respond(['error' => 'Auto-save percentage must be between 0 and 100.'], 422);
        }
        $budget = valid_amount($data['monthly_budget'] ?? 0, true);
        $goal = valid_amount($data['savings_goal'] ?? 0, true);
        $theme = (string)($data['theme'] ?? '');
        if (!in_array($theme, ['dark', 'light'], true)) {
            respond(['error' => 'Choose a valid appearance.'], 422);
        }
        $statement = $pdo->prepare('UPDATE wallet SET owner = :owner, currency = :currency, savings_rate = :rate, monthly_budget = :budget, savings_goal = :goal, theme = :theme WHERE id = 1');
        $statement->execute([
            'owner' => $owner,
            'currency' => $currency,
            'rate' => $rate,
            'budget' => $budget,
            'goal' => $goal,
            'theme' => $theme,
        ]);
        respond(['success' => true, 'dashboard' => dashboard($pdo, selected_month())]);
    }

    if ($_SERVER['REQUEST_METHOD'] === 'POST' && $action === 'investment') {
        $data = request_data();
        $title = required_string($data, 'title', 100);
        $description = trim((string)($data['description'] ?? ''));
        if (mb_strlen($description) > 255) {
            respond(['error' => 'Notes must be 255 characters or fewer.'], 422);
        }
        $amount = valid_amount($data['amount']);
        $progress = filter_var($data['progress'] ?? 0, FILTER_VALIDATE_INT);
        if ($progress === false || $progress < 0 || $progress > 100) {
            respond(['error' => 'Progress must be between 0 and 100.'], 422);
        }
        $statement = $pdo->prepare('INSERT INTO investments (title, amount, description, progress) VALUES (:title, :amount, :description, :progress)');
        $statement->execute(['title' => $title, 'amount' => $amount, 'description' => $description, 'progress' => $progress]);
        respond(['success' => true, 'dashboard' => dashboard($pdo, selected_month())], 201);
    }

    if ($_SERVER['REQUEST_METHOD'] === 'PATCH' && $action === 'investment') {
        $id = filter_var($_GET['id'] ?? null, FILTER_VALIDATE_INT);
        $data = request_data();
        $progress = filter_var($data['progress'] ?? null, FILTER_VALIDATE_INT);
        if (!$id || $id < 1 || $progress === false || $progress < 0 || $progress > 100) {
            respond(['error' => 'Choose a valid milestone and progress from 0 to 100.'], 422);
        }
        $statement = $pdo->prepare('UPDATE investments SET progress = :progress WHERE id = :id');
        $statement->execute(['progress' => $progress, 'id' => $id]);
        if ($statement->rowCount() === 0) {
            $exists = $pdo->prepare('SELECT id FROM investments WHERE id = :id');
            $exists->execute(['id' => $id]);
            if (!$exists->fetch()) {
                respond(['error' => 'That milestone no longer exists.'], 404);
            }
        }
        respond(['success' => true, 'dashboard' => dashboard($pdo, selected_month())]);
    }

    if ($_SERVER['REQUEST_METHOD'] === 'DELETE' && $action === 'investment') {
        $id = filter_var($_GET['id'] ?? null, FILTER_VALIDATE_INT);
        if (!$id || $id < 1) {
            respond(['error' => 'Choose a valid milestone.'], 422);
        }
        $statement = $pdo->prepare('DELETE FROM investments WHERE id = :id');
        $statement->execute(['id' => $id]);
        if ($statement->rowCount() === 0) {
            respond(['error' => 'That milestone no longer exists.'], 404);
        }
        respond(['success' => true, 'dashboard' => dashboard($pdo, selected_month())]);
    }

    respond(['error' => 'Unknown action.'], 404);
} catch (Throwable $error) {
    if (isset($pdo) && $pdo instanceof PDO && $pdo->inTransaction()) {
        $pdo->rollBack();
    }
    error_log('SpendWise API error: ' . $error->getMessage());
    respond(['error' => 'SpendWise could not complete that request. Check your MySQL connection and database setup.'], 500);
}
