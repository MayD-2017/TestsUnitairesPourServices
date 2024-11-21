using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using TestsUnitairesPourServices.Data;
using TestsUnitairesPourServices.Exceptions;
using TestsUnitairesPourServices.Models;

namespace TestsUnitairesPourServices.Services.Tests
{
    [TestClass()]
    public class CatsServiceTests
    {
        ApplicationDBContext _db;

        private const int CLEAN_HOUSE_ID = 1;
        private const int DIRTY_HOUSE_ID = 2;

        private const int WILD_CAT_ID = 1;
        private const int CAT_IN_DIRTY_HOUSE_ID = 2;

        [TestInitialize]
        public void Init()
        {
            // En utilisant un nom de BD différent pour chaque test, pas besoin de faire de clean up de la BD à chaque fois
            string dbName = Guid.NewGuid().ToString();
            DbContextOptions<ApplicationDBContext> options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .UseLazyLoadingProxies(true)
                .Options;

            // TODO avoir la durée de vie d'un context la plus petite possible
            _db = new ApplicationDBContext(options);

            _db.Cat.Add(new Cat()
            {
                Id = WILD_CAT_ID,
                Name = "Lonely",
                Age = 12
            });

            House maisonPropre = new House()
            {
                Id = CLEAN_HOUSE_ID,
                Address = "Tite maison propre et orange",
                OwnerName = "Ludwig"
            };

            House maisonSale = new House()
            {
                Id = DIRTY_HOUSE_ID,
                Address = "Grosse maison sale",
                OwnerName = "Bob"
            };

            _db.House.Add(maisonPropre);
            _db.House.Add(maisonSale);

            Cat chatPasPropre = new Cat()
            {
                Id = CAT_IN_DIRTY_HOUSE_ID,
                Name = "ToutSale",
                Age = 3,
                House = maisonSale
            };
            _db.Cat.Add(chatPasPropre);
            _db.SaveChanges();
        }

        [TestCleanup]
        public void Dispose()
        {
            _db.Dispose();
        }

        [TestMethod()]
        public void MoveTest()
        {
            var catsService = new CatsService(_db);
            var maisonPropre = _db.House.Find(CLEAN_HOUSE_ID)!;
            var maisonSale = _db.House.Find(DIRTY_HOUSE_ID)!;

            // Tout est bon, le chat va être dans une maison propre
            var chatMaintenantPropre = catsService.Move(CAT_IN_DIRTY_HOUSE_ID, maisonSale, maisonPropre);
            Assert.IsNotNull(chatMaintenantPropre);
        }

        [TestMethod()]
        public void MoveTestCatNotFound()
        {
            var catsService = new CatsService(_db);
            var maisonPropre = _db.House.Find(CLEAN_HOUSE_ID)!;
            var maisonSale = _db.House.Find(DIRTY_HOUSE_ID)!;

            //Retourne null si le chat ne peut pas être trouvé (aucun chat avec Id: 42)
            var cat = catsService.Move(42, maisonSale, maisonPropre);
            Assert.IsNull(cat);
        }

        [TestMethod()]
        public void MoveTestNoHouse()
        {
            var catsService = new CatsService(_db);
            var maisonPropre = _db.House.Find(CLEAN_HOUSE_ID)!;
            var maisonSale = _db.House.Find(DIRTY_HOUSE_ID)!;

            //Le chat avec l'Id 1 n'a pas de maison
            Exception e = Assert.ThrowsException<WildCatException>(() => catsService.Move(WILD_CAT_ID, maisonSale, maisonPropre));
            Assert.AreEqual("On n'apprivoise pas les chats sauvages", e.Message);
        }

        [TestMethod()]
        public void MoveTestWrongHouse()
        {
            var catsService = new CatsService(_db);
            var maisonPropre = _db.House.Find(CLEAN_HOUSE_ID)!;
            var maisonSale = _db.House.Find(DIRTY_HOUSE_ID)!;

            // Les maisons sont inversées
            Exception e = Assert.ThrowsException<DontStealMyCatException>(() => catsService.Move(CAT_IN_DIRTY_HOUSE_ID, maisonPropre, maisonSale));
            Assert.AreEqual("Touche pas à mon chat!", e.Message);
        }
    }
}