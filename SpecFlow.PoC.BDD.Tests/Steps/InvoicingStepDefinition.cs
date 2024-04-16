using BoDi;
using TechTalk.SpecFlow.Assist;
using Xunit;

namespace SpecFlow.PoC.BDD.Tests.Steps;

[Binding]
public class InvoicingStepDefinition
{
    private readonly IObjectContainer _objectContainer;
    private readonly ScenarioContext _scenarioContext;

    public InvoicingStepDefinition(IObjectContainer objectContainer, ScenarioContext scenarioContext)
    {
        _objectContainer = objectContainer;
        _scenarioContext = scenarioContext;
        _objectContainer.RegisterTypeAs<CardRepository,ICardRepository>();
        _invoiceService = new InvoiceService(new CardRepository());
    }
    
    private readonly IInvoiceService _invoiceService;
    private bool _isCurrentInvoiceWithHealthInsurance;

    [Given(@"an invoice in an open status and is not Health insurance")]
    public void GivenAnInvoiceInAnOpenStatusAndIsNotHealthInsurance()
    {
        _scenarioContext["currentInvoice"] = new Invoice { Insurance = new Insurance(),IsLamal = false};
        _invoiceService.Invoice = (Invoice)_scenarioContext["currentInvoice"];
    }

    [When(@"I want to check the insured card data")]
    public void WhenInsuranceIsNotLaMal()
    {
        _isCurrentInvoiceWithHealthInsurance = (_scenarioContext["currentInvoice"] as Invoice)!.IsLamal;
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
        _scenarioContext["currentInvoiceHealth"] = 
            new Invoice { Status = Status.Open, CardNumber = 42, Insurance = new Insurance {Gln = "7600E239842"}, IsLamal = true };
        //ScenarioContext.StepIsPending();
    }

    [When(@"I check the insured card data by CaDa number")]
    public void WhenICheckTheInsuredCardDataByCaDaNumber()
    {
        var cardRepo = _objectContainer.Resolve<ICardRepository>();
        //var insuranceFromCard = _invoiceService.GetInsurance(_invoice.Insurance.Gln);
        var cardInformations = 
            cardRepo.GetCardInformations((_scenarioContext["currentInvoiceHealth"] as Invoice)!.CardNumber.GetValueOrDefault());
        
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

public class CardRepository : ICardRepository
{
    public CardInformation[] GetCardInformations(int cardNumber)
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

public interface ICardRepository
{
    CardInformation[] GetCardInformations(int cardNumber);
}

public record CardInformation : IAuditable
{
    public int Id { get; set; }
    public bool IsLamal { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
    public string CreatedBy { get; set; } = "Me";
    public string UpdatedBy { get; set; } = "Me";
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
    private readonly ICardRepository _cardRepository;

    public InvoiceService(ICardRepository cardRepository)
    {
        _cardRepository = cardRepository;
    }
    
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
    public string CreatedBy { get; set; } = "Me";
    public DateTime UpdatedOn { get; set; }
    public string UpdatedBy { get; set; } = "Me";

    public Status Status { get; set; }

    public int? CardNumber { get; set; }
    public Insurance? Insurance { get; set; }
    
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