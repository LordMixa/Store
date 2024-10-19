﻿using Store.Entities.Entities;

namespace Store.Data.Dapper.Repositories.Interfaces
{
    public interface IBookRepository
    {
        Task<Book?> GetAsync(int id);
    }
}