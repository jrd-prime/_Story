namespace _StoryGame.Core.WalletNew.Interfaces
{
    /// <summary>
    /// Интерфейс для объектов, которые могут иметь кошелек
    /// </summary>
    public interface IWalletOwner
    {
        /// <summary>
        /// Кошелек владельца
        /// </summary>
        IWallet Wallet { get; }
    }
}
