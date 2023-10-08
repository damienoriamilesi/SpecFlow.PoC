Feature: Calculator https://specflow.org/wp-content/uploads/2020/09/calculator.png
![Calculator](https://specflow.org/wp-content/uploads/2020/09/calculator.png)
Simple calculator for adding **two** numbers

Link to a feature: [Calculator]($projectname$/Features/Calculator.feature)
***Further read***: **[Learn more about how to generate Living Documentation](https://docs.specflow.org/projects/specflow-livingdoc/en/latest/LivingDocGenerator/Generating-Documentation.html)**

@my-tag @AX-22-AZERTY
Scenario: Add two numbers
	
	***Further read***: 
	**[Learn more about how to generate Living Documentation](https://docs.specflow.org/projects/specflow-livingdoc/en/latest/LivingDocGenerator/Generating-Documentation.html)**
	#Rule: [(https://docs.specflow.org/projects/specflow-livingdoc/en/latest/LivingDocGenerator/Generating-Documentation.html)]
	
		Given the first number is 50
		And the second number is 70
		When the two numbers are added
		Then the result should be 120