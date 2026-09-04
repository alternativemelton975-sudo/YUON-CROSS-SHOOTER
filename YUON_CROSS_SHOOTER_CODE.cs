using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.Use(async (context, next) =>
{
    var path = context.Request.Path.HasValue ? context.Request.Path.Value : string.Empty;
    if (string.Equals(path, "/game", System.StringComparison.OrdinalIgnoreCase) && string.Equals(context.Request.Method, "GET", System.StringComparison.OrdinalIgnoreCase))
    {
        context.Response.Redirect("/cross-shooter");
        return;
    }

    await next();
});

app.UseRouting();

app.MapRazorPages();

app.MapGet("/cross-shooter", async context =>
{
    context.Response.ContentType = "text/html; charset=utf-8";

    var html = """
<!doctype html>
<html lang="en">
<head>
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width,initial-scale=1">
  <title>Cross Shooter</title>
  <style>
    html,body{height:100%;margin:0;background:#111;color:#eee;font-family:sans-serif}
    #game{display:block;margin:0 auto;background:#000;border:4px solid #222}
    #hud{position:fixed;left:12px;top:12px;z-index:10}
    button{padding:6px 10px;margin-left:8px}
  </style>
</head>
<body>
  <div id="hud">Score: <span id="score">0</span> Lives: <span id="lives">3</span><button id="restart">Restart</button></div>
  <canvas id="game" width="800" height="600"></canvas>
  <script>
  (function(){
    function start(){
      const canvas = document.getElementById('game');
      const scoreEl = document.getElementById('score');
      const livesEl = document.getElementById('lives');
      const restartBtn = document.getElementById('restart');
      if(!canvas) return;

      const ctx = canvas.getContext('2d');
      const W = canvas.width, H = canvas.height;

      canvas.style.touchAction = 'none';

      let mouse = {x: W/2, y: H/2};
      let bullets = [];
      let enemies = [];
      let particles = [];
      let score = 0;
      let lives = 3;
      let running = true;

      scoreEl.textContent = score;
      livesEl.textContent = lives;

      canvas.addEventListener('pointermove', e => {
        const rect = canvas.getBoundingClientRect();
        mouse.x = (e.clientX - rect.left) * (canvas.width / rect.width);
        mouse.y = (e.clientY - rect.top) * (canvas.height / rect.height);
      });

      canvas.addEventListener('pointerdown', () => {
        if (!running) return;
        const angle = Math.atan2(mouse.y - H/2, mouse.x - W/2);
        bullets.push({x:W/2, y:H/2, vx:Math.cos(angle)*6, vy:Math.sin(angle)*6, life:0});
      });

      let firing = false, fireTimer = 0;
      canvas.addEventListener('pointerdown', () => firing = true);
      window.addEventListener('pointerup', () => firing = false);

      restartBtn.addEventListener('click', () => reset());

      function spawnEnemy(){
        const side = Math.floor(Math.random()*4);
        let x = 0, y = 0;
        const margin = 20;
        if(side===0){ x = -margin; y = Math.random()*H; }
        else if(side===1){ x = W+margin; y = Math.random()*H; }
        else if(side===2){ x = Math.random()*W; y = -margin; }
        else { x = Math.random()*W; y = H+margin; }
        const angle = Math.atan2(H/2 - y, W/2 - x);
        const speed = 1 + Math.random()*1.2;
        enemies.push({x,y, vx:Math.cos(angle)*speed, vy:Math.sin(angle)*speed, r:12 + Math.random()*10, hp:1 + Math.floor(Math.random()*2)});
      }

      function reset(){
        bullets = []; enemies = []; particles = [];
        score = 0; lives = 3; running = true;
        scoreEl.textContent = score;
        livesEl.textContent = lives;
      }

      function update(){
        if(!running) return;

        if(firing){
          fireTimer++;
          if(fireTimer % 8 === 0){
            const angle = Math.atan2(mouse.y - H/2, mouse.x - W/2);
            bullets.push({x:W/2, y:H/2, vx:Math.cos(angle)*6, vy:Math.sin(angle)*6, life:0});
          }
        } else fireTimer = 0;

        if(Math.random() < 0.02) spawnEnemy();

        for(let i=bullets.length-1;i>=0;i--){
          const b = bullets[i];
          b.x += b.vx; b.y += b.vy; b.life++;
          if(b.x<-50 || b.x>W+50 || b.y<-50 || b.y>H+50 || b.life>120)
            bullets.splice(i,1);
        }

        for(let j=enemies.length-1;j>=0;j--){
          const e = enemies[j];
          e.x += e.vx; e.y += e.vy;

          const dx = e.x - W/2, dy = e.y - H/2;
          if(Math.hypot(dx,dy) < 18){
            enemies.splice(j,1);
            lives--;
            livesEl.textContent = lives;
            for(let p=0;p<12;p++) createParticle(W/2,H/2);
            if(lives<=0) running=false;
            continue;
          }

          for(let i=bullets.length-1;i>=0;i--){
            const b = bullets[i];
            const ddx = b.x - e.x, ddy = b.y - e.y;
            if(Math.hypot(ddx,ddy) < e.r){
              bullets.splice(i,1);
              e.hp--;
              if(e.hp<=0){
                enemies.splice(j,1);
                score += 10;
                scoreEl.textContent = score;
                for(let p=0;p<8;p++) createParticle(e.x,e.y);
              }
              break;
            }
          }
        }

        for(let i=particles.length-1;i>=0;i--){
          const p = particles[i];
          p.x += p.vx; p.y += p.vy; p.life--;
          if(p.life<=0) particles.splice(i,1);
        }
      }

      function createParticle(x,y){
        const ang = Math.random()*Math.PI*2;
        const sp = Math.random()*3;
        particles.push({x,y, vx:Math.cos(ang)*sp, vy:Math.sin(ang)*sp, life:20+Math.random()*20, c:`hsl(${Math.floor(Math.random()*360)},80%,60%)`});
      }

      function draw(){
        ctx.fillStyle = '#000';
        ctx.fillRect(0,0,W,H);

        ctx.save();
        ctx.translate(W/2,H/2);
        ctx.strokeStyle = '#0f0';
        ctx.lineWidth = 2;
        ctx.beginPath();
        ctx.moveTo(-14,0); ctx.lineTo(14,0);
        ctx.moveTo(0,-14); ctx.lineTo(0,14);
        ctx.stroke();
        ctx.restore();

        ctx.fillStyle = '#ff0';
        bullets.forEach(b => {
          ctx.beginPath();
          ctx.arc(b.x,b.y,3,0,Math.PI*2);
          ctx.fill();
        });

        enemies.forEach(e => {
          ctx.beginPath();
          ctx.fillStyle = 'tomato';
          ctx.arc(e.x,e.y,e.r,0,Math.PI*2);
          ctx.fill();
          ctx.strokeStyle = '#222';
          ctx.lineWidth = 2;
          ctx.stroke();
        });

        particles.forEach(p => {
          ctx.fillStyle = p.c;
          ctx.fillRect(p.x-1,p.y-1,2,2);
        });

        if(!running){
          ctx.fillStyle = 'rgba(0,0,0,0.6)';
          ctx.fillRect(0,H/2-40,W,80);
          ctx.fillStyle = '#fff';
          ctx.font = '28px sans-serif';
          ctx.textAlign = 'center';
          ctx.fillText('Game Over - Click Restart', W/2, H/2);
        }
      }

      function loop(){
        update();
        draw();
        requestAnimationFrame(loop);
      }
      loop();
    }

    if(document.readyState === 'loading')
      document.addEventListener('DOMContentLoaded', start);
    else start();
  })();
  </script>
</body>
</html>
""";

    await context.Response.WriteAsync(html);
});

app.MapGet("/Game", ctx => {
    ctx.Response.Redirect("/cross-shooter");
    return Task.CompletedTask;
});

app.MapGet("/game", ctx => {
    ctx.Response.Redirect("/cross-shooter");
    return Task.CompletedTask;
});

app.Run();
