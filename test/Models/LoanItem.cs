using System.ComponentModel.DataAnnotations;

namespace test.Models
{
    public class LoanItem
    {
        public LoanItem( DateOnly date, double amountOfPayment, double mainDebt, double interestRate, double balanceOwed )
        {
            Date = date;
            AmountOfPayment = amountOfPayment;
            MainDebt = mainDebt;
            InterestRate = interestRate;
            BalanceOwed = balanceOwed;
        }
        
        [Required]
        public DateOnly Date { get; set; }

        [Required]
        public double AmountOfPayment { get; set; }
        
        [Required]
        public double MainDebt { get; set; }
        
        [Required]
        public double InterestRate { get; set; }
        
        [Required]
        public double BalanceOwed { get; set;}
    }
}