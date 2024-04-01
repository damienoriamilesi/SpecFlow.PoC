using BoDi;
using Xunit;

namespace SpecFlow.PoC.BDD.Tests.Steps;

[Binding]
public sealed class CalculatorStepDefinitions
{
    // For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef

    private readonly ScenarioContext _scenarioContext;
    private readonly IObjectContainer _objectContainer;

    public CalculatorStepDefinitions(ScenarioContext scenarioContext, IObjectContainer objectContainer)
    {
        _scenarioContext = scenarioContext;
        _objectContainer = objectContainer;
    }

    [Given("the first number is (.*)")]
    public void GivenTheFirstNumberIs(int number)
    {
        //TODO: implement arrange (precondition) logic
        // For storing and retrieving scenario-specific data see https://go.specflow.org/doc-sharingdata
        // To use the multiline text or the table argument of the scenario,
        // additional string/Table parameters can be defined on the step definition
        // method. 
        _scenarioContext.Add("FirstNumber", number);
    }

    [Given("the second number is (.*)")]
    public void GivenTheSecondNumberIs(int number)
    {
        //TODO: implement arrange (precondition) logic
        // For storing and retrieving scenario-specific data see https://go.specflow.org/doc-sharingdata
        // To use the multiline text or the table argument of the scenario,
        // additional string/Table parameters can be defined on the step definition
        // method. 

        _scenarioContext.Add("SecondNumber", number);
    }

    [When("the two numbers are added")]
    public void WhenTheTwoNumbersAreAdded()
    {
        //TODO: implement act (action) logic
        var sum = (int) _scenarioContext["FirstNumber"] + (int)_scenarioContext["SecondNumber"] ;
        _scenarioContext.Add("Sum", sum);
    }

    [Then("the result should be (.*)")]
    public void ThenTheResultShouldBe(int result)
    {
        //TODO: implement assert (verification) logic

        Assert.Equal(result, (int)_scenarioContext["Sum"]);
    }
}