using Autofac;
using NUnit.Framework;
using StaffingPurchase.Core.Data;
using StaffingPurchase.Core.Domain;
using StaffingPurchase.Data;
using StaffingPurchase.Services.LevelGroups;

namespace StaffingPurchase.Tests.Services
{
    public class LevelGroupServiceTests : TestSuiteBase
    {
        [Test]
        [Category(TestCategory.IntegrationTest)]
        public void InsertLevelGroup_Ideal_ShouldInsertSuccessfully()
        {
            // arrange
            var levelGroupService = ContainerManager.ResolveUnregistered<LevelGroupService>();
            var group = new LevelGroup()
            {
                Name = "Group test",
                PV = 100
            };

            // act
            levelGroupService.InsertLevelGroup(group);

            // assert
            Assert.Greater(group.Id, 0); // id will be generated when inserting successfully

            var savedGroup = levelGroupService.GetLevelGroupById(group.Id);
            Assert.IsNotNull(savedGroup);
            Assert.AreEqual("Group test", savedGroup.Name);
            Assert.AreEqual(100, savedGroup.PV);

            // clean up
            CleanUp(levelGroupService, group);
        }

        [SetUp]
        public void SetUp()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<SqlServerDataProvider>().As<IDataProvider>().SingleInstance();
            builder.RegisterType<StaffingPurchaseDataContext>().As<IDbContext>().InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();
            builder.RegisterType<LevelGroupService>().As<ILevelGroupService>().InstancePerLifetimeScope();
            builder.RegisterType<LevelService>().As<ILevelService>().InstancePerLifetimeScope();
            builder.Update(ContainerManager.Container);
        }

        [TearDown]
        public void TearDown()
        {
            // TODO
        }

        [Test]
        [Category(TestCategory.IntegrationTest)]
        public void UpdateLevelGroup_Ideal_ShouldInsertSuccessfully()
        {
            // #1 Insert
            // arrange
            var levelGroupService = ContainerManager.ResolveUnregistered<LevelGroupService>();
            var group = new LevelGroup()
            {
                Name = "Group test",
                PV = 100
            };

            // act
            levelGroupService.InsertLevelGroup(group);

            // assert
            Assert.Greater(group.Id, 0); // id will be generated when inserting successfully

            // #2 Update
            // arrange
            var savedGroup = levelGroupService.GetLevelGroupById(group.Id);
            savedGroup.Name = "Updated group";
            savedGroup.PV = 120;

            // act
            levelGroupService.UpdateLevelGroup(savedGroup);
            var updatedGroup = levelGroupService.GetLevelGroupById(group.Id);

            // assert
            Assert.AreEqual("Updated group", updatedGroup.Name);
            Assert.AreEqual(120, updatedGroup.PV);

            // clean up
            CleanUp(levelGroupService, group);
        }

        private void CleanUp(ILevelGroupService levelGroupService, LevelGroup group)
        {
            levelGroupService.DeleteLevelGroup(group.Id);
        }
    }
}