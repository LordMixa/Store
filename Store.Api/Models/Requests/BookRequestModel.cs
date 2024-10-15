﻿namespace Store.Api.Models.Requests
{
    public record BookRequestModel
    {
        public string Title { get; init; }
        public string Description { get; init; }
        public decimal Price { get; init; }
        public DateOnly DateOfPublication { get; init; }
    }
}