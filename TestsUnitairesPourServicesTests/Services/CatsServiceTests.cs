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
using Microsoft.Extensions.Options;
using TestsUnitairesPourServices.Exceptions;
using System.Security.Cryptography;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;

namespace TestsUnitairesPourServices.Services.Tests
{

    [TestClass]
    public class CatsServiceTests
    {
        const int CHAT_1 = 1;
        const int CHAT_2 = 2;
        const int MAISON_1 = 1;
        const int MAISON_2 = 2;
        
        private ApplicationDBContext _db;

        [TestInitialize]
        public void Init()
        {
            // En utilisant un nom différent à chaque fois, on n'a pas besoin de retirer les données
            string dbName = "CardsService" + Guid.NewGuid().ToString();
            // TODO On initialise les options de la BD, on utilise une InMemoryDatabase
            DbContextOptions<ApplicationDBContext> options = new DbContextOptionsBuilder<ApplicationDBContext>()
                // TODO il faut installer la dépendance Microsoft.EntityFrameworkCore.InMemory
                .UseInMemoryDatabase(databaseName: dbName)
                .UseLazyLoadingProxies(true) // Active le lazy loading
                .Options;

            _db = new ApplicationDBContext(options);
            // TODO on ajoute des données de tests
            House[] Maisons = new House[] {
                new House
                {
                    Id = MAISON_1,
                    Address = "5555 rue quoi",
                    OwnerName = "Kayla"
                },
                new House()
                {
                    Id = MAISON_2,
                    Address = "2341 rue quoi",
                    OwnerName = "Jay"
                }
            };

            Cat[] Chats = new Cat[] {
                new Cat
                {
                    Id = CHAT_1,
                    Name = "Todd",
                    Age = 12,
                    House = Maisons[0]
                },
                new Cat()
                {
                    Id = CHAT_2,
                    Name = "Billie",
                    Age = 4
                }
            };
            _db.AddRange(Maisons);
            _db.AddRange(Chats);
            _db.SaveChanges();
        }

        [TestCleanup]
        public void Dispose()
        {
            _db.Dispose();

        }
        [TestMethod]
        public void MoveSuccesTest()
        {
            //Arrange
            CatsService _catservice = new CatsService(_db);
            House? maison1 = _db.House.Find(MAISON_1);
            House? maison2 = _db.House.Find(MAISON_2);
            //Act
            var cat = _catservice.Move(CHAT_1, maison1, maison2);
            //Assert
            Assert.AreEqual(cat.House, maison2);
        }

        [TestMethod]
        public void Move_Succes_RetournePasNullTest()
        {
            //Arrange
            var _catservice = new CatsService(_db);
            House? maison1 = _db.House.Find(MAISON_1);
            House? maison2 = _db.House.Find(MAISON_2);
            //Act
            var cat = _catservice.Move(CHAT_1, maison1, maison2);
            //Assert
            Assert.AreNotEqual(cat, null);
        }

        [TestMethod]
        public void Move_ChatIdInconnue_RetourneNullTest()
        {
            //Arrange
            var _catservice = new CatsService(_db);
            House? maison1 = _db.House.Find(MAISON_1);
            House? maison2 = _db.House.Find(MAISON_2);
            //Act
            int id = 3;
            if (_db.Cat.Any(c => c.Id == id)) Assert.Fail(); //Si id existe, le test devient inutile. Il ne retournera pas null.
            var cat = _catservice.Move(id, maison1, maison2);
            //Assert
            Assert.AreEqual(cat.House, null);
        }

        [TestMethod]
        public void Move_ChatAucuneMaison_LanceWildCatExceptionTest()
        {
            //Arrange
            var _catservice = new CatsService(_db);
            House? maison1 = _db.House.Find(MAISON_1);
            House? maison2 = _db.House.Find(MAISON_2);
            //Act
            var cat = _catservice.Move(CHAT_2, maison2, maison1);
            //Assert
            var exception = Assert.ThrowsException<WildCatException>(()=> cat);
            Assert.AreEqual("On n'apprivoise pas les chats sauvages", exception.Message);
        }

        [TestMethod]
        public void Move_ChatNeCommencePasAPartirDeSaMaison_LanceDontStealMyCatExceptionTest()
        {
            //Arrange
            var _catservice = new CatsService(_db);
            House? maison1 = _db.House.Find(MAISON_1);
            House? maison2 = _db.House.Find(MAISON_2);
            //Act
            var cat = _catservice.Move(CHAT_2, maison2, maison1);
            //Assert
            var exception = Assert.ThrowsException<DontStealMyCatException>(() => cat);
            Assert.AreEqual("Touche pas à mon chat!", exception.Message);
        }
    }
    

}  
