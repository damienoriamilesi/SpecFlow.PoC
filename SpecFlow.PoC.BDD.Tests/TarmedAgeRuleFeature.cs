using Xunit;

namespace SpecFlow.PoC.BDD.Tests;

[Binding]
public class TarmedAgeRuleFeature
{
    private readonly BillingControlRequest _billingRequest = new ();
    private BillingControlResponse _billingControlResponse = new ();

    [Given(@"a service with position with age constraint")]
    public void GivenAServiceWithPositionWithAgeConstraint()
    {
        var service = new TarmedService();
        service.PositionCode = "25.1234";

        _billingRequest.TarmedServices = new[] { service };
    }

    [Given(@"a patient who is outside the rule age")]
    public void GivenAPatientWhoIsOutsideTheRuleAge()
    {
        var patient = new Patient();
        patient.BirthDate = new DateTime(2014, 4, 10);
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

public class BillingControlResponse
{
    public string[] Errors { get; set; }
    public string[] Warnings { get; set; }
}

public class BillingValorization
{
    public BillingControlResponse Control(BillingControlRequest billingRequest)
    {
        var response = new BillingControlResponse();
        var errors = new List<string>();
        var age = DateTime.Now.Ticks - billingRequest.Patient!.BirthDate.Ticks;
        if (age > 6)
        {
            errors.Add("TAR_AGE_MIN - Age minimum requis pour cette position");
        }

        response.Errors = errors.ToArray();

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
