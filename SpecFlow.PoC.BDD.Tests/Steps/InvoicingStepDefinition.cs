using BoDi;
using TechTalk.SpecFlow.Assist;
using Xunit;

namespace SpecFlow.PoC.BDD.Tests.Steps;

[Binding]
public class InvoicingStepDefinition
{
    //private readonly IObjectContainer _objectContainer;
    private readonly ScenarioContext _scenarioContext;

    public InvoicingStepDefinition(IObjectContainer objectContainer, ScenarioContext scenarioContext)
    {
        //_objectContainer = objectContainer;
        _scenarioContext = scenarioContext;
        //_invoiceService = new InvoiceService();
    }
    
    [Before()]
    public void Init()
    {
        //_objectContainer.RegisterTypeAs<InvoiceService,IInvoiceService>();
    } 
    
    private readonly IInvoiceService _invoiceService = new InvoiceService();
    private bool _isCurrentInvoiceWithHealthInsurance;

    private Invoice _invoice = new();
    private CardRepository _cardService = new();

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
        var expectedResultSet = table.CreateSet<CardInformation>();
        
        var cardInfos = (CardInformation[])_scenarioContext["CardInformationsFromInvoice"];
        Assert.Equal(table.RowCount, cardInfos.Length);
        
        //string id = table.Rows.First()["Id"];
        //Assert.Contains(cardInfos, x => x.Id == int.Parse(id));

        Assert.Equal(expectedResultSet.ToArray(), cardInfos);
    }

    [Given(@"an invoice in an open status and has Health insurance")]
    public void GivenAnInvoiceInAnOpenStatusAndHasHealthInsurance()
    {
        _invoice = _invoiceService.GetInvoice(Guid.NewGuid()); 
        //new Invoice { Status = Status.Open, Insurance = new Insurance {Gln = 7600E239842}, IsLamal = true };
        //ScenarioContext.StepIsPending();
    }

    [When(@"I check the insured card data by CaDa number")]
    public void WhenICheckTheInsuredCardDataByCaDaNumber()
    {
        //var insuranceFromCard = _invoiceService.GetInsurance(_invoice.Insurance.Gln);
        var cardInformations = _cardService.GetCardInformations(_invoice.CardNumber.GetValueOrDefault());
        
        _scenarioContext["CardInformationsFromInvoice"] = cardInformations;
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

internal class CardRepository
{
    internal CardInformation[] GetCardInformations(int cardNumber)
    {
        if (cardNumber == default) return Array.Empty<CardInformation>(); 
        return new[]
        {
            new CardInformation
            {
                Id = 42, IsLamal = true, CreatedOn = new DateTime(2023, 1,9)
                ,CreatedBy = "Me", UpdatedBy = "Me", UpdatedOn = new DateTime(2023, 7,8)
            }
            , new CardInformation  {
                Id = 66, IsLamal = false, CreatedOn = new DateTime(2023, 1,9)
                ,CreatedBy = "You", UpdatedBy = "You", UpdatedOn = new DateTime(2023, 7,8)
            }
        };
    }
}

internal record CardInformation : IAuditable
{
    public int Id { get; set; }
    public bool IsLamal { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
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
    public Invoice GetInvoice(Guid id) => new(){ CardNumber = 42, IsLamal = false, Insurance = new Insurance { Gln = "7612030345082"}};
    
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
    public string CreatedBy { get; set; }
    public DateTime UpdatedOn { get; set; }
    public string UpdatedBy { get; set; }

    public Status Status { get; set; }

    public int? CardNumber { get; set; }
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
    string CreatedBy { get; set; }
    string UpdatedBy { get; set; }
}

public class BaseEntity
{
    public Guid Id { get; set; }
}