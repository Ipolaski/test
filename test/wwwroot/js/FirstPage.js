function ChangeMode() {
	let isAlternative = document.getElementById("AlternativeCalculation").checked
	if (isAlternative) {
		document.getElementById("PaymentStep").required = true;
		document.getElementById("LoanTermLabel").innerHTML = 'Срок займа (в днях)'
		document.getElementById("InterestRateLabel").innerHTML = 'Ставка (в день)'
		document.getElementById("LoanTermSpan").innerHTML = 'дн.'
		document.getElementById("PaymentStepDiv").hidden = false;
	}
	else {
		document.getElementById("PaymentStep").required = false;
		document.getElementById("PaymentStepDiv").hidden = true;
		document.getElementById("LoanTermLabel").innerText = 'Срок займа (в месяцах)';
		document.getElementById("InterestRateLabel").innerHTML = 'Ставка (в год)'
		document.getElementById("LoanTermSpan").innerHTML = 'мес.'
	}
}