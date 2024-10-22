﻿using Store.Business.Models.BookModels;

namespace Store.Business.Models.Authors
{
    public record AuthorModel
    {
        public int Id { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Biography { get; init; }
        public IEnumerable<BookModel> Books { get; init; } = new List<BookModel>();
    }
}