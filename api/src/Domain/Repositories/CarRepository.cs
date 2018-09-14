using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Foundatio.Skeleton.Domain.Repositories {
    public class CarRepository : EFRepositoryBase<Car>, ICarRepository {

        public CarRepository(IEFRepositoryContext efRepositoryContext)
           : base(efRepositoryContext, null) {
        }

        public async Task<Car> GetByCarNumAsync(string carNum) {

            if (String.IsNullOrEmpty(carNum))
                return null;

            carNum = carNum.ToLower();

            var cars = await this.FindAsync(x => x.CarNum.Equals(carNum));
            return cars.FirstOrDefault();
        }
    }
}
