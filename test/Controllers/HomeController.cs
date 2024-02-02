using System.Diagnostics;
using System.Globalization;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using test.Models;

namespace test.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController( ILogger<HomeController> logger )
        {
            _logger = logger;
            CultureInfo culture = new CultureInfo( "en-US" );
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }

        public IActionResult FirstPage()
        {
            CultureInfo.CurrentCulture = new CultureInfo( "en-Us" );
            CultureInfo.CurrentUICulture = new CultureInfo( "en-Us" );

            return View( "FirstPage" );
        }

        public IActionResult SecondPage()
        {
            return View( "SecondPage" );
        }

        [ResponseCache( Duration = 0, Location = ResponseCacheLocation.None, NoStore = true )]
        public IActionResult Error()
        {
            return View( new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier } );
        }

        [HttpPost]
        public IActionResult SecondPage( double loanAmount, int loanTerm, double interestRate, string? alternativeCalculation, int? paymentStep )
        {
            if (ModelState.IsValid == false)
                return View( "FirstPage", 422 );

            bool isAlternativeCalculatuion = alternativeCalculation == "on";
            interestRate = interestRate / 100;
            List<LoanItem> loanItem = new List<LoanItem>();
            var loanBalance = loanAmount;
            DateOnly firstPayDate = DateOnly.FromDateTime( DateTime.Today ).AddMonths( 1 );
            var monthPrecent = interestRate / 12;
            double annuityPayment;

            if (isAlternativeCalculatuion)
            {
                if (paymentStep == 0 || paymentStep == null)
                    return View( "FirstPage", 422 );

                annuityPayment = CalculateAlternativeAnnuitetPayment( loanAmount, loanTerm, interestRate, (int)paymentStep );
                loanItem = GetAlternativePayoutList( loanTerm, loanItem, loanBalance, firstPayDate, interestRate, annuityPayment, (int)paymentStep );
            }
            else
            {
                annuityPayment = CalculateClassicAnnuitetPayment( loanAmount, loanTerm, monthPrecent );
                loanItem = GetClassicPayoutList( loanTerm, loanItem, loanBalance, firstPayDate, monthPrecent, annuityPayment );
            }

            return View( "SecondPage", loanItem );
        }

        List<LoanItem> GetAlternativePayoutList( int loanTerm, List<LoanItem> loanItem, double loanBalance, DateOnly firstPayDate, double interestRate, double annuityMonthPayment, int paymentStep )
        {
            // Я не смог адаптировать формулу из тз под расчёты дневных займов
            var periodProcents = (paymentStep * interestRate * 100);
            double percentPart, bodyPart;

            var countOfPayments = loanTerm / paymentStep;
            for (int i = 0; i < countOfPayments; i++)
            {
                if (loanBalance < annuityMonthPayment)
                    annuityMonthPayment = loanBalance;

                percentPart = loanBalance * periodProcents;
                bodyPart = annuityMonthPayment - percentPart;

                loanBalance -= annuityMonthPayment;

                loanItem.Add( new LoanItem( firstPayDate.AddDays( paymentStep ), Math.Round( annuityMonthPayment, 2 ), Math.Round( bodyPart, 2 ), Math.Round( percentPart, 2 ), Math.Round( loanBalance, 2 ) ) );
            }

            return loanItem;
        }

        List<LoanItem> GetClassicPayoutList( int loanTerm, List<LoanItem> loanItem, double loanBalance, DateOnly firstPayDate, double monthPrecent, double annuityMonthPayment )
        {
            //Возможно вы заметили, что остаток платежа не такой, как в кредитном калькуляторе, но я не смог вывести формулу начисления процентов на остаток
            double percentPart, bodyPart;

            for (int i = 0; i < loanTerm; i++)
            {
                if (loanBalance < annuityMonthPayment)
                    annuityMonthPayment = loanBalance;

                percentPart = loanBalance * monthPrecent;
                bodyPart = annuityMonthPayment - percentPart;

                loanBalance -= annuityMonthPayment;

                loanItem.Add( new LoanItem( firstPayDate.AddMonths( i ), Math.Round( annuityMonthPayment, 2 ), Math.Round( bodyPart, 2 ), Math.Round( percentPart, 2 ), Math.Round( loanBalance, 2 ) ) );
            }

            return loanItem;
        }

        double CalculateClassicAnnuitetPayment( double loanBalance, int loanTerm, double monthPrecent )
        {
            var exponentiation = Math.Pow( ( 1 + monthPrecent ), loanTerm );
            var annuityKoef = ( monthPrecent * exponentiation ) / ( exponentiation - 1 );
            var annuityMonthPayment = loanBalance * annuityKoef;

            return annuityMonthPayment;
        }

        double CalculateAlternativeAnnuitetPayment( double loanAmount, int loanTerm, double interestRate, int paymentStep )
        {
            var countOfPayments = loanTerm / paymentStep;
            var monthPercent = interestRate * DateTime.DaysInMonth( DateTime.Now.Year, DateTime.Now.Month );
            var exponentiation = Math.Pow( (1 + monthPercent ), countOfPayments );
            var annuityKoef = ( monthPercent * exponentiation ) / ( exponentiation - 1 );
            var annuityMonthPayment = annuityKoef * loanAmount;

            return annuityMonthPayment;
        }
    }
}
