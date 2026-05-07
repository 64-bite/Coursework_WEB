const { createClient } = supabase;
const supabaseUrl = 'https://jpwxjotloclulyazpjao.supabase.co';
const supabaseKey = 'sb_publishable_zwTKLjb6H09UytWATCV54g_t79gzrte';
const db = createClient(supabaseUrl, supabaseKey);

const authManager = {
  isValidEmail(email) {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  },

  async register() {
    console.log("1. Кнопка натиснута!"); 
    const email = document.getElementById('reg-email').value;
    const password = document.getElementById('reg-password').value;
    const nickname = document.getElementById('reg-nickname').value;

    console.log("2. Дані з полів:", { email, password, nickname });

    if (!email || !password || !nickname) {
      console.log("3. Зупинка: не всі поля заповнені");
      return alert('Заповніть всі поля!');
    }
    
    if (!this.isValidEmail(email)) {
      console.log("3. Зупинка: неправильний формат пошти");
      return alert('Введіть дійсну електронну адресу!');
    }
    
    if (password.length < 6) {
      console.log("3. Зупинка: надто короткий пароль");
      return alert('Пароль має містити мінімум 6 символів!');
    }

    console.log("4. Відправляємо запит у Supabase...");

    const { data, error } = await db.auth.signUp({
      email: email,
      password: password,
      options: { data: { username: nickname } } 
    });

    if (error) {
      console.error("5. Помилка від Supabase:", error.message);
      alert('Помилка: ' + error.message);
    } else {
      console.log("5. УСПІХ! Supabase прийняв реєстрацію.");
      alert('Реєстрація успішна! Перевірте вашу пошту для підтвердження акаунта.');
      document.getElementById('modal-register').classList.add('hidden');
    }
  },

  async login() {
    const email = document.getElementById('login-email').value;
    const password = document.getElementById('login-password').value;

    const { data, error } = await db.auth.signInWithPassword({
      email: email,
      password: password
    });

    if (error) alert('Невірний логін або пароль (або пошта ще не підтверджена)!');
    else {
      document.getElementById('modal-login').classList.add('hidden');
      this.checkSession();
    }
  },

  async loginWithGoogle() {
    const { data, error } = await db.auth.signInWithOAuth({
      provider: 'google',
      options: {
        redirectTo: window.location.origin 
      }
    });
    if (error) alert('Помилка Google авторизації: ' + error.message);
  },

  async logout() {
    await db.auth.signOut();
    this.checkSession();
    alert('Ви вийшли з акаунту.');
  },

  async checkSession() {
    const { data: { user } } = await db.auth.getUser();

    const authButtons = document.getElementById('auth-buttons');
    const userProfile = document.getElementById('user-profile');
    const gamePlayerName = document.getElementById('game-player-name');

    if (user) {
      const nickname = user.user_metadata.username || user.user_metadata.full_name || 'PLAYER';
      
      authButtons.classList.add('hidden');
      userProfile.classList.remove('hidden');
      userProfile.classList.add('flex');
      
      document.getElementById('nav-username').innerHTML = `<i data-lucide="user" class="w-5 h-5 text-green-400"></i> ${nickname}`;
      gamePlayerName.innerText = nickname; 
    } else {
      authButtons.classList.remove('hidden');
      userProfile.classList.add('hidden');
      userProfile.classList.remove('flex');
      gamePlayerName.innerText = 'GUEST';
    }
    
    if (typeof lucide !== 'undefined') lucide.createIcons();
  }
};

const store = {
  getHighScores: async (mode = 'classic') => {
    const { data, error } = await db
      .from('scores')
      .select('*')
      .eq('mode', mode) 
      .order('score', { ascending: false })
      .limit(50);
    
    if (error) console.error("Помилка завантаження:", error);
    return data || [];
  },
  saveScore: async (username, score, mode, mapSize) => {
    const { error } = await db
      .from('scores')
      .insert([{ 
        player_name: username, 
        score: score,
        mode: mode,
        map_size: mapSize
      }]);

    if (error) console.error("Помилка збереження:", error);
  }
};

async function renderLeaderboard(mode = 'classic') {
  document.querySelectorAll('.lb-tab').forEach(btn => {
    btn.classList.remove('bg-green-500', 'text-slate-950');
    btn.classList.add('bg-slate-800', 'text-slate-400');
  });
  const activeBtn = document.getElementById(`btn-lb-${mode}`);
  if (activeBtn) {
    activeBtn.classList.remove('bg-slate-800', 'text-slate-400');
    activeBtn.classList.add('bg-green-500', 'text-slate-950');
  }

  const tbody = document.getElementById('leaderboard-body');
  tbody.innerHTML = `<tr><td colspan="4" class="px-6 py-8 text-center text-slate-500 animate-pulse">Завантаження рекордів ${mode.toUpperCase()}...</td></tr>`;
  
  const scores = await store.getHighScores(mode);
  
  if (scores.length === 0) {
    tbody.innerHTML = '<tr><td colspan="4" class="px-6 py-8 text-center text-slate-500">Немає рекордів у цьому режимі. Станьте першим!</td></tr>';
    return;
  }

  tbody.innerHTML = scores.map((s, i) => {
    const dateObj = new Date(s.created_at);
    const dateStr = dateObj.toLocaleDateString() + ' ' + dateObj.toLocaleTimeString([], {hour: '2-digit', minute:'2-digit'});
    
    return `
    <tr class="hover:bg-slate-800/50">
      <td class="px-6 py-4 font-medium text-slate-200">#${i+1} ${s.player_name}</td>
      <td class="px-4 py-4 text-center text-slate-400 text-sm">${s.map_size} x ${s.map_size}</td>
      <td class="px-4 py-4 text-center text-slate-500 text-xs">${dateStr}</td>
      <td class="px-6 py-4 text-right font-black text-white">${s.score}</td>
    </tr>
  `}).join('');
}

function navigate(viewId) {
  document.querySelectorAll('.view').forEach(v => v.classList.remove('active-view'));
  
  if (viewId !== 'game') {
    gameEngine.stop();
  } else {
    if (gameEngine.domCells.length === 0) {
      gameEngine.initGrid();
      gameEngine.reset();
    }
  }

  document.getElementById(`view-${viewId}`).classList.add('active-view');

  if (viewId === 'leaderboard') renderLeaderboard('classic');
  
  if (typeof lucide !== 'undefined') {
    lucide.createIcons();
  }
}

document.addEventListener("DOMContentLoaded", () => {
  if (typeof lucide !== 'undefined') lucide.createIcons();
  setTimeout(() => navigate('game'), 50);
  
  authManager.checkSession();
});