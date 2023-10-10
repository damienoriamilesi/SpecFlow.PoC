using BoDi;
using Xunit;

namespace SpecFlow.PoC.BDD.Tests.Steps;

[Binding]
public class InvoicingStepDefinition
{
    private readonly IObjectContainer _objectContainer;
    public InvoicingStepDefinition(IObjectContainer objectContainer)
    {
        _objectContainer = objectContainer;
        //_invoiceService = invoiceService;
    }

    [Before()]
    public void Init()
    {
        _objectContainer.RegisterTypeAs<InvoiceService,IInvoiceService>();
    } 
    
    private readonly IInvoiceService _invoiceService = new InvoiceService();
    private bool _isCurrentInvoiceWithHealthInsurance;

    private Invoice _invoice = new();

    [Given(@"an invoice in an open status and is not Health insurance")]
    public void GivenAnInvoiceInAnOpenStatusAndIsNotHealthInsurance()
    {
        _invoiceService.Invoice = _invoiceService.GetInvoice(Guid.NewGuid());
        //_invoice = _objectContainer.Resolve<IInvoiceService>().GetInvoice(Guid.NewGuid());
    }

    [When(@"I want to check the insured card data")]
    public void WhenInsuranceIsNotLaMal()
    {
        _isCurrentInvoiceWithHealthInsurance = _invoice.IsLamal;
        //_invoiceService.Invoice.IsLamal = false;
    }

    [Then(@"a business exception is raised with the message ""(.*)""")]
    public void ThenABusinessExceptionIsRaisedWithTheMessage(string p0)
    {
        if (!_isCurrentInvoiceWithHealthInsurance) 
            Assert.Throws<InvoiceIsNotHealthInsuranceException>(() =>_invoiceService.SaveChanges());
    }

    [Then(@"I get the expected data")]
    public void ThenIGetTheExpectedData(Table table)
    {
        
    }

    [Given(@"an invoice in an open status and has Health insurance")]
    public void GivenAnInvoiceInAnOpenStatusAndHasHealthInsurance()
    {
        //_invoice = new Invoice { Status = Status.Open, IsLamal = true };
        //ScenarioContext.StepIsPending();
    }

    [When(@"I check the insured card data by CaDa number")]
    public void WhenICheckTheInsuredCardDataByCaDaNumber()
    {
        //var insuranceFromCard = _invoiceService.GetInsurance(_invoice.Insurance.Gln);
        
    }

    [When(@"the system suggests a disabled insurance")]
    public void WhenTheSystemSuggestsADisabledInsurance()
    {
        
    }

    [Then(@"I get an active insurance by the disabled GLN")]
    public void ThenIGetAnActiveInsuranceByTheDisabledGln()
    {
        
    }
}

public class InvoiceIsNotHealthInsuranceException : Exception
{
}

public interface IInvoiceService
{
    Invoice GetInvoice(Guid id);
    Insurance GetInsurance(string? gln);
    Invoice? Invoice { get; set; }
    void SaveChanges();
}

public class Insurance
{
    public string? Gln { get; set; }
}

public class InvoiceService : IInvoiceService
{
    public Guid InvoiceId { get; set; }
    public Invoice GetInvoice(Guid id) => new(){ IsLamal = false, Insurance = new Insurance { Gln = "7612030345082"}};
    public Insurance GetInsurance(string? gln)
    {
        return new Insurance();
    }
    public Invoice? Invoice { get; set; }
    public void SaveChanges()
    {
        if(!Invoice!.IsLamal) 
            throw new InvoiceIsNotHealthInsuranceException();
    }
}

public class Invoice : BaseEntity, IAuditable
{
    internal bool IsLamal { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime UpdatedOn { get; set; }
    public Guid ModifiedBy { get; set; }

    public Status Status { get; set; }

    public Insurance Insurance { get; set; }
    
    //public string? InsuranceGln { get; set; }
}

public enum Status
{
    Open,
    Paid
}

internal interface IAuditable
{
    DateTime CreatedOn { get; set; }
    DateTime UpdatedOn { get; set; }
    Guid CreatedBy { get; set; }
    Guid ModifiedBy { get; set; }
}

public class BaseEntity
{
    public Guid Id { get; set; }
}