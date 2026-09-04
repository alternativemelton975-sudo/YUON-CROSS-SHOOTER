# Yuon Cross Shooter

A browser-based 2D shooter game built with ASP.NET Core and HTML5 Canvas. Control a crosshair and shoot incoming enemies!

## Features

- **Real-time Gameplay** - Smooth 60 FPS rendering
- **Pointer Controls** - Move crosshair with mouse/touch
- **Auto-Fire** - Hold to continuously shoot
- **Enemy Waves** - Enemies spawn from all sides
- **Particle Effects** - Colorful explosion particles
- **Score Tracking** - Track your score and remaining lives
- **Game Over Screen** - Restart the game anytime

## Requirements

- .NET 8.0
- Any modern web browser (Chrome, Firefox, Edge, Safari)

## Installation

```bash
git clone https://github.com/yourusername/yuon-cross-shooter.git
cd yuon-cross-shooter
dotnet run
```

## Gameplay

- **Move** - Move your mouse/touch to control the crosshair
- **Shoot** - Click/tap to fire a single shot
- **Auto-Fire** - Hold mouse button to continuously shoot
- **Enemies** - Red circles approaching from all sides
- **Lives** - You have 3 lives, each enemy hit costs 1 life
- **Score** - 10 points per enemy defeated

## Game Rules

- Enemies spawn from screen edges
- Enemies move toward the center
- Colliding with an enemy costs 1 life
- Defeating an enemy gives 10 points
- Restart the game when all lives are lost

## Docker

Build and run with Docker:

```bash
docker build -t yuon-cross-shooter .
docker run -p 8080:8080 yuon-cross-shooter:latest
```

Then visit: http://localhost:8080/cross-shooter

## Technical Details

- **Framework:** ASP.NET Core 8.0
- **Rendering:** HTML5 Canvas 2D
- **Physics:** Custom collision detection
- **Animation:** RequestAnimationFrame for smooth gameplay

## Controls

- **Mouse/Touch Move** - Control crosshair position
- **Click/Tap** - Fire shots
- **Hold Button** - Auto-fire mode
- **Restart Button** - Reset the game

## License

MIT

## Author

Your Name
