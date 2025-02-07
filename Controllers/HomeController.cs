using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NOTISSIMUSTest.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace NOTISSIMUSTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult Index()
        {
            //ModelDataFromIndex[] oleg = { new ModelDataFromIndex { X = 1, Y = 1, T = "0.0.0 0:0:0:0" }, new ModelDataFromIndex { X = 0, Y = 0, T = "0.0.0 0:0:0:0" }, new ModelDataFromIndex { X = 0, Y = 0, T = "0.0.0 0:0:0:0" } };
            
            //oleg[0] = new ModelDataFromIndex { X = 0, Y = 0, T = "0.0.0 0:0:0:0" };
            //Task task = GetDataforDB(oleg);
            //task.WaitAsync(new TimeSpan());
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        /// <summary>
        /// Получение данных из POST запроса и внесение в БД с возращением на главную страницу
        /// </summary>
        /// <param name="Dataa">Данные POST запроса в виде string Data в моделе ArrayJSONInput</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetDataforDB(ArrayJSONInput Dataa)
        {
            ModelDataFromIndex[] json = null;
            try
            {
                json = JsonSerializer.Deserialize<ModelDataFromIndex[]>(Dataa.Data);
            }
            catch
            {
                Console.WriteLine("Error with Deseralize!");
                return RedirectToRoute(new { controller = "Home", action = "Index" });
            }
            try { 
            DataContext db = new DataContext();

                EntityService entityService = new EntityService(db);
                bool created = db.Database.EnsureCreated();
                if (created)
                {
                    Console.WriteLine("Ensure is Created!");
                }
                else
                {
                   // Console.WriteLine("Ensure was created!");
                }

                await entityService.SaveEntityAsync(json);
                Console.WriteLine("Cool");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return RedirectToRoute(new { controller = "Home", action = "Index" });

        }


        public class DataContext : DbContext
        {
            public DbSet<MyEntity> Entities { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=password;");
            }
        }

        public class MyEntity
        {
            public int Id { get; set; }
            public string JsonData { get; set; }
        }

        public class EntityService
        {
            private readonly DataContext _context;

            public EntityService(DataContext context)
            {
                _context = context;
                
            }

            public async Task SaveEntityAsync(object data)
            {
                string jsonData = JsonSerializer.Serialize(data);
                MyEntity entity = new MyEntity { JsonData = jsonData };

                _context.Entities.Add(entity);
                await _context.SaveChangesAsync();
            }

        }

    }
}
