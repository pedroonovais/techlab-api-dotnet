using data.Context;
using library.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace service.Service
{
    public class LeituraRfidService
    {
        private readonly AppDbContext _context;

        public LeituraRfidService(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<LeituraRfid> GetAll()
        {
            return _context.LeituraRfid.ToList();
        }

        public LeituraRfid? GetById(Guid id)
        {
            return _context.LeituraRfid.FirstOrDefault(l => l.Id == id);
        }

        public LeituraRfid Create(LeituraRfid leitura)
        {
            if (leitura == null)
                throw new ArgumentNullException(nameof(leitura));

            leitura.Id = Guid.NewGuid();
            leitura.Timestamp = DateTime.UtcNow;

            _context.LeituraRfid.Add(leitura);
            _context.SaveChanges();
            return leitura;
        }

        public bool Delete(Guid id)
        {
            var leitura = GetById(id);
            if (leitura == null)
                return false;

            _context.LeituraRfid.Remove(leitura);
            _context.SaveChanges();
            return true;
        }

        public LeituraRfid? Update(Guid id, LeituraRfid leituraAtualizada)
        {
            var leituraExistente = _context.LeituraRfid.FirstOrDefault(l => l.Id == id);

            if (leituraExistente == null)
                return null;

            leituraExistente.RfidId = leituraAtualizada.RfidId;
            leituraExistente.SensorId = leituraAtualizada.SensorId;
            leituraExistente.Timestamp = leituraAtualizada.Timestamp;

            _context.SaveChanges();
            return leituraExistente;
        }
    }
}
