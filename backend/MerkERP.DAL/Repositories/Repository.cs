using Microsoft.EntityFrameworkCore;
using MerkERP.Core.Interfaces;
using MerkERP.DAL.Context;

namespace MerkERP.DAL.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
	protected readonly MerkDbContext _db;
	protected readonly DbSet<T> _set;

	public Repository(MerkDbContext db) { _db = db; _set = db.Set<T>(); }

	public async Task<IEnumerable<T>> GetAllAsync() => await _set.ToListAsync();

	public async Task<T?> GetByIdAsync(long id) => await _set.FindAsync(id);

	public async Task<T> CreateAsync(T entity)
	{
		_set.Add(entity);
		await _db.SaveChangesAsync();
		return entity;
	}

	public async Task<T> UpdateAsync(T entity)
	{
		_set.Update(entity);
		await _db.SaveChangesAsync();
		return entity;
	}

	public async Task<bool> DeleteAsync(long id)
	{
		var entity = await _set.FindAsync(id);
		if (entity is null) return false;
		_set.Remove(entity);
		await _db.SaveChangesAsync();
		return true;
	}
}
