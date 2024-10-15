namespace Store.ViewModels.ViewModels.OrderViewModels
{
    public record OrderViewModel
    {
        public int Id { get; init; }
        public decimal Sum { get; init; }
        public DateOnly Date { get; init; }
    }
}
