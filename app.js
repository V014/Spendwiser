const API = 'api.php';
const state = { dashboard: null, month: currentMonth(), allTransactions: false, searchTimer: null };
const $ = (selector, root = document) => root.querySelector(selector);
const $$ = (selector, root = document) => [...root.querySelectorAll(selector)];

function currentMonth() {
  const now = new Date();
  return `${now.getFullYear()}-${String(now.getMonth() + 1).padStart(2, '0')}`;
}

function escapeHTML(value) {
  return String(value ?? '').replace(/[&<>"']/g, character => ({
    '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;'
  })[character]);
}

function money(value, currency = state.dashboard?.wallet?.currency || '$') {
  const amount = Number(value || 0);
  return `${currency}${new Intl.NumberFormat(undefined, {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  }).format(amount)}`;
}

async function request(path, options = {}) {
  const response = await fetch(`${API}?${path}`, {
    headers: { 'Content-Type': 'application/json', ...(options.headers || {}) },
    ...options
  });
  const contentType = response.headers.get('content-type') || '';
  if (!contentType.includes('application/json')) {
    if (!response.ok) throw new Error('The server could not complete that request.');
    return response;
  }
  const data = await response.json();
  if (!response.ok) throw new Error(data.error || 'The server could not complete that request.');
  return data;
}

function showToast(message) {
  const toast = $('#toast');
  toast.textContent = message;
  toast.classList.add('visible');
  window.clearTimeout(showToast.timeout);
  showToast.timeout = window.setTimeout(() => toast.classList.remove('visible'), 3000);
}

function showError(message) {
  const notice = $('#connection-notice');
  notice.hidden = false;
  notice.innerHTML = `<strong>Couldn’t load your money data.</strong> ${escapeHTML(message)} <span>Import <code>schema.sql</code> into MySQL and check <code>config.php</code> to connect SpendWise.</span>`;
}

function hideError() {
  $('#connection-notice').hidden = true;
}

function applyTheme(theme) {
  document.documentElement.dataset.theme = theme === 'light' ? 'light' : 'dark';
  $('#theme-toggle').textContent = theme === 'light' ? '☾' : '☼';
  $('#theme-toggle').title = theme === 'light' ? 'Switch to dark mode' : 'Switch to light mode';
}

function initializeMonthPicker() {
  const picker = $('#month-filter');
  const now = new Date();
  picker.innerHTML = '';
  for (let offset = 0; offset < 12; offset++) {
    const date = new Date(now.getFullYear(), now.getMonth() - offset, 1);
    const value = `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}`;
    const option = document.createElement('option');
    option.value = value;
    option.textContent = new Intl.DateTimeFormat(undefined, { month: 'long', year: 'numeric' }).format(date);
    picker.append(option);
  }
  picker.value = state.month;
}

