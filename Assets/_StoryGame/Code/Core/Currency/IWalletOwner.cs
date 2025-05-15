using System;

namespace _StoryGame.Core.Currency
{
    /// <summary>
    /// Интерфейс для объектов, которые могут иметь кошелек
    /// </summary>
    public interface IWalletOwner
    {
        /// <summary>
        /// Уникальный идентификатор владельца
        /// </summary>
        string Id { get; }
        
        /// <summary>
        /// Кошелек владельца
        /// </summary>
        IWallet Wallet { get; }
        
        /// <summary>
        /// Событие изменения кошелька
        /// </summary>
        IObservable<IWallet> OnWalletChanged { get; }
    }
} 