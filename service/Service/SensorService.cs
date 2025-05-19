using data.Context;
using library.Model;
using Microsoft.EntityFrameworkCore;

namespace service.Service
{
    public class SensorService
    {
        private readonly AppDbContext _context;

        public SensorService(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Sensor> GetAll()
        {
            return _context.Sensor.ToList();
        }

        public Sensor? GetById(Guid id)
        {
            return _context.Sensor.Find(id);
        }

        public Sensor Create(Sensor sensor)
        {
            if (sensor == null)
                throw new ArgumentNullException(nameof(sensor));

            sensor.id = Guid.NewGuid();
            sensor.dtInstalacao = DateTime.UtcNow;
            sensor.dtUltimaLeitura = DateTime.UtcNow;

            _context.Sensor.Add(sensor);
            _context.SaveChanges();
            return sensor;
        }

        public bool Update(Guid id, Sensor updatedSensor)
        {
            var existing = GetById(id);
            if (existing == null)
                return false;

            updatedSensor.id = id;
            updatedSensor.dtUltimaLeitura = DateTime.UtcNow;

            _context.Entry(existing).State = EntityState.Detached;
            _context.Sensor.Update(updatedSensor);
            _context.SaveChanges();
            return true;
        }

        public bool Delete(Guid id)
        {
            var sensor = GetById(id);
            if (sensor == null)
                return false;

            _context.Sensor.Remove(sensor);
            _context.SaveChanges();
            return true;
        }
    }
}
