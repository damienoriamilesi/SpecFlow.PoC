using Xunit;

namespace SpecFlow.PoC.BDD.Tests;

[Binding]
public class TarmedAgeRuleFeature
{
    private readonly BillingControlRequest _billingRequest = new ();
    private BillingControlResponse _billingControlResponse = new(null,null);

    [Given(@"a service with age position constraint like '(.*)'")]
    public void GivenAServiceWithPositionWithAgeConstraint(string positionCode)
    {
        var service = new TarmedService { PositionCode = positionCode };
        _billingRequest.TarmedServices = new[] { service };
    }

    [Given(@"a patient whose age is outside the rule age of the position constraint : '(.*)'")]
    public void GivenAPatientWhoIsOutsideTheRuleAge(int limitAge)
    {
        var patient = new Patient();
        patient.BirthDate = DateTime.Now.AddYears(-6).AddDays(-1);
        _billingRequest.Patient = patient;
    }

    [When(@"I check the validity of the service")]
    public void WhenICheckTheValidityOfTheService()
    {
        var billingValorization = new BillingValorization();
        _billingControlResponse = billingValorization.Control(_billingRequest);
    }

    [Then(@"I receive an Error message")]
    public void ThenIReceiveAnError()
    {
        Assert.Single(_billingControlResponse.Errors);
    }
}

public record BillingControlResponse(string[] Errors, string[] Warnings);

public class BillingValorization
{
    public BillingControlResponse Control(BillingControlRequest billingRequest)
    {
        var errors = new List<string>();
        var age = DateTime.Now.Ticks - billingRequest.Patient!.BirthDate.Ticks;
        if (age > 6) errors.Add($"TAR_AGE_MIN - Age minimum requis pour cette position : { age }");
        var response = new BillingControlResponse(errors.ToArray(), null!);
        return response;
    }
}

public class BillingControlRequest
{
    public Patient? Patient { get; set; }
    public TarmedService[]? TarmedServices { get; set; }
}

public class Patient
{
    public DateTime BirthDate { get; set; }
}

public class TarmedService
{
    public string? PositionCode { get; set; }
}
