using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProAgil.Domain;

namespace ProAgil.Repository
{
    public class ProAgilRepository : IProAgilRespository
    {
        private readonly ProAgilContext _context;

        public ProAgilRepository(ProAgilContext context)
        {
           _context = context;
           _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        //GERAIS
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }
        public void Update<T>(T entity) where T : class
        {
            _context.Update(entity);
        }
        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }
        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }

        //EVENTOS
        public async Task<Evento[]> GetAllEventoAsync(bool includePalestrantes = false)
        {
            IQueryable<Evento> query = _context.Eventos
                .Include(e => e.Lotes)
                .Include(e => e.RedesSociais);
            
            if(includePalestrantes)
            {
                query = query
                    .Include(pe => pe.PalestrantesEventos)
                    .ThenInclude(p => p.Palestrante);
            }

            query = query.OrderByDescending(e => e.DataEvento);

            return await query.ToArrayAsync();
        }
        public async Task<Evento[]> GetAllEventoAsyncByTema(string temaEvento, bool includePalestrantes = false)
        {
            IQueryable<Evento> query = _context.Eventos
                .Include(e => e.Lotes)
                .Include(e => e.RedesSociais);
            
            if(includePalestrantes)
            {
                query = query
                    .Include(pe => pe.PalestrantesEventos)
                    .ThenInclude(p => p.Palestrante);
            }

            query = query.OrderByDescending(e => e.DataEvento)
                        .Where(e => e.Tema.ToLower().Contains(temaEvento.ToLower()));

            return await query.ToArrayAsync();
        }
        public async Task<Evento> GetEventoAsyncById(int EventoId, bool includePalestrantes = false)
        {
            IQueryable<Evento> query = _context.Eventos
                .Include(e => e.Lotes)
                .Include(e => e.RedesSociais);
            
            if(includePalestrantes)
            {
                query = query
                    .Include(pe => pe.PalestrantesEventos)
                    .ThenInclude(p => p.Palestrante);
            }

            query = query.OrderByDescending(e =>e.DataEvento)
                        .Where(e => e.Id == EventoId);

            return await query.FirstOrDefaultAsync();
        }
        
        //PALESTRANTE
        public async Task<Palestrante> GetPalestranteAsyncById(int PalestranteId, bool includeEventos = false)
        {
            IQueryable<Palestrante> query = _context.Palestrantes
                .Include(p => p.RedesSociais);
            
            if(includeEventos)
            {
                query = query
                    .Include(pe => pe.PalestrantesEventos)
                    .ThenInclude(e => e.Evento);
            }

            query = query.OrderBy(p => p.Nome)
                        .Where(p => p.Id == PalestranteId);

            return await query.FirstOrDefaultAsync();
        }
        public async Task<Palestrante[]> GetAllPalestranteAsyncByName(string nomePalestrante, bool includeEventos = false)
        {
            IQueryable<Palestrante> query = _context.Palestrantes
                .Include(p => p.RedesSociais);
            
            if(includeEventos)
            {
                query = query
                    .Include(pe => pe.PalestrantesEventos)
                    .ThenInclude(e => e.Evento);
            }

            query = query.Where(p => p.Nome.ToLower().Contains(nomePalestrante.ToLower()));

            return await query.ToArrayAsync();
        }
        public async Task<Palestrante[]> GetAllPalestranteAsync(bool includeEventos = false)
        {
            IQueryable<Palestrante> query = _context.Palestrantes
                .Include(p => p.RedesSociais);
            
            if(includeEventos)
            {
                query = query
                    .Include(pe => pe.PalestrantesEventos)
                    .ThenInclude(e => e.Evento);
            }

            query = query.OrderBy(e => e.Nome);

            return await query.ToArrayAsync();
        }
            
    }
}