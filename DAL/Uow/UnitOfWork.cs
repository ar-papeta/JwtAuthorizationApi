using DAL.Entities;
using DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Uow;

internal class UnitOfWork : IUnitOfWork
{
    private readonly SqlContext _context;
    private IRepository<User> _userRepository;
    public IRepository<User> Users
    {
        get
        {
            if (_userRepository is null)
            {
                _userRepository = new GenericRepository<User>(_context);
            }
            return _userRepository;
        }
    }

    public UnitOfWork(SqlContext context)
    {
        _context = context;
    }

    public void Save()
    {
        _context.SaveChanges();
    }
}