function renderDashboard(data) {
  state.dashboard = data;
  hideError();
  const { wallet, totals } = data;
  const currency = wallet.currency || '$';
  applyTheme(wallet.theme);
  $('#profile-name').textContent = wallet.owner || 'Your wallet';
  $('#profile-avatar').textContent = [...(wallet.owner || 'S')][0].toUpperCase();
  $('#balance-value').textContent = money(totals.balance, currency);
  $('#income-value').textContent = money(totals.month_income, currency);
  $('#spending-value').textContent = money(totals.month_expenses, currency);
  $('#savings-value').textContent = money(totals.saved, currency);
  $('#income-caption').textContent = `This month · ${totals.transaction_count} transactions total`;
  $('#spending-caption').textContent = `${totals.month_income > 0 ? Math.round((totals.month_expenses / totals.month_income) * 100) : 0}% of this month’s income`;
  $('#savings-caption').textContent = `${Number(wallet.savings_rate)}% auto-save rate`;
  $('#total-income').textContent = money(totals.income, currency);
  $('#starting-capital').textContent = money(totals.starting_capital, currency);
  $('#tracking-since').textContent = totals.first_income ? new Date(`${totals.first_income}T00:00:00`).toLocaleDateString(undefined, { month: 'short', year: 'numeric' }) : 'Not started';
  const growth = totals.growth_rate;
  $('#growth-rate').textContent = growth === null ? '—' : `${Number(growth).toFixed(1)}%`;
  $('#growth-ring').style.setProperty('--growth-progress', `${Math.min(Math.max(Number(growth) || 0, 0), 100)}%`);
  $('#growth-description').textContent = growth === null
    ? 'Build positive capital and track it for at least one full year to see your annualized growth rate.'
    : 'Annualized growth from your starting capital to your current balance.';
  $('#chart-period').textContent = `12 months · ${new Date(`${state.month}-02T00:00:00`).toLocaleDateString(undefined, { month: 'long', year: 'numeric' })} selected`;
  const selectedData = data.monthly.find(item => item.month === state.month);
  const selectedNet = Number(selectedData?.income || 0) - Number(selectedData?.expenses || 0);
  $('#chart-net').textContent = `${selectedNet >= 0 ? '+' : '−'}${money(Math.abs(selectedNet), currency)}`;
  $('#chart-net').classList.toggle('negative-value', selectedNet < 0);
  renderChart(data.monthly);
  renderTransactions(data.transactions);
  renderGoal(wallet, totals);
  renderBudget(wallet, totals);
  renderInvestments(data.investments);
  populateSettings(wallet);
  $('#currency-prefix').textContent = currency;
  $('#investment-currency').textContent = currency;
}

function renderChart(months) {
  const host = $('#cashflow-chart');
  const width = 670;
  const height = 190;
  const padding = { top: 16, right: 12, bottom: 32, left: 10 };
  const chartWidth = width - padding.left - padding.right;
  const chartHeight = height - padding.top - padding.bottom;
  const values = months.flatMap(item => [Number(item.income), Number(item.expenses)]);
  const maximum = Math.max(...values, 1) * 1.18;
  const x = index => padding.left + index * (chartWidth / Math.max(months.length - 1, 1));
  const y = value => padding.top + chartHeight - (value / maximum) * chartHeight;
  const incomePoints = months.map((item, index) => `${x(index)},${y(Number(item.income))}`).join(' ');
  const expensePoints = months.map((item, index) => `${x(index)},${y(Number(item.expenses))}`).join(' ');
  const formatMonth = value => {
    const date = new Date(`${value}-02T00:00:00`);
    return new Intl.DateTimeFormat(undefined, { month: 'short' }).format(date);
  };
  const grids = [0, 0.5, 1].map(fraction => {
    const lineY = padding.top + chartHeight * fraction;
    return `<line x1="${padding.left}" y1="${lineY}" x2="${width - padding.right}" y2="${lineY}" class="chart-gridline"/>`;
  }).join('');
  const labels = months.map((item, index) => `<text x="${x(index)}" y="${height - 7}" class="chart-label" text-anchor="middle">${formatMonth(item.month)}</text>`).join('');
  const dots = months.map((item, index) => `
    <circle cx="${x(index)}" cy="${y(Number(item.income))}" r="3.5" class="chart-income-dot"/>
    <circle cx="${x(index)}" cy="${y(Number(item.expenses))}" r="3.5" class="chart-expense-dot"/>
  `).join('');
  host.innerHTML = `<svg viewBox="0 0 ${width} ${height}" role="img" aria-label="Monthly income and expense comparison">
    ${grids}
    <polyline points="${incomePoints}" class="chart-income-line"/>
    <polyline points="${expensePoints}" class="chart-expense-line"/>
    ${dots}${labels}
  </svg>`;
}

const categoryIcons = {
  food: '◉', transport: '↗', housing: '⌂', shopping: '◇',
  health: '+', salary: '↙', freelance: '✳', investment: '◈'
};

function renderTransactions(transactions) {
  const body = $('#transaction-list');
  if (!transactions.length) {
    body.innerHTML = '<tr><td class="table-empty" colspan="5">No transactions here yet. Add one to get started.</td></tr>';
    return;
  }
  body.innerHTML = transactions.map(item => {
    const isIncome = item.type === 'income';
    const category = String(item.category || 'General');
    const icon = categoryIcons[category.toLowerCase()] || '◦';
    const date = new Date(`${item.transaction_date}T00:00:00`).toLocaleDateString(undefined, { month: 'short', day: 'numeric', year: 'numeric' });
    return `<tr>
      <td><span class="transaction-name"><span class="transaction-icon ${isIncome ? 'income-transaction-icon' : ''}">${icon}</span><strong>${escapeHTML(item.description)}</strong></span></td>
      <td><span class="category-pill">${escapeHTML(category)}</span></td>
      <td class="date-cell">${escapeHTML(date)}</td>
      <td class="amount-column"><span class="transaction-amount ${isIncome ? 'income-amount' : ''}">${isIncome ? '+' : '−'}${money(item.amount)}</span></td>
      <td class="delete-column"><button class="delete-transaction" data-id="${Number(item.id)}" aria-label="Delete ${escapeHTML(item.description)}" title="Delete transaction">×</button></td>
    </tr>`;
  }).join('');
}

function renderGoal(wallet, totals) {
  const goal = Number(wallet.savings_goal || 0);
  const saved = Number(totals.saved || 0);
  const percent = goal > 0 ? Math.min((saved / goal) * 100, 100) : 0;
  $('#goal-saved').textContent = money(saved);
  $('#goal-target').textContent = goal > 0 ? `of ${money(goal)}` : 'goal not set';
  $('#goal-progress').style.width = `${percent}%`;
  $('#goal-percent').textContent = goal > 0 ? `${Math.round(percent)}% of your goal` : 'Set a goal in preferences';
}

function renderBudget(wallet, totals) {
  const budget = Number(wallet.monthly_budget || 0);
  const spent = Number(totals.month_expenses || 0);
  const percent = budget > 0 ? Math.min((spent / budget) * 100, 100) : 0;
  const exceeded = budget > 0 && spent > budget;
  $('#budget-spent').textContent = money(spent);
  $('#budget-limit').textContent = budget > 0 ? `of ${money(budget)}` : 'limit not set';
  $('#budget-progress').style.width = `${percent}%`;
  $('#budget-progress').classList.toggle('over-budget', exceeded);
  $('#budget-caption').textContent = budget > 0
    ? exceeded ? `${money(spent - budget)} over your limit` : `${money(budget - spent)} left this month`
    : 'Set a budget in preferences';
}

function renderInvestments(investments) {
  const list = $('#investment-list');
  if (!investments.length) {
    list.innerHTML = '<div class="investment-empty"><span>◈</span><strong>No milestones yet</strong><small>Give your next goal a name and a number.</small></div>';
    return;
  }
  list.innerHTML = investments.map(item => `
    <article class="investment-item">
      <div class="investment-mark">◈</div>
      <div class="investment-copy"><strong>${escapeHTML(item.title)}</strong><small>${escapeHTML(item.description || 'Investment milestone')}</small></div>
      <div class="investment-target"><strong>${money(item.amount)}</strong><small>target</small></div>
      <div class="investment-progress"><div class="progress-track"><div class="progress-fill" style="width:${Math.min(Number(item.progress), 100)}%"></div></div><span>${Number(item.progress)}%</span></div>
      <div class="investment-controls"><input type="range" min="0" max="100" step="5" value="${Number(item.progress)}" data-investment-progress="${Number(item.id)}" aria-label="Progress for ${escapeHTML(item.title)}"><button class="delete-investment" data-investment-delete="${Number(item.id)}" aria-label="Delete ${escapeHTML(item.title)}">×</button></div>
    </article>
  `).join('');
}

function populateSettings(wallet) {
  const form = $('#settings-form');
  for (const [key, value] of Object.entries(wallet)) {
    if (form.elements.namedItem(key)) form.elements.namedItem(key).value = value;
  }
}

async function loadDashboard() {
  try {
    const data = await request(`action=dashboard&month=${encodeURIComponent(state.month)}`);
    renderDashboard(data);
  } catch (error) {
    showError(error.message);
  }
}

