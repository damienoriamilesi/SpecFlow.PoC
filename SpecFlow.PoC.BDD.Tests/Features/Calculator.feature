@calculator
Feature: Calculator
![Calculator2](https://specflow.org/wp-content/uploads/2020/09/calculator.png)
Simple calculator for adding **two** numbers

***Further read***: **[Learn more about how to generate Living Documentation](https://docs.specflow.org/projects/specflow-livingdoc/en/latest/LivingDocGenerator/Generating-Documentation.html)**

[BUG] - LINK TO FEATURE => The search input is located on the [Home Screen](<~/Projects/SpecFlow.PoC/SpecFlow.PoC.BDD.Tests/Features/FirstScenario.feature>).
	
<a href="#/document/Standalone/feature/f380c9824c9be2e696ef7cbdbac6f572">Context Diagram</a>
	


	
@my-tag @AX-22-AZERTY
Scenario: Add two numbers
	
	***Further read***: 
	**[Learn more about how to generate Living Documentation](https://docs.specflow.org/projects/specflow-livingdoc/en/latest/LivingDocGenerator/Generating-Documentation.html)**
	#Rule: [(https://docs.specflow.org/projects/specflow-livingdoc/en/latest/LivingDocGenerator/Generating-Documentation.html)]
	
		Given the first number is 50
		And the second number is 70
		When the two numbers are added
		Then the result should be 120