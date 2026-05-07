const gameEngine = {
  config: {
    gridSize: 15, 
    mode: 'classic',
    snakeSkin: 'green',
    foodSkin: 'red',
    foodCount: 1,
    speed: 120
  },
  
  snake: [],
  dirQueue: [], 
  currentDir: {x: 0, y: -1}, 
  foods: [], 
  score: 0,
  gameOver: false,
  gameStarted: false,
  intervalId: null,
  domCells: [],

  showSettings() {
    this.stop();
    document.getElementById('overlay-settings').classList.remove('hidden');
    document.getElementById('overlay-victory').classList.add('hidden');
  },

  applySettings() {
    this.config.gridSize = parseInt(document.getElementById('set-map').value);
    this.config.mode = document.getElementById('set-mode').value;
    this.config.snakeSkin = document.getElementById('set-snake').value;
    this.config.foodSkin = document.getElementById('set-fruit').value;
    this.config.foodCount = parseInt(document.getElementById('set-fruit-count').value);
    this.config.speed = parseInt(document.getElementById('set-speed').value);
    
    document.getElementById('mode-display').innerText = this.config.mode.toUpperCase() + " MODE";

    document.getElementById('overlay-settings').classList.add('hidden');
    this.initGrid();
    
    this.score = 0;
    this.reset();
  },

  initGrid() {
    const gridEl = document.getElementById('game-grid');
    gridEl.innerHTML = '';
    gridEl.style.gridTemplateColumns = `repeat(${this.config.gridSize}, 1fr)`;
    this.domCells = [];
    
    for (let y = 0; y < this.config.gridSize; y++) {
      for (let x = 0; x < this.config.gridSize; x++) {
        const wrap = document.createElement('div');
        wrap.className = 'aspect-square';
        const cell = document.createElement('div');
        wrap.appendChild(cell);
        gridEl.appendChild(wrap);
        this.domCells.push({ x, y, el: cell });
      }
    }
  },

  reset() {
    this.stop();
    const center = Math.floor(this.config.gridSize / 2);
    this.snake = [{x: center, y: center}, {x: center, y: center + 1}, {x: center, y: center + 2}];
    this.currentDir = {x: 0, y: -1}; 
    this.dirQueue = [];
    
    if (this.gameOver) {
      this.score = 0;
    }

    this.foods = [];
    this.gameOver = false;
    this.gameStarted = false;
    
    document.getElementById('game-score').textContent = this.score.toString().padStart(4, '0');
    document.getElementById('game-score').className = `text-2xl font-black tabular-nums text-${this.config.snakeSkin}-400`;
    
    document.getElementById('overlay-start').classList.remove('hidden');
    document.getElementById('overlay-gameover').classList.add('hidden');
    document.getElementById('overlay-victory').classList.add('hidden');
    
    this.replenishFood();
    this.render();
  },

  handleEndlessNextLevel() {
    const center = Math.floor(this.config.gridSize / 2);
    this.snake = [{x: center, y: center}, {x: center, y: center + 1}, {x: center, y: center + 2}];
    this.currentDir = {x: 0, y: -1}; 
    this.dirQueue = [];
    this.foods = [];
    this.replenishFood();
  },

  replenishFood() {
    let attempts = 0;
    const maxFoods = Math.min(this.config.foodCount, (this.config.gridSize * this.config.gridSize) - this.snake.length);
    
    while (this.foods.length < maxFoods && attempts < 100) {
      const pos = {
        x: Math.floor(Math.random() * this.config.gridSize),
        y: Math.floor(Math.random() * this.config.gridSize)
      };
      
      const inSnake = this.snake.some(s => s.x === pos.x && s.y === pos.y);
      const inFoods = this.foods.some(f => f.x === pos.x && f.y === pos.y);
      
      if (!inSnake && !inFoods) {
        this.foods.push(pos);
      }
      attempts++;
    }
  },

  start() {
    if (this.gameStarted) return;
    this.gameStarted = true;
    document.getElementById('overlay-start').classList.add('hidden');
    this.intervalId = setInterval(() => this.tick(), this.config.speed);
  },

  stop() {
    if (this.intervalId) clearInterval(this.intervalId);
    this.intervalId = null;
  },

  tick() {
    if (this.dirQueue.length > 0) {
      this.currentDir = this.dirQueue.shift();
    }

    const head = { x: this.snake[0].x + this.currentDir.x, y: this.snake[0].y + this.currentDir.y };

    if (head.x < 0 || head.x >= this.config.gridSize || head.y < 0 || head.y >= this.config.gridSize) {
      return this.handleGameOver();
    }

    if (this.snake.some(s => s.x === head.x && s.y === head.y)) {
      return this.handleGameOver();
    }

    this.snake.unshift(head);

    const foodIndex = this.foods.findIndex(f => f.x === head.x && f.y === head.y);
    if (foodIndex !== -1) {
      this.score += 10;
      document.getElementById('game-score').textContent = this.score.toString().padStart(4, '0');
      this.foods.splice(foodIndex, 1);
      
      if (this.snake.length === this.config.gridSize * this.config.gridSize) {
        if (this.config.mode === 'endless') {
          this.handleEndlessNextLevel();
          return;
        } else {
          return this.handleVictory();
        }
      }
      
      this.replenishFood();
    } else {
      this.snake.pop(); 
    }

    this.render();
  },

  handleVictory() {
    this.gameOver = true;
    this.stop();
    document.getElementById('overlay-victory').classList.remove('hidden');
    document.getElementById('victory-score').textContent = this.score;
    if (this.score > 0) {
      const playerName = document.getElementById('game-player-name').innerText || 'Guest';
      store.saveScore(playerName, this.score, this.config.mode, this.config.gridSize);
    }
  },

  handleGameOver() {
    this.gameOver = true;
    this.stop();
    document.getElementById('overlay-gameover').classList.remove('hidden');
    document.getElementById('final-score').textContent = this.score;
    if (this.score > 0) {
      const playerName = document.getElementById('game-player-name').innerText || 'Guest';
      store.saveScore(playerName, this.score, this.config.mode, this.config.gridSize);
    }
  },

  handleJoystick(dir, e) {
    if (e) e.preventDefault();
    
    if (!document.getElementById('overlay-settings').classList.contains('hidden')) return;

    if (this.gameOver) return;
    if (!this.gameStarted) this.start();

    const lastDir = this.dirQueue.length > 0 ? this.dirQueue[this.dirQueue.length - 1] : this.currentDir;
    let nextDir = null;

    if (dir === 'UP' && lastDir.y !== 1) nextDir = {x: 0, y: -1};
    if (dir === 'DOWN' && lastDir.y !== -1) nextDir = {x: 0, y: 1};
    if (dir === 'LEFT' && lastDir.x !== 1) nextDir = {x: -1, y: 0};
    if (dir === 'RIGHT' && lastDir.x !== -1) nextDir = {x: 1, y: 0};

    if (nextDir && this.dirQueue.length < 2) {
      this.dirQueue.push(nextDir);
    }
  },

  render() {
    const head = this.snake[0];

    for (let i = 0; i < this.domCells.length; i++) {
      const cell = this.domCells[i];
      let classes = "w-full h-full transition-colors duration-100 ";
      
      if (this.config.mode === 'blind' && this.gameStarted && !this.gameOver) {
        const distance = Math.max(Math.abs(cell.y - head.y), Math.abs(cell.x - head.x));
        if (distance > 3) {
          cell.el.className = classes + "bg-black w-full h-full border border-black";
          continue;
        } else if (distance > 1) {
          classes += "opacity-40 "; 
        }
      }

      let isSnakeHead = (head.x === cell.x && head.y === cell.y);
      let isSnakeBody = this.snake.some(s => s.x === cell.x && s.y === cell.y && !isSnakeHead);
      let isFood = this.foods.some(f => f.x === cell.x && f.y === cell.y);

      if (isSnakeHead) {
        classes += `snake-head skin-${this.config.snakeSkin}`;
      } else if (isSnakeBody) {
        classes += `snake-body skin-${this.config.snakeSkin} opacity-80`;
      } else if (isFood) {
        classes += `food-item skin-${this.config.foodSkin}`;
      } else {
        classes += "bg-slate-900 border-b border-r border-slate-800/50";
      }
      
      cell.el.className = classes;
    }
  }
};

document.addEventListener('keydown', e => {
  if (['ArrowUp', 'ArrowDown', 'ArrowLeft', 'ArrowRight'].includes(e.key)) {
    e.preventDefault();
    gameEngine.handleJoystick(e.key.replace('Arrow', '').toUpperCase());
  }
});