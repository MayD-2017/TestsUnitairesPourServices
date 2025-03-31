using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestsUnitairesPourServices.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestsUnitairesPourServices.Data;
using TestsUnitairesPourServices.Models;

namespace TestsUnitairesPourServices.Services.Tests
{

    [TestClass()]
    public class CatsServiceTests
    {
        const int CHAT_1 = 1;
        const int CHAT_2 = 2;
        const int MAISON_1 = 1;
        const int MAISON_2 = 2;
        // TODO Mettre seulement les optionms ici et non la BD en entier
        // La BD doit être crée et détruite pour chacun des tests, sinon il y aura des problèmes avec le tracking des éléments
        private DbContextOptions<ApplicationDBContext> _options;
        public CatsServiceTests()
        {
            // TODO On initialise les options de la BD, on utilise une InMemoryDatabase
            _options = new DbContextOptionsBuilder<ApplicationDBContext>()
                // TODO il faut installer la dépendance Microsoft.EntityFrameworkCore.InMemory
                .UseInMemoryDatabase(databaseName: "CatsService")
                .UseLazyLoadingProxies(true) // Active le lazy loading
                .Options;
        }

        [TestInitialize]
    public void Init()
    {
        // TODO avoir la durée de vie d'un context la plus petite possible
        using ApplicationDBContext db = new ApplicationDBContext(_options);
            // TODO on ajoute des données de tests
            House[] Maisons = new House[] {
                new House
                {
                    Id = 1,
                    Address = "5555 rue quoi",
                    OwnerName = "Kayla"
                },
                new House()
                {
                    Id = 2,
                    Address = "2341 rue quoi",
                    OwnerName = "Jay"
                }
            };

            Cat[] Chats = new Cat[] {
                new Cat
                {
                    Id = 1,
                    Name = "Todd",
                    Age = 12,
                    House = Maisons[0]
                },
                new Cat()
                {
                    Id = 2,
                    Name = "Billie",
                    Age = 4
                }
            };
            db.AddRange(Maisons);
            db.AddRange(Chats);
            db.SaveChanges();
    }
    [TestCleanup]
    public void Dispose()
    {
        //TODO on efface les données de tests pour remettre la BD dans son état initial
        using ApplicationDBContext db = new ApplicationDBContext(_options);
        
    }
    [TestMethod()]
        public void MoveTest()
        {
            Assert.Fail();
        }
    }
}