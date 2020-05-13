using System;
using Xunit;
using Moq;

using StoreApp.Data;
using StoreApp.Repository;

namespace TestStoreApp
{
    public class TestLocationRepository
    {
        [Fact]
        public void GetsMostStockedLocation()
        {
            var options = TestUtil.GetMemDbOptions("GetsMostStockedLocation");

            using (var db = new StoreContext(options))
            {
                var repo = (ILocation) new LocationRepository(db);
                var mostStockedLocation = repo.GetMostStocked();
                Assert.Equal(Guid.Parse("BBD4B6EB-CF72-4313-9C92-BD1BE7CAF949"), mostStockedLocation.LocationId);
            }
        }
    }
}
