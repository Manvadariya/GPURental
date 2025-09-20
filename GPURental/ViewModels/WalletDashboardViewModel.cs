using System.Collections.Generic;
using GPURental.Models;

namespace GPURental.ViewModels
{
    public class WalletDashboardViewModel
    {
        public int CurrentBalanceInCents { get; set; }
        public IEnumerable<WalletLedgerEntry> TransactionHistory { get; set; }
    }
}