using _StoryGame.Core.Messaging.Interfaces;

namespace _StoryGame.Core.WalletNew.Messages
{
    public record ItemAmountChangedMsg(string ItemId, long Amount) : IWalletMsg;
}
