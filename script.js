lucide.createIcons();

const Store = {
    getActiveUser: () => localStorage.getItem('activeUser'),
    getHighScores: () => JSON.parse(localStorage.getItem('scores') || '[]'),
    saveScore: (score) => {
        const scores = Store.getHighScores();
        scores.push({ 
            username: Store.getActiveUser() || 'Guest', 
            score, 
            date: new Date().toISOString() 
        });
        localStorage.setItem('scores', JSON.stringify(scores));
    }
};

function navigate(viewId) {
    document.querySelectorAll('.view').forEach(v => v.classList.remove('active'));
    document.getElementById(`view-${viewId}`).classList.add('active');

    if (viewId === 'game') initGameUI();
    if (viewId === 'leaderboard') renderLeaderboard();
    updateNav();
}

function updateNav() {
    const user = Store.getActiveUser();
    const nav = document.getElementById('nav-user');
    nav.innerHTML = user 
        ? `<span class="text-green-400">${user}</span> <button onclick="Store.logout(); navigate('game')" class="hover:text-white">Logout</button>`
        : `<button onclick="navigate('login')" class="hover:text-white">Login</button> <button onclick="navigate('register')" class="bg-green-500/10 text-green-400 px-3 py-1 rounded hover:bg-green-500/20">Register</button>`;
}

function initGameUI() {
    document.getElementById('game-player').textContent = Store.getActiveUser() || "GUEST";
}

function renderLeaderboard() {
    const body = document.getElementById('leaderboard-body');
    const scores = Store.getHighScores().sort((a, b) => b.score - a.score);

    body.innerHTML = scores.map((s, i) => `
        <tr class="border-b border-slate-800 hover:bg-slate-800/50">
            <td class="px-6 py-4 font-bold text-slate-500">${i+1}</td>
            <td class="px-6 py-4 text-white">${s.username}</td>
            <td class="px-6 py-4 text-right text-green-400 font-black">${s.score}</td>
        </tr>
    `).join('');
}

// Запуск
updateNav();
initGameUI();