using Microsoft.AspNetCore.Mvc;
using crud_cache_vehicle.Models;
using MySqlConnector;
using Dapper;
using StackExchange.Redis;
using Newtonsoft.Json;

namespace crud_cache_vehicle.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehiclesController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IDatabase _redisDatabase;
        private const string CacheKey = "all_vehicles";

       
        public VehiclesController(IConfiguration configuration, IConnectionMultiplexer redis)
        {
            _configuration = configuration;
            _redisDatabase = redis.GetDatabase();
        }

        // GET: api/vehicles
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // 1. Tenta buscar os dados do cache Redis primeiro
            var cachedVehicles = await _redisDatabase.StringGetAsync(CacheKey);
            if (!cachedVehicles.IsNullOrEmpty)
            {
                var vehicles = JsonConvert.DeserializeObject<List<Vehicle>>(cachedVehicles!);
                return Ok(vehicles); // Retorna do cache se encontrar
            }

            // 2. Se não encontrar no cache, busca no banco de dados MySQL
            using var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            var dbVehicles = await connection.QueryAsync<Vehicle>("SELECT * FROM vehicles");

            // 3. Salva o resultado no cache para as próximas requisições
            var serializedVehicles = JsonConvert.SerializeObject(dbVehicles);
            await _redisDatabase.StringSetAsync(CacheKey, serializedVehicles, TimeSpan.FromMinutes(10)); // O cache expira em 10 minutos [cite: 245]

            return Ok(dbVehicles); // Retorna do banco
        }

        // POST: api/vehicles
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Vehicle vehicle)
        {
            using var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            var sql = "INSERT INTO vehicles (Brand, Model, Year, Plate) VALUES (@Brand, @Model, @Year, @Plate);";
            await connection.ExecuteAsync(sql, vehicle);

            // Deleta o cache, pois um novo veículo foi adicionado
            await _redisDatabase.KeyDeleteAsync(CacheKey);
            return Created();
        }

        // PUT: api/vehicles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Vehicle vehicle)
        {
            vehicle.Id = id;
            using var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            var sql = "UPDATE vehicles SET Brand = @Brand, Model = @Model, Year = @Year, Plate = @Plate WHERE Id = @Id";
            await connection.ExecuteAsync(sql, vehicle);

            // Deleta o cache, pois um veículo foi alterado
            await _redisDatabase.KeyDeleteAsync(CacheKey);
            return NoContent();
        }

        // DELETE: api/vehicles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            using var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            var sql = "DELETE FROM vehicles WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new { Id = id });

            // Deleta o cache, pois um veículo foi removido
            await _redisDatabase.KeyDeleteAsync(CacheKey);
            return NoContent();
        }
    }
}