async function loadTransactions() {
  try {
    const query = new URLSearchParams({
      action: 'transactions',
      q: $('#transaction-search').value.trim(),
      month: state.month,
      period: state.allTransactions ? 'all' : 'month'
    });
    const data = await request(query);
    renderTransactions(data.transactions);
  } catch (error) {
    showError(error.message);
  }
}

function openDialog(dialog) {
  if (typeof dialog.showModal === 'function') dialog.showModal();
  else dialog.setAttribute('open', '');
}

function closeDialog(dialog) {
  if (typeof dialog.close === 'function') dialog.close();
  else dialog.removeAttribute('open');
}

function openTransactionForm() {
  const form = $('#transaction-form');
  form.reset();
  form.elements.date.value = new Date(Date.now() - new Date().getTimezoneOffset() * 60000).toISOString().slice(0, 10);
  form.elements.type.value = 'expense';
  $$('.type-option').forEach(button => button.classList.toggle('selected', button.dataset.type === 'expense'));
  $('#saving-hint').hidden = true;
  openDialog($('#transaction-modal'));
}

function openSettings() {
  if (state.dashboard) populateSettings(state.dashboard.wallet);
  openDialog($('#settings-modal'));
}

function attachEvents() {
  $('#today-label').textContent = new Intl.DateTimeFormat(undefined, { weekday: 'short', month: 'short', day: 'numeric' }).format(new Date());
  $('#footer-year').textContent = new Date().getFullYear();
  initializeMonthPicker();
  $('#month-filter').addEventListener('change', event => {
    state.month = event.target.value;
    state.allTransactions = false;
    $('#view-all-transactions').textContent = 'View all transactions →';
    loadDashboard();
  });
  $('#top-add').addEventListener('click', openTransactionForm);
  $('#open-settings').addEventListener('click', openSettings);
  $('#profile-settings').addEventListener('click', openSettings);
  $('#edit-goal').addEventListener('click', openSettings);
  $('#edit-budget').addEventListener('click', openSettings);
  $('#add-investment').addEventListener('click', () => {
    $('#investment-form').reset();
    $('#investment-form').elements.progress.value = 0;
    $('#investment-progress-value').textContent = '0%';
    $('#investment-currency').textContent = state.dashboard?.wallet?.currency || '$';
    openDialog($('#investment-modal'));
  });
  $('#mobile-menu').addEventListener('click', () => $('#sidebar').classList.toggle('sidebar-open'));
  $$('.nav-link[data-section]').forEach(link => link.addEventListener('click', () => {
    $$('.nav-link[data-section]').forEach(item => item.classList.toggle('active', item === link));
    $('#page-title').textContent = link.textContent.trim();
    $('#sidebar').classList.remove('sidebar-open');
  }));
  $$('.type-option').forEach(button => button.addEventListener('click', () => {
    $$('.type-option').forEach(item => item.classList.toggle('selected', item === button));
    $('#transaction-form').elements.type.value = button.dataset.type;
    $('#saving-hint').hidden = button.dataset.type !== 'income';
  }));
  $$('.close-modal, .cancel-modal').forEach(button => button.addEventListener('click', () => closeDialog(button.closest('dialog'))));
  $$('dialog').forEach(dialog => dialog.addEventListener('click', event => {
    if (event.target === dialog) closeDialog(dialog);
  }));
  $('#transaction-form').addEventListener('submit', async event => {
    event.preventDefault();
    const form = event.currentTarget;
    const values = Object.fromEntries(new FormData(form));
    try {
      state.month = values.date.slice(0, 7);
      $('#month-filter').value = state.month;
      const result = await request(`action=transaction&month=${encodeURIComponent(state.month)}`, { method: 'POST', body: JSON.stringify(values) });
      renderDashboard(result.dashboard);
      closeDialog($('#transaction-modal'));
      showToast('Transaction added.');
    } catch (error) {
      showToast(error.message);
    }
  });
  $('#settings-form').addEventListener('submit', async event => {
    event.preventDefault();
    const form = event.currentTarget;
    const values = Object.fromEntries(new FormData(form));
    const previousTheme = state.dashboard?.wallet?.theme || 'dark';
    applyTheme(values.theme);
    try {
      const result = await request(`action=settings&month=${encodeURIComponent(state.month)}`, { method: 'POST', body: JSON.stringify(values) });
      renderDashboard(result.dashboard);
      closeDialog($('#settings-modal'));
      showToast('Preferences saved.');
    } catch (error) {
      applyTheme(previousTheme);
      showToast(error.message);
    }
  });
  $('#investment-form').addEventListener('submit', async event => {
    event.preventDefault();
    const values = Object.fromEntries(new FormData(event.currentTarget));
    try {
      const result = await request(`action=investment&month=${encodeURIComponent(state.month)}`, { method: 'POST', body: JSON.stringify(values) });
      renderDashboard(result.dashboard);
      closeDialog($('#investment-modal'));
      showToast('Milestone added.');
    } catch (error) {
      showToast(error.message);
    }
  });
  $('#investment-form').elements.progress.addEventListener('input', event => {
    $('#investment-progress-value').textContent = `${event.target.value}%`;
  });
  $('#transaction-search').addEventListener('input', () => {
    window.clearTimeout(state.searchTimer);
    state.searchTimer = window.setTimeout(loadTransactions, 250);
  });
  $('#view-all-transactions').addEventListener('click', async () => {
    state.allTransactions = !state.allTransactions;
    $('#view-all-transactions').textContent = state.allTransactions ? 'Show this month →' : 'View all transactions →';
    await loadTransactions();
  });
  $('#transaction-list').addEventListener('click', async event => {
    const button = event.target.closest('[data-id]');
    if (!button || !window.confirm('Delete this transaction? This will also adjust your savings.')) return;
    try {
      const result = await request(`action=transaction&id=${encodeURIComponent(button.dataset.id)}&month=${encodeURIComponent(state.month)}`, { method: 'DELETE' });
      renderDashboard(result.dashboard);
      showToast('Transaction deleted.');
    } catch (error) {
      showToast(error.message);
    }
  });
  $('#investment-list').addEventListener('change', async event => {
    const input = event.target.closest('[data-investment-progress]');
    if (!input) return;
    try {
      const result = await request(`action=investment&id=${encodeURIComponent(input.dataset.investmentProgress)}&month=${encodeURIComponent(state.month)}`, {
        method: 'PATCH',
        body: JSON.stringify({ progress: Number(input.value) })
      });
      renderDashboard(result.dashboard);
      showToast('Milestone progress updated.');
    } catch (error) {
      showToast(error.message);
      await loadDashboard();
    }
  });
  $('#investment-list').addEventListener('click', async event => {
    const button = event.target.closest('[data-investment-delete]');
    if (!button || !window.confirm('Delete this milestone?')) return;
    try {
      const result = await request(`action=investment&id=${encodeURIComponent(button.dataset.investmentDelete)}&month=${encodeURIComponent(state.month)}`, { method: 'DELETE' });
      renderDashboard(result.dashboard);
      showToast('Milestone deleted.');
    } catch (error) {
      showToast(error.message);
    }
  });
  $('#export-link').addEventListener('click', event => {
    event.preventDefault();
    window.location.href = `${API}?action=export`;
  });
  $('#theme-toggle').addEventListener('click', async () => {
    if (!state.dashboard) return;
    const form = $('#settings-form');
    const values = Object.fromEntries(new FormData(form));
    const previousTheme = state.dashboard.wallet.theme;
    values.theme = previousTheme === 'light' ? 'dark' : 'light';
    applyTheme(values.theme);
    try {
      const result = await request(`action=settings&month=${encodeURIComponent(state.month)}`, { method: 'POST', body: JSON.stringify(values) });
      renderDashboard(result.dashboard);
    } catch (error) {
      applyTheme(previousTheme);
      showToast(error.message);
    }
  });
}

attachEvents();
loadDashboard();
