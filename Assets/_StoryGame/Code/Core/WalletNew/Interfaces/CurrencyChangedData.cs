namespace _StoryGame.Core.WalletNew.Interfaces
{
    public record CurrencyChangedData(string Id, long PreviousAmount, long NewAmount)
    {
        public string Id { get; } = Id;
        public long PreviousAmount { get; } = PreviousAmount;
        public long NewAmount { get; } = NewAmount;
        public long Delta => NewAmount - PreviousAmount;
    }
}
