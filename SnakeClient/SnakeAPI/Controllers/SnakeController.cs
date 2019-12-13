using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SnakeAPI.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SnakeAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    public class SnakeController : Controller
    {
        // Initialization of Game
        public static GameBoard Game = new GameBoard(false);

        // GET: api/<controller>
        [HttpGet]
        [ProducesResponseType(typeof(String), 200)]
        [ProducesResponseType(400)]
        public IActionResult Get()
        {
            try
            { 
                if (!Game.IsActive)
                    return BadRequest("Use POST method \"CreateMap\" with following fields:\n'TimeUntilNextTurnMS'\n'Width'\n'Height'");
                else
                    if (Game._Snake != null)
                        return Ok(Game);
                else
                    return BadRequest("Start the game with 'PlayOrStop' method");
            }
            catch (Exception b)
            {
                return (BadRequest(b));
            }
        }

        // POST Creating map
        [HttpPost]
        public IActionResult CreateMap([FromBody] GameBoard game)
        {
            try
            {
                if (!((bool)Game.IsActive))
                {
                    if (game.Width > 3 && game.Height > 3 && game.TimeUntilNextTurnMS > 0)
                    {
                        Game.TimeUntilNextTurnMS = game.TimeUntilNextTurnMS;
                        Game.Width = game.Width;
                        Game.Height = game.Height;
                        Game.IsActive = true;
                        return (Ok("Map Created!"));
                    }
                    else
                        return (BadRequest("Incorrect params"));
                }
                else
                    return (BadRequest("Map already created"));
            }
            catch (Exception b)
            {
                return BadRequest(b);
            }
        }

        //To see map in Postman
        [HttpGet]
        public IActionResult GetMap()
        {
            try
            {
                List<List<int>> Map = new List<List<int>>();

                for (int i = 0; i < Game.Height; i++)
                {
                    Map.Add((Enumerable.Repeat(0, Game.Width).ToList()));
                }
                Map[Game._Snake.Food.Y - 1][Game._Snake.Food.X - 1] = 2;
                foreach (var snakePart in Game._Snake.PartsOfSnake)
                {
                    Map[snakePart.Y - 1][snakePart.X - 1] = 1;
                }

                string MapString = String.Join("", Map[0]) + "\n";
                for (int i = 1; i < Map.Count; i++)
                {
                    MapString = MapString + String.Join("", Map[i]) + "\n";
                }
                MapString = MapString + "Snake is alive: " + Game._Snake.IsAlive.ToString() + "\n";
                MapString = MapString + "Turn nubmer: " + Game.TurnNumber.ToString() + "\n";
                return (Ok(MapString));
            }
            catch (Exception b)
            {
                return BadRequest(b);
            }
        }

        //With this method u can Recreate the map
        [HttpPost]
        public IActionResult RecreateMap([FromBody] GameBoard game)
        {
            try
            {
                if (game.WantPlaying)
                    return BadRequest("U must stop the game with 'playorstop' method");
                Game.IsActive = false;
                return (CreateMap(game));
            }
            catch (Exception b)
            {
                return BadRequest(b);
            }
        }

        //This request start or stop the game
        [HttpPost]
        public IActionResult PlayOrStop()
        {
            try
            {
                if (!Game.WantPlaying)
                    Game.StartGame();
                else
                {
                    Game.WantPlaying = false;
                    Game.IsActive = false;
                    Game._Snake.IsAlive = false;
                }
                return (Ok());
            }
            catch (Exception b)
            {
                return (BadRequest(b));
            }
        }

        //Change the direction of snake's moving with this method
        [HttpPost]
        public IActionResult ChangeDirection([FromBody] string Direction)
        {
            try
            {
                if (Game.ChangeDirectionOfSnake(Direction))
                    return (Ok("It's rotated"));
                else
                    return (BadRequest("U can't turn to 180 degrees"));
            }
            catch (Exception b)
            {
                return BadRequest(b);
            }
        }
    }
}